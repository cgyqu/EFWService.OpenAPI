using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EFWService.OpenAPI.Emums;
using EFWService.OpenAPI.ExceptionProcess;
using EFWService.OpenAPI.Exceptions;
using EFWService.OpenAPI.Logger;
using EFWService.OpenAPI.Model;
using EFWService.OpenAPI.Models;
using EFWService.OpenAPI.OutputProcessor;
using EFWService.OpenAPI.Utils;
using static System.String;

namespace EFWService.OpenAPI
{
    public abstract partial class ApiMethodBase<RequestModelType, ResponseModelType>
        where RequestModelType : ApiRequestModelBase
        where ResponseModelType : ApiResponseModelBase
    {
        private string postData = string.Empty;
        /// <summary>
        /// /返回数据序列化容器
        /// </summary>
        internal IOutputProcessor OutputProcessor { get; private set; }
        /// <summary>
        /// post数据
        /// </summary>
        public string PostData
        {
            get
            {
                if (!IsNullOrEmpty(postData))
                {
                    return postData;
                }
                if (HttpRequest.InputStream != null)
                {
                    using (Stream stream = HttpRequest.InputStream)
                    {
                        try
                        {
                            stream.Position = 0;
                            using (StreamReader strReader = new StreamReader(stream, Encoding.UTF8))
                            {
                                postData = strReader.ReadToEnd();
                                return postData;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("读取Post流异常", ex);
                        }
                    }
                }
                postData = "";
                return postData;
            }
        }
        /// <summary>
        /// 执行接口
        /// </summary>
        /// <returns></returns>
        public ActionResult Execute(RequestModelType request)
        {
            ExecuteBegin();

            BeginLog(apiLogEntity, request);

            //返回内容
            string content = string.Empty;
            //自定义ResultType
            SwitchResultType(request);

            //选择内容输出器
            SwitchOutputProcessor(request);

            ResponseModelType responseModel = null;
            try
            {
                //执行验证器
                Verify(request);

                //结构化数据处理例如 XML JSON 等 采用自动序列化方式
                StructuredPostDataProcess(ref request);

                //参数验证
                DoRequestParamsCheck(SuperContext.ModelState, request);

                //执行业务逻辑
                responseModel = ExecuteLogic(request);

                if (responseModel == null)
                {
                    throw new ApiException(ApiResultCode.ResponseNull) { ErrorMessage = "执行结果返回为空" };
                }
                content = CustomOutputFun(request, responseModel);
                //自定义内容输出
                if (content == NoCustomOutputFun)
                {
                    content = GetOutPutContent(responseModel, request);
                }
            }
            catch (Exception ex)
            {
                MonitorException(ex);
                content = ErrorHandler.Process<RequestModelType, ResponseModelType>(GetErrorContent, ex, request, apiLogEntity, apiMethodMetaInfo);
            }
            //结束
            #region 日志处理
            _EndLog(apiLogEntity, request, responseModel);
            apiLogEntity.RespContent = content ?? string.Empty;
            apiLogEntity.Params = postData ?? string.Empty;
            SaveLog(apiLogEntity);
            #endregion

            ExecuteEnd();
            return new APIContentResult() { Content = content, ContentEncoding = Encoding.UTF8, ContentType = GetContentType(request) };
        }
        /// <summary>
        /// 参数合法性检查
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public virtual RequestParamsCheckResult RequestParamsCheck(ModelStateDictionary modelState, RequestModelType request, ParamsCheckHelper paramsCheckHelper)
        {
            var result = new RequestParamsCheckResult() { Success = modelState.IsValid };

            foreach (var item in modelState.Values)
            {
                foreach (var error in item.Errors)
                {
                    if (!string.IsNullOrEmpty(error.ErrorMessage))
                    {
                        result.ErrorMessage = error.ErrorMessage.TrimEnd('。');
                        break;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 参数合法性验证
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        private void DoRequestParamsCheck(ModelStateDictionary modelState, RequestModelType request)
        {
            if (!modelState.IsValid) 
            {
                var list = modelState.Values.SelectMany(x => x.Errors).ToList()
                  .Select(x => x.ErrorMessage).ToList();
                var message = string.Join("\r\n", list);
                throw new ApiException(ApiResultCode.ParamsError) { ErrorMessage = message };
            }

            ParamsCheckHelper paramsCheckHelper = new ParamsCheckHelper();
            var result = RequestParamsCheck(modelState, request, paramsCheckHelper);
            if (!result.Success)
            {
                string msg = string.Format("参数验证不通过:[{0}]", result.ErrorMessage);
                apiLogEntity.AddLogMessage(msg);
                apiLogEntity.LogType = LogType.Warning;
                throw new ApiException(ApiResultCode.ParamsError) { ErrorMessage = msg };
            }
        }
        /// <summary>
        /// 执行接口业务逻辑
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        public abstract ResponseModelType ExecuteLogic(RequestModelType request);

        /// <summary>
        /// 自定义输出内容拼接方法
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public virtual string CustomOutputFun(RequestModelType request, ResponseModelType response)
        {
            return NoCustomOutputFun;
        }
        /// <summary>
        /// 设置输出数据格式
        /// </summary>
        /// <param name="request"></param>
        public virtual void SwitchResultType(RequestModelType request)
        {
        }
        /// <summary>
        /// 监控异常
        /// </summary>
        /// <param name="ex"></param>
        public virtual void MonitorException(Exception ex)
        {
        }
        /// <summary>
        /// 参数反序列化
        /// </summary>
        /// <param name="request"></param>
        protected virtual void StructuredPostDataProcess(ref RequestModelType request)
        {
            try
            {
                if (ApiMethodMetaInfo.IsStructuredPost && !string.IsNullOrEmpty(PostData))
                {
                    request = JsonConvertExd.Deserialize<RequestModelType>(PostData);
                }
            }
            catch (Exception ex)
            {
                throw new ApiException(ApiResultCode.ParamsError) { ErrorMessage = "提交数据存在异常:" + ex.Message };
            }
        }
        /// <summary>
        /// 保存日志
        /// </summary>
        /// <param name="apiLogEntity"></param>
        private void SaveLog(ApiLogEntity apiLogEntity)
        {
            //使用默认日记记录器
            if (WebBaseUtil.ApiLogger == null)
            {
                WebBaseUtil.ApiLogger = new GenericLogger();
            }
            if (WebBaseUtil.ApiLogger != null)
            {
                //请求IP列表
                apiLogEntity.ClientIPList = ClientIPList != null ? ClientIPList : new List<string> { };
                apiLogEntity.TextBoxFilterItem1 = apiLogEntity.TextBoxFilterItem1 ?? "";
                apiLogEntity.TextBoxFilterItem2 = apiLogEntity.TextBoxFilterItem2 ?? "";
                //方法名作为日志信息
                if (string.IsNullOrWhiteSpace(apiLogEntity.Message))
                {
                    apiLogEntity.Message = this.apiMethodMetaInfo.APIMethodDesc.Desc ?? "";
                }
                if (string.IsNullOrEmpty(apiLogEntity.ModuleName))
                {
                    apiLogEntity.ModuleName = this.ApiMethodMetaInfo.Module;
                }
                if (string.IsNullOrEmpty(apiLogEntity.CategoryName))
                {
                    apiLogEntity.CategoryName = this.ApiMethodMetaInfo.Category;
                }
                if (string.IsNullOrEmpty(apiLogEntity.SubcategoryName))
                {
                    apiLogEntity.SubcategoryName = this.ApiMethodMetaInfo.MethodName;
                }

                apiLogEntity.RequestURL = HttpRequest.RawUrl;
                apiLogEntity.HttpMethod = HttpRequest.HttpMethod;
                apiLogEntity.LogFinish();
                WebBaseUtil.ApiLogger.Log(apiLogEntity);
            }
        }
        /// <summary>
        /// 接口执行前验证
        /// </summary>
        /// <param name="request"></param>
        private void Verify(RequestModelType request)
        {
            #region 执行验证接口管道
            foreach (var auth in WebBaseUtil.AuthenticationPipeline)
            {
                auth.Verify<RequestModelType, ResponseModelType>(this, request);
            }
            #endregion
        }
        /// <summary>
        /// 选择输出参数序列化方法，xml或者json
        /// </summary>
        /// <param name="request"></param>
        private void SwitchOutputProcessor(RequestModelType request)
        {
            if (outPrDict.ContainsKey(request.ResultType))
            {
                OutputProcessor = outPrDict[request.ResultType];
            }
            else
            {
                OutputProcessor = outPrDict.FirstOrDefault().Value;
            }
        }
        /// <summary>
        /// 输出参数类型
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static string GetContentType(RequestModelType request)
        {
            string contentType = ApiContentType.Json;

            if (request.ResultType.Contains("xml"))
            {
                contentType = ApiContentType.Xml;
            }
            else if (request.ResultType.Contains("html"))
            {
                contentType = ApiContentType.Html;
            }
            else if (request.ResultType.Contains("plain"))
            {
                contentType = ApiContentType.Plain;
            }
            else
            {
                contentType = ApiContentType.Json;
            }
            return contentType;
        }
        /// <summary>
        /// 执行前操作
        /// </summary>
        private void ExecuteBegin()
        {
            foreach (var auth in WebBaseUtil.ApiExecuteAroundPipeline)
            {
                auth.Before(new DefaultApiExecuteAround.BeforeParam()
                {
                    HttpRequest = this.HttpRequest,
                    HttpResponse = this.HttpResponse,
                });
            }
        }
        /// <summary>
        /// 执行后操作
        /// </summary>
        private void ExecuteEnd()
        {
            foreach (var auth in WebBaseUtil.ApiExecuteAroundPipeline)
            {
                auth.After(new DefaultApiExecuteAround.AfterParam()
                {
                    HttpRequest = this.HttpRequest,
                    HttpResponse = this.HttpResponse,
                    Stopwatch = this.Stopwatch,
                    ApiMethodMetaInfo = this.ApiMethodMetaInfo,
                    HasError = apiLogEntity.Exception != null,
                    LogType = apiLogEntity.LogType
                });
            }
        }

        public virtual ResponseModelType GetUnSuccessResponseModel(ApiResponseModelBase responseModel)
        {
            return null;
        }
        /// <summary>
        /// 自定义异常输出
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public virtual string GetCustomErrorOutput(RequestModelType request, ApiResponseModelBase response, Exception ex = null)
        {
            return NoCustomOutputFun;
        }
        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseModel"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private string GetErrorContent(RequestModelType request, ApiResponseModelBase responseModel, Exception ex)
        {
            string custError = GetCustomErrorOutput(request, responseModel, ex);
            if (custError != NoCustomOutputFun)
            {
                return custError;
            }
            //responseModel.respTime = DateTime.Now;
            var resp = GetUnSuccessResponseModel(responseModel);
            if (resp == null)
            {
                return OutputProcessor.OutPut<ApiResponseModelBase>(responseModel);
            }
            else
            {
                return OutputProcessor.OutPut<ResponseModelType>(resp);
            }
        }
        /// <summary>
        /// 执行接口序列化
        /// </summary>
        /// <param name="responseModel"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private string GetOutPutContent(ResponseModelType responseModel, RequestModelType requestModel)
        {
            //responseModel.respTime = DateTime.Now;
            return OutputProcessor.OutPut<ResponseModelType>(responseModel);
        }
        /// <summary>
        /// 开始日志
        /// </summary>
        /// <param name="apiLogEntity"></param>
        /// <param name="request"></param>
        private void _BeginLog(ApiLogEntity apiLogEntity, RequestModelType request)
        {
            try
            {
                BeginLog(apiLogEntity, request);
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// 结束日志
        /// </summary>
        /// <param name="apiLogEntity"></param>
        /// <param name="request"></param>
        /// <param name="responseModel"></param>
        private void _EndLog(ApiLogEntity apiLogEntity, RequestModelType request, ResponseModelType responseModel)
        {
            apiLogEntity.ElapsedMilliseconds = Stopwatch.ElapsedMilliseconds;
            try
            {
                if (responseModel == null)
                {
                    apiLogEntity.AddLogMessage("responseModel为空,EndLog将不被调用");
                }
                else
                {
                    EndLog(apiLogEntity, request, responseModel);
                }
            }
            catch (Exception)
            {
                apiLogEntity.AddLogMessage("调用EndLog时出现异常");
            }
        }
        /// <summary>
        /// 开始自定义日志
        /// </summary>
        /// <param name="apiLog"></param>
        /// <param name="request"></param>
        public virtual void BeginLog(ApiLogEntity apiLog, RequestModelType request)
        {

        }
        /// <summary>
        /// 结束自定义日志
        /// </summary>
        /// <param name="apiLog"></param>
        /// <param name="request"></param>
        /// <param name="responseModel"></param>
        public virtual void EndLog(ApiLogEntity apiLog, RequestModelType request, ResponseModelType responseModel)
        {

        }
    }
}

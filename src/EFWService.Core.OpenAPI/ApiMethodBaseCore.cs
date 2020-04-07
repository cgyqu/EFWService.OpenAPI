using EFWService.Core.OpenAPI.ExceptionProcess;
using EFWService.Core.OpenAPI.Exceptions;
using EFWService.Core.OpenAPI.Logger;
using EFWService.Core.OpenAPI.Models;
using EFWService.Core.OpenAPI.OutputProcessor;
using EFWService.Core.OpenAPI.Utils;
using EFWService.OpenAPI.Emums;
using EFWService.OpenAPI.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFWService.Core.OpenAPI
{
    /// <summary>
    /// API基类
    /// </summary>
    /// <typeparam name="RequestModelType"></typeparam>
    /// <typeparam name="ResponseModelType"></typeparam>
    public abstract partial class ApiMethodBase<RequestModelType, ResponseModelType>
        where RequestModelType : ApiRequestModelBase
        where ResponseModelType : ApiResponseModelBase
    {
        private string postData = null;
        private IOutputProcessor OutputProcessor;
        public string PostData
        {
            get
            {
                if (postData != null)
                {
                    return postData;
                }
                if (HttpRequest.Body != null)
                {
                    using (Stream stream = HttpRequest.Body)
                    {
                        try
                        {
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
                postData = string.Empty;
                return postData;
            }
        }
        private string content = string.Empty;
        /// <summary>
        /// 执行接口
        /// </summary>
        /// <returns></returns>
        public string Execute(RequestModelType request)
        {
            ResponseModelType responseModel = null;
            try
            {
                ExecuteBegin();
                BeginLog(apiLogEntity, request);
                //选择输出
                SwitchOutputProcessor(request);
                //执行验证器
                Verify(request);
                //post请求需要进行数据解析，默认进行json解析
                StructuredPostDataProcess(ref request);
                //参数验证
                DoRequestParamsCheck(request);
                //执行业务逻辑
                responseModel = ExecuteLogic(request);
                if (responseModel == null)
                {
                    throw new ApiException(ApiResultCode.ResponseNull) { ErrorMessage = "执行结果返回为空" };
                }
                content = CustomOutputFun(request, responseModel);
                ////自定义内容输出
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
            ExecuteEnd();
            _EndLog(apiLogEntity, request, responseModel);
            HttpResponse.ContentType = GetContentType(request);
            return content;
            // return new ContentResult() { Content = content, ContentType = GetContentType(request), StatusCode = 200 };
        }

        /// <summary>
        /// 参数合法性检查
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public virtual RequestParamsCheckResult RequestParamsCheck(RequestModelType request, ParamsCheckHelper paramsCheckHelper)
        {
            var result = new RequestParamsCheckResult() { Success = true };
            return result;
        }
        /// <summary>
        /// 参数合法性验证
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        private void DoRequestParamsCheck(RequestModelType request)
        {
            ParamsCheckHelper paramsCheckHelper = new ParamsCheckHelper();
            if (this.ApiMethodMetaInfo.APIMethodDesc.CheckParams)
            {
                var result = RequestParamsCheck(request, paramsCheckHelper);
                if (!result.Success)
                {
                    string msg = string.Format("参数验证不通过:[{0}]", result.ErrorMessage);
                    apiLogEntity.AddLogMessage(msg);
                    apiLogEntity.LogType = LogType.Warning;
                    throw new ApiException(ApiResultCode.ParamsError) { ErrorMessage = msg };
                }
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
        /// 自定义输出，当结果返回不为json的时候
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
        public virtual void StructuredPostDataProcess(ref RequestModelType request)
        {
            try
            {
                if (apiMethodMetaInfo.IsStructuredPost && !string.IsNullOrEmpty(this.PostData))
                {
                    request = JsonConvertExd.Deserialize<RequestModelType>(this.PostData);
                }
            }
            catch (ApiException)
            {
                throw;
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
            if (WebBaseUtil.ApiLogger != null)
            {
                //请求IP列表
                apiLogEntity.ClientIPList = new List<string> { ClientIPList };
                apiLogEntity.TextBoxFilterItem1 = apiLogEntity.TextBoxFilterItem1 ?? "";
                apiLogEntity.TextBoxFilterItem2 = ClientIPList ?? string.Empty;
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
                apiLogEntity.RequestURL = HttpRequest.Path;
                apiLogEntity.HttpMethod = HttpRequest.Method;
                WebBaseUtil.ApiLogger.Log(apiLogEntity);
            }
        }
        /// <summary>
        /// 日志过滤（手机号，身份证，邮箱）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual string LogFilter(string input)
        {
            return input;
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

            if (request.ResultType == "xml")
            {
                contentType = ApiContentType.Xml;
            }
            else if (request.ResultType == "html")
            {
                contentType = ApiContentType.Html;
            }
            else if (request.ResultType == "text")
            {
                contentType = ApiContentType.Plain;
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
                try
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
                catch { }
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
            var custError = GetCustomErrorOutput(request, responseModel, ex);
            if (custError != NoCustomOutputFun)
            {
                return custError;
            }
            var resp = GetUnSuccessResponseModel(responseModel);
            if (resp != null)
            {
                return OutputProcessor.OutPut<ResponseModelType>(resp);
            }
            return OutputProcessor.OutPut<ApiResponseModelBase>(responseModel);
        }

        private string GetOutPutContent(ResponseModelType responseModel, RequestModelType requestModel)
        {
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
            try
            {
                apiLogEntity.ElapsedMilliseconds = Stopwatch.ElapsedMilliseconds;
                apiLogEntity.RespContent = content;
                if (responseModel == null)
                {
                    apiLogEntity.AddLogMessage("responseModel为空,EndLog将不被调用");
                }
                else
                {
                    EndLog(apiLogEntity, request, responseModel);
                }
                if (!string.IsNullOrWhiteSpace(postData))
                {
                    apiLogEntity.Params = postData;
                }
            }
            catch (Exception)
            {
                apiLogEntity.AddLogMessage("调用EndLog时出现异常");
            }
            SaveLog(apiLogEntity);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFWService.OpenAPI.Emums;

namespace EFWService.OpenAPI.Model
{
    [Serializable]
    public class ApiResponseModelBase
    {
        public ApiResponseModelBase()
        {
            this.respCode = (int)ApiResultCode.HasResultSuccess;
            this.respMsg = "获取数据成功";
        }

        /// <summary>
        /// 响应码
        /// </summary>
        public int respCode { get; set; }

        /// <summary>
        /// 响应信息
        /// </summary>
        public string respMsg { get; set; }

        /// <summary>
        /// 响应时间
        /// </summary>
        public string respTime { get { return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"); } }

        public void HasResult(string respMsg = "获取数据成功")
        {
            respCode = (int)ApiResultCode.HasResultSuccess;
            this.respMsg = respMsg;
        }
        public void NoResult(string respMsg = "未获取到数据")
        {
            respCode = (int)ApiResultCode.NoResultSuccess;
            this.respMsg = respMsg;
        }
    }

    [Serializable]
    public class NormalResponseModel : ApiResponseModelBase
    {
        /// <summary>
        /// 返回内容
        /// </summary>
        public dynamic result { get; set; }
    }
}

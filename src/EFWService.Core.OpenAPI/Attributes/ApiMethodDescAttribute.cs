using EFWService.OpenAPI.Emums;
using System;

namespace EFWService.Core.OpenAPI.Attributes
{
    /// <summary>
    /// API方法描述
    /// </summary>
    public class ApiMethodDescAttribute : Attribute
    {
        public ApiMethodDescAttribute(string module, string category,
            HttpMethodType httpMethodType = HttpMethodType.ALL,
            bool checkParams = true,
            string desc = "",
            string methodName = "",
            bool isClose = false
            )
        {
            Category = category;//分类
            Module = module;//子分类
            HttpMethodType = httpMethodType;//请求方式
            CheckParams = checkParams;//是否进行参数检查
            Desc = desc;//描述
            MethodName = methodName;//方法名
            IsClose = isClose;//接口是否关闭
        }
        /// <summary>
        /// API分类
        /// </summary>
        public string Category { get; private set; }
        /// <summary>
        /// API模块
        /// </summary>
        public string Module { get; private set; }
        /// <summary>
        /// API模块
        /// </summary>
        public HttpMethodType HttpMethodType { get; private set; }
      

        public bool CheckParams { get; set; }

        public string Desc { get; set; }

        /// <summary>
        /// 自定义方法名
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// 是否关闭
        /// </summary>
        public bool IsClose { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFWService.OpenAPI.Emums;
using EFWService.OpenAPI.Enums;

namespace EFWService.OpenAPI.Attributes
{
    /// <summary>
    /// API方法描述
    /// </summary>
    public class ApiMethodDescAttribute : Attribute
    {
        public ApiMethodDescAttribute(string module, string category,
            HttpMethodType httpMethodType = HttpMethodType.ALL,
            string desc = "",
            string methodName = "",
            bool isClose = false, bool isShow = true
            )
        {
            Category = category;//分类
            Module = module;//子分类
            HttpMethodType = httpMethodType;//请求方式
            Desc = desc;//描述
            MethodName = methodName;//方法名
            IsClose = isClose;//接口是否关闭
            IsShow = isShow;//接口是否可以被查看
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

        public string Desc { get; set; }

        /// <summary>
        /// 自定义方法名
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// 是否关闭
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public bool IsClose { get; set; }
        /// <summary>
        /// 接口信息是否可以被查看
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public bool IsShow { get; set; }

    }
}

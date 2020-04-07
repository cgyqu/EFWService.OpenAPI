using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFWService.OpenAPI.Attributes;

namespace EFWService.OpenAPI.DynamicController
{
    /// <summary>
    /// API元数据
    /// </summary>
    public class ApiMethodMeta
    {
        public string Module
        {
            get
            {
                return APIMethodDesc.Module;
            }
        }

        public string Category
        {
            get
            {
                return APIMethodDesc.Category;
            }
        }

        public string MethodName { get; set; }

        public string Controller
        {
            get;
            set;
        }

        public string TypeName { get; set; }

        public Type IApiRequestModelType { get; set; }

        public Type MethodType { get; set; }

        public string FullTypeName { get; set; }

        public string FullTypeNameSHA1 { get; set; }

        public string Fap
        {
            get;
            set;
        }

        public bool IsStructuredPost { get; set; }
        public ApiMethodDescAttribute APIMethodDesc { get; set; }
    }
}

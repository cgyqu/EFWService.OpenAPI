using System;
using EFWService.Core.OpenAPI.Attributes;

namespace EFWService.Core.OpenAPI.DynamicController
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

        public ApiMethodDescAttribute APIMethodDesc { get; set; }

        public string MethodName { get; set; }

        public string Controller
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Format("Controller:{0},Method:{1}",
                              Controller,
                                MethodName
                                );
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
    }
}

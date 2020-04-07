using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFWService.OpenAPI.Model
{
    public class ApiRequestModelBase
    {
        /// <summary>
        /// 结果类型 json 、 xml
        /// </summary>
        public string ResultType { get; set; } = "json";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFWService.OpenAPI.Models
{
    public class RequestParamsCheckResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}

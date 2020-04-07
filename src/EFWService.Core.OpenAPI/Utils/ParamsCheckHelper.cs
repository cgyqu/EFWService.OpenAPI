using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFWService.Core.OpenAPI.Models;

namespace EFWService.Core.OpenAPI.Utils
{
    public class CheckResultFuns
    {
        public Func<bool> Funs { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ParamsCheckHelper
    {
        private List<RequestParamsCheckResult> checkResultList = new List<RequestParamsCheckResult>();
        private List<CheckResultFuns> checkResultFunsList = new List<CheckResultFuns>();

        public ParamsCheckHelper Check(Func<bool> checkFunc, string errorMessage)
        {
            checkResultFunsList.Add(new CheckResultFuns() { Funs = checkFunc, ErrorMessage = errorMessage });
            return this;
        }

        public RequestParamsCheckResult Finish()
        {
            foreach (var item in checkResultFunsList)
            {
                if (item.Funs() == false)
                {
                    return new RequestParamsCheckResult() { Success = false, ErrorMessage = item.ErrorMessage };
                }
            }
            return new RequestParamsCheckResult() { Success = true };
        }
    }
}

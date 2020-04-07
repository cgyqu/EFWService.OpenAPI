using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFWService.OpenAPI.Models;

namespace EFWService.OpenAPI.Utils
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
        private Action action;

        public ParamsCheckHelper Check(Func<bool> checkFunc, string errorMessage)
        {
            checkResultFunsList.Add(new CheckResultFuns() { Funs = checkFunc, ErrorMessage = errorMessage });
            return this;
        }

        public ParamsCheckHelper Then(Action action)
        {
            this.action = action;
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
            if (action != null)
            {
                action();
            }
            return new RequestParamsCheckResult() { Success = true };
        }
    }
}

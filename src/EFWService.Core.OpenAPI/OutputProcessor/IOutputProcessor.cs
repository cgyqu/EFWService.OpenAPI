using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFWService.Core.OpenAPI.OutputProcessor
{
    public interface IOutputProcessor
    {
        string OutPut<ResponseModel>(ResponseModel model);
    }
}

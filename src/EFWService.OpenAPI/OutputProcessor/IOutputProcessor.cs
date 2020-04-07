using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFWService.OpenAPI.OutputProcessor
{
    public interface IOutputProcessor
    {
        string OutPut<RequestModelType>(RequestModelType request);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFWService.OpenAPI.Logger
{
    public interface IApiLogger<T> where T : ApiLogEntity
    {
        void Log(T log);
    }
}

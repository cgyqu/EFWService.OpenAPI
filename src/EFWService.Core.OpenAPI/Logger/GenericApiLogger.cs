using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFWService.Core.OpenAPI.Logger;

namespace EFWService.Core.OpenAPI.Logger
{
    public class GenericLogger : IApiLogger<ApiLogEntity>
    {
        /// <summary>
        /// 是否开启异常日志
        /// </summary>
        public Func<bool> CloesNotExceptionLogSwitch { get; set; }

        public void Log(ApiLogEntity log)
        {
            log.LogFinish();
            if (log.Exception == null && CloesNotExceptionLogSwitch != null && CloesNotExceptionLogSwitch() == true)
            {
                return;
            }
            if (log.LogType == LogType.Warning)
            {
            }
            if (log.LogType == LogType.Debug)
            {
            }
            if (log.Exception != null)
            {

            }
            else
            {
            }
        }
    }
}

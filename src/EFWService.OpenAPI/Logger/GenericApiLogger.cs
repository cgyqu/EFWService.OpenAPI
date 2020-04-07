using System;

namespace EFWService.OpenAPI.Logger
{
    /// <summary>
    /// 内置日志记录器
    /// </summary>
    public class GenericLogger : IApiLogger<ApiLogEntity>
    {

        public void Log(ApiLogEntity log)
        {
            if (log.LogType == LogType.Warning)
            {
                //TODO warnging log 
                return;
            }

            if (log.LogType == LogType.Debug)
            {
                //TODO debug log
            }
            if (log.Exception != null)
            {
                //TODO errro log
                return;
            }
        }
    }
}

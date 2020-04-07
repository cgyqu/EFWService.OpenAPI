using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFWService.OpenAPI.Enums
{
    public enum RequestCountLimit
    {
        /// <summary>
        /// 不限制
        /// </summary>
        Not = 0,
        /// <summary>
        /// 必须限制
        /// </summary>
        Must = 1,
        /// <summary>
        /// 根据设置
        /// </summary>
        BySetting = 2
    }
}

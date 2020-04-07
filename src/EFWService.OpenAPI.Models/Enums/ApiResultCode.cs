using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace EFWService.OpenAPI.Emums
{
    /// <summary>
    /// 返回结果code
    /// </summary>
    public enum ApiResultCode
    {
        /// <summary>
        /// 执行成功并有结果返回
        /// </summary>
        [Description("获取数据成功")]
        HasResultSuccess = 1000,
        /// <summary>
        /// 执行成功但无结果返回
        /// </summary>
        [Description("未获取到数据")]
        NoResultSuccess = 1001,
        /// <summary>
        /// 参数验证失败
        /// </summary>
        [Description("参数验证失败")]
        ParamsError = 3000,
        /// <summary>
        /// http请求方法不合法
        /// </summary>
        [Description("该接口必须以:{0}方式请求")]
        HttpMethodError = 4001,
        /// <summary>
        /// 接口业务逻辑部分返回空值
        /// </summary>
        [Description("执行结果返回为空")]
        ResponseNull = 4002,
        /// <summary>
        /// 其他异常
        /// </summary>
        [Description("其他错误")]
        OtherError = 6000,
        /// <summary>
        /// 未知系统错误
        /// </summary>
        [Description("未知系统错误")]
        SystemError = 7000,
        /// <summary>
        /// HTTP错误，一般是请求第三方接口超时、504等
        /// </summary>
        [Description("请求内部接口出错")]
        HttpError = 7001,
        /// <summary>
        /// 账号认证失败
        /// </summary>
        [Description("账号认证失败")]
        AccountAuthenticationError = 9000,
        /// <summary>
        /// 其他认证失败，统一使用此code
        /// </summary>
        [Description("认证失败")]
        AuthenticationError = 9001
    }
}

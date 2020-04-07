using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFWService.Core.OpenAPI.Logger
{
    /// <summary>
    /// 日志消息
    /// </summary>
    public class LogMessageItem
    {
        public LogMessageItem()
        {
            LogTime = DateTime.Now;
        }
        public string Message { get; set; }
        public DateTime LogTime { get; private set; }

        public int LogIndex { get; set; }

        public override string ToString()
        {
            return string.Format("idx:{2} t:{0} msg:{1}|", LogTime.ToString("yyyy-MM-dd HH:mm:ss"), Message, LogIndex);
        }
    }
    public enum LogType
    {
        Info = 1,
        Warning = 2,
        Debug = 3
    }
    public class ApiLogEntity
    {
        public ApiLogEntity()
        {
            LogTime = DateTime.Now;
            LogMessageItemList = new List<LogMessageItem>();
            DisplayItems = new Dictionary<string, string>();
        }

        public DateTime LogTime { get; set; }

        private List<LogMessageItem> LogMessageItemList { get; set; }

        private static readonly object lockObj = new object();

        private int logIndex = 0;

        /// <summary>
        /// 模块
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 大分类
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 小分类
        /// </summary>
        public string SubcategoryName { get; set; }

        /// <summary>
        /// 日志主要消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 搜索条件1
        /// </summary>
        public string TextBoxFilterItem2 { get; set; }

        /// <summary>
        /// 搜索条件2
        /// </summary>
        public string TextBoxFilterItem1 { get; set; }

        /// <summary>
        /// 显示杂项
        /// </summary>
        public Dictionary<string, string> DisplayItems { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public long ElapsedMilliseconds { get; set; }

        public LogType LogType { get; set; }

        public string RequestURL { get; set; }

        public string HttpMethod { get; set; }

        public string Params { get; set; }

        public Exception Exception { get; set; }
        public string ExceptionId { get; set; }
        public string RespContent { get; set; }

        public List<string> ClientIPList { get; set; }
        /// <summary>
        /// 增加日志内容
        /// </summary>
        /// <param name="message"></param>
        /// <param name="_params"></param>
        public void AddLogMessage(string message, params object[] _params)
        {
            lock (lockObj)
            {
                logIndex++;
                if (_params != null && _params.Length > 0)
                {
                    message = string.Format(message, _params);
                }
                LogMessageItemList.Add(new LogMessageItem() { Message = message, LogIndex = logIndex });
            }
        }

        private bool logFinished = false;

        public void LogFinish()
        {
            lock (lockObj)
            {
                if (!logFinished)
                {
                    if (DisplayItems == null)
                    {
                        DisplayItems = new Dictionary<string, string>();
                    }
                    DisplayItems.Add("Url", RequestURL);
                    DisplayItems.Add("Method", HttpMethod);
                    DisplayItems.Add("Logs", string.Join(",", LogMessageItemList)?.TrimEnd('|') ?? "无");
                    DisplayItems.Add("ES", ElapsedMilliseconds.ToString());
                    if (!string.IsNullOrWhiteSpace(Params))
                    {
                        Message += $"{Environment.NewLine}请求参数:{Params}";
                    }
                    if (!string.IsNullOrWhiteSpace(RespContent))
                    {
                        Message += $"{Environment.NewLine}返回结果:{RespContent}";
                    }
                    if (ClientIPList != null)
                    {
                        DisplayItems.Add("CIP", string.Join(",", ClientIPList));
                    }
                    logFinished = true;
                }
            }
        }
    }
}

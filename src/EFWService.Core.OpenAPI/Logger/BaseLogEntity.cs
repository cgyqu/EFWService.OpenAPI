using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFWService.Core.OpenAPI.Logger
{
    public class BaseLogEntity
    {
        public BaseLogEntity()
        {
            LogTime = DateTime.Now;
            LogMessageItemList = new List<LogMessageItem>();
            DisplayItems = new Dictionary<string, string>();
        }

        public DateTime LogTime { get; set; }
        private List<LogMessageItem> LogMessageItemList { get; set; }

        public object lockObj = new object();

        private int logIndex = 0;

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
                string _message = string.Format(message, _params);
                LogMessageItemList.Add(new LogMessageItem() { Message = _message, LogIndex = logIndex });
            }
        }
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

        private bool logFinished = false;
        public void LogFinish()
        {
            if (!logFinished)
            {
                DisplayItems.Add("Logs", string.Join(",", LogMessageItemList));
                DisplayItems.Add("ES", ElapsedMilliseconds.ToString());
            }
        }
        /// <summary>
        /// 执行时间
        /// </summary>
        public long ElapsedMilliseconds { get; set; }
        public LogType LogType { get; set; }
        public Exception Exception { get; set; }
        public string ExceptionId { get; set; }
    }
}

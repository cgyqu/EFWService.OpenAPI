/* ======================================================================== 
 * Copyright (c) 同程网络科技股份有限公司. All rights reserved.
 * 文 件 名：APIOutPutFormater.cs      
 * 创 建 人：cgy6094
 * 创建日期： 2018/5/14 10:58:57
 * 用    途：APIOutPutFormater Class File
 * ======================================================================== */
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EFWService.Core.OpenAPI.Formater
{
    public class APIOutputFormatter : IOutputFormatter
    {
        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            return true;
        }

        public Task WriteAsync(OutputFormatterWriteContext context)
        {
            var response = context.HttpContext.Response;
            using (var writer = context.WriterFactory(response.Body, Encoding.UTF8))
            {
                writer.Write(context.Object);
                return writer.FlushAsync();
            }
        }
    }
}

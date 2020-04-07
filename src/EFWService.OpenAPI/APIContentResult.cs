using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace EFWService.OpenAPI
{
    internal class APIContentResult : ActionResult
    {
        public string Content { get; set; }

        public Encoding ContentEncoding { get; set; }

        public string ContentType { get; set; }

        public override void ExecuteResult(System.Web.Mvc.ControllerContext context)
        {
            ContentResult result = new ContentResult()
            {
                Content = Content,
                ContentEncoding = ContentEncoding,
                ContentType = ContentType
            };

            result.ExecuteResult(context);
        }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.Core
{
    public class TestMiddleware
    {
        private RequestDelegate _next;
        public TestMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public Task Invoke(HttpContext context)
        {
            var request = context.Request;
            return _next(context);
        }
    }
}

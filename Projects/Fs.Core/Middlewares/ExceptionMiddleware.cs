using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Fs.Core.Contracts;
using Fs.Core.Interfaces.Services;
using Fs.Core.Exceptions;

namespace Fs.Core.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerSettings _jsonSettings;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;

            _next = next ?? throw new ArgumentNullException(nameof(next));

            _jsonSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public async Task Invoke(HttpContext context, IErrorLogService service)
        {
            try
            {
                await _next(context);
            }
            catch (RulesException re)
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }

                context.Response.StatusCode = 400;
                context.Response.Headers.Add("exception", "validationException");
                var modelState = new ModelStateDictionary();
                re.AddModelStateErrors(modelState);
                var json = JsonConvert.SerializeObject(modelState.Errors(true), _jsonSettings);
                await context.Response.WriteAsync(json);
            }
            catch (CustomMessageException cm)
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }

                object[] args = new object[2];

                args[0] = cm.Message;
                args[1] = cm.StackTrace;

                _logger.LogError(cm, "Exception: Message {0}, Stack {1}", args);

                ErrorLogDto value = new ErrorLogDto();

                value.CustomMessage = cm.ExceptionMessage;
                value.Message = cm.Message;
                value.Page = context.Request.Path;
                value.Stack = cm.StackTrace;
                value.Type = "Exception";

                await service.SaveErrorLog(value);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                context.Response.Headers.Add("exception", "messageException");
                var json = JsonConvert.SerializeObject(new { Message = cm.ExceptionMessage }, _jsonSettings);
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }

                object[] args = new object[2];

                args[0] = ex.Message;
                args[1] = ex.StackTrace;

                _logger.LogError(ex, "Exception: Message {0}, Stack {1}", args);

                ErrorLogDto value = new ErrorLogDto();

                value.CustomMessage = ex.Message;
                value.Message = ex.Message;
                value.Page = context.Request.Path;
                value.Stack = ex.StackTrace;
                value.Type = "Exception";

                await service.SaveErrorLog(value);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                context.Response.Headers.Add("exception", "messageException");
                var json = JsonConvert.SerializeObject(new { Message = ex.Message }, _jsonSettings);
                await context.Response.WriteAsync(json);
            }
        }
    }


}

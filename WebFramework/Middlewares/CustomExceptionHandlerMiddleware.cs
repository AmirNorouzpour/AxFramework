using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Common;
using System.Net;
using Common.Exception;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Web;
using WebFramework.Api;
using LogLevel = NLog.LogLevel;

namespace WebFramework.Middlewares
{
    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }

    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly Logger _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        public CustomExceptionHandlerMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            string message = null;
            var httpStatusCode = HttpStatusCode.InternalServerError;
            var apiStatusCode = ApiResultStatusCode.ServerError;
            var theEvent = new LogEventInfo(LogLevel.Error, "Exception Handler", "");
            theEvent.Properties["LogType"] = apiStatusCode.ToString();

            try
            {
                await _next(context);
            }
            catch (AppException exception)
            {
                httpStatusCode = exception.HttpStatusCode;
                apiStatusCode = exception.ApiStatusCode;
                theEvent.Message = exception.Message;
                theEvent.Exception = exception;
                theEvent.Properties["LogType"] = apiStatusCode.ToString();
                theEvent.Level = httpStatusCode == HttpStatusCode.Unauthorized ? LogLevel.Warn : LogLevel.Error;
                if (exception.ToString().Contains("A task was canceled."))
                    theEvent.Level = LogLevel.Info;

                _logger.Log(theEvent);
                if (_env.IsDevelopment())
                {
                    var dic = new Dictionary<string, string>
                    {
                        ["Exception"] = exception.Message,
                        ["StackTrace"] = exception.StackTrace,
                    };
                    if (exception.InnerException != null)
                    {
                        dic.Add("InnerException.Exception", exception.InnerException.Message);
                        dic.Add("InnerException.StackTrace", exception.InnerException.StackTrace);
                    }

                    if (exception.AdditionalData != null)
                        dic.Add("AdditionalData", JsonConvert.SerializeObject(exception.AdditionalData));

                    message = JsonConvert.SerializeObject(dic);
                }
                else
                {
                    message = exception.Message;
                }

                await WriteToResponseAsync();
            }
            catch (SecurityTokenExpiredException exception)
            {
                theEvent.Message = exception.Message;
                theEvent.Exception = exception;
                _logger.Error(theEvent);
                SetUnAuthorizeResponse(exception);
                await WriteToResponseAsync();
            }
            catch (UnauthorizedAccessException exception)
            {
                theEvent.Message = exception.Message;
                theEvent.Exception = exception;
                _logger.Error(theEvent);
                SetUnAuthorizeResponse(exception);
                await WriteToResponseAsync();
            }
            catch (ValidationException validationException)
            {
                theEvent.Message = validationException.Message;
                theEvent.Exception = validationException;
                apiStatusCode = ApiResultStatusCode.LogicError;
                httpStatusCode = HttpStatusCode.OK;
                message = theEvent.Message;
                _logger.Info(theEvent);
                await WriteToResponseAsync();
            }
            catch (Exception exception)
            {
                theEvent.Message = exception.Message;
                theEvent.Exception = exception;
                if (!exception.ToString().Contains("A task was canceled."))
                    _logger.Error(theEvent);
                else
                    _logger.Info(theEvent);
                //if (_env.IsDevelopment())
                {
                    var dic = new Dictionary<string, string>
                    {
                        ["Exception"] = exception.Message,
                        ["StackTrace"] = exception.StackTrace,
                    };
                    message = JsonConvert.SerializeObject(dic);
                }

                await WriteToResponseAsync();

            }

            async Task WriteToResponseAsync()
            {
                if (context.Response.HasStarted)
                    throw new InvalidOperationException("The response has already started, the http status code middleware will not be executed.");

                var result = new ApiResult(false, apiStatusCode, message);
                var json = JsonConvert.SerializeObject(result);

                context.Response.StatusCode = (int)httpStatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }

            void SetUnAuthorizeResponse(Exception exception)
            {
                httpStatusCode = HttpStatusCode.Unauthorized;
                apiStatusCode = ApiResultStatusCode.UnAuthorized;

                if (_env.IsDevelopment())
                {
                    var dic = new Dictionary<string, string>
                    {
                        ["Exception"] = exception.Message,
                        ["StackTrace"] = exception.StackTrace
                    };
                    if (exception is SecurityTokenExpiredException tokenException)
                        dic.Add("Expires", tokenException.Expires.ToString(CultureInfo.InvariantCulture));

                    message = JsonConvert.SerializeObject(dic);
                }
            }
        }
    }
}

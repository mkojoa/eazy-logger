﻿using eazy.logger.EfCore;
using eazy.logger.ui.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace eazy.logger.ui
{
    public class MiddlewareExtention
    {
        private const string EmbeddedFileNamespace = "eazy.logger.ui.wwwroot.dist";
        private readonly UiOptions _options;
        private readonly StaticFileMiddleware _staticFileMiddleware;
        private readonly JsonSerializerSettings _jsonSerializerOptions;
        private readonly ILogger<MiddlewareExtention> _logger;

        public MiddlewareExtention(
            RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory,
            UiOptions options,
            ILogger<MiddlewareExtention> logger
            )
        {
            _options = options;
            _logger = logger;
            _staticFileMiddleware = CreateStaticFileMiddleware(next, hostingEnv, loggerFactory);
            _jsonSerializerOptions = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.None
            };
        }


        public async Task Invoke(HttpContext httpContext)
        {
            var httpMethod = httpContext.Request.Method;
            var path = httpContext.Request.Path.Value;

            // If the RoutePrefix is requested (with or without trailing slash), redirect to index URL
            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/{Regex.Escape(_options.RoutePrefix)}/api/logs/?$", RegexOptions.IgnoreCase))
            {
                try
                {
                    httpContext.Response.ContentType = "application/json;charset=utf-8";
                    //if (!CanAccess(httpContext))
                    //{
                    //    httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    //    return;
                    //}

                    var result = await FetchLogsAsync(httpContext);
                    httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                    await httpContext.Response.WriteAsync(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    var errorMessage = httpContext.Request.IsLocal()
                        ? JsonConvert.SerializeObject(new { errorMessage = ex.Message })
                        : JsonConvert.SerializeObject(new { errorMessage = "Internal server error" });

                    await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { errorMessage }));
                }

                return;
            }

            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/?{Regex.Escape(_options.RoutePrefix)}/?$", RegexOptions.IgnoreCase))
            {
                var indexUrl = httpContext.Request.GetEncodedUrl().TrimEnd('/') + "/index.html";
                RespondWithRedirect(httpContext.Response, indexUrl);
                return;
            }

            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/{Regex.Escape(_options.RoutePrefix)}/?index.html$", RegexOptions.IgnoreCase))
            {
                await RespondWithIndexHtml(httpContext.Response);
                return;
            }

            await _staticFileMiddleware.Invoke(httpContext);
        }


        private StaticFileMiddleware CreateStaticFileMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory)
        {
            var staticFileOptions = new StaticFileOptions
            {
                RequestPath = $"/{_options.RoutePrefix}",
                FileProvider = new EmbeddedFileProvider(typeof(MiddlewareExtention).GetTypeInfo().Assembly, EmbeddedFileNamespace),
            };

            return new StaticFileMiddleware(next, hostingEnv, Options.Create(staticFileOptions), loggerFactory);
        }


        private void RespondWithRedirect(HttpResponse response, string location)
        {
            response.StatusCode = 301;
            response.Headers["Location"] = location;
        }

        private async Task RespondWithIndexHtml(HttpResponse response)
        {
            response.StatusCode = 200;
            response.ContentType = "text/html;charset=utf-8";

            await using var stream = IndexStream();
            var htmlBuilder = new StringBuilder(await new StreamReader(stream).ReadToEndAsync());
            htmlBuilder.Replace("%(Configs)", JsonConvert.SerializeObject(
                new { _options.RoutePrefix, _options.AuthType }, _jsonSerializerOptions));

            await response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
        }

        private Func<Stream> IndexStream { get; } = () => typeof(AuthorizationOptions).GetTypeInfo().Assembly
            .GetManifestResourceStream("eazy.logger.ui.wwwroot.index.html");

        private async Task<string> FetchLogsAsync(HttpContext httpContext)
        {
            httpContext.Request.Query.TryGetValue("page", out var pageStr);
            httpContext.Request.Query.TryGetValue("count", out var countStr);
            httpContext.Request.Query.TryGetValue("level", out var levelStr);
            httpContext.Request.Query.TryGetValue("search", out var searchStr);

            int.TryParse(pageStr, out var currentPage);
            int.TryParse(countStr, out var count);
            currentPage = currentPage == default ? 1 : currentPage;
            count = count == default ? 10 : count;

            var provider = httpContext.RequestServices.GetService<IEfDataProvider>();
            var (logs, total) = await provider.FetchDataAsync(currentPage, count, levelStr, searchStr);

            //var result = JsonSerializer.Serialize(logs, _jsonSerializerOptions);
            var result = JsonConvert.SerializeObject(new { logs, total, count, currentPage }, _jsonSerializerOptions);
            return result;
        }

        //private static bool CanAccess(HttpContext httpContext)
        //{
        //    if (httpContext.Request.IsLocal())
        //        return true;

        //    var authOptions = httpContext.RequestServices.GetService<AuthorizationOptions>();
        //    if (!authOptions.Enabled)
        //        return false;

        //    if (!httpContext.User.Identity.IsAuthenticated)
        //        return false;

        //    var userName = httpContext.User.Identity.Name?.ToLower();
        //    if (authOptions.Usernames != null &&
        //        authOptions.Usernames.Any(u => u.ToLower() == userName))
        //        return true;

        //    if (authOptions.Roles != null &&
        //        authOptions.Roles.Any(role => httpContext.User.IsInRole(role)))
        //        return true;

        //    return false;
        //}

    }
}

﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace eazy.logger.ui.Helper
{
    public static class HelperExtention
    {
        public static bool IsLocal(this HttpRequest request)
        {
            var ipAddress = request.HttpContext.Request.Headers["X-forwarded-for"].FirstOrDefault();
            if (!string.IsNullOrEmpty(ipAddress))
                return false;

            var connection = request.HttpContext.Connection;
            if (connection.RemoteIpAddress != null)
            {
                return connection.LocalIpAddress != null
                    ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                    : IPAddress.IsLoopback(connection.RemoteIpAddress);
            }

            // for in memory TestServer or when dealing with default connection info
            return connection.RemoteIpAddress == null && connection.LocalIpAddress == null;
        }
    }
}

using Microsoft.AspNetCore.Http;
using System;

namespace ContactsCore.Api.Helpers
{
    public static class LocationUriHelper
    {
        public static string GetLocationUriForPost(HttpRequest request, Guid uid)
        {
            return GetLocationUri(request, uid);
        }

        public static string GetLocationUriForPut(HttpRequest request)
        {
            return GetLocationUri(request, null);
        }

        private static string GetLocationUri(HttpRequest request, Guid? uid)
        {
            var builder = new UriBuilder
            {
                Scheme = request.Scheme, 
                Host = request.Host.Host
            };            

            if (request.Host.Port != null)
                builder.Port = request.Host.Port.GetValueOrDefault();

            if (uid != null)
                builder.Path = string.Format($"{request.Path}{uid.Value:N}");
            else
                builder.Path = request.Path;
            
            return builder.Uri.ToString();
        }
    }
}

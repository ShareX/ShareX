using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.UploadersLib
{
    public class ResponseHelpers : Uploader
    {
        public static string UrlRedirectLink(string url)
        {
            if (!URLHelpers.IsValidURL(url)) return null;

            ResponseHelpers responseHelper = new ResponseHelpers();
            var response = responseHelper.GetResponse(HttpMethod.GET, url);

            if (response == null) return null;

            if (response.ResponseUri.ToString().ToLower() == url.ToLower()) return null;

            return response.ResponseUri.ToString();
        }

    }
}
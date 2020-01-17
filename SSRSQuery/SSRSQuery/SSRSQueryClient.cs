using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SSRSQuery
{
    public static class SSRSQueryClient
    {
        public static string GenerateQueryUrl( SSRSQueryOptions options )
        {
            var format = "&rs:Format=" + (options.FileType == FileType.PDF ? "PDF" : "EXCELOPENXML");
            var url = options.ServerURL + "?/" + options.ReportName + "&rs:Command=Render" + format;

            string parametersJoined = "";
            if (options.Parameters?.Count > 0)
            {
                var parametersParsed = new List<string>();
                foreach (var parameter in options.Parameters)
                {
                    string? str;
                    str = parameter.Value switch
                    {
                        null => null,
                        string value => value,
                        int value => value.ToString(),
                        short value => value.ToString(),
                        long value => value.ToString(),
                        float value => value.ToString(),
                        double value => value.ToString(),
                        decimal value => value.ToString(),
                        DateTime value => value.ToString("yyyy/MM/dd"),
                        _ => throw new Exception("Unsupported type of parameter: " + parameter.Key)
                    };

                    parametersParsed.Add(parameter.Key + "=" + (url == null ? $"{null}" : System.Web.HttpUtility.UrlEncode(str)));
                }

                parametersJoined = "&" + string.Join("&", parametersParsed);
            }

            url += parametersJoined;

            return url;
        }

        public static async Task<Stream> DownloadReport_AsStream(SSRSQueryOptions options)
        {
            var url = GenerateQueryUrl(options);
            using var handler = new System.Net.Http.HttpClientHandler()
            {
                Credentials = options.Credentials
            };
            using var client = new System.Net.Http.HttpClient(handler);

            var res = await client.GetAsync(url, options.CancellationToken);
            if( !res.IsSuccessStatusCode)
            {
                var ex = new Exception("Invalid status code:" + res.StatusCode + ". Result inline in exception data");
                ex.Data.Add("Query result", res);
                throw ex;
            }

            return await res.Content.ReadAsStreamAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SSRSQuery
{
    public class SSRSQueryOptions
    {
        public SSRSQueryOptions(string serverUrl, string reportName)
        {
            ServerURL = serverUrl;
            ReportName = reportName;
        }

        public string ServerURL { get; set; }

        public string ReportName { get; set; }

        public FileType FileType { get; set; } = FileType.PDF;

        public Dictionary<string, object>? Parameters { get; set; } = null;

        public System.Threading.CancellationToken CancellationToken { get; set; } = default;
    }
}

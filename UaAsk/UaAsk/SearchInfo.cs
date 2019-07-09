using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UaAsk
{
    class SearchInfo
    {
        public string request;
        public string[] headers;
        public string[] urls;
        public string[] adv;
        public int Count = 0;

        public SearchInfo(string request, string[] headers, string[] urls, string[] adv)
        {
            this.request = request;
            this.headers = headers;
            this.urls = urls;
            this.adv = adv;
            this.Count = headers.Length;
        }

        public SearchInfo(string request, List<string> headers, List<string> urls, List<string> adv)
        {
            this.request = request;
            this.headers = headers.ToArray();
            this.urls = urls.ToArray();
            this.adv = adv.ToArray();
            this.Count = headers.Count;
        }
    }
}

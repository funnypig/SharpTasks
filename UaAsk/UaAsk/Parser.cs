using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.IO;

namespace UaAsk
{
    class Parser
    {
        public static SearchInfo getData(string request)
        {
            string url = "https://uk.ask.com/web?o=0&l=dir&qo=serpSearchTopBox&q=" + WebUtility.UrlEncode(request);
            
            List<string> header = new List<string>();
            List<string> urls = new List<string>();
            List<string> adv = new List<string>();

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HtmlDocument html = new HtmlDocument();

                WebRequest wr = WebRequest.Create(url);

                Stream stream = wr.GetResponse().GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                
                html.LoadHtml(reader.ReadToEnd());

                var sr = html.GetElementbyId("adBlock");
                if(sr!=null)
                foreach (var t in sr.Elements("div"))
                {
                    var a = t.SelectSingleNode("a");
                    header.Add(a.InnerText.Trim());
                    urls.Add(a.Attributes["href"].Value);
                    adv.Add("+");
                }
                

                foreach (var t in html.DocumentNode.SelectNodes("//div[@class='PartialSearchResults-body']/div/div/a"))
                {
                    header.Add(t.InnerText.Trim());
                    urls.Add(t.Attributes["href"].Value);
                    adv.Add("-");
                }
            }
            catch(Exception e)
            {

            }

            return new SearchInfo(request, header, urls, adv);
        }
    }
}

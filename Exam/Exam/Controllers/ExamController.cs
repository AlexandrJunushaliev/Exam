using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace Exam.Controllers
{
    public static class Ext
    {
        public static HtmlDocument TryWebLoad(this HtmlAgilityPack.HtmlWeb web, ref bool isSuccess, string url)
        {
            try
            {
                var document = web.Load(url);
                isSuccess = true;
                return document;
            }
            catch (Exception e)
            {
                isSuccess = false;
                return null;
            }
        }
    }


    public class ExamController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ScanPage(string url, int deep)
        {
            var checkedUrls = new List<string>();
            var t = Task.Run(() => Scan(checkedUrls, deep, url));
            Task.WaitAll(t);
            var distinctedUrls = checkedUrls.Distinct();
            return View(distinctedUrls);
        }

        public async Task Save(string[] urlsSave)
        {
            var a = Request.Form;
        }


        private void Scan(List<string> checkedUrls, int deep, params string[] urls)
        {
            HtmlWeb web = new HtmlWeb();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.GetEncoding("windows-1254");
            foreach (var url in urls)
            {
                if (deep == 0)
                {
                    var isSuccess = false;

                    HtmlDocument document = web.TryWebLoad(ref isSuccess, url);
                    if (isSuccess)
                    {
                        checkedUrls.AddRange(document.DocumentNode.Descendants("a")
                            .Where(x => x.Attributes.Contains("href")).Select(x => x.Attributes["href"].Value)
                            .Where(x => !x.Contains("http")).Select(x =>"https://" + web.ResponseUri.Host + x));
                    }
                }
                else
                {
                    var isSuccess = false;
                    HtmlDocument document = web.TryWebLoad(ref isSuccess, url);
                    if (isSuccess)
                    {
                        var urlsToCheck = document.DocumentNode.Descendants("a")
                            .Where(x => x.Attributes.Contains("href"))
                            .Select(x => x.Attributes["href"].Value)
                            .Where(x => !x.Contains("http")).Select(x => "https://" + web.ResponseUri.Host + x);
                        checkedUrls.Add(url);
                        Scan(checkedUrls, deep - 1, urlsToCheck.ToArray());
                    }
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;

namespace MvcApplication1.Controllers
{
    public class ReplacedWords
    {
        public string Source { get; set; }
        public string Replace { get; set; }
    }
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private List<ReplacedWords> _replaced = new List<ReplacedWords>();
        public ActionResult Index()
        {
            var sourceString = GetHtmlCode(@"http://habrahabr.ru/company/yandex/blog/258673", Request.Url.Authority);
            return Content(sourceString);
        }

        public string ProccessUri(string uri, string authority)
        {
            var habrlink = uri.Replace(authority, "habrahabr.ru");
            return GetHtmlCode(habrlink, authority);
        }
        public void AddTradeMark(string source)
        {
            const char replacedChar = '\u2122';
            var pattern = @"[^a-zA-Zа-яА-Я\w" + replacedChar + @"]";
            var regex = new Regex(pattern);
            var words = source.Split(' ');
            foreach (var word in words)
            {
                var rWord = word;
                var cWord = regex.Replace(word, "");
                if (cWord.Length != 6) continue;
                rWord = cWord + replacedChar + word.Replace(cWord, "");
                if (_replaced.All(el => el.Source != cWord))
                    _replaced.Add(new ReplacedWords { Source = cWord, Replace = rWord });
            }
        }
        public string GetHtmlCode(string site, string authority)
        {
            var html = new HtmlDocument();
            var windows1251 = Encoding.UTF8;
            html.LoadHtml(new WebClient { Encoding = windows1251 }.DownloadString(site));
            var root = html.DocumentNode;
            var patternScript = @"<script[^>]*>[\s\S]*?</script>";
            var regex = new Regex(patternScript);
            var onlyText = regex.Replace(root.InnerHtml, "");
            var pattern = @"<(([^>])*)>";
            regex = new Regex(pattern);
            onlyText = regex.Replace(onlyText, "");
            AddTradeMark(onlyText);
            foreach (var replacedWordse in _replaced)
            {
                pattern = @"[^a-zA-Zа-яА-Я/]" + replacedWordse.Source + @"[^a-zA-Zа-яА-Я<]";
                regex = new Regex(pattern);
                root.InnerHtml = regex.Replace(root.InnerHtml, " " + replacedWordse.Replace);
                pattern = replacedWordse.Source + @"[<]";
                regex = new Regex(pattern);
                root.InnerHtml = regex.Replace(root.InnerHtml, replacedWordse.Replace+"<");
            }
            pattern = @"https://";
            regex = new Regex(pattern);
            root.InnerHtml = regex.Replace(root.InnerHtml, "http://");
            root.InnerHtml = root.InnerHtml.Replace("habrahabr.ru", authority);
            return html.DocumentNode.OuterHtml;
        }
    }
}

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using PuppeteerSharp;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace PuppeteerDownloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var options = new LaunchOptions
            {
                Headless = false,
                IgnoreHTTPSErrors = true
            };

            var browserFetcher = new BrowserFetcher();
            Console.WriteLine("Downloading Chromium...");
            var x = browserFetcher.DownloadAsync(BrowserFetcher.DefaultRevision).Result;
            Console.WriteLine("Downloaded finished!");

            Console.WriteLine("Login to Facebook using Chromium before continuing");
            Console.ReadKey();

            using (var browser = await Puppeteer.LaunchAsync(options))
            {
                using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync("https://www.facebook.com", WaitUntilNavigation.Networkidle0);
                    var cookies = await page.GetCookiesAsync("https://facebook.com");

                    List<string> serializedCookies = new List<string>();
                    foreach (var cookie in cookies)
                    {
                        serializedCookies.Add(JsonConvert.SerializeObject(cookie));
                    }
                    File.WriteAllLines("cookies.dat", serializedCookies.ToArray());
                    Console.WriteLine("Cookies saved to cookies.dat!");
                }
            }
        }
    }
}

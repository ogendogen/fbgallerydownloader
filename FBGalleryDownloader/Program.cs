using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PuppeteerSharp;

namespace TestPuppeteer
{
    class Program
    {
        public static WebClient WebClient { get; set; }

        public static Config Config { get; set; }
        static async Task Main(string[] args)
        {
            if (!File.Exists("config.json"))
            {
                Console.WriteLine("Missing config.json");
                Console.WriteLine("Exiting");

                Environment.Exit(0);
            }

            string config = File.ReadAllText("config.json");
            Config = JsonConvert.DeserializeObject<Config>(config);

            var options = new LaunchOptions
            {
                Headless = true,
                UserDataDir = Config.UserDataDirAbsolutePath,
                IgnoreHTTPSErrors = true
            };

            WebClient = new WebClient();

            string startImage = Config.StartImageUrl;

            Console.WriteLine("Downloading chromium");
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            Console.WriteLine("Navigating to start image");

            using (var browser = await Puppeteer.LaunchAsync(options))
            {
                using (var page = await browser.NewPageAsync())
                {
                    try
                    {
                        Console.WriteLine("Reloading cookies from file");
                        CookieParam[] cookies = ReadCookies().ToArray();
                        await page.DeleteCookieAsync(cookies);
                        await page.SetCookieAsync(cookies);

                        Console.WriteLine("Opening start image");
                        await page.GoToAsync(startImage, WaitUntilNavigation.Networkidle0);

                        int counter = 1;

                        while (!String.IsNullOrEmpty(await page.EvaluateExpressionAsync<string>(Config.Condition)))
                        {
                            Console.WriteLine($"Processing image {counter}");
                            string imageUrl = await GetImageUrl(page);
                            if (!String.IsNullOrEmpty(imageUrl))
                            {
                                DownloadAndSaveFile(imageUrl, Path.Combine(Config.OutputPath, $"{counter}.jpg"));
                                counter++;
                            }
                            else
                            {
                                string movieUrl = await GetMovieUrl(page);

                                if (!String.IsNullOrEmpty(movieUrl))
                                {
                                    DownloadAndSaveFile(imageUrl, Path.Combine(Config.OutputPath, $"{counter}.mp4"));
                                    counter++;
                                }
                            }

                            await ClickNextImage(page);
                            Thread.Sleep(1000);
                        }
                    }
                    catch (Exception e)
                    {
                        string exceptionData = $"Exception message: {e.Message}{Environment.NewLine}Stack Trace: {e.StackTrace}";
                        Console.WriteLine(exceptionData);
                        File.WriteAllText("error.log", exceptionData);

                        throw e;
                    }

                    Thread.Sleep(10000);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadLine();
                }
            }
        }

        private static IEnumerable<CookieParam> ReadCookies()
        {
            if (!File.Exists("cookies.dat"))
            {
                throw new Exception("There is no cookies file :(");
            }

            string[] serializedCookies = File.ReadAllLines("cookies.dat");
            foreach (var cookie in serializedCookies)
            {
                yield return JsonConvert.DeserializeObject<CookieParam>(cookie);
            }
        }

        private static async Task<string> GetImageUrl(Page page)
        {
            try
            {
                return await page.EvaluateExpressionAsync<string>(Config.ImageUrlSelector);
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static async Task<string> GetMovieUrl(Page page)
        {
            try
            {
                return await page.EvaluateExpressionAsync<string>(Config.MovieUrlSelector);
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static async Task ClickNextImage(Page page)
        {
            await page.EvaluateExpressionAsync(Config.NextImageButtonClick);
        }

        private static void DownloadAndSaveFile(string url, string path)
        {
            WebClient.DownloadFile(new Uri(url), path);
        }
    }
}

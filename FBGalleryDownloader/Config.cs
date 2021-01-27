using System;
using System.Collections.Generic;
using System.Text;

namespace TestPuppeteer
{
    public class Config
    {
        public string OutputPath { get; set; }
        public string UserDataDirAbsolutePath { get; set; }
        public string NextImageButtonClick { get; set; }
        public string Condition { get; set; }
        public string ImageUrlSelector { get; set; }
        public string MovieUrlSelector { get; set; }
        public string StartImageUrl { get; set; }
        public string CookiesPath { get; set; }
        public int CounterStart { get; set; } = 1;
    }
}

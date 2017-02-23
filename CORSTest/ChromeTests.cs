using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CORSTest
{
    public class ChromeTests : IUseFixture<ChromeFixture>
    {
        ChromeDriver driver;

        public void SetFixture(ChromeFixture data)
        {
            driver = data.GetDriver();
        }

        [Fact]
        public void ProjectsReturnedToWebPage()
        {
            string filePath = Path.Combine(Environment.GetEnvironmentVariable("APPVEYOR_BUILD_FOLDER"), "index.html");

            File.WriteAllText(filePath, File.ReadAllText(filePath).Replace("_token_placeholder_", Environment.GetEnvironmentVariable("API_TOKEN")));

            driver.Navigate().GoToUrl((new Uri(filePath)).AbsoluteUri);

            string resultText = "";
            DateTime end = DateTime.Now + TimeSpan.FromSeconds(20);
            while (resultText == "" && DateTime.Now < end)
            {
                try
                {
                    resultText = driver.FindElementByTagName("p").Text;
                }
                catch (Exception)
                {
                    Thread.Sleep(500);
                } 
            }

            Assert.True(resultText != "", "Could not get result from Chrome");
            Assert.Contains("TotalProjects=", resultText, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotThrow(() => int.Parse(resultText.Substring(resultText.LastIndexOf("=") + 1)));
        }
    }

    public class ChromeFixture : IDisposable
    {
        ChromeDriver driver;

        public ChromeFixture()
        {
            driver = new ChromeDriver(@"C:\tools\selenium");
        }

        public ChromeDriver GetDriver()
        {
            return driver;
        }

        public void Dispose()
        {
            driver.Quit();
        }
    }
}

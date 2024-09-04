using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Diagnostics;
// using System.Drawing;
using System.IO;
 
namespace Publix

{
    public class Browser_ops
    {
        IWebDriver webDriver;
        public void Init_Browser()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo() {
                FileName = "/opt/chrome-linux64/chrome",
                Arguments = "--version",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            Process proc = new Process() { StartInfo = startInfo, };
            proc.Start();
            string chromeMajorVersion = proc.StandardOutput.ReadToEnd().Split('.').First().Split(' ').Last();
            proc.WaitForExit();

            string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/"
                             + chromeMajorVersion
                             + ".0.0.0 Safari/537.36";

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--headless", "--disable-gpu", "--no-sandbox",
                "--disable-dev-shm-usage", "--disable-blink-features=AutomationControlled",
                $"--user-agent={userAgent}");

            var chromeService = ChromeDriverService.CreateDefaultService();
            chromeService.SuppressInitialDiagnosticInformation = true;

            webDriver = new ChromeDriver(chromeService,chromeOptions);
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
            webDriver.Manage().Window.Maximize();
        }

        public void Goto(string url)
        {
            webDriver.Url = url;
        }

        public void Close()
        {
            webDriver.Quit();
        }

        public IWebDriver getDriver
        {
            get { return webDriver; }
        }
    }
    class Tests
    {
        private static readonly string LOGIN_USER = Environment.GetEnvironmentVariable("LOGIN_USER");
        private static readonly string LOGIN_PASS = Environment.GetEnvironmentVariable("LOGIN_PASS");

        Browser_ops browser = new Browser_ops();
        String test_url = "https://www.publix.com";
        string test_root = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        string epoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        IWebDriver driver;
 
        [SetUp]
        public void start_Browser()
        {
            browser.Init_Browser();
            driver = browser.getDriver;
        }
 
        [Test]
        public void verifyLogo()
        {
            browser.Goto(test_url);

            IWebElement Logo = driver.FindElement(By.CssSelector("img[alt='Publix']"));
            Assert.IsTrue(Logo.Displayed);
        }
 
        [Test]
        public void verifyTouText()
        {
            browser.Goto(test_url);
 
            IWebElement TermsOfUse = driver.FindElement(By.XPath("//*[@class='tou-cookie-bar']/div/div/span"));
            Assert.IsTrue(TermsOfUse.Text.Contains("By using the Services, you agree to our terms,"));
            Assert.IsTrue(TermsOfUse.Text.Contains("including our use of cookies and tracking technologies."));
        }

        [Test]
        public void verifyFlyOut()
        {
            browser.Goto(test_url);
 
            IWebElement FlyOut = driver.FindElement(By.CssSelector("div.club-publix-flyout"));
            Assert.IsTrue(FlyOut.Displayed);

            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile(test_root + @"/Publix_frontpage_" + epoch + ".jpg");
        }

        [Test]
        public void verifyLogin()
        {
            browser.Goto(test_url);
 
            IWebElement LogIn = driver.FindElement(By.Id("userLogIn"));
            string LogInHref = LogIn.GetAttribute("href");

            browser.Goto(LogInHref);

            IWebElement Username = driver.FindElement(By.Id("signInName"));
            IWebElement Password = driver.FindElement(By.Id("password"));
            IWebElement LoginButton = driver.FindElement(By.Id("next"));
            Username.SendKeys(LOGIN_USER);
            Password.SendKeys(LOGIN_PASS);
            LoginButton.Click();

            System.Threading.Thread.Sleep(10000);

            /* Alternate login attempts 
            Password.SendKeys(Keys.Enter);

            IWebElement LoginButton = driver.FindElement(By.Id("next"));

            LoginButton.SendKeys(Keys.Tab);
            LoginButton.SendKeys(Keys.Enter);

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click();", LoginButton);

            Actions login = new Actions(driver).MoveToElement(LoginButton).Pause(TimeSpan.FromSeconds(3));
            login.Click().Perform();
            */

            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile(test_root + @"/Publix_loginpage_" + epoch + ".jpg");

            /* Uncomment when bot detection is beaten
            wait.Until(ExpectedConditions.ElementExists(By.Id("userAccount")));
            IWebElement UserAccount = driver.FindElement(By.Id("userAccount"));
            UserAccount.Click();

            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[class='club-publix-sidebar']")));

            Screenshot screenshot2 = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot2.SaveAsFile(test_root+@"/Publix_accountpage.jpg");
            */
        }

        [TearDown]
        public void close_Browser()
        {
            browser.Close();
        }
    }
}

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
            // Save Chrome major version so we can plug it into the useragent override
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

            // Get screen resolution from environment
            string screenHeight = Environment.GetEnvironmentVariable("SCREEN_HEIGHT");
            string screenWidth  = Environment.GetEnvironmentVariable("SCREEN_WIDTH");

            // Set up Chromedriver and options for automated testing
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--disable-gpu", "--no-sandbox", "--disable-dev-shm-usage",
                "--disable-blink-features=AutomationControlled", $"--user-agent='{userAgent}'",
                $"--window-size={screenWidth},{screenHeight}");
            chromeOptions.AddLocalStatePreference("prefs",
                new { enabled_labs_experiments = new string[] { "profile.managed_default_content_settings.javascript@2" }});

            var chromeService = ChromeDriverService.CreateDefaultService();
            chromeService.SuppressInitialDiagnosticInformation = true;

            webDriver = new ChromeDriver(chromeService,chromeOptions);
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
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
        // Add some test resources and variables
        int randsleep = new Random().Next(2,6);

        private static readonly string LOGIN_USER = Environment.GetEnvironmentVariable("LOGIN_USER");
        private static readonly string LOGIN_PASS = Environment.GetEnvironmentVariable("LOGIN_PASS");

        Browser_ops browser = new Browser_ops();
        String test_url = "https://www.publix.com";
        string test_root = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        string test_time = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        IWebDriver driver;
 
        // Initialize browser and run tests
        [SetUp]
        public void start_Browser()
        {
            browser.Init_Browser();
            driver = browser.getDriver;
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("Object.defineProperty(navigator, 'platform', {get: () => 'Win64'})");
        }
 
        [Test]
        public void verify_01Logo()
        {
            browser.Goto(test_url);

            IWebElement Logo = driver.FindElement(By.CssSelector("img[alt='Publix']"));
            Assert.IsTrue(Logo.Displayed);
        }
 
        [Test]
        public void verify_02TouText()
        {
            browser.Goto(test_url);
 
            IWebElement TermsOfUse = driver.FindElement(By.XPath("//*[@class='tou-cookie-bar']/div/div/span"));
            Assert.IsTrue(TermsOfUse.Text.Contains("By using the Services, you agree to our terms,"));
            Assert.IsTrue(TermsOfUse.Text.Contains("including our use of cookies and tracking technologies."));
        }

        [Test]
        public void verify_03FlyOut()
        {
            browser.Goto(test_url);
 
            IWebElement FlyOut = driver.FindElement(By.CssSelector("div.club-publix-flyout"));
            Assert.IsTrue(FlyOut.Displayed);

            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile(test_root + @"/" + test_time + "_Publix_frontpage.jpg");
        }

        [Test]
        public void verify_04Login()
        {
            browser.Goto(test_url);
 
            IWebElement LogIn = driver.FindElement(By.Id("userLogIn"));
            string LogInHref = LogIn.GetAttribute("href");

            browser.Goto(LogInHref);

            IWebElement userName = driver.FindElement(By.Id("signInName"));
            IWebElement passWord = driver.FindElement(By.Id("password"));
            IWebElement loginButton = driver.FindElement(By.Id("next"));

            System.Threading.Thread.Sleep(5000);

            Actions Username = new Actions(driver).MoveToElement(userName).Pause(TimeSpan.FromSeconds(randsleep));
            Username.Click().Perform();
            userName.SendKeys(LOGIN_USER);
            System.Threading.Thread.Sleep(randsleep);

            Actions Password = new Actions(driver).MoveToElement(passWord).Pause(TimeSpan.FromSeconds(randsleep));
            Password.Click().Perform();
            passWord.SendKeys(LOGIN_PASS);
            System.Threading.Thread.Sleep(randsleep);

            Actions Login = new Actions(driver).MoveToElement(loginButton).Pause(TimeSpan.FromSeconds(randsleep));
            Login.Click().Perform();

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
            screenshot.SaveAsFile(test_root + @"/" + test_time + "_Publix_loginpage.jpg");

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

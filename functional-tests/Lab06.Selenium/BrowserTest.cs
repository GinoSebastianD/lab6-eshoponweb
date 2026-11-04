using System.Diagnostics;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Lab06.Selenium;

public abstract class BrowserTest : IDisposable
{
    protected const string Origin = "https://localhost:5001";
    protected readonly ChromeDriver browser;

    protected BrowserTest()
    {
        var options = new ChromeOptions { AcceptInsecureCertificates = true };
        options.AddArguments("--headless=new", "--window-size=1440,1050", "--lang=en-US");
        var driverDir = Environment.GetEnvironmentVariable("CHROMEDRIVER_DIR");
        var service = driverDir is null ? ChromeDriverService.CreateDefaultService() : ChromeDriverService.CreateDefaultService(driverDir);
        browser = new ChromeDriver(service, options);
    }

    protected void Capture(string name)
    {
        var dir = Environment.GetEnvironmentVariable("LAB06_CAPTURAS");
        if (dir is null) return;
        Directory.CreateDirectory(dir);
        browser.GetScreenshot().SaveAsFile(Path.Combine(dir, name + ".png"));
    }

    protected static void Wait(Func<bool> ready, string action)
    {
        var timer = Stopwatch.StartNew();
        while (timer.Elapsed < TimeSpan.FromSeconds(25))
        {
            try { if (ready()) return; } catch (StaleElementReferenceException) { }
            Thread.Sleep(120);
        }
        throw new TimeoutException(action);
    }

    protected void SubmitPage(IWebElement submit)
    {
        var oldPage = browser.FindElement(By.TagName("html"));
        submit.Click();
        Wait(() =>
        {
            try { _ = oldPage.TagName; return false; }
            catch (StaleElementReferenceException) { return true; }
        }, "wait for the submitted page to be replaced");
        Wait(() => Equals(browser.ExecuteScript("return document.readyState"), "complete"), "finish loading response");
    }

    public void Dispose() => browser.Dispose();
}

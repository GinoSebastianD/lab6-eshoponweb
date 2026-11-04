using System.Diagnostics;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace Lab06.Selenium;

public abstract class AdminTest : BrowserTest
{
    protected IWebElement Dialog => browser.FindElements(By.CssSelector(".modal")).Single(m => m.Displayed);
    protected IReadOnlyCollection<IWebElement> Rows => browser.FindElements(By.CssSelector("tbody tr"));
    protected IWebElement Button(string text) => browser.FindElement(By.XPath($"//button[normalize-space()='{text}']"));


    protected AdminTest()
    {
        try
        {
            browser.Navigate().GoToUrl(Origin + "/Identity/Account/Login");
            browser.FindElement(By.Id("Input_Email")).SendKeys("productmgr@microsoft.com");
            // Public demo account seeded only for the local lab.
            browser.FindElement(By.Id("Input_Password")).SendKeys("Pass@word1");
            browser.FindElement(By.CssSelector("form[method=post] button[type=submit]")).Click();
            Wait(() => !browser.Url.Contains("/Account/Login"), "sign in");
            browser.Navigate().GoToUrl(Origin + "/admin");
            Wait(() => Rows.Count > 0, "load product catalog");
        }
        catch { browser.Dispose(); throw; }
    }
    protected bool Closed() => !browser.FindElements(By.CssSelector(".modal")).Any(m => m.Displayed);
    protected IWebElement Row(string name) =>
        Rows.Single(r => r.FindElements(By.TagName("td"))[4].Text == name);
    protected void OpenAction(string name, string action)
    {
        Row(name).FindElement(By.XPath($".//button[normalize-space()='{action}']")).Click();
        Wait(() => !Closed(), "open " + action);
    }
    protected void ModalButton(string text) =>
        Dialog.FindElement(By.XPath($".//button[normalize-space()='{text}']")).Click();
}

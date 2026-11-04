using System.Diagnostics;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace Lab06.Selenium;

public sealed class BasketQuantityTests : BrowserTest
{
    private void SetQuantity(string quantity)
    {
        browser.Navigate().GoToUrl(Origin);
        SubmitPage(browser.FindElement(By.XPath("//form[.//span[text()='Roslyn Red Sheet']]//input[@type='submit']")));
        var input = browser.FindElement(By.CssSelector(".esh-basket-input"));
        input.SendKeys(Keys.Control + "a");
        input.SendKeys(quantity + Keys.Tab);
        SubmitPage(browser.FindElement(By.Name("updatebutton")));
    }
    [Fact] public void CA01_DosUnidades()
    {
        SetQuantity("2");
        Assert.Equal("2", browser.FindElement(By.CssSelector(".esh-basket-input")).GetDomProperty("value"));
        Assert.Equal("$ 17.00", browser.FindElements(By.CssSelector(".esh-basket-item--mark")).Last().Text);
        Capture("08-carrito");
    }
    [Fact] public void CA02_RetirarArticulo()
    {
        SetQuantity("0");
        Assert.Contains("Basket is empty.", browser.FindElement(By.TagName("h3")).Text);
        Assert.Empty(browser.FindElements(By.CssSelector(".esh-basket-input")));
    }
}

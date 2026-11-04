using System.Diagnostics;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace Lab06.Selenium;

public sealed class CatalogFilterTests : BrowserTest
{
    private string[] Filter(string brand, string type)
    {
        browser.Navigate().GoToUrl(Origin);
        foreach (var (label, value) in new[] { ("brand",brand), ("type",type) })
            if (value.Length > 0)
                browser.FindElement(By.XPath($"//label[@data-title='{label}']//option[normalize-space()='{value}']")).Click();
        SubmitPage(browser.FindElement(By.CssSelector(".esh-catalog-send")));
        return browser.FindElements(By.CssSelector(".esh-catalog-name")).Select(e => e.Text).Order().ToArray();
    }
    [Fact] public void FI01_FiltrarPorTipo()
    {
        Assert.Equal(new[] { ".NET BLACK & WHITE MUG", "CUP<T> WHITE MUG" }, Filter("", "Mug"));
    }
    [Fact] public void FI02_CombinarMarcaYTipo()
    {
        Assert.Equal(new[] { ".NET BLACK & WHITE MUG" }, Filter(".NET", "Mug"));
        Capture("07-filtros");
    }
    [Fact] public void FI03_SinCoincidencias()
    {
        Assert.Empty(Filter("Azure", "Mug"));
        Assert.Contains("NO RESULTS", browser.FindElement(By.CssSelector(".esh-catalog-items")).Text);
    }
}

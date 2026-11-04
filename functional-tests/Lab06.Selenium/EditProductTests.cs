using System.Diagnostics;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace Lab06.Selenium;

public sealed class EditProductTests : AdminTest
{
    private const string Item = ".NET Black & White Mug";
    private void SetPrice(string value)
    {
        var price = Dialog.FindElement(By.CssSelector("input[type=number]"));
        price.SendKeys(Keys.Control + "a");
        price.SendKeys(value + Keys.Tab);
    }
    [Fact] public void ED01_GuardarPrecio()
    {
        OpenAction(Item, "Edit");
        SetPrice("15.75");
        ModalButton("Save");
        Wait(() => Closed() && Row(Item).FindElements(By.TagName("td"))[6].Text == "15.75", "verify saved price");
        Capture("05-edicion");
    }
    [Fact] public void ED02_CancelarPrecio()
    {
        var before = Row(Item).FindElements(By.TagName("td"))[6].Text;
        OpenAction(Item, "Edit");
        SetPrice("28.50");
        ModalButton("Cancel");
        Wait(Closed, "cancel edit");
        Assert.Equal(before, Row(Item).FindElements(By.TagName("td"))[6].Text);
    }
}

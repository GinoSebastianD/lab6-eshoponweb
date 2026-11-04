using System.Diagnostics;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace Lab06.Selenium;

public sealed class CreateProductTests : AdminTest
{
    private readonly string product = $"Lab06-{Guid.NewGuid():N}"[..16];
    private const string Description = "Producto de prueba";
    private const string RangeError = "between 0.01 and 1000";
    public CreateProductTests()
    {
        Button("Create New").Click();
        Wait(() => !Closed(), "open form");
    }
    [Fact] public void CP01_PrecioHabitual() => Accepted("12.50", "CP01");
    [Fact] public void CP02_PrecioMinimo() => Accepted("0.01", "CP02");
    [Fact] public void CP03_PrecioMaximo() => Accepted("1000", "CP03");
    [Fact] public void CP04_PrecioCero() => Rejected(product, Description, "0", RangeError, "CP04");
    [Fact] public void CP05_SuperaMaximo() => Rejected(product, Description, "1000.01", RangeError, "CP05");
    [Fact] public void CP06_TresDecimales() => Rejected(product, Description, "12.345", "maximum two decimals", "CP06");
    [Fact] public void CP07_NombreObligatorio() => Rejected("", Description, "12.50", "The Name field is required", "CP07");
    [Fact] public void CP08_DescripcionObligatoria() => Rejected(product, "", "12.50", "The Description field is required", "CP08");

    [Fact]
    public void CP09_Cancelar()
    {
        var before = Rows.Count;
        Fill(product, Description, "12.50");
        Dialog.FindElement(By.XPath(".//button[normalize-space()='Cancel']")).Click();
        Wait(Closed, "close form");
        Assert.Equal(before, Rows.Count);
        Assert.DoesNotContain(Rows, r => r.Text.Contains(product));
    }

    private void Fill(string name, string description, string price)
    {
        var inputs = Dialog.FindElements(By.CssSelector("input"));
        foreach (var (input, value) in inputs.Zip(new[] { name, description, price }))
        {
            input.SendKeys(Keys.Control + "a");
            input.SendKeys(Keys.Backspace);
            if (value.Length > 0) input.SendKeys(value);
        }
        inputs.Last().SendKeys(Keys.Tab);
    }

    private void Submit() => Dialog.FindElement(By.CssSelector("button[type=submit]")).Click();

    private void Accepted(string price, string id)
    {
        Fill(product, Description, price);
        var chosen = Dialog.FindElements(By.CssSelector("select option:checked")).Select(e => e.Text).ToArray();
        if (id == "CP01") Capture("01-formulario");
        Submit();
        Wait(() => Closed() && Rows.Any(r => r.Text.Contains(product)), "show new catalog row");
        var row = Assert.Single(Rows, r => r.Text.Contains(product));
        var cells = row.FindElements(By.TagName("td"));
        Assert.Equal(chosen[0], cells[2].Text);
        Assert.Equal(chosen[1], cells[1].Text);
        Assert.Equal(product, cells[4].Text);
        Assert.Equal(Description, cells[5].Text);
        Assert.Equal(decimal.Parse(price, CultureInfo.InvariantCulture), decimal.Parse(cells[6].Text, CultureInfo.InvariantCulture));
        if (id == "CP01")
        {
            browser.ExecuteScript("arguments[0].scrollIntoView({block:'center'})", row);
            Capture("02-producto-creado");
        }
    }

    private void Rejected(string name, string description, string price, string message, string id)
    {
        var before = Rows.Count;
        Fill(name, description, price);
        Submit();
        Wait(() => !Closed() && Dialog.Text.Contains(message), "display validation: " + message);
        Assert.Contains(message, Dialog.Text);
        Assert.Equal(before, Rows.Count);
        if (id == "CP04") Capture("03-precio-invalido");
    }

}

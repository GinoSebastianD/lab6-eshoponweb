using System.Diagnostics;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace Lab06.Selenium;

public sealed class DeleteProductTests : AdminTest
{
    [Fact] public void EL01_ConfirmarEliminacion()
    {
        const string name = ".NET Blue Sweatshirt";
        var before = Rows.Count;
        OpenAction(name, "Delete");
        Assert.Contains(name, Dialog.Text);
        Capture("06-confirmacion");
        ModalButton("Delete");
        Wait(() => Closed() && Rows.Count == before - 1, "delete one item");
        Assert.DoesNotContain(Rows, row => row.Text.Contains(name));
    }
    [Fact] public void EL02_CancelarEliminacion()
    {
        const string name = "Kudu Purple Sweatshirt";
        var before = Rows.Count;
        OpenAction(name, "Delete");
        ModalButton("Cancel");
        Wait(Closed, "cancel deletion");
        Assert.Equal(before, Rows.Count);
        Assert.True(Row(name).Displayed);
    }
}

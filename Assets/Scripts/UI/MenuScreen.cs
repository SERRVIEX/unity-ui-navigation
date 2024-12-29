using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : ScreenController
{
    public void ShowAlert()
    {
        var alert = Alert.Present();
        var btn1 = new AlertDefaultButtonData();
        btn1.Label = "Cancel";
        btn1.OnClick = () => alert.Dismiss();

        var btn2 = new AlertDefaultButtonData();
        btn1.Label = "OK";
        btn2.OnClick = () => alert.Dismiss();

        alert.UpdateContent("This is an alert", true, btn1, btn2);
    }

    public void ShowToast()
    {
        Toast.Present("toast", "This is a toast message");
    }

    public async void ShowLoading()
    {
        Loading.Present("Loading for 2 seconds");

        await Task.Delay(2000);

        Loading.Release();
    }
}

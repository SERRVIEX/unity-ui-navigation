using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : ScreenController
{
    public void ShowAlert()
    {
        var alert = Alert.Present();

        var ok = new AlertLabelButtonData();
        ok.Label = "OK!";
        ok.OnClick += () =>
        {
            alert.Dismiss();
        };

        var good = new AlertLabelButtonData();
        good.Label = "Good";
        good.OnClick += () =>
        {
            alert.Dismiss();
        };

        var cancel = new AlertLabelButtonData();
        cancel.Label = "Cancel";
        cancel.OnClick += () =>
        {
            alert.Dismiss();
        };

        alert.UpdateContent("This is my alert", false, ok, good, cancel);
    }

    public void ShowToast()
    {
        Toast.Present("kjqwd", "This is my toast message");
    }

    public async void ShowLoading()
    {
        Loading.Present("My loading message");

        await Task.Delay(1000);

        Loading.Release();
    }

    public override ScreenTransition GetPresentTransition()
    {
        return new PushSwitchScreenTransition(this);
    }

    public override ScreenTransition GetDismissTransition()
    {
        return new DismissSwitchScreenTransition(this);
    }
}

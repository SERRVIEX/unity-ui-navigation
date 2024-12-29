using UnityEngine;
using UnityEngine.UI;

public class MyTestScreen : ScreenController
{
    public void Popup()
    {
        Navigator.Present<MyPopupScreen>();
    }

    public void Settings()
    {
        Navigator.Present<SettingsScreen>();
    }
}

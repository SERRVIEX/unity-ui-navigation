using UnityEngine;
using UnityEngine.UI;

public class MyPopupScreen : ScreenController
{
    public override ScreenTransition GetPresentTransition()
    {
        return new PushRotationTransition(this);
    }

    public override ScreenTransition GetDismissTransition()
    {
        return new DismissAlertTransition(this);
    }
}

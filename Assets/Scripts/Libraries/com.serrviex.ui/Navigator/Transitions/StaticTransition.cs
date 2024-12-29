namespace UnityEngine.UI
{
    public class StaticTransition : ScreenTransition
    {
        public override float Duration => 0;

        public StaticTransition(ScreenController target) : base(target) { }
    }
}

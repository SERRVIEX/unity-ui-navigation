namespace UnityEngine.UI
{
    public abstract class ScreenTransition
    {
        public ScreenController Target { get; private set; }

        public virtual float Duration { get; } = .35f;

        public ScreenTransition(ScreenController target)
        {
            Target = target;
        }

        public virtual void OnBegin() { }

        public virtual void Animate(float value) { }

        public virtual void OnEnd() { }
    }
}
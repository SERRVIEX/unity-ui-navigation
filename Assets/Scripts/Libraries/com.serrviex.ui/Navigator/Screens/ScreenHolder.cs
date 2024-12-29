namespace UnityEngine.UI
{
    using System;
    
    public sealed class ScreenHolder
    {
        public bool Unloaded;
        public Type Type;
        public ScreenStates States;
        public ScreenController Controller;

        public bool IsAvailable => Controller != null;

        public ScreenHolder(ScreenController controller)
        {
            States = new ScreenStates();
            Type = controller.GetType();
            Controller = controller;
        }

        public ScreenController Take()
        {
            ScreenController view = Controller;
            Controller = null;
            return view;
        }

        public void Return(ScreenController view)
        {
            Controller = view;
        }

        public void Dispose()
        {
            States.Dispose();
            if(Controller != null)
                UnityEngine.Object.Destroy(Controller.gameObject);
        }
    }
}
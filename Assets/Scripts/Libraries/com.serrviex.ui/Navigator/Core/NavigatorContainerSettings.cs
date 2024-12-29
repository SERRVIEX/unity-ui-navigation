namespace UnityEngine.UI
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;

    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    using Object = UnityEngine.Object;

    public sealed class NavigatorContainerSettings : ScriptableObject
    {
        [SerializeField] private List<string> _screens = new List<string>();

        public T GetViewController<T>() where T : ScreenController
        {
            var type = typeof(T).FullName;
            return Resources.Load<T>($"Screens/{type}");
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            var viewControllerTypes = new List<Type>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                var types = assembly.GetTypes()
                    .Where(type => type.IsSubclassOf(typeof(ScreenController)))
                    .ToArray();

                viewControllerTypes.AddRange(types);
            }

            var viewControllerScripts = Resources.FindObjectsOfTypeAll<MonoScript>()
                .Where(script => script != null && viewControllerTypes.Contains(script.GetClass()))
                .Cast<Object>()
                .ToList();

            // Remove view controllers that does not exist anymore.
            for (int i = _screens.Count - 1; i >= 0; i--)
            {
                // Convert string to a type.
                var currentType = Type.GetType(_screens[i]);

                var found = false;
                for (int j = viewControllerScripts.Count - 1; j >= 0; j--)
                {
                    var targetType = viewControllerScripts[j].GetType();
                    if(currentType == targetType)
                    {
                        found = true;
                        viewControllerScripts.RemoveAt(j);
                        break;
                    }
                }

                if(!found)
                    _screens.RemoveAt(i);
            }

            for (int i = 0; i < viewControllerScripts.Count; i++)
            {
                var name = viewControllerTypes[i].FullName;
                var viewController = Resources.Load<ScreenController>($"Screens/{name}");
                if(viewController != null)
                    _screens.Add(name);
                else
                    Debug.LogWarning($"Type '{name}' does not have a prefab or its location is not under: Resources/Screens.");
            }

            EditorUtility.SetDirty(this);
        }
#endif
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

namespace PluginTaskbar
{
    class PluginTaskbar
    {
        // INCLUDE THIS CLASS IN YOUR PROJECT AND FOLLOW THE INSTRUCTIONS
        // REFERENCE EXMAPLES IN THE REGION BELOW

        #region TaskbarHooker HELPER CLASS

        public interface ITaskbarModule
        {
            Texture ToolbarIcon();
            void Clicked(bool leftClick);
            string TooltipText();
        }

        public class TaskbarHooker
        {
            private ITaskbarModule m_Module;
            private Callback<Callback<Texture>, bool> m_IconUpdate;
            private Callback<bool> m_Clicked = null;
            private static string m_ModuleName = "";

            public static bool Hooked
            {
                get { return PluginTaskbar.isHooked; }
            }

            public TaskbarHooker(ITaskbarModule module, string moduleName)
            {
                m_Module = module;
                m_ModuleName = moduleName;
                m_IconUpdate = new Callback<Callback<Texture>, bool>(updateIcon);
                if(m_Clicked == null)
                    m_Clicked = new Callback<bool>(module.Clicked);
            }

            private void updateIcon(Callback<Texture> callback, bool clicked)
            {
                //return your icon
                callback.Invoke(m_Module.ToolbarIcon());
            }

            public bool Start()
            {
                PluginTaskbar.UnhookFromTaskbar(m_ModuleName);
                return PluginTaskbar.HookToTaskbar(m_Clicked, m_IconUpdate, m_ModuleName);
            }

            public bool Stop()
            {
                return PluginTaskbar.UnhookFromTaskbar(m_ModuleName);
            }
        }

        #endregion

        #region Reflection Method Discovery Definitions

        // Arguments for TaskBar.Hook(Callback function, Callback<Callback<Texture>, bool>)
        private static Type[] args = { typeof(Callback<bool>), typeof(Callback<Callback<Texture>, bool>), typeof(string) };
        
        // Arguments for Taskbar.Unhook(Callback function)
        private static Type[] uargs = { typeof(string) };
        
        #endregion

        private static bool isHooked = false;

        #region TaskBar Discovery and Hooking

        // Checks for the plugin
        public static bool TaskbarLoaded()
        {
            GameObject go = null;
            
            if ((go = GameObject.Find("PluginTaskbarImmortal")) == null)
                return false;

            if (go.GetComponent("TaskBar") == null)
                return false;

            return true;
        }

        // Hooks your callback to the TaskBar
        public static bool HookToTaskbar(Callback<bool> function, Callback<Callback<Texture>, bool> callback, string moduleName)
        {
            if (!TaskbarLoaded())
                return false;

            if (isHooked)
                return true;

            GameObject go = GameObject.Find("PluginTaskbarImmortal");

            object o = GetTaskbar();

            Type t = o.GetType();

            
            MethodInfo m = t.GetMethod("Hook", args);
            
            if (m != null && o != null)
            {
                if ((bool)m.Invoke(o, new object[] { function, callback, moduleName }))
                {
                    Debug.Log("HOOKTOTASKBAR TRUE");
                    isHooked = true;
                    return true;
                }
            }
            else if (m == null)
            {
                Debug.Log("HOOKTOTASKBAR FALSE MethodInfo null");
            }
            else if (o == null)
            {
                Debug.Log("HOOKTOTASKBAR FALSE object null");
            }
            isHooked = false;
            return false;
        }

        // Unhooks your callback from the TaskBar
        public static bool UnhookFromTaskbar(string moduleName)
        {
            if (!TaskbarLoaded())
                return false;

            if (!isHooked)
                return false;

            object o = GetTaskbar();

            Type t = o.GetType();

            MethodInfo m = t.GetMethod("Unhook", uargs);

            if (m != null)
            {
                if ((bool)m.Invoke(o, new object[] { moduleName }))
                {
                    isHooked = false;
                    return true;
                }
            }
            isHooked = false;
            return false;
        }

        public static bool Hooked()
        {
            return isHooked;
        }

        // Returns the current TaskBar instance
        public static object GetTaskbar()
        {
            return (object)GameObject.Find("PluginTaskbarImmortal").GetComponent("TaskBar");
        }

        #endregion
    }
}

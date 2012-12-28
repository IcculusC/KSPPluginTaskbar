/*
    KSP Plugin Taskbar
  
    Copyright (C) 2012  Leath Cooper

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

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
        // IN THE README FOR USING THE HELPER CLASS

        #region TaskbarHooker HELPER CLASS

        public interface ITaskbarModule
        {
            Texture TaskbarIcon();
            void TaskbarClicked(bool leftClick);
            void TaskbarHover(Vector3 mousePosition);
            string TaskbarTooltip();
            void TaskbarDraw(Rect buttonRect, bool visible);
        }

        public class TaskbarHooker
        {
            private ITaskbarModule m_Module;
            private Callback<Callback<Texture>, bool> m_IconUpdate;
            private Callback<bool> m_Clicked = null;
            private Callback<Vector3> m_Hover = null;
            private Callback<Callback<string>> m_TooltipText = null;
            private Callback<Rect, bool> m_Draw = null;
            private string m_ModuleName;

            public static bool Hooked
            {
                get { return PluginTaskbar.isHooked; }
            }

            public TaskbarHooker(ITaskbarModule module, string moduleName)
            {
                Debug.Log(String.Format("CREATING HOOK FOR MODULE: {0}", moduleName));
                m_Module = module;
                m_ModuleName = moduleName;
                m_IconUpdate = new Callback<Callback<Texture>, bool>(updateIcon);
                m_Clicked = new Callback<bool>(module.TaskbarClicked);
                m_Hover = new Callback<Vector3>(module.TaskbarHover);
                m_TooltipText = new Callback<Callback<string>>(updateTooltip);
                m_Draw = new Callback<Rect, bool>(module.TaskbarDraw);
            }

            private void updateIcon(Callback<Texture> callback, bool clicked)
            {
                //return your icon
                callback.Invoke(m_Module.TaskbarIcon());
            }

            private void updateTooltip(Callback<string> tooltip)
            {
                tooltip.Invoke(m_Module.TaskbarTooltip());
            }
                        
            public bool Start()
            {
                Debug.Log(String.Format("STARTING MODULE: {0}", m_ModuleName));
                PluginTaskbar.UnhookFromTaskbar(m_ModuleName);
                return PluginTaskbar.HookToTaskbar(m_Clicked, m_IconUpdate, m_Hover, m_Draw, m_TooltipText, m_ModuleName);
            }

            public bool Stop()
            {
                Debug.Log(String.Format("STOPPING MODULE: {0}", m_ModuleName));
                return PluginTaskbar.UnhookFromTaskbar(m_ModuleName);
            }
        }

        #endregion

        #region Reflection Method Discovery Definitions

        // Arguments for TaskBar.Hook(Callback<bool>, Callback<Callback<Texture>, bool>, string)
        private static Type[] args = { typeof(Callback<bool>), typeof(Callback<Callback<Texture>, bool>), typeof(Callback<Vector3>), typeof(Callback<Rect, bool>), typeof(Callback<Callback<string>>), typeof(string) };
        
        // Arguments for Taskbar.Unhook(string)
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
        public static bool HookToTaskbar(Callback<bool> function, Callback<Callback<Texture>, bool> callback, Callback<Vector3> hover, Callback<Rect, bool> draw, Callback<Callback<string>> tooltip, string moduleName)
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
                if ((bool)m.Invoke(o, new object[] { function, callback, hover, draw, tooltip, moduleName }))
                {
                    //Debug.Log("HOOKTOTASKBAR TRUE");
                    isHooked = true;
                    return true;
                }
            }
            else if (m == null)
            {
                //Debug.Log("HOOKTOTASKBAR FALSE MethodInfo null");
            }
            else if (o == null)
            {
                //Debug.Log("HOOKTOTASKBAR FALSE object null");
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

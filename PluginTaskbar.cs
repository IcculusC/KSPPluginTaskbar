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

        #region Reference Examples

        // icon delcaration

        Texture2D anIcon = new Texture2D(30, 30, TextureFormat.ARGB32, true);
        string kspDir = KSPUtil.ApplicationRootPath.Replace("\\", "/");
        string partPath = "Parts/path_to_your_part/";
        string iconFile = "an_icon_name.png";
        Callback windowFunction;

        // example of hooking

        public void FunctionToLoadOn()
        {
            // load the icon from your part directory into a predeclared Texture2D
            WWW msIconOnImg = new WWW("file://" + kspDir + partPath + iconFile);
            msIconOnImg.LoadImageIntoTexture(anIcon);

            // define window callback and store for unhooking later
            windowFunction = new Callback(drawGUI);

            // hook to taskbar if it exists, add your own handling if not
            if (!PluginTaskbar.HookToTaskbar(windowFunction, new Callback<Callback<Texture2D>, bool>(updateIcon)))
            {
                RenderingManager.AddToPostDrawQueue(131313, windowFunction);
            }

        }

        // example of unhooking

        public void FunctionToUnloadOn()
        {

            // unhook from taskbar if it exists, add your own handling if not
            if (!PluginTaskbar.UnhookFromTaskbar(windowFunction))
            {
                RenderingManager.RemoveFromPostDrawQueue(131313, windowFunction);
            }

        }

        // window callback example
         
        private void drawGUI()
        {
            //draw stuff
        }
         
        // icon callback example
         
        private void updateIcon(Callback<Texture2D> callback, bool clicked)
        {
            //return your icon
            callback.Invoke(anIcon);
        }
                  
        #endregion

        #region Reflection Method Discovery Definitions

        // Arguments for TaskBar.Hook(Callback function, Callback<Callback<Texture2D>, bool>)
        private static Type[] args = { typeof(Callback), typeof(Callback<Callback<Texture2D>, bool>) };
        
        // Arguments for Taskbar.Unhook(Callback function)
        private static Type[] uargs = { typeof(Callback) };
        
        #endregion

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
        public static bool HookToTaskbar(Callback function, Callback<Callback<Texture2D>, bool> callback)
        {
            if (!TaskbarLoaded())
                return false;

            GameObject go = GameObject.Find("PluginTaskbarImmortal");

            object o = GetTaskbar();

            Type t = o.GetType();

            MethodInfo m = t.GetMethod("Hook", args);

            if (m != null)
            {
                m.Invoke(o, new object[] { function, callback });
            }

            return true;
        }

        // Unhooks your callback from the TaskBar
        public static bool UnhookFromTaskbar(Callback function)
        {
            if (!TaskbarLoaded())
                return false;

            object o = GetTaskbar();

            Type t = o.GetType();

            MethodInfo m = t.GetMethod("Unhook", uargs);

            if (m != null)
            {
                m.Invoke(o, new object[] { function });
            }

            return true;
        }
        
        // Returns the current TaskBar instance
        public static object GetTaskbar()
        {
            return GameObject.Find("PluginTaskbarImmortal").GetComponent("TaskBar");
        }

        #endregion
    }
}

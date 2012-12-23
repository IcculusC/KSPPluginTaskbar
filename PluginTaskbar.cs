using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

namespace TaskbarHook
{
    /**
     *          PLUGIN TASKBAR USAGE
     *          
     *      This API provides you with several options for hooking your Minimize/Maximize function to the toolbar.
     * 
     *      This file must be included in your project, but there are no DLL dependencies.
     *      
     *      Method 1 will be the simplest to implement, and is generally probably the best way to go.
     *      
     *              Step 1: Load your icons
     *              
     *                  // get KSP directory
     *                  string kspDir = KSPUtil.ApplicationRootPath.Replace("\\", "/");
     *                  
     *                  // intialize icons
     *                  private static Texture2D onIcon = new Texture2D(30, 30, TextureFormat.ARGB32, false);
     *                  private static Texture2D offIcon = new Texture2D(30, 30, TextureFormat.ARGB32, false);
     *                  
     *                  // load icons, change part folder/icon name!
     *                  WWW IconOnImg = new WWW("file://" + kspDir + "Parts/YOURPARTNAMEHERE/YOURICONNAMEHERE..png");
     *                  IconOnImg.LoadImageIntoTexture(onIcon);
     *                  WWW IconOffImg = new WWW("file://" + kspDir + "Parts/YOURPARTNAMEHERE/YOURICONNAMEHERE.png");
     *                  IconOffImg.LoadImageIntoTexture(offIcon);
     *                  
     *              Step 2:
     *              
     *                  Method 1
     * 
     *                  What you need:
     *                  30x30 On/Off icons loaded as Texture2D
     *                  a public void drawGUI() function containing your draw calls
     *                  
     *                  // check for taskbar hook
     *                  if(!PluginTaskbar.HookToTaskbar(new Callback(drawGUI), onIcon, offIcon));
     *                  {
     *                      // alternative drawing method
     *                  }
     *                  
     *                  Method 2
     *                  
     *                  What you need:
     *                  30x30 On/Off icons loaded as Texture2D
     *                  int[] of window ids
     *                  Rect[] of window positions
     *                  GUIContent[] of titles
     *                  GUI.WindowFunction[] of void(int id) functions
     *                  
     *                  // check for taskbar hook
     *                  if(!PluginTaskbar.HookToTaskbar(idArray, windowPosarray, windowTitleArray, onIcon, offIcon, windowFunctionArray));
     *                  {
     *                      // alternative drawing method
     *                  }
     * 
     **/

    class PluginTaskbar
    {
        private static Type[] args = { typeof(int[]), typeof(Rect[]), typeof(GUIContent[]), typeof(Texture2D), typeof(Texture2D), typeof(GUI.WindowFunction[]) };
        private static Type[] argsd = { typeof(Callback), typeof(Texture2D), typeof(Texture2D) };

        public static bool TaskbarLoaded()
        {
            GameObject go = null;
            
            if ((go = GameObject.Find("PluginTaskbarImmortal")) == null)
                return false;

            if (go.GetComponent("TaskBar") == null)
                return false;

            return true;
        }

        public static bool HookToTaskbar(Callback function, Texture2D onIcon, Texture2D offIcon)
        {
            if (!TaskbarLoaded())
                return false;

            GameObject go = GameObject.Find("PluginTaskbarImmortal");

            object o = go.GetComponent("TaskBar");

            Type t = o.GetType();

            MethodInfo m = t.GetMethod("Hook", argsd);

            if (m != null)
            {
                m.Invoke(o, new object[] { function, onIcon, offIcon });
            }

            return true;
        }

        public static bool HookToTaskbar(int[] id, Rect[] windowPos, GUIContent[] content, Texture2D onIcon, Texture2D offIcon, GUI.WindowFunction[] WindowGUI)
        {
            if (!TaskbarLoaded())
                return false;

            GameObject go = GameObject.Find("PluginTaskbarImmortal");

            object o = go.GetComponent("TaskBar");

            Type t = o.GetType();
            
            MethodInfo m = t.GetMethod("Hook", args);

            if (m != null)
            {
                m.Invoke(o, new object[] { id , windowPos , content , onIcon, offIcon, WindowGUI  });
            }

            return true;
        }
    }
}

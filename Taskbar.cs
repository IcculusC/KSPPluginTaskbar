using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PluginTaskbar
{
    class TaskbarModule : PartModule 
    {
        public override void OnAwake()
        {
            Immortal.AddImmortal<TaskBar>();
        }
    }
        
    public class TaskBar : MonoBehaviour
    {
        private List<TaskBarHook> m_Modules = new List<TaskBarHook>();
        private List<TaskBarDelegate> m_Delegates = new List<TaskBarDelegate>();
        private Rect windowPos;
        private Vector2 scrollPos = new Vector2(0, 0);

        public void Start()
        {
            windowPos = new Rect(Screen.width - 300, 0, 200, 40);
        }

        public void window(int id)
        {
            GUILayout.BeginHorizontal();

            RectOffset temp = GUI.skin.window.margin;

            GUI.skin.window.margin.top = GUI.skin.window.margin.bottom;
            
            if(m_Modules.Count > 0)
            {
                for (int i = 0; i < m_Modules.Count; i++)
                {
                    TaskBarHook hook = m_Modules[i];

                    if(GUILayout.Button(hook.Minimized ? hook.Icon.IconOn : hook.Icon.IconOff, GUILayout.MinWidth(30.0f), GUILayout.MinHeight(30.0f), GUILayout.MaxWidth(30.0f), GUILayout.MaxHeight(30.0f)))
                        hook.Minimized = !hook.Minimized;  
                }
            }

            GUI.skin.window.margin = temp;

            GUILayout.EndHorizontal();
        }

        public void OnGUI()
        {
            if (!HighLogic.LoadedSceneIsFlight || !FlightGlobals.ready)
                return;

            GUI.skin = AssetBase.GetGUISkin("KSP window 2");

            GUI.skin.label.normal.textColor = Color.white;
            GUI.skin.label.padding = GUI.skin.button.padding;

            //windowPos = GUILayout.Window(30038, windowPos, window, GUIContent.none, GUILayout.MinWidth(200.0f), GUILayout.MaxWidth(200.0f), GUILayout.MinWidth(40.0f));
            GUILayout.BeginArea(new Rect(200, 0, 200, 60));

            GUILayout.BeginHorizontal();

            if (m_Modules.Count > 0)
            {
                //scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);

                for (int i = 0; i < m_Modules.Count; i++)
                {
                    TaskBarHook hook = m_Modules[i];

                    if (GUILayout.Button(hook.Minimized ? hook.Icon.IconOn : hook.Icon.IconOff, GUILayout.MinWidth(30.0f), GUILayout.MinHeight(30.0f), GUILayout.MaxWidth(30.0f), GUILayout.MaxHeight(30.0f)))
                        hook.Minimized = !hook.Minimized;
                }

                //GUILayout.EndScrollView();
                
            }

            if (m_Delegates.Count > 0)
            {
                for (int i = 0; i < m_Delegates.Count; i++)
                {
                    TaskBarDelegate hook = m_Delegates[i];

                    if (GUILayout.Button(hook.Minimized ? hook.Icon.IconOn : hook.Icon.IconOff, GUILayout.MinWidth(30.0f), GUILayout.MinHeight(30.0f), GUILayout.MaxWidth(30.0f), GUILayout.MaxHeight(30.0f)))
                        hook.Minimized = !hook.Minimized;
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
            foreach (TaskBarHook hook in m_Modules)
            {
                if (!hook.Minimized)
                    hook.Draw();
            }

            foreach (TaskBarDelegate hook in m_Delegates)
            {
                if (!hook.Minimized)
                    hook.Draw();
            }
        }

        public void Hook(int[] id, Rect[] startPos, GUIContent[] content, Texture2D iconOn, Texture2D iconOff, GUI.WindowFunction[] function)
        {
            TaskBarHook temp = new TaskBarHook(id, startPos, content, new TaskBarIcon(iconOn, iconOff), function);

            HookModule(temp);

            return;
        }

        public void Hook(Callback function, Texture2D iconOn, Texture2D iconOff)
        {
            TaskBarDelegate temp = new TaskBarDelegate(function, new TaskBarIcon(iconOn, iconOff));

            HookModule(temp);

            return;
        }

        public void HookModule(TaskBarDelegate hook)
        {
            if (!m_Delegates.Contains(hook))
            {
                m_Delegates.Add(hook);
            }
        }
        public void HookModule(TaskBarHook hook)
        {
            if (!m_Modules.Contains(hook))
            {
                m_Modules.Add(hook);
            }

            return;
        }
    }
}

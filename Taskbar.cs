using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PluginTaskbar
{
    // I WOULDN'T WORRY ABOUT HOW IT WORKS, JUST BE HAPPY THAT IT DOES.

    class TaskbarModule : PartModule 
    {
        public override void OnAwake()
        {
            Immortal.AddImmortal<TaskBar>();
        }
    }
        
    public class TaskBar : MonoBehaviour
    {
        private List<TaskBarDelegate> m_Delegates = new List<TaskBarDelegate>();
        private Rect windowPos;
        private Vector2 scrollPos = new Vector2(0, 0);
        private bool showGUI = true;

        public void Update()
        {
            if (Input.GetKeyUp("f2"))
                showGUI = !showGUI;
        }

        public void OnGUI()
        {
            if (!HighLogic.LoadedSceneIsFlight || !FlightGlobals.ready || !showGUI)
                return;

            GUI.skin = AssetBase.GetGUISkin("KSP window 2");

            GUI.skin.label.normal.textColor = Color.white;
            GUI.skin.label.padding = GUI.skin.button.padding;

            
            if (m_Delegates.Count > 0)
            {
                GUILayout.BeginArea(new Rect(200, 0, 200, 60));
                
                GUILayout.BeginHorizontal();

                scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);

                for (int i = 0; i < m_Delegates.Count; i++)
                {
                    TaskBarDelegate hook = m_Delegates[i];

                    hook.UpdateIcon();

                    if (GUILayout.Button(hook.Icon, GUILayout.MinWidth(30.0f), GUILayout.MinHeight(30.0f), GUILayout.MaxWidth(30.0f), GUILayout.MaxHeight(30.0f)))
                        hook.Minimized = !hook.Minimized;
                }


                GUILayout.EndScrollView();

                GUILayout.EndHorizontal();

                GUILayout.EndArea();

                foreach (TaskBarDelegate hook in m_Delegates)
                {
                    if (!hook.Minimized)
                        hook.Draw();
                }
            }
        }

        public void Hook(Callback function, Callback<Callback<Texture2D>, bool> callback)
        {
            TaskBarDelegate temp = new TaskBarDelegate(function, callback);

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

        public void Unhook(Callback callback)
        {
            for (int i = 0; i < m_Delegates.Count; i++)
            {
                if (m_Delegates[i].Delegate.Equals(callback))
                    m_Delegates.RemoveAt(i);
            }
        }
    }
}

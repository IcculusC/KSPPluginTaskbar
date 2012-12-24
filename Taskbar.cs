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
        private Dictionary<string, TaskBarDelegate> m_Delegates = new Dictionary<string, TaskBarDelegate>();
        private Vector2 scrollPos = new Vector2(0, 0);
        private bool showGUI = false;

        private float areaWidth = 160.0f;
        private bool minimized = false;

        private GUIStyle scrollbarStyle = new GUIStyle();

        private Animation currentAnimation = Animation.NONE;

        private Texture2D[] buttonImage = new Texture2D[2] { new Texture2D(20, 40, TextureFormat.ARGB32, true), new Texture2D(20, 40, TextureFormat.ARGB32, true) };
        private string kspDir = KSPUtil.ApplicationRootPath.Replace("\\", "/");

        private enum Animation
        {
            NONE,
            MINIMIZE,
            MAXIMIZE
        }

        public void Awake()
        {
            WWW msIconOnImg = new WWW("file://" + kspDir + "Parts/SwingToolbar/maximized.png");
            msIconOnImg.LoadImageIntoTexture(buttonImage[0]);
            msIconOnImg = new WWW("file://" + kspDir + "Parts/SwingToolbar/minimized.png");
            msIconOnImg.LoadImageIntoTexture(buttonImage[1]);
        }

        public void Update()
        {
            if (Input.GetKeyUp("f2") && m_Delegates.Count > 0)
                showGUI = !showGUI;

            if (currentAnimation == Animation.MAXIMIZE)
            {
                scrollbarStyle = GUIStyle.none;
                areaWidth += 30.0f;
                if (areaWidth >= 160.0f)
                {
                    areaWidth = 160.0f;
                    currentAnimation = Animation.NONE;
                }
            }
            else if (currentAnimation == Animation.MINIMIZE)
            {
                scrollbarStyle = GUIStyle.none;
                areaWidth -= 30.0f;
                if (areaWidth <= 0.0f)
                {
                    areaWidth = 0.0f;
                    currentAnimation = Animation.NONE;
                }
            }
        }

        public void OnGUI()
        {
            if (!HighLogic.LoadedSceneIsFlight || !FlightGlobals.ready || !showGUI)
                return;
                        
            GUI.skin = AssetBase.GetGUISkin("KSP window 5");

            GUI.skin.label.normal.textColor = Color.white;
            GUI.skin.label.padding = GUI.skin.button.padding;

            GUIStyle button = new GUIStyle(GUI.skin.button);
            button.padding = new RectOffset(0, 0, 0, 0);

            GUIStyle win = new GUIStyle();
            win.normal.background = win.hover.background = win.active.background = GUI.skin.window.normal.background;

            GUILayout.BeginArea(new Rect(180, 0, 20, 40));
            //minimized ? new GUIContent(buttonImage[1]) : new GUIContent(buttonImage[0])
                if (GUILayout.Button("", button, GUILayout.Width(20.0f), GUILayout.Height(40.0f), GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f), GUILayout.MinHeight(40.0f), GUILayout.MaxHeight(40.0f)))
                {
                    if (Event.current.button == 0)
                    {
                        minimized = !minimized;
                        if (minimized)
                            currentAnimation = Animation.MINIMIZE;
                        else
                            currentAnimation = Animation.MAXIMIZE;
                    }
                }
                GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(200, 0, areaWidth, 40), win);
            
            if (m_Delegates.Count > 0)
            {
                GUI.skin.scrollView.margin = new RectOffset(0, 0, 0, 0);
                GUI.skin.scrollView.padding = new RectOffset(0, 0, 0, 0);

                if (currentAnimation == Animation.NONE)
                    scrollbarStyle = GUI.skin.horizontalScrollbar;
                else
                    scrollbarStyle = GUIStyle.none;
                
                scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, scrollbarStyle, GUIStyle.none);

                GUILayout.BeginHorizontal();

                foreach (KeyValuePair<string, TaskBarDelegate> kvp in m_Delegates)
                {
                    kvp.Value.UpdateIcon();

                    if (GUILayout.Button(kvp.Value.Icon, button, GUILayout.MinWidth(30.0f), GUILayout.MinHeight(30.0f), GUILayout.MaxWidth(30.0f), GUILayout.MaxHeight(30.0f)))
                    {
                        if (Event.current.button == 0)
                        {
                            kvp.Value.ClickEvent(true);
                        }
                        else if (Event.current.button == 1)
                        {
                            kvp.Value.ClickEvent(false);
                        }
                    }
                }

                GUILayout.EndHorizontal();
                GUILayout.EndScrollView();
            }           

            GUILayout.EndArea();

            foreach (KeyValuePair<string, TaskBarDelegate> kvp in m_Delegates)
            {

            }
        }

        public bool Hook(Callback<bool> function, Callback<Callback<Texture>, bool> callback, string moduleName)
        {
            TaskBarDelegate temp = new TaskBarDelegate(function, callback, moduleName);

            if (HookModule(temp, moduleName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public bool HookModule(TaskBarDelegate hook, string moduleName)
        {
            if (m_Delegates == null)
                m_Delegates = new Dictionary<string, TaskBarDelegate>();

            foreach (KeyValuePair<string, TaskBarDelegate> kvp in m_Delegates)
            {
                if (kvp.Key.Equals(moduleName))
                {
                    Debug.Log(String.Format("HOOKMODULE FALSE moduleName already in Dictionary: {0}", moduleName));
                    return false;
                }
            }

            showGUI = true;
            m_Delegates.Add(moduleName, hook);
            Debug.Log("HOOKMODULE TRUE");
            return true;
        }

        public bool Unhook(string module)
        {
            if (m_Delegates == null)
                m_Delegates = new Dictionary<string, TaskBarDelegate>();

            if(m_Delegates.Count > 0)
            {
                //foreach (KeyValuePair<string, TaskBarDelegate> kvp in m_Delegates)
                //{
                //    if (kvp.Key.Equals(module))
                //    {
                //        m_Delegates.Remove(kvp.Key);

                //        Debug.Log("UNHOOK TRUE");
                //        return true;
                //    }
                //
                //}

                if(m_Delegates.Remove(module))
                {
                    Debug.Log("UNHOOK TRUE");
                    return true;
                }
                else
                {
                    Debug.Log(String.Format("UNHOOK FALSE module not found in Dictionary: {0}", module));
                    return false;
                }
            }
            Debug.Log("UNHOOK FALSE delegate count is 0");
            return false;
        }
    }
}

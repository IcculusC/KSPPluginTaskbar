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
        #region Declarations

        private Dictionary<string, TaskBarDelegate> m_Delegates = new Dictionary<string, TaskBarDelegate>();
        private Vector2 scrollPos = new Vector2(0, 0);
        private bool showGUI = false;

        private string toolTip = "";
        private string displayTooltip = "";

        private bool dragging = false;

        private float areaWidth = 160.0f;
        private bool minimized = false;

        private float barWidth = 0;

        private GUIStyle scrollbarStyle = new GUIStyle();

        private Animation currentAnimation = Animation.NONE;

        private Texture2D[] buttonImage = new Texture2D[2] { new Texture2D(20, 40, TextureFormat.ARGB32, true), new Texture2D(20, 40, TextureFormat.ARGB32, true) };
        private string kspDir = KSPUtil.ApplicationRootPath.Replace("\\", "/");

        #endregion

        private enum Animation
        {
            NONE,
            MINIMIZE,
            MAXIMIZE
        }

        //public void Awake()
        //{
        //    WWW msIconOnImg = new WWW("file://" + kspDir + "Parts/SwingToolbar/maximized.png");
        //    msIconOnImg.LoadImageIntoTexture(buttonImage[0]);
        //    msIconOnImg = new WWW("file://" + kspDir + "Parts/SwingToolbar/minimized.png");
        //    msIconOnImg.LoadImageIntoTexture(buttonImage[1]);
        //}

        public void Start()
        {
            barWidth = (float)(Screen.width / 4.75);
            int temp = Mathf.RoundToInt((float)(barWidth / 42));
            barWidth = (float)(temp * 42);
            areaWidth = barWidth;
        }

        public void Update()
        {
            if (Input.GetKeyUp("f2") && m_Delegates.Count > 0)
                showGUI = !showGUI;

            if (currentAnimation == Animation.MAXIMIZE)
            {
                areaWidth += 30.0f;
                if (areaWidth >= barWidth)
                {
                    areaWidth = barWidth;
                    currentAnimation = Animation.NONE;
                    minimized = false;
                }
            }
            else if (currentAnimation == Animation.MINIMIZE)
            {
                areaWidth -= 30.0f;
                if (areaWidth <= 0.0f)
                {
                    areaWidth = 0.0f;
                    currentAnimation = Animation.NONE;
                    minimized = true;
                }
            }

            displayTooltip = toolTip;
        }

        public void OnGUI()
        {
            if (!HighLogic.LoadedSceneIsFlight || !FlightGlobals.ready || !showGUI)
                return;
            
            GUI.skin = AssetBase.GetGUISkin("OrbitMapSkin");
            
            GUIStyle button = new GUIStyle(GUI.skin.button);
            button.padding = new RectOffset(0, 0, 0, 0);
                        
            GUIStyle win = new GUIStyle();
            win.normal.background = win.hover.background = win.active.background = GUI.skin.scrollView.normal.background;
            win.margin = new RectOffset(0, 0, 0, 0);
            win.padding = new RectOffset(0, 0, 0, 0);

            GUIStyle hScrollbarStyle = new GUIStyle(GUI.skin.horizontalScrollbar);
            GUIStyle vScrollbarStyle = new GUIStyle(GUI.skin.verticalScrollbar);

            hScrollbarStyle.fixedWidth = hScrollbarStyle.fixedHeight = 0;
            vScrollbarStyle.fixedWidth = vScrollbarStyle.fixedHeight = 0;

            /*
            GUI.skin.customStyles = new GUIStyle[3];
            GUI.skin.customStyles[0] = new GUIStyle();
            GUI.skin.customStyles[0].name = "upbutton";
            GUI.skin.customStyles[1] = new GUIStyle();
            GUI.skin.customStyles[1].name = "upbutton";
            GUI.skin.customStyles[2] = new GUIStyle();
            GUI.skin.customStyles[2].name = "thumb";
            */

            GUILayout.BeginArea(new Rect(125, 0, 20, 40));
            
            if (GUILayout.Button("", button, GUILayout.Width(20.0f), GUILayout.Height(40.0f), GUILayout.MinWidth(20.0f), GUILayout.MaxWidth(20.0f), GUILayout.MinHeight(40.0f), GUILayout.MaxHeight(40.0f)))
            {
                if (Event.current.button == 0)
                {
                    //minimized = !minimized;
                    if (!minimized)
                        currentAnimation = Animation.MINIMIZE;
                    else
                    {
                        minimized = false;
                        currentAnimation = Animation.MAXIMIZE;
                    }
                }
            }

            GUILayout.EndArea();

            if (!minimized && areaWidth > 0)
            {
                GUILayout.BeginArea(new Rect(145, 0, areaWidth, 40), win);

                if (m_Delegates.Count > 0)
                {
                    scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, hScrollbarStyle, vScrollbarStyle);

                    GUILayout.BeginHorizontal();

                    foreach (KeyValuePair<string, TaskBarDelegate> kvp in m_Delegates)
                    {
                        if (GUILayout.Button(new GUIContent(kvp.Value.Icon, kvp.Value.moduleName), button, GUILayout.MinWidth(30.0f), GUILayout.MinHeight(30.0f), GUILayout.MaxWidth(30.0f), GUILayout.MaxHeight(30.0f)))
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

                    if (Event.current.button == 0 && Event.current.type == EventType.mouseDown && new Rect(145, 0, areaWidth, 40).Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
                    {
                        if (Input.mousePosition.x > (145 + (areaWidth / 2)))
                        {
                            scrollPos = new Vector2(scrollPos.x + 33, scrollPos.y);
                        }
                        else if(Input.mousePosition.x < (145 + (areaWidth / 2)))
                            scrollPos = new Vector2(scrollPos.x - 33, scrollPos.y);

                    }

                    GUILayout.EndHorizontal();

                    GUILayout.EndScrollView();
                }

                GUILayout.EndArea();
            }

            foreach (KeyValuePair<string, TaskBarDelegate> kvp in m_Delegates)
            {
                if (kvp.Key.Equals(toolTip))
                {
                    kvp.Value.HoverEvent(Input.mousePosition);
                }
            }

            if (displayTooltip != null && displayTooltip != "")
            {
                TaskBarDelegate tbd;
                if (m_Delegates.TryGetValue(displayTooltip, out tbd))
                {
                    if (tbd.TooltipText != null)
                    {
                        GUILayout.BeginArea(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 160, 30));
                        GUILayout.Label(tbd.TooltipText);
                        GUILayout.EndArea();
                    }
                }
            }

            if (toolTip != GUI.tooltip)
                toolTip = GUI.tooltip;

        }

        #region Icon Hooking functions

        public bool Hook(Callback<bool> function, Callback<Callback<Texture>, bool> callback, Callback<Vector3> hover, Callback<Callback<string>> tooltip, string moduleName)
        {
            TaskBarDelegate temp = new TaskBarDelegate(function, callback, hover, tooltip, moduleName);

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
                    //Debug.Log(String.Format("HOOKMODULE FALSE moduleName already in Dictionary: {0}", moduleName));
                    return false;
                }
            }

            showGUI = true;
            m_Delegates.Add(moduleName, hook);
            //Debug.Log("HOOKMODULE TRUE");
            return true;
        }

        public bool Unhook(string module)
        {
            if (m_Delegates == null)
                m_Delegates = new Dictionary<string, TaskBarDelegate>();

            if(m_Delegates.Count > 0)
            {
                if(m_Delegates.Remove(module))
                {
                    //Debug.Log("UNHOOK TRUE");
                    return true;
                }
                else
                {
                    //Debug.Log(String.Format("UNHOOK FALSE module not found in Dictionary: {0}", module));
                    return false;
                }
            }
            //Debug.Log("UNHOOK FALSE delegate count is 0");
            return false;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PluginTaskbar
{
    public class TaskBarHook
    {
        private GUI.WindowFunction[] m_Function;
        private int[] m_WindowID;
        private Rect[] m_WindowPos;
        private GUIContent[] m_Content;
        private bool m_Minimized = true;

        public TaskBarIcon Icon;

        public int[] WindowIDs
        {
            get
            {
                return m_WindowID;
            }
            set
            {
                m_WindowID = value;
            }
        }

        public bool Minimized
        {
            get
            {
                return m_Minimized;
            }
            set
            {
                m_Minimized = value;
            }
        }

        public TaskBarHook(int[] id, Rect[] startPos, GUIContent[] content, TaskBarIcon icon, GUI.WindowFunction[] function)
        {
            m_Function = function;
            m_WindowID = id;
            m_WindowPos = startPos;
            m_Content = content;
            m_Minimized = false;
            Icon = icon;
        }

        public void Draw()
        {
            for (int i = 0; i < m_WindowID.Length; i++)
            {
                m_WindowPos[i] = GUILayout.Window(m_WindowID[i], m_WindowPos[i], m_Function[i], m_Content[i]);
            }
            return;
        }

    }

    public class TaskBarDelegate
    {
        private Callback m_Function;
        
        public TaskBarIcon Icon;

        private bool m_Minimized = true;

        public bool Minimized
        {
            get
            {
                return m_Minimized;
            }
            set
            {
                m_Minimized = value;
            }
        }

        public TaskBarDelegate(Callback function, TaskBarIcon icon)
        {
            m_Function = function;
            Icon = icon;
        }

        public void Draw()
        {
            if (!m_Minimized)
                m_Function.Invoke();
        }
    }

    public class TaskBarIcon
    {
        public Texture2D IconOff;// = new Texture2D(30, 30, TextureFormat.ARGB32, false);
        public Texture2D IconOn;// = new Texture2D(30, 30, TextureFormat.ARGB32, false);

        public TaskBarIcon(Texture2D on, Texture2D off)
        {
            IconOn = on;
            IconOff = off;
        }
    }
}

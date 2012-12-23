using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PluginTaskbar
{
    public class TaskBarDelegate
    {
        private Callback m_Function;
        private Callback<Callback<Texture2D>, bool> m_Icon;
        
        public Texture2D Icon;

        private bool m_Minimized = true;

        public Callback Delegate
        {
            get
            {
                return m_Function;
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

        public TaskBarDelegate(Callback function, Callback<Callback<Texture2D>, bool> taskBarIcon)//TaskBarIcon icon, Callback<Callback<Texture2D>> taskBarIcon)
        {   
            m_Function = function;
            m_Icon = taskBarIcon;
            m_Icon.Invoke(new Callback<Texture2D>(updateIcon), m_Minimized);
        }

        private void updateIcon(Texture2D texture)
        {
            Icon = texture;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PluginTaskbar
{
    public class TaskBarDelegate
    {
        private Callback<bool> m_Function;
        private Callback<Callback<Texture>, bool> m_Icon;
        private string m_ModuleName = "";

        public Texture Icon;

        private bool m_Minimized = false;

        public Callback<bool> Delegate
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

        public string moduleName
        {
            get { return m_ModuleName; }
        }

        public TaskBarDelegate(Callback<bool> function, Callback<Callback<Texture>, bool> taskBarIcon, string moduleName)//TaskBarIcon icon, Callback<Callback<Texture>> taskBarIcon)
        {   
            m_Function = function;
            m_Icon = taskBarIcon;
            m_Icon.Invoke(new Callback<Texture>(updateIcon), m_Minimized);
        }

        public void UpdateIcon()
        {
            m_Icon.Invoke(new Callback<Texture>(updateIcon), m_Minimized);
        }

        public void ClickEvent(bool leftClick)
        {
            m_Function.Invoke(leftClick);
        }

        private void updateIcon(Texture texture)
        {
            Icon = texture;
        }

        public bool Draw()
        {
            return false;
        }
    }
}

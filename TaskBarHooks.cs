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
    public class TaskBarDelegate
    {
        private Callback<bool> m_Function;
        private Callback<Callback<Texture>, bool> m_Icon;
        private Callback<Vector3> m_Hover;
        private Callback<Callback<string>> m_Tooltip;
        private Callback<Rect, bool> m_Draw;
        private string m_ModuleName = "";
        private string m_TooltipText = "";
        private Texture m_IconTexture;
        
        public Texture Icon
        {
            get
            {
                UpdateIcon();
                return m_IconTexture;
            }
            set
            {
                m_IconTexture = value;
            }
        }

        private bool m_Minimized = false;

        public Callback<bool> Delegate
        {
            get
            {
                return m_Function;
            }
        }

        public string TooltipText
        {
            get 
            {
                UpdateTooltip();
                return m_TooltipText; 
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

        public TaskBarDelegate(Callback<bool> function, Callback<Callback<Texture>, bool> taskBarIcon, Callback<Vector3> hover, Callback<Rect, bool> draw, Callback<Callback<string>> tooltip, string moduleName)//TaskBarIcon icon, Callback<Callback<Texture>> taskBarIcon)
        {
            m_ModuleName = moduleName;
            m_Function = function;
            m_Icon = taskBarIcon;
            m_Hover = hover;
            m_Draw = draw;
            m_Tooltip = tooltip;
            m_Icon.Invoke(new Callback<Texture>(updateIcon), m_Minimized);
            m_Tooltip.Invoke(new Callback<string>(updateTooltip));
        }

        public void UpdateTooltip()
        {
            m_Tooltip.Invoke(new Callback<string>(updateTooltip));
        }

        public void UpdateIcon()
        {
            m_Icon.Invoke(new Callback<Texture>(updateIcon), m_Minimized);
        }

        public void ClickEvent(bool leftClick)
        {
            m_Function.Invoke(leftClick);
        }

        public void Draw(Rect icon, bool visible)
        {
            m_Draw.Invoke(icon, visible);
            return;
        }

        public void HoverEvent(Vector3 mousePosition)
        {
            
            m_Hover.Invoke(mousePosition);
        }

        private void updateTooltip(string tooltip)
        {
            m_TooltipText = tooltip;
        }

        private void updateIcon(Texture texture)
        {
            Icon = texture;
        }
    }
}

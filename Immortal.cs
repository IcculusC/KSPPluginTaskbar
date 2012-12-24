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
    // PERSISTENT GAME OBJECT CODE, DON'T WORRY ABOUT HOW IT WORKS

    public static class Immortal
    {
       private static GameObject _gameObject;
        public static void AddImmortal<T>() where T : Component
        {
            if (_gameObject == null)
            {
                _gameObject = new GameObject("PluginTaskbarImmortal", typeof(T));
                UnityEngine.Object.DontDestroyOnLoad(_gameObject);
            }
            else if (_gameObject.GetComponent<T>() == null)
                _gameObject.AddComponent<T>();
        }
                
        public static TaskBar GetInstance()
        {
            if (_gameObject.GetComponent<TaskBar>() != null)
                return _gameObject.GetComponent<TaskBar>();
            else
                return null;
        }
    }
}

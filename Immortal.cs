using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PluginTaskbar
{
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

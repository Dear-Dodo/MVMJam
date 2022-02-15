using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Management
{
    /// <summary>
    /// Wrapper class for scene loading to centralise all scene lookups.
    /// </summary>
    public class SceneLoader
    {
        private static readonly Dictionary<string, string> _scenes = new Dictionary<string, string>
        {
            {"MainMenu", "MainMenu" },
            {"Game", "Game" }
        };

        public static AsyncOperation LoadAsync(string scene)
        {
            if (_scenes.TryGetValue(scene, out string actualName))
            {
                return SceneManager.LoadSceneAsync(actualName);
            }
            throw new ArgumentException($"Scene '{scene}' does not exist.");
        }
    } 
}

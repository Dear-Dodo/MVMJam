using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management
{
    public class GameManager : MonoBehaviour
    {
        void Awake()
        {
            _ = AssetLoader.LoadAssets();
            DontDestroyOnLoad(gameObject);
        }
    } 
}

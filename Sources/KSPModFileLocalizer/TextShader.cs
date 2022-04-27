using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSPModFileLocalizer
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    class TextShader : MonoBehaviour
    {
        public static Shader textShader = null;// = new Dictionary<string, Shader>();


        /// <summary>
        /// Initial Unity Awake call
        /// </summary>
        public void Awake()
        {
            LoadShaders();
        }

        internal void LoadShaders()
        {
            // here we force unity to load all shaders, even those that are not used by any material, so we can find ours.
            Shader.WarmupAllShaders();
            foreach (Shader shader in Resources.FindObjectsOfTypeAll<Shader>())
            {
                Debug.Log("[TEXT] Found Shader: " + shader.name);
            }
        }
    }
}

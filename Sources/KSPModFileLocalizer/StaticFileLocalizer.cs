using UnityEngine;
using KSP.Localization;
using System;
using System.IO;

namespace KSPediaLocalizer
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class StaticFileLocalizer : MonoBehaviour
    {
        string[] Languages = { "en-us", "es-es", "es-mx", "ja", "ru", "zh-cn" };

        private ConfigNode[] nodes;

        /// <summary>
        /// Called when the script instance is being loaded
        /// </summary>
        private void Awake()
        {


            string currentLanguage = Localizer.CurrentLanguage;

            //try to get the config node
            try
            {
                nodes = GameDatabase.Instance.GetConfigNodes("FileLocalization");
            }
            catch (Exception e)
            {
                Debug.LogError("[StaticFileLocalizer] ERROR config exception: " + e.Message);
            }

            for (int nodeID = 0; nodeID < nodes.Length; nodeID++)
            {
                try
                {
                    string filePath = nodes[nodeID].GetValue("path");
                    string fileName = nodes[nodeID].GetValue("filename");
                    string defaultLang = nodes[nodeID].GetValue("default");

                    //if (System.IO.File.Exists(filePath + fileName + "_" + currentLanguage + ".ksp"))

                    //when the target file does not exist, create it
                    if (!File.Exists(filePath + fileName) || !File.Exists(filePath + fileName + ".currentLang"))
                    {
                        replace(filePath, fileName, defaultLang);
                    }
                    //check for correct or default language
                    else
                    {
                        string[] lines = File.ReadAllLines(filePath + fileName + ".currentLang");
                        //when the language is not set the to the right language
                        if (lines[0] != currentLanguage)
                        {
                            //when it is possible to set the right language, do it
                            if (File.Exists(filePath + fileName + "_" + currentLanguage + ".lang"))
                            {
                                replace(filePath, fileName, defaultLang);
                            }
                            //else check if the default language is set or can be set
                            else if ((lines[0] != defaultLang) && File.Exists(filePath + fileName + "." + defaultLang + ".lang"))
                            {
                                replace(filePath, fileName, defaultLang);
                            }
                            else
                            {
                                Debug.LogError("[StaticFileLocalizer] ERROR: cannot switch to default or right language");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("[StaticFileLocalizer] ERROR switching static files. Please make sure that KSP has full file access to its directories: " + e.Message);
                }
            }
        }

        private void replace(string filePath, string fileName, string defaultLang)
        {
            int numLanguages = Languages.Length;
            string currentLanguage = Localizer.CurrentLanguage;

            //clean up all old language files and flags
            if (File.Exists(filePath + fileName + ".currentLang"))
            {
                string[] lines = File.ReadAllLines(filePath + fileName + ".currentLang");
                //reset the current language
                if (!File.Exists(filePath + fileName + "." + lines[0] + ".lang") && File.Exists(filePath + fileName))
                {
                    File.Copy(filePath + fileName, filePath + fileName + "." + lines[0] + ".lang");
                }
            }

            //set the new correct language
            if (File.Exists(filePath + fileName + "." + currentLanguage + ".lang"))
            {
                //when the translated file already exists
                if (File.Exists(filePath + fileName))
                {
                    File.Delete(filePath + fileName);
                }

                //create the file with the right translation
                File.Move(filePath + fileName + "." + currentLanguage + ".lang", filePath + fileName);
                File.WriteAllLines(filePath + fileName + ".currentLang", new string[] { currentLanguage });

                Debug.Log("[StaticFileLocalizer] " + filePath + fileName + " localized to " + currentLanguage);
            }
            //when the new language cannot be set, use the default one
            else if (File.Exists(filePath + fileName + "." + defaultLang + ".lang"))
            {
                //when the translated file already exists
                if (File.Exists(filePath + fileName))
                {
                    File.Delete(filePath + fileName);
                }

                //create the file with the right translation
                File.Move(filePath + fileName + "_" + defaultLang + ".lang", filePath + fileName);
                File.WriteAllLines(filePath + fileName + ".currentLang", new string[] { defaultLang });

                Debug.Log("[StaticFileLocalizer] " + filePath + fileName + " localized to default language");
            }
            else
            {
                Debug.LogError("[StaticFileLocalizer] ERROR: cannot switch files. Is a file missing?");
            }
        }
    }
}

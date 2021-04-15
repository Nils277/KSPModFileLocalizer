using UnityEngine;
using KSP.Localization;
using System.IO;
using System;

namespace KSPediaLocalizer
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class KSPediaLocalizer : MonoBehaviour
    {
        string[] Languages = { "en-us", "es-es", "es-mx", "ja", "ru", "zh-cn", "de-de", "it-it", "fr-fr", "pt-br"};

        private ConfigNode[] nodes;

        /// <summary>
        /// Called when the script instance is being loaded
        /// </summary>
        private void Awake()
        {
            int numLanguages = Languages.Length;

            string currentLanguage = Localizer.CurrentLanguage;

            Debug.Log("[KSPediaLocalizer] Current language is: " + currentLanguage);

            //try to get the config node
            try
            {
                nodes = GameDatabase.Instance.GetConfigNodes("KSPediaLocalization");
            }
            catch (Exception e)
            {
                Debug.LogError("[KSPediaLocalizer] Config exception: " + e.Message);
                Debug.LogException(e);
            }

            for (int nodeID = 0; nodeID < nodes.Length; nodeID++)
            {
                try
                {
                    string filePath = nodes[nodeID].GetValue("path");
                    string fileName = nodes[nodeID].GetValue("filename");
                    string defaultLang = nodes[nodeID].GetValue("default");

                    string setLanguage = currentLanguage;

                    //check of the right language is already set
                    if (!File.Exists(filePath + fileName + "_" + currentLanguage + ".ksp"))
                    {
                        Debug.Log("[KSPediaLocalizer] Right language is NOT set: " + filePath + fileName + "_" + currentLanguage + ".ksp");

                        //when the right language can be set
                        if (File.Exists(filePath + fileName + "_" + currentLanguage + ".lang")) {
                            Debug.Log("[KSPediaLocalizer] Setting language to: " + filePath + fileName + "_" + currentLanguage + ".ksp");
                            File.Move(filePath + fileName + "_" + currentLanguage + ".lang", filePath + fileName + "_" + currentLanguage + ".ksp");
                            setLanguage = currentLanguage;
                        }
                        //else set to the default language
                        else if (File.Exists(filePath + fileName + "_" + defaultLang + ".lang"))
                        {
                            Debug.LogError("[KSPediaLocalizer] Cannot find translation file for " + currentLanguage);
                            Debug.Log("[KSPediaLocalizer] Setting default language to: " + filePath + fileName + "_" + defaultLang + ".ksp");
                            File.Move(filePath + fileName + "_" + defaultLang + ".lang", filePath + fileName + "_" + defaultLang + ".ksp");
                            setLanguage = defaultLang;
                        }
                        else
                        {
                            Debug.LogError("[KSPediaLocalizer] Cannot switch to correct or default language, maybe a file is missing?");
                        }
                    }
                    else
                    {
                        Debug.Log("[KSPediaLocalizer] Right language is set: " + filePath + fileName + "_" + currentLanguage + ".ksp");
                    }

                    //remove the old file when a language is set
                    if (!string.IsNullOrEmpty(setLanguage))
                    {
                        for (int i = 0; i < numLanguages; i++)
                        {
                            if ((setLanguage != Languages[i]) && (File.Exists(filePath + fileName + "_" + Languages[i] + ".ksp")))
                            {
                                if (File.Exists(filePath + fileName + "_" + Languages[i] + ".lang"))
                                {
                                    Debug.Log("[KSPediaLocalizer] Deleting file : " + filePath + fileName + "_" + Languages[i] + ".ksp");
                                    File.Delete(filePath + fileName + "_" + Languages[i] + ".ksp");
                                }
                                else
                                {
                                    Debug.Log("[KSPediaLocalizer] Renaming file : " + filePath + fileName + "_" + Languages[i] + ".ksp");
                                    File.Move(filePath + fileName + "_" + Languages[i] + ".ksp", filePath + fileName + "_" + Languages[i] + ".lang");
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("[KSPediaLocalizer] Error while switching KSPedia files. Please make sure that KSP has full file access to its directories: " + e.Message);
                    Debug.LogException(e);
                }
            }
        }
    }
}

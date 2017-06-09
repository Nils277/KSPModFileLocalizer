using UnityEngine;
using KSP.Localization;
using System;
using System.IO;

namespace KSPediaLocalizer
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class StaticFileLocalizer : MonoBehaviour
    {
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
                    string defaultLanguage = nodes[nodeID].GetValue("default");
                    string[] languages = nodes[nodeID].GetValue("languages").Split(',');

                    //when the target file does not exist, create it
                    if (!File.Exists(filePath + fileName))
                    {
                        //try to set the current language
                        if (!setNewLanguage(filePath, fileName, currentLanguage, languages))
                        {
                            //try to set the default language
                            if (!setNewLanguage(filePath, fileName, defaultLanguage, languages))
                            {
                                Debug.LogError("[StaticFileLocalizer] Cannot set current or default language for " + filePath + fileName + " it seems a file is missing");
                            }
                        }
                    }
                    //check for correct or default language
                    else
                    {
                        //when another language is set currently
                        if (File.Exists(filePath + fileName + "_" + currentLanguage + ".lang"))
                        {
                            if (!setNewLanguage(filePath, fileName, currentLanguage, languages))
                            {
                                Debug.LogError("[StaticFileLocalizer] ERROR: error convert " + filePath + fileName + " into " + currentLanguage);
                            }
                        }
                        //when the current language is not supported and the default language can be set
                        else if (!contains(languages, currentLanguage) && File.Exists(filePath + fileName + "." + defaultLanguage + ".lang"))
                        {
                            if (!setNewLanguage(filePath, fileName, defaultLanguage, languages))
                            {
                                Debug.LogError("[StaticFileLocalizer] ERROR: error convert " + filePath + fileName + " into default language");
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

        //get whether the string array contains a certain string
        private bool contains(string[] languages, string language)
        {
            for (int i = 0; i < language.Length; i++)
            {
                if (languages[i].Equals(language))
                {
                    return true;
                }
            }
            return false;
        }

        //find out which languge us currently used
        private string getSetLanguage(string path, string name, string[] languages)
        {
            for (int i = 0; i < languages.Length; i++)
            {
                if (!File.Exists(path + name + "." + languages[i].Trim() + ".lang"))
                {
                    return languages[i];
                }
            }
            return string.Empty;
        }

        //replaces the language with the new one
        private bool setNewLanguage(string filePath, string fileName, string newLanguage, string[] languages)
        {
            //clean up all old language files and flags
            if (File.Exists(filePath + fileName))
            {
                string setLanguage = getSetLanguage(filePath, fileName, languages);

                //when the set language could be determined
                if (!string.IsNullOrEmpty(setLanguage))
                {
                    File.Move(filePath + fileName, filePath + fileName + "." + setLanguage + ".lang");
                }
                //else delete the file and log a warning
                else
                {
                    File.Delete(filePath + fileName);
                    Debug.LogWarning("[StaticFileLocalizer] cannot find out the currently used language for " + filePath + fileName);
                }
            }

            //set the new correct language
            if (File.Exists(filePath + fileName + "." + newLanguage + ".lang"))
            {
                //create the file with the right translation
                File.Move(filePath + fileName + "." + newLanguage + ".lang", filePath + fileName);
                Debug.Log("[StaticFileLocalizer] " + filePath + fileName + " localized to " + newLanguage);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

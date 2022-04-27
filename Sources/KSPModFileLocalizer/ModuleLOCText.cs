using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KSPModLocalizer
{
    class ModuleLOCText : PartModule
    {
        //--------------------Public fields---------------------

        /// <summary>
        /// The color of the text
        /// </summary>
        [KSPField]
        public Vector3 color = new Vector3(1, 1, 1);

        /// <summary>
        /// The transparency of the text
        /// </summary>
        [KSPField]
        public float alpha = 1f;

        /// <summary>
        /// Sets wheter the text should be bold
        /// </summary>
        [KSPField]
        public bool bold = false;

        /// <summary>
        /// Sets whether the text should be italic
        /// </summary>
        [KSPField]
        public bool italic = false;

        /// <summary>
        /// Sets wheter the text should be underlined
        /// </summary>
        [KSPField]
        public bool underlined = false;

        /// <summary>
        /// Sets wheter the text should be striked 
        /// </summary>
        [KSPField]
        public bool strikeThrough = false;

        /// <summary>
        /// The displayed text
        /// </summary>
        [KSPField]
        public string text = string.Empty;

        /// <summary>
        /// The name of the transform to display the text on
        /// </summary>
        [KSPField]
        public string textQuadName = string.Empty;

        /// <summary>
        /// The Size of the font. Font will be resized automatically when set to 0
        /// </summary>
        [KSPField]
        public float fontSize = 0.0f;


        /// <summary>
        /// The Size of the font. Font will be resized automatically when set to 0
        /// </summary>
        [KSPField]
        public string alignment = "Left";

        /// <summary>
        /// The Size of the font. Font will be resized automatically when set to 0
        /// </summary>
        [KSPField]
        public bool wrapping = false;

        /// <summary>
        /// The Size of the font. Font will be resized automatically when set to 0
        /// </summary>
        [KSPField]
        public float lineSpacing = 0.0f;

        /// <summary>
        /// The shader that should be used for the text
        /// </summary>
        [KSPField]
        public string shader = "KSP/Alpha/Translucent";



        //-----------------Internal fields-------------------

        //-----------------Internal fields-------------------

        //The textmesh to display the text on
        List<TextMeshPro> textMeshes = null;

        //----------------Life Cycle------------------

        //Delete the textmesh when the part is copied
        public override void OnCopy(PartModule fromModule)
        {
            base.OnCopy(fromModule);
            List<TextMeshPro> oldMeshes = GetComponentInChildren<List<TextMeshPro>>();
            if (oldMeshes != null)
            {
                while (oldMeshes.Count > 0)
                {
                    TextMeshPro mesh = oldMeshes[0];
                    oldMeshes.RemoveAt(0);
                    mesh.gameObject.DestroyGameObject();
                }
                oldMeshes = null;
            }
        }

        //Init and show texts
        public override void OnStart(StartState state)
        {
            Shader[] shaders = Resources.FindObjectsOfTypeAll<Shader>();
            Shader usedShader = null;
            for (int i = 0; i < shaders.Length; i++)
            {
                //Debug.Log("[LOC] Shader: " + shaders[i].name);
                if (shaders[i].name == shader)//"KSP/Emissive/Diffuse")
                {
                    usedShader = shaders[i];
                    //Debug.Log("[LOC] Found Shader: " + shaders[i].name);
                    //break;
                }
            }
            TMP_FontAsset font = UISkinManager.TMPFont;
            textMeshes = new List<TextMeshPro>();

            //get the transforms to display the text on
            Transform[] screenTransforms = part.FindModelTransforms(textQuadName);

            for (int i = 0; i < screenTransforms.Length; i++)
            {
                GameObject LCDScreen = new GameObject();
                LCDScreen.transform.parent = screenTransforms[i];
                LCDScreen.transform.localRotation = screenTransforms[i].localRotation;
                LCDScreen.transform.localRotation = Quaternion.Euler(0, 180f, 0);

                //set the properties of the used mesh for the LCDScreen
                TextMeshPro textMesh = LCDScreen.AddComponent<TextMeshPro>();
                


                Mesh M = screenTransforms[i].GetComponent<MeshFilter>().mesh;
                RectTransform T = textMesh.gameObject.GetComponent<RectTransform>();
                T.sizeDelta = new Vector2(M.bounds.size.x, M.bounds.size.y);
                LCDScreen.transform.localPosition = new Vector3(0, 0, (M.bounds.size.z / 2) + 0.002f);

                //set the parameters of the textmesh
                
                textMesh.enableAutoSizing = (fontSize == 0.0f);
                if (fontSize != 0.0f)
                {
                    textMesh.fontSize = fontSize;
                }
                else
                {
                    textMesh.fontSizeMin = 0.01f;
                }

                textMesh.lineSpacing = lineSpacing;
                textMesh.overflowMode = TextOverflowModes.Overflow;
                textMesh.enableWordWrapping = wrapping;

                switch (alignment.ToLower())
                {
                    case "center":
                        textMesh.alignment = TextAlignmentOptions.Center;
                        break;
                    case "right":
                        textMesh.alignment = TextAlignmentOptions.Right;
                        break;
                    default:
                        textMesh.alignment = TextAlignmentOptions.Left;
                        break;
                }

                textMesh.color = new Color(color.x, color.y, color.z, alpha);
                textMesh.font = font;

                //textMesh.fontMaterial = new Material(Instantiate(usedShader));
                textMesh.material = new Material(Instantiate(usedShader));
                //textMesh.

                //"effects" of the displayed text
                string displayedText = text;
                displayedText = bold ? "<b>" + displayedText + "</b>" : displayedText;
                displayedText = italic ? "<i>" + displayedText + "</i>" : displayedText;
                displayedText = underlined ? "<u>" + displayedText + "</u>" : displayedText;
                displayedText = strikeThrough ? "<s>" + displayedText + "</s>" : displayedText;
                textMesh.text = displayedText;

                textMeshes.Add(textMesh);
            }
        }
    }
}
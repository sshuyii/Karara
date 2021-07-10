using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[System.Serializable]
    public struct TextUI
    {
        [SerializeField]
        public string key;
        [SerializeField]
        public TextMeshProUGUI objInScene;

        [SerializeField]
        public string contentENG;
        [SerializeField]
        public string contentCHN;

    }

[System.Serializable]
    public struct Text2D
    {
        [SerializeField]
        public string key;
        [SerializeField]
        public TextMeshPro objInScene;
        

        [SerializeField]
        public string contentENG;
        [SerializeField]
        public string contentCHN;

    }
public class Translator : MonoBehaviour
{
   
    public bool generateFile;
    
    public int languageCode;


    [SerializeField]
    public List<Text2D> Text2DList;
    [SerializeField]
    public List<TextUI> TextUIList; 
    // Start is called before the first frame update

    public TMP_FontAsset CHNFont;
    public Material CHNFontMaterial;
    public TMP_FontAsset ENGFont;
    
    public Material ENGFontMaterial;
    void Start()
    {
        languageCode = PlayerPrefs.GetInt("languageCode",1);

        for(int i = 0; i < Text2DList.Count; i ++)
        {
            Text2D currentTextObj  = Text2DList[i];
            if(languageCode == 0)
            {
                currentTextObj.objInScene.text= currentTextObj.contentENG;
            }
            else currentTextObj.objInScene.text= currentTextObj.contentCHN;
        }

        for(int i = 0; i < TextUIList.Count; i ++)
        {
            TextUI currentTextObj  = TextUIList[i];
            if(languageCode == 0)
            {
                currentTextObj.objInScene.text= currentTextObj.contentENG;
            }
            else currentTextObj.objInScene.text= currentTextObj.contentCHN;
        }

        if(Text2DList!= null)
        {
            foreach(Text2D t2d in Text2DList)
            {
                if(languageCode == 0)t2d.objInScene.text = t2d.contentENG;
                else t2d.objInScene.text = t2d.contentENG;
            }
        }

        if(TextUIList != null)
        {
            foreach(TextUI tUI in TextUIList)
            {
                if(languageCode == 0)tUI.objInScene.text = tUI.contentENG;
                else tUI.objInScene.text = tUI.contentENG;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateFontGUI(TextMeshProUGUI tmpGUI, TMP_FontAsset fontAsset, Material fontMaterial)
    {
        tmpGUI.font = fontAsset;
        tmpGUI.fontMaterial = fontMaterial;

    }

        public void UpdateFont(TextMeshPro tmp, TMP_FontAsset fontAsset, Material fontMaterial)
    {
        tmp.font = fontAsset;
        tmp.fontMaterial = fontMaterial;

    }
}

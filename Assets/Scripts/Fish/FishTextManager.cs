using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

public struct FishText
{
    [SerializeField]
    public string key;
    [SerializeField]
    public List<string> content;

    [SerializeField]
    public List<string> contentENG;

    [SerializeField]
    public List<string> contentCHN;

    [SerializeField]
    public bool isPlaying;
    [SerializeField]
    public int playingIdx;
}



public class FishTextManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Translator Translator;

    [SerializeField]
    public List<FishText> FishTextList;

    public Dictionary<string,FishText> FishTextDict;



    void Start()
    {

        Translator = GameObject.Find("---Translator").GetComponent<Translator>();
        
        FishTextDict = new Dictionary<string, FishText>();
        for(int i = 0; i < FishTextList.Count; i++)
        {
            FishText ft = FishTextList[i];            
            if(Translator.languageCode == 0)  ft.content = ft.contentENG;
            else ft.content = ft.contentCHN;
            FishTextDict.Add(ft.key, ft);

            // Debug.Log(ft.key + " fish text length " +ft.content.Count + " playing " + ft.playingIdx);
        }


        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public FishText GetText(string keyWord)
    {
        FishText newFishText = new FishText();
        if (FishTextDict.ContainsKey(keyWord)) return FishTextDict[keyWord];
        else{
            newFishText.content = new List<string>(){keyWord};
        }
        return newFishText;
    }
}

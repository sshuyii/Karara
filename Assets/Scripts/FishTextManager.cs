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
    public bool isPlaying;
    [SerializeField]
    public int playingIdx;
}
public class FishTextManager : MonoBehaviour
{
    // Start is called before the first frame update


    [SerializeField]
    public List<FishText> FishTextList;

    public Dictionary<string,FishText> FishTextDict;

    void Start()
    {

        FishTextDict = new Dictionary<string, FishText>();
        for(int i = 0; i < FishTextList.Count; i++)
        {
            FishTextDict.Add(FishTextList[i].key, FishTextList[i]);
        }


        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public FishText GetText(string keyWord)
    {
        FishText newFishText = new FishText();
        if (FishTextDict.ContainsKey(keyWord)) newFishText = FishTextDict[keyWord];
        return newFishText;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class AdsController : MonoBehaviour
{
    // Start is called before the first frame update

    public HashSet<string> backgrounds = new HashSet<string>();
    private Dictionary<string, Ad> AdsDic = new Dictionary<string, Ad>();
    Queue<Ad> AdsWaitinglist = new Queue<Ad>();
    private List<Sprite> poses = new List<Sprite>();



    private string AdsPath = "Images/Poster/Background/Chapt1";
    private string PosePath = "Images/Karara/Pose/Chapt1";

    //hard coding 4 poses
    // 1 top only 2 top + bottom  3 (top + bottom)/Everything + shoes 
    private List<int> ClothesNumShownInPose = new List<int> { 1, 3, 2, 1 };

    public SpriteRenderer photoBackground;
    public SpriteRenderer bodyImage;

    FinalCameraController FinalCameraController;
    SpriteLoader SpriteLoader;

    public string currentAd;
    public int currentPoseIdx;

    

    public Sprite transparent;


    // 0 top 1 bottom 2 shoe 3 everything
    // 记住cloth
    // 记住位置换图

    [SerializeField]
    private Cloth[] wearingClothes = new Cloth[4] { new Cloth(), new Cloth(), new Cloth(), new Cloth() };
    [SerializeField]
    private SpriteRenderer[] ImagesInPosture = new SpriteRenderer[4];



    public List<int> usedPoses = new List<int>();
    public HashSet<string> usedAds = new HashSet<string>();

    [SerializeField]
    private GameObject[] AdsInSubway;
    [Space(20)]
    public int nextAdIdx; //第三个

    int adsShown = 0;



     int S1UpperBound;
     int S2UpperBound;
    LevelManager LevelManager;


    // [Header("pose poster relationship")]
    public List<PosterPosePair> PosterPosePairs_Chap1;

    [Serializable]
    public struct PosterPosePair
    {
        public string AdName;
        public List<int> myPoses;
    }

    ValueEditor ValueEditor;
    void Start()
    {

        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        SpriteLoader = GameObject.Find("---SpriteLoader").GetComponent<SpriteLoader>();
        LevelManager = FinalCameraController.LevelManager;
        ValueEditor = GameObject.Find("---ValueEditor").GetComponent<ValueEditor>();

        //StreamWriter writer = new StreamWriter("./file2.txt");
        //StreamReader reader = new System.IO.StreamReader("./file2.txt");

        StringReader reader = null;
        TextAsset file = (TextAsset)Resources.Load("AdAttributes", typeof(TextAsset));
        reader = new StringReader(file.text);


        object[] loadedPics = Resources.LoadAll(AdsPath, typeof(Sprite));
        for (int i = 0; i < loadedPics.Length; i++)
        {
            Sprite newPic = (Sprite)loadedPics[i];
            backgrounds.Add(newPic.name);
            string atbs = "a 1 1 1 1";
            atbs = reader.ReadLine();
            if (atbs == null) atbs = "a 1 1 1 1";
            Ad newAd = new Ad(newPic.name, newPic, atbs);
            AdsDic.Add(newPic.name, newAd);
            AdsWaitinglist.Enqueue(newAd);
        }


        //Start from one post

        for (int i = 0; i < nextAdIdx; i++)
        {
            AdsWaitinglist.Dequeue();
            adsShown++;
        }



        //writer.Close();
        loadedPics = Resources.LoadAll(PosePath, typeof(Sprite));
        for (int i = 0; i < loadedPics.Length; i++)
        {
            poses.Add((Sprite)loadedPics[i]);
        }


        wearingClothes[3] = SpriteLoader.defaultClothes[3];
        wearingClothes[2] = SpriteLoader.defaultClothes[2];

        S1UpperBound = ValueEditor.PosterRelated.S1UpperBound;
        S2UpperBound = ValueEditor.PosterRelated.S2UpperBound;
        S2UpperBound = S1UpperBound + S2UpperBound;

        foreach (PosterPosePair pair in PosterPosePairs_Chap1)
        {
            if (pair.myPoses != null)
            {
                if (pair.myPoses.Count == 0)
                {
                    List<int> l = new List<int>() { 0, 1, 2, 3 };
                    AdsDic[pair.AdName].AddUsalbePose(l);
                }

                else AdsDic[pair.AdName].AddUsalbePose(pair.myPoses);
            }


            Debug.Log(pair.AdName + " pose count " + AdsDic[pair.AdName].myPoses.Count);

        }

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ClickBackground(string bg)
    {

        currentAd = bg;
        Debug.Log("背景" + currentAd);
        photoBackground.sprite = AdsDic[currentAd].sprite;

        ResetPosture(currentAd);

        // todo：换衣服

        ChangeClothEnterAds();
        //=========

        FinalCameraController.ChangeToAdvertisement();

    }



    public void ResetPosture(string currentAd)
    {
        //需要reset到能用的第一个姿势
        foreach (PosterPosePair pair in PosterPosePairs_Chap1)
        {
            if(currentAd == pair.AdName)
            {
                currentPoseIdx = pair.myPoses[0];
            }
        }
        bodyImage.sprite = poses[currentPoseIdx];

    }



    public void ChangePosture()
    {
        Debug.Log("poster " + currentAd);
        // currentPoseIdx++;
        int loopCounter = poses.Count;
        while (loopCounter > 0)
        {

            currentPoseIdx++;
            if (currentPoseIdx >= poses.Count) currentPoseIdx = 0;
            List<int> thisPoses = AdsDic[currentAd].myPoses;
            if (thisPoses.Contains(currentPoseIdx))
            {
                bodyImage.sprite = poses[currentPoseIdx];
                ChangeClothEnterAds();
                break;
            }
            loopCounter--;
        }

    }

    public void ChangeClothInAds(int typeIdx, Cloth cloth)
    {


        if (typeIdx == 3)
        {
            wearingClothes[0] = new Cloth();
            wearingClothes[1] = new Cloth();
            wearingClothes[2] = wearingClothes[2];
            wearingClothes[3] = cloth;
        }
        else
        {

            wearingClothes[typeIdx] = cloth;
            wearingClothes[3] = new Cloth();
        }



    }

    public void TakeOffInAds(int typeIdx)
    {
        wearingClothes[typeIdx] = new Cloth();
    }



    private void ChangeClothEnterAds()
    {

        bool shouldHideEverything = false;
        for (int i = 0; i < wearingClothes.Length; i++)
        {
            if (wearingClothes[i].name != "empty")
            {
                ImagesInPosture[i].sprite = wearingClothes[i].spritesInPos[currentPoseIdx];
                if (i < 2) shouldHideEverything = true;
            }
            else
            {
                ImagesInPosture[i].sprite = SpriteLoader.defaultClothes[i].spritesInPos[currentPoseIdx];
            }

            if (shouldHideEverything)
            {
                ImagesInPosture[3].sprite = transparent;
            }

        }



    }


    public bool IsThisPoseOk()
    {
        if (usedPoses.Contains(currentPoseIdx)) return false;
        else return true;
    }


    public bool IsThisAdOk()
    {
        if (usedAds.Contains(currentAd)) return false;
        else return true;
    }

    public void UseAdAndPose()
    {
        usedAds.Add(currentAd);
        usedPoses.Add(currentPoseIdx);


        // update stage step 1: inform levelmanager to start the upgrade
        if (usedAds.Count == S1UpperBound && LevelManager.stage == 1) LevelManager.upgradeReadyOrNot = true;
        if (usedAds.Count == S2UpperBound && LevelManager.stage == 2) LevelManager.upgradeReadyOrNot = true;
    }



    public void UpdatePosters()
    {

        if (adsShown -  ValueEditor.PosterRelated.shownUsedDifference >= usedAds.Count) return;

        nextAdIdx = -1;

        for (int i = 0; i < AdsInSubway.Length; i++)
        {
            GameObject nextAdSpot = AdsInSubway[i];
            SpriteRenderer nextAdRender = nextAdSpot.GetComponent<SpriteRenderer>();

            if (nextAdSpot.active == false)
            {

                nextAdSpot.SetActive(true);
                AdsWaitinglist.Dequeue();
                adsShown++;
                StartCoroutine(RendererFadeIn(nextAdRender));
                break;
            }
            else if (AdsDic[nextAdRender.sprite.name].hasTaken)
            {
                nextAdIdx = i;
            }

        }

        if (nextAdIdx > 0)
        {
            Ad newAd = AdsWaitinglist.Dequeue();
            adsShown++;
            SpriteRenderer r = AdsInSubway[nextAdIdx].GetComponent<SpriteRenderer>();
            r.sprite = newAd.sprite;
            StartCoroutine(RendererFadeIn(r));
        }

    }


    private IEnumerator RendererFadeIn(Renderer renderer)
    {
        Color c = renderer.material.color;
        c.a = 0f;
        renderer.material.color = c;

        //fade in effect
        for (float f = 0.05f; f < 1f; f += 0.05f)
        {
            c.a = f;
            renderer.material.color = c;
            yield return new WaitForSeconds(0.05f);

        }
    }

    public int GetNewFollowerNum()
    {
        int num = 0;

        int attNum = 4;
        //先确定这个pose时展示了多少件东西 1= top only 2 = top+bottom

        int ClothesNum = ClothesNumShownInPose[currentPoseIdx];
        int originalNum = ClothesNum;
        // 有时候只能看见everything


        if (ClothesNum > 0 && wearingClothes[3].name != "empty")
        {
            ClothesNum = 4;
        }


        for (int i = 0; i < ClothesNum; i++)
        {
            if (wearingClothes[i].name == "empty") continue;
            if (originalNum > 0 && originalNum < 3 && i == 2) continue;

            for (int j = 0; j < attNum; j++)
            {

                num += (int)wearingClothes[i].attributes[j] * AdsDic[currentAd].attributes[j];

                Debug.Log(wearingClothes[i].name + wearingClothes[i].attributes[j]);
                Debug.Log(AdsDic[currentAd].name + AdsDic[currentAd].attributes[j]);
                Debug.Log("新增粉丝 " + wearingClothes[i].attributes[j] * AdsDic[currentAd].attributes[j]);
            }
        }
        Debug.Log("总共新增粉丝 " + num);

        return (int)num;
    }
}

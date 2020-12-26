using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class AdsController : MonoBehaviour
{
    // Start is called before the first frame update

    public HashSet<string> backgrounds = new HashSet<string>();
    private Dictionary<string, Ad> AdsDic = new Dictionary<string, Ad>();
    private List<Sprite> poses = new List<Sprite>();



    private string AdsPath = "Images/Poster/Background/Chapt1";
    private string PosePath = "Images/Karara/Pose/Chapt1";

    //hard coding
    // 1 top only 2 top + bottom  3 (top + bottom)/Everything + shoes 
    private List<int> ClothesNumShownInPose = new List<int> { 1, 3, 2, 1};

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
    private Cloth[] wearingClothes = new Cloth[4] { new Cloth(), new Cloth(), new Cloth(), new Cloth()};
    [SerializeField]
    private SpriteRenderer[] ImagesInPosture = new SpriteRenderer[4];



    public List<int> usedPoses = new List<int>();
    public HashSet<string> usedAds = new HashSet<string>();

    [SerializeField]
    private GameObject[] AdsInSubway;
    int nextAdIdx = 2; //第三个

    void Start()
    {
        
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        SpriteLoader = GameObject.Find("---SpriteLoader").GetComponent<SpriteLoader>();


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
            if(atbs == null) atbs = "a 1 1 1 1";
            AdsDic.Add(newPic.name, new Ad(newPic.name, newPic,atbs));

        }

        //writer.Close();
        loadedPics = Resources.LoadAll(PosePath, typeof(Sprite));
        for(int i = 0; i < loadedPics.Length; i++)
        {
            poses.Add((Sprite)loadedPics[i]);
        }


        wearingClothes[3] = SpriteLoader.defaultClothes[3];
        wearingClothes[2] = SpriteLoader.defaultClothes[2];

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

        ResetPosture();

        // todo：换衣服

        ChangeClothEnterAds();
        //=========

        FinalCameraController.ChangeToAdvertisement();

    }



    public void ResetPosture()
    {
        currentPoseIdx = 0;
        bodyImage.sprite = poses[currentPoseIdx];

    }



    public void ChangePosture()
    {
       
        currentPoseIdx++;
        if (currentPoseIdx >= poses.Count) currentPoseIdx = 0;
        bodyImage.sprite = poses[currentPoseIdx];

        ChangeClothEnterAds();
    }

    public void ChangeClothInAds(int typeIdx,Cloth cloth)
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
        for (int i = 0; i < wearingClothes.Length; i ++)
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
    }



    public void UpdatePosters()
    {
        int adsNumInSubway = 0;
        foreach(GameObject ad in AdsInSubway)
        {
            if (ad.active) adsNumInSubway++;
        }

        if (adsNumInSubway -2 >= usedAds.Count) return;

        if(nextAdIdx < AdsInSubway.Length) AdsInSubway[nextAdIdx].SetActive(true);
        nextAdIdx++;
    }


    public int GetNewFollowerNum()
    {
        int num = 0;

        int attNum = 4;
        //先确定这个pose时展示了多少件东西 1= top only 2 = top+bottom

        int ClothesNum = ClothesNumShownInPose[currentPoseIdx];
        int originalNum = ClothesNum;
        // 有时候只能看见everything


        if(ClothesNum > 0 && wearingClothes[3].name!= "empty")
        {
            ClothesNum = 4;
        }


        for(int i = 0; i < ClothesNum; i++)
        {
            if(wearingClothes[i].name == "empty") continue;
            if (originalNum > 0 && originalNum < 3 && i == 2) continue;

            for (int j = 0; j <attNum; j ++)
            {

                num += wearingClothes[i].attributes[j] * AdsDic[currentAd].attributes[j];

                //Debug.Log(wearingClothes[i].name + wearingClothes[i].attributes[j]);
                //Debug.Log(AdsDic[currentAd].name + AdsDic[currentAd].attributes[j]);
                //Debug.Log("新增粉丝 " + wearingClothes[i].attributes[j] * AdsDic[currentAd].attributes[j]);
            }
        }
        Debug.Log("总共新增粉丝 " + num);

        return num;
    }
}

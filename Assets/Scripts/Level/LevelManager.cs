﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
// using I2.Loc;



public class LevelManager : MonoBehaviour
{

    [SerializeField]
    private GameObject MapHint, MapTutorialBubble, GoBackButton, transitComic, MapCar, MapCar_Tut_S2;

    [SerializeField]
    private CanvasGroup Map;

    [SerializeField]
    private GameObject MapInSubway;

    //    public CanvasGroup startScreen;
    public CanvasGroup chapterOne;
    public GameObject hintArrow;
    public CanvasGroup GoBackSubway;
    //private CanvasGroup hintArrowCG;
    //public CanvasGroup clear;
    public CanvasGroup subwayCG;
    public CanvasGroup Stations;
    private CanvasGroup carCG;
    private PathFollower PathFollower;


    public GameObject station, bagIn;
    public Sprite bagOut;
    private FinalCameraController FinalCameraController;
    private AdsController AdsController;


    public CanvasGroup arrowButton;

    public GameObject StationDetail;
    public TextMeshProUGUI instructionText;

    public bool isInstruction = false;
    private Vector2 stationV = new Vector2(-50, 80);

    public GameObject InstructionScroll, closeInstructionButton;
    //[SerializeField]
    //private Button closeInstructionButton;
    public bool skip{
        get{return fakeMatch;}
        set{fakeMatch = value;}
        }
    public GameObject fishBubble;
    public TextMeshPro fishText;

    private SubwayMovement SubwayMovement;
    private RatingSystem RatingSystem;
    // Start is called before the first frame update

    private bool HintShowing = false;
    private bool Practicing = false;
    public bool fakeMatch = false;

    public int stage = 1;
    FishBoss FishBoss;


    public bool upgradeReadyOrNot = false;

    public bool stageTransiting = false;
    int comicClick = 0;

    public Sprite[] comics1, comics2;

    List<List<Sprite>> comicList = new List<List<Sprite>>();
    public bool neverGoLandF = true;

    public bool neverGoMap = true;
    public int countStationS2 = 0;


    [Header("stage 3")]
    public GameObject matchButton;
    public int targetStationS2;

    [Header("UI Rate")]
    public bool UIRateShown = false;

    [Header("Station Scroll")]
    public bool FishReturnBagShown = false;
    public bool LandFinLeaveStation = false;
    public GameObject FishBagReturnComic;
    public bool ending = false;
    public GameObject LostAndFoundReturnComic, waypoint1;
    private InstagramController InstagramController;
    private LostAndFound LostAndFound;

    public GameObject[] MiniNotices;
    public GameObject vibration;
    enum returnBagState
    {
        stageOne
    }


    private void Reset()
    {   
        ///use this function to inactivate gameObjects and canvasgroups

    }
    void Start()
    {
        Resources.UnloadUnusedAssets();

        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        SubwayMovement = GameObject.Find("---StationController").GetComponent<SubwayMovement>();
        RatingSystem = GameObject.Find("FloatingUI").GetComponent<RatingSystem>();
        LostAndFound = GameObject.Find("Lost&Found_basket").GetComponent<LostAndFound>();
        FishBoss = GameObject.Find("---FishBoss").GetComponent<FishBoss>();
        InstagramController = GameObject.Find("---InstagramController").GetComponent<InstagramController>();
        AdsController = GameObject.Find("---AdsController").GetComponent<AdsController>();

        PathFollower = MapCar.GetComponent<PathFollower>();
        carCG = MapCar.GetComponent<CanvasGroup>();

        int skipInstruction = PlayerPrefs.GetInt("skip", -1);
        if (skipInstruction == 1) skip = true;

        GameObject.Find("Main Camera").transform.position = new Vector3(0, 0, -20);

        // if (!skip)
        // {

        // }
        // else
        // {
        //     //FinalCameraController.myCameraState = CameraState.Subway;
        //     //ShowRatingSys(false);
        //     //CloseInstruction();
        //     EndMapTutorial();
        // }
        if (skip) SubwayMovement.trainStop();


        comicList.Add(new List<Sprite>());
        comicList[0].AddRange(comics1);
        comicList.Add(new List<Sprite>());
        comicList[1].AddRange(comics2);

    }

    // Update is called once per frame
    void Update()
    {
        //只有滚到底的时候才会显示关闭按钮





        //只有滚到底的时候才会显示关闭按钮
        if (isInstruction && InstructionScroll.active &&
            InstructionScroll.GetComponent<ScrollRect>().verticalNormalizedPosition < 0.3f)
        {
            closeInstructionButton.GetComponent<Button>().interactable = true;
        }

        if (Practicing) CheckMatch();


    }


    public void UpdateStage()
    {
        Debug.Log("let's see ");
        // Hide(transitComic.GetComponent<CanvasGroup>());
        Stage1EndingComicWindow.SetActive(false);
        HideHint();
        MapCar_Tut_S2.SetActive(false);
        Show(MapCar.GetComponent<CanvasGroup>());
        isInstruction = false;
        stageTransiting = false;
        comicClick = 0;
        upgradeReadyOrNot = false;
        stageTransiting = false;
        SubwayMovement.pauseBeforeMove = false;

        stage++;

        //Shuyi-进到新的stage后直接出现一张海报
        AdsController.UpdatePosters();
        


        //ShowRatingSys(true);


        return;


    }

    public Sprite demoEndPost;
    public GameObject endingParticleEffect,endingPopupWindow,InsParticleParent;
    public bool vibrationHold = false;
    public void StageTrasiting()
    {
        stageTransiting = true;
        if (stage == 1)
        {
            // Show(transitComic.GetComponent<CanvasGroup>());
            Stage1EndingComicWindow.SetActive(true);
            Debug.Log("stransiting 1");
        }

        if (stage == 2)
        {
            Debug.Log("transiting 2");
            //@ demo stop at here
            // Show(transitComic.GetComponent<CanvasGroup>());
            //终点

            ending = true;

            if(FinalCameraController.myCameraState == CameraState.Subway)
            {
                StartEndingProcess();
            }
            else{
                vibrationHold = true;
            }

            if(FinalCameraController.myCameraState != CameraState.App)
                InstagramController.AddInsPost("A", demoEndPost);

            SubwayMovement.pauseBeforeMove = true;
            
        }

    }

    public void StartEndingProcess()
    {
        FinalCameraController.enableScroll = false;
        FinalCameraController.DisableInput(true);
        Handheld.Vibrate();
        vibration.SetActive(true);
        
    }
    public IEnumerator MoveParticleEffect()
    {
        FinalCameraController.DisableInput(false);
        endingParticleEffect.SetActive(true);
        endingParticleEffect.transform.localPosition = new Vector3(0,0,0);
        endingParticleEffect.transform.SetParent(InsParticleParent.transform);
        
        yield return new WaitForSeconds(1f);
        endingParticleEffect.SetActive(false);
        
    }

    public void ClickComic()
    {

        transitComic.GetComponent<Image>().sprite = comicList[stage - 1][comicClick];
        comicClick++;
        if (comicClick == comicList[stage - 1].Count)
        {
            // 前置教程
            if (stage == 1) Tut_S2();
            //@ demo stop at here
            // if (stage == 2) Tut_S3();
        }

        Debug.Log("comic click" + comicClick);
    }

    private void CheckMatch()
    {
        //todo: 
        if (fakeMatch)
        {
            Practicing = false;

        }
    }

    public void ShowHint()
    {
        HintShowing = true;
        MapHint.SetActive(true);
    }

    public void HideHint()
    {
        HintShowing = false;
        MapHint.SetActive(false);
    }

    public void HideBubble()
    {
        MapTutorialBubble.SetActive(false);
    }

    public void ShowInstructionInMap()
    {

        if (!isInstruction) return;

        if (stage == 2 && stageTransiting)
        {
            StartCoroutine(EnterStationDetail_Tut_S3());
        }


        HideHint();
        HideBubble();

        //Practicing = true;



        Debug.Log("show instruction in map");
    }



    private void ShowRatingSys(bool withParticleEffect)
    {
        RatingSystem.ShowRatingSys(withParticleEffect);


    }

    // 如果是chap 1 开屏在地铁内显示图片instruction 关闭时用这个
    public void CloseInstruction()
    {

        //if (Practicing) return;
        ////StationDetail.SetActive(false);
        ////GoBackButton.SetActive(true);

        if (stageTransiting && stage == 1)
        {
            // ShowHint();
            // MapHint.gameObject.transform.parent = GoBackButton.transform;
            // MapHint.gameObject.transform.localPosition = new Vector3(20f, 0, 0);
            GoBackButton.SetActive(true);
            isInstruction = false;

        }

    }

    public void EndMapTutorial()
    {
        // click back to subway button from map

        if (stageTransiting)
        {
            UpdateStage();
            SubwayMovement.EndTrainPause();
        }

        //SubwayMovement.timerStay = SubwayMovement.stayTime - 0.01f;
    }

    public void Tut_S2()
    {
        // map animaiton
        //// block SCROLLING
        Hide(transitComic.GetComponent<CanvasGroup>());
        comicClick = 0;
        HideHint();

        FinalCameraController.ChangeToSubway();
        FinalCameraController.GotoPage(3);
        FinalCameraController.ChangeToMap();
        // FinalCameraController.GotoPage(3);
        // Animator myAnim = MapInSubway.GetComponent<Animator>();
        // myAnim.SetTrigger("blingbling");
        // FinalCameraController.enableScroll = false;

    }

    public void Tut_S3()
    {
        // map animaiton
        //// block SCROLLING
        Hide(transitComic.GetComponent<CanvasGroup>());
        comicClick = 0;
        HideHint();

        // todo: 看看该不该蹦出来漫画

        FinalCameraController.ChangeToSubway();
        FinalCameraController.GotoPage(3);



        Animator myAnim = MapInSubway.GetComponent<Animator>();
        myAnim.SetTrigger("blingbling");

    }


    public IEnumerator EnterMap_Tut_S2()
    {
        //进入map教学阶段后，教学用地铁持续播放转圈+教学包，直到点击确认后进入下一阶段
        isInstruction = true;
        

        MapTutorialBubble.SetActive(true);

        Animator myAnim = MapInSubway.GetComponent<Animator>();
        myAnim.SetTrigger("idle");

        FinalCameraController.enableScroll = true;


        GoBackButton.SetActive(false);
        Hide(MapCar.GetComponent<CanvasGroup>());
        MapCar_Tut_S2.SetActive(true);

        yield return new WaitForSeconds(2f);

        // ShowHint();
        // MapTutorialBubble.SetActive(false);

    }

    public void ClickMapTutorialBubble()
    {
        ShowHint();
        MapCar_Tut_S2.SetActive(false);

        MapTutorialBubble.SetActive(false);
    }

    public IEnumerator EnterMap_Tut_S3()
    {

        isInstruction = true;
        Animator myAnim = MapInSubway.GetComponent<Animator>();
        myAnim.SetTrigger("idle");


        //car animation
        MapHint.gameObject.transform.parent = waypoint1.transform;
        MapHint.gameObject.transform.localPosition = new Vector3(40f, -30f, 0);

        ShowHint();


        GoBackButton.SetActive(true);
        GoBackButton.GetComponent<Button>().enabled = false;
        yield return null;


    }

    public IEnumerator EnterStationDetail_Tut_S3()
    {


        Debug.Log("LETS SEE THE TRANSITION????");

        Animator myAnim = MapCar.GetComponent<Animator>();
        myAnim.SetTrigger("idle");

        myAnim = matchButton.GetComponent<Animator>();
        myAnim.SetTrigger("blingbling");
        HideHint();

        yield return null;
    }



    public IEnumerator EnterMatch_Tut_S3()
    {
        // called from StationForButton.StationDetail()
        Debug.Log("?? EnterMatch_Tut_S3");
        Animator myAnim = matchButton.GetComponent<Animator>();
        myAnim.SetTrigger("idle");

        yield return new WaitForSeconds(3f);

        myAnim.SetTrigger("blingbling");

    }


    public void EndMatch_Tut_S3()
    {
        // called from StationForButton.StationDetail()
        //弹窗漫画
        Debug.Log("?? EndMatch_Tut_S3");
        GoBackButton.GetComponent<Button>().enabled = true;
        isInstruction = false;
        Animator myAnim = matchButton.GetComponent<Animator>();
        myAnim.SetTrigger("idle");


    }

    public void ShowUIRate()
    {
        if (stage < 2 || UIRateShown) return;

        UIRateShown = true;
        RatingSystem.ShowRatingSys(true); // with particle effect

    }

    public void ShowFishReturnBagComic()
    {

        FishReturnBagShown = true;
        FishBagReturnComic.SetActive(true);
        // show comic and confirm
    }



    public void ShowLFReturnComic()
    {

        LostAndFoundReturnComic.SetActive(true);
    }
    public void ConfirmLFReturnComic()
    {
        // ShowUIRate();
        LostAndFoundReturnComic.SetActive(false);
    }

    IEnumerator ShowInstruction()
    {
        FinalCameraController.enableScroll = false;

        yield return new WaitForSeconds(2f);

        FinalCameraController.InstructionShow();
    }

    private bool stage1Returned = false;
    public bool stage2UIRateFirst = false;
    public IEnumerator StopToMinusStar( GameObject notice, int absentClothNum, int oneStarForBag)
    {
        

        if(stage == 1)
        {
            // notice.GetComponent<ReturnNotice>().SetStarNum(0);
            notice.GetComponent<ReturnNotice>().HideStarSign();
            notice.GetComponent<ReturnNotice>().SetClothNum(absentClothNum);
            if(absentClothNum > -1) yield break;

            if(!stage1Returned)
            {
                stage1Returned = true;
                FishBoss.ResetFish();
                FishBoss.FishTalk("A",false,false);
                FinalCameraController.Show(FinalCameraController.fishShoutCG);
            }
            
        }
        else if(stage == 2)
        {
            if(absentClothNum == 0 && oneStarForBag == 1) {
                notice.GetComponent<ReturnNotice>().SetStarNum(1);
                notice.GetComponent<ReturnNotice>().SetClothNum(0);
            }
            else
            {
                notice.GetComponent<ReturnNotice>().SetStarNum(absentClothNum);
                notice.GetComponent<ReturnNotice>().SetClothNum(absentClothNum);
            }
            
            if(absentClothNum > -1) yield break;
            if(!UIRateShown)
            {
                //强制滚动未开+stage2第一次还包
                stage2UIRateFirst = true;
                FinalCameraController.DisableInput(true);
                FinalCameraController.enableScroll = false;
                yield return new WaitForSeconds(1.5f);
                ShowUIRate();
                yield return new WaitForSeconds(1.5f);
                FishBoss.ResetFish();
                FishBoss.FishTalk("A",false,false);
                // FinalCameraController.FishTalkAccessFromScript("A",true);
                if(absentClothNum < 0) FinalCameraController.Show(FinalCameraController.fishShoutCG);
                FinalCameraController.DisableInput(false);
                FinalCameraController.enableScroll = true;
            }
            
           
        }
        yield return null;

    }

    
    public IEnumerator GoLostAndFoundFirstTime()
    {
        FinalCameraController.enableScroll =false;
        FinalCameraController.DisableInput(true);

        yield return new WaitForSeconds(0.3f);
        SubwayMovement.BlackScreen.SetActive(true);

        FinalCameraController.ChangeToSubway();
        FinalCameraController.GotoPage(4);

        yield return new WaitForSeconds(1f);
        SubwayMovement.BlackScreen.SetActive(false);
        LostAndFound.transform.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        LostAndFound.ShaderOn();

        yield return new WaitForSeconds(2f);
        LostAndFound.clickLostFound();

        yield return new WaitForSeconds(2f);
        SubwayMovement.BlackScreen.SetActive(true);//不知道为啥会造成闪烁
        FinalCameraController.ChangeToCloth();
        FinalCameraController.CameraMovement.previousPage = 2;

        yield return new WaitForSeconds(2f);
        SubwayMovement.BlackScreen.SetActive(false);
        LostAndFound.clickLostFound();

        FinalCameraController.enableScroll = true;
        FinalCameraController.DisableInput(false);
        

    }
    private IEnumerator FishTalkClothesInInventory()
    {
        FishBoss.ResetFish();
        FishBoss.FishTalk("Put all clothes back into bag  before return it! Or we'll get bad ratings!",false,false);
        // FinalCameraController.FishTalkAccessFromScript("Put all clothes back into bag  before return it! Or we'll get bad ratings!",true);
        yield return new WaitForSeconds(2f);
        FishBoss.FishTalk("",false,false);
    }

    public GameObject Stage1EndingComicWindow;
    public void ClickStage1EndingComicYes()
    {
        Stage1EndingComicWindow.SetActive(false);
        Tut_S2();
    }
    private bool lastTextFinish;


     public void ClickEndingYes()
    {
         SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void Hide(CanvasGroup UIGroup)
    {
        UIGroup.alpha = 0f; //this makes everything transparent
        UIGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
        UIGroup.interactable = false;
    }

    public void Show(CanvasGroup UIGroup)
    {
        UIGroup.alpha = 1f;
        UIGroup.blocksRaycasts = true;
        UIGroup.interactable = true;
    }



}

﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using I2.Loc;



public class LevelManager : MonoBehaviour
{

    [SerializeField]
    private GameObject MapHint,MapTutorialBubble,GoBackButton, transitComic,MapCar,MapCar_Tut_S2;

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
    
    
    public GameObject station,bagIn;
    public Sprite bagOut;
    private FinalCameraController FinalCameraController;

    public CanvasGroup arrowButton;

    public GameObject StationDetail;
    public TextMeshProUGUI instructionText;

    public bool isInstruction = false;
    private Vector2 stationV = new Vector2(-50, 80);

    public GameObject InstructionScroll, closeInstructionButton;
    //[SerializeField]
    //private Button closeInstructionButton;
    public bool skip;
    public GameObject fishBubble;
    public TextMeshPro fishText;

    private SubwayMovement SubwayMovement;
    private RatingSystem RatingSystem;
    // Start is called before the first frame update

    private bool HintShowing = false;
    private bool Practicing = false;
    public bool fakeMatch = false;

    public int stage = 1;
    FishBossNotification FishBossNotification;


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
    
    public GameObject LostAndFoundReturnComic,waypoint1;
    void Start()
    {
        Resources.UnloadUnusedAssets();

        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        SubwayMovement = GameObject.Find("---StationController").GetComponent<SubwayMovement>();
        RatingSystem = GameObject.Find("FloatingUI").GetComponent<RatingSystem>();
        FishBossNotification = GameObject.Find("FishBossUI").GetComponent<FishBossNotification>();



        PathFollower = MapCar.GetComponent<PathFollower>();
        carCG = MapCar.GetComponent<CanvasGroup>();

        int skipInstruction = PlayerPrefs.GetInt("skip",-1);
        if(skipInstruction == 1) skip = true;

        GameObject.Find("Main Camera").transform.position = new Vector3(0, 0, -20);

        if (!skip)
        {

        }
        else
        {
            //FinalCameraController.myCameraState = FinalCameraController.CameraState.Subway;
            //ShowRatingSys(false);
            //CloseInstruction();
            EndMapTutorial();
        }


        


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
        if (isInstruction&& InstructionScroll.active &&
            InstructionScroll.GetComponent<ScrollRect>().verticalNormalizedPosition < 0.3f)
        {
            closeInstructionButton.GetComponent<Button>().interactable = true;
        }

        if (Practicing) CheckMatch();

        
    }


    public void UpdateStage()
    {
        Debug.Log("let's see ");
        Hide(transitComic.GetComponent<CanvasGroup>());
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


        //ShowRatingSys(true);


        return;
             

    }
    public void StageTrasiting()
    {
        stageTransiting = true;
        if (stage == 1)
        {
            Show(transitComic.GetComponent<CanvasGroup>());
            Debug.Log("stransiting 1");
        }

        if (stage == 2)
        {
            Debug.Log("transiting 2");
            Show(transitComic.GetComponent<CanvasGroup>());
        }

    }

    public void ClickComic()
    {

        transitComic.GetComponent<Image>().sprite = comicList[stage - 1][comicClick];
        comicClick++;
        if (comicClick == comicList[stage - 1].Count)
        {
            // 前置教程
            if (stage == 1) Tut_S2();
            if (stage == 2) Tut_S3();
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

        if(stage == 2 && stageTransiting)
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
            ShowHint();
            MapHint.gameObject.transform.parent = GoBackButton.transform;
            MapHint.gameObject.transform.localPosition = new Vector3(20f, 0, 0);
            GoBackButton.SetActive(true);
            isInstruction = false;

        }

    }

    public void EndMapTutorial()
    {
        // click back to subway button from map

        if(stageTransiting)
        {
            UpdateStage();
        }

       
        SubwayMovement.trainStop();
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
        Animator myAnim = MapInSubway.GetComponent<Animator>();
        myAnim.SetTrigger("blingbling");
        FinalCameraController.enableScroll = false;

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
        
        isInstruction = true;
        Animator myAnim = MapInSubway.GetComponent<Animator>();
        myAnim.SetTrigger("idle");

        FinalCameraController.enableScroll = true;


        GoBackButton.SetActive(false);
        Hide(MapCar.GetComponent<CanvasGroup>());
        MapCar_Tut_S2.SetActive(true);
        MapTutorialBubble.SetActive(true);

        yield return new WaitForSeconds(2f);

        ShowHint();
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

    public IEnumerator CheckUIRateCondition(int rate)
    {
        if (stage < 2 || UIRateShown) yield break;

        if((countStationS2 > 0 && countStationS2 < targetStationS2 && rate == 0)||
           (countStationS2 > targetStationS2))
        {
            
            countStationS2 = -10000;
            UIRateShown = true;

            FinalCameraController.ChangeToSubway();
            FinalCameraController.GotoPage(1);

            yield return new WaitForSeconds(1f);
            RatingSystem.ShowRatingSys(true);

        }

        yield return null;



    }

    public void ShowFishReturnBagComic()
    {

        FishReturnBagShown = true;
        FishBagReturnComic.SetActive(true);
        // show comic and confirm
    }

    public void ConfirmFishBagReturnComic()
    {
        
        FishBagReturnComic.SetActive(false);
        StartCoroutine(SubwayMovement.TrainPauseResume());
    }

    public void ShowLFReturnComic()
    {

        LostAndFoundReturnComic.SetActive(true);
    }
    public void ConfirmLFReturnComic()
    {
        LostAndFoundReturnComic.SetActive(false);
    }

    IEnumerator ShowInstruction()
    {
        FinalCameraController.enableScroll = false;

        yield return new WaitForSeconds(2f);

        FinalCameraController.InstructionShow();
    }

    private bool lastTextFinish;


    
    public void Hide(CanvasGroup UIGroup) {
        UIGroup.alpha = 0f; //this makes everything transparent
        UIGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
        UIGroup.interactable = false;
    }
    
    public void Show(CanvasGroup UIGroup) {
        UIGroup.alpha = 1f;
        UIGroup.blocksRaycasts = true;
        UIGroup.interactable = true;
    }



}

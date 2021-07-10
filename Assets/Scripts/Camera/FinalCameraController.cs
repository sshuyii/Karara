using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class FinalCameraController : MonoBehaviour
{

    public Canvas CanvasPopUpNotice;

    public TouchController TouchController;
    public AllMachines AllMachines;
    public LevelManager LevelManager;
    public InstagramController InstagramController;

    public CanvasGroup disableInputCG, ChapterOneEndComic, Mask, TakePhoto, fishShoutCG,
        ChapterOneFailCG, Inventory, SubwayMap,inventory, appBackground, albumBackground, albumCG, albumDetailCG, frontPage, SavingPage, subPage;
    public int entryTime = 1;
    public bool ChapterOneEnd;

    public bool machineOpen;

    public bool chapterOneSucceed;
    public bool chapterOneFail;

    public int returnMachineNum;//this is used to record the machine that should be turned to when retucn cloth
    private bool fastSwipeBool;

    public TutorialManager TutorialManager;

    public bool isSwipping = false;
    public bool isTutorial;

    public GameObject fishTalk;
    public TextMeshPro fishTalkText;
    private bool isfishTalking = false;

    [HideInInspector]
    public CameraMovement CameraMovement;
    

    //public CameraMovement myHSS;

    //a dictionary of all the clothes that comes from station 0 and are currently in the machines 
    public Dictionary<string, List<Sprite>> AllStationClothList = new Dictionary<string, List<Sprite>>();

    public Sprite startSprite;


    public GameObject instruction, instructionContent, inventoryInstruction, mapInstruction, cameraInstruction, Posture,CheckInstructionButton;
    private GameObject activeInstruction;
    public InventorySlotMgt inventorySlotMgt;



    public bool lateReturnComic;
    public GameObject setting;

    public ScrollRect mainpageScrollRect,albumScrollRect,MPsubScrollRect;
    public bool enableScroll = true;

    private BagsController BagsController;


    
    public enum AppState
    {
        Mainpage,
        KararaPage,
        RetroPage,
        DesignerPage,
        NPCPage,
        SavingPage,
        Post,
        Album,
        AlbumDetail,
        MainpageSub
    }

    public AppState lastAppState;

    public enum CameraState
    {
        Subway,
        Closet,
        Map,
        App,
        Ad
    }

    public enum SubwayState
    {
        One,
        Two,
        Three,
        Four,
        None
    }

    public SubwayState mySubwayState;

    //if notice UI is on display, this is true
    [HideInInspector]
    public bool alreadyNotice = false;
    [HideInInspector]
    public bool alreadyClothUI = false;
    public GameObject currentClothUI;

    private FishTextManager FishTextManager;
    FishText currentFT;


    //public GameObject generatedNotice;



    private bool isLerp = false;

    public CameraState lastCameraState;

    public CameraState myCameraState;
    public AppState myAppState;
    private Vector3 movement;



    public List<CanvasGroup> pageList = new List<CanvasGroup>();

    public RatingSystem RatingSys;

    private bool InventoryInstructionShown = false;
    private bool AdInstructionShown = false;

    public FishBossNotification FishBossNotification;
    public ValueEditor ValueEditor;
    public bool fishShouting = false;

    public Vector3 PosturePos;
    public GameObject phoneFrame;
    
    
    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("PERSISTENT:" + Application.persistentDataPath);  
        if (!isTutorial)
        {
            setting.SetActive(false);
            instruction.SetActive(false);
        }
        //        myCameraState = CameraState.Subway;
        myAppState = AppState.Mainpage;

        CameraMovement = transform.gameObject.GetComponent<CameraMovement>();

        InstagramController = GameObject.Find("---InstagramController").GetComponent<InstagramController>();
        RatingSys = GameObject.Find("FloatingUI").GetComponent<RatingSystem>();
        inventorySlotMgt = GameObject.Find("---InventoryController").GetComponent<InventorySlotMgt>();
        FishBossNotification = GameObject.Find("FishBossUI").GetComponent<FishBossNotification>();
        BagsController = GameObject.Find("---BagsController").GetComponent<BagsController>();
        FishTextManager = GameObject.Find("---FishTextManager").GetComponent<FishTextManager>();
        ValueEditor = GameObject.Find("---ValueEditor").GetComponent<ValueEditor>();
        FishBossNotification.HideFish();
   

        fishTalk.SetActive(false);
        Hide(frontPage);
        Hide(TakePhoto);
        Posture.SetActive(false);



        fishTalkText = fishTalk.gameObject.GetComponentInChildren<TextMeshPro>();
        Hide(Inventory);
        Hide(fishShoutCG);

        ChapterOneEnd = false;

        alreadyClothUI = false;
    }


public void CancelAllUI(bool clickMachine)
    {
        inventorySlotMgt.CloseAllUI();
        if (!clickMachine) AllMachines.CloseAllMachines();
        BagsController.HideNotice();
        
        InstructionDismiss();

        Hide(TakePhoto);
        Hide(SubwayMap);
        Hide(Inventory);
        Hide(frontPage);
        Hide(appBackground);
        Hide(albumBackground);
        Hide(albumCG);
        Hide(albumDetailCG);
        Hide(SavingPage);


        isShown = false;
        
    }
    public void CancelAllUI(bool clickMachine, CanvasGroup skipCG)
    {
        inventorySlotMgt.CloseAllUI();
        if (!clickMachine) AllMachines.CloseAllMachines();
        BagsController.HideNotice();
        
        InstructionDismiss();

        List<CanvasGroup> allCG = new List<CanvasGroup>(){TakePhoto,SubwayMap,Inventory,frontPage,appBackground,
            albumBackground,albumCG,SavingPage};
        foreach(CanvasGroup cg in allCG)
        {
            if(skipCG != cg) Hide(cg);
        }

        Hide(TakePhoto);
        Hide(SubwayMap);
        Hide(Inventory);
        Hide(frontPage);
        Hide(appBackground);
        Hide(albumBackground);
        Hide(albumCG);
        Hide(albumDetailCG);
        Hide(SavingPage);


        isShown = false;
        
    }

    

    // Update is called once per frame
    void Update()
    {

        ChapterEndChecker();


        // if(fishShouting && myCameraState == CameraState.Subway && CameraMovement.currentPage == 1)
        // {
        //     fishShoutCG.alpha = 0f;
        // }
        // else if(fishShouting){
        //     fishShoutCG.alpha = 1f;
        // }


        if (TouchController.myInputState == TouchController.InputState.LeftSwipe||
            TouchController.myInputState == TouchController.InputState.RightSwipe)
        {
            isSwipping = true;
        }
        else
        {
            isSwipping = false;
        }

        if (isSwipping && !machineOpen)
        {
            Debug.Log("????");
            CancelAllUI(false);
        }

        ////print(CameraMovement.CurrentPage);
        //change camera state to page number

        if (myCameraState == CameraState.Subway && !isSwipping)
        {
            CheckScreenNum();

            if (transform.position.x < 5 && transform.position.x > -5 )
            {
                if(FishBossNotification.isActive)FishBossNotification.HideFish();
                fishShoutCG.alpha = 0f;
            }
            else if (transform.position.x > 5 )
            {
                if(fishShouting)
                {
                    fishShoutCG.alpha = 1f;
                }
                //FishBossNotification.ShowFish();
            }

            //Show(subwayBackground);
        }
        else if (myCameraState != CameraState.Subway && FishBossNotification.isActive)
        {
            FishBossNotification.HideFish();
        }



        //if changing clothes, don't show some UIs
        if (myCameraState == CameraState.Map || myCameraState == CameraState.App || myCameraState == CameraState.Ad)
        {
            if (myCameraState == CameraState.App)
            {
                Hide(inventory);
                //Hide(basicUI);
                Show(appBackground);
            }
            else
            {
                Hide(inventory);
                //Hide(basicUI);
                Hide(appBackground);
            }
        }

        else if (myCameraState == CameraState.Closet)
        {
            Show(inventory);
            //Hide(basicUI);
            //Hide(subwayBackground);
            Hide(appBackground);

        }

        else
        {
            Hide(inventory);
            //Show(basicUI);
            Hide(appBackground);
        }

        //如果没到达30fo
        if (chapterOneFail)
        {
            StartCoroutine(ChapterOneFailComic());
            PlayerPrefs.SetInt("ongoingChapter", 1);
            PlayerPrefs.SetInt("skipInstruction", 0);
            PlayerPrefs.Save();
        }

        
    }

    IEnumerator ChapterOneFailComic()
    {
        DisableInput(true);
        yield return new WaitForSeconds(1f);

        ChangeToSubway();
        CameraMovement.Go2Page(1);


        Show(ChapterOneFailCG);
    }

    public void BossTalk()
    {
        
        if (isfishTalking == false)
        {
            fishTalk.SetActive(true);
            FishTalk("Concentrate");
        }
        else fishTalk.SetActive(false);
        isfishTalking = !isfishTalking;
    }

    public void FishTalkAccessFromScript(string keyWord, bool reset)
    {
        // CancelAllUI(false);
        if(reset) isfishTalking = false;
        if (isfishTalking == false)
        {
            fishTalk.SetActive(true);
            FishTalk(keyWord);
        }
        else fishTalk.SetActive(false);
        isfishTalking = !isfishTalking;

    }

    public void FishTalk(string keyWord)
    {
        currentFT = FishTextManager.GetText(keyWord);
        int idx = Random.Range(0, currentFT.content.Count);
        fishTalkText.text = currentFT.content[currentFT.playingIdx];
    }

    

    public void ClickWhileFishTalking()
    {

    }

        void CheckScreenNum()
    {

        switch (CameraMovement.currentPage)
        {
            case 1:
                mySubwayState = SubwayState.One;
                break;
            case 2:
                mySubwayState = SubwayState.Two;
                break;
            case 3:
                mySubwayState = SubwayState.Three;
                break;
            case 4:
                mySubwayState = SubwayState.Four;
                break;
            default:
                mySubwayState = SubwayState.None;
                break;


        }

    }

    public bool isShown = true;
    public void clickKarara()
    {

        Debug.Log("click KARARA");
        
    }

    public void ChangeToCloth()
    {
        if(myCameraState != CameraState.Subway)
        {
            return;
        }
        CancelAllUI(false,inventory);
  
        myCameraState = CameraState.Closet;
        CameraMovement.atInventoryOrAd = true;
        // phone.SetActive(false);
        CameraMovement.previousPage = CameraMovement.currentPage;
        RatingSys.LeaveSubway(false);
        Show(Inventory);


        // Mask.alpha = 0;
        if (!isTutorial) CheckInstructionButton.SetActive(true);


        lastCameraState = myCameraState;
        transform.position = new Vector3(-25, 0, -10);
        if (!InventoryInstructionShown)
        {

            StartCoroutine(WaitingInstruction());

        }

    }

    public void clickLateComic()
    {
        //        lateReturnComic = false;
        //        lateReturnImage.enabled = false;
        // Hide(fishTalk);
        fishTalk.SetActive(false);

        //print("clickLateComic");
    }


    [SerializeField]
    private StationForButton StationForButton;
    public void ClickInvisibleBackButton()
    {
        
        if(myCameraState == CameraState.App)
        {
            AppBackButton();
        }
        else if(myCameraState == CameraState.Map)
        {
            ChangeToSubway();
            LevelManager.EndMapTutorial();
            StationForButton.ExitMap();
            
        }
        else if(myCameraState == CameraState.Ad)
        {
            ChangeToSubway();
        }
        
    }

    public void AppBackButton()
    {
        if (myAppState == AppState.Album)
        {
            Hide(albumCG);
            Hide(albumBackground);
            lastCameraState = CameraState.App;
            ChangeToSubway();
            //}
        }
        else if (myAppState == AppState.Mainpage)
        {
            if (ChapterOneEnd)//第一章结束，出现漫画？如果点了一下之后进入第二章
            {
                if (chapterOneSucceed)
                {
                    Show(ChapterOneEndComic); //漫画
                    PlayerPrefs.SetInt("ongoingChapter", 2);
                    PlayerPrefs.SetInt("skipInstruction", 1);
                    PlayerPrefs.Save();
                }

            }
            else
            {
                lastCameraState = CameraState.App;
                ChangeToSubway();
            }


        }

        else if (myAppState == AppState.AlbumDetail)
        {
            Hide(albumDetailCG);
            Hide(albumBackground);
            lastCameraState = CameraState.Subway;
            ChangeToSubway();
        }
       
        else if (myAppState == AppState.MainpageSub)
        {
            ReturnFromSub();
        }
        else if (myAppState == AppState.SavingPage)
        {
            //to do 待定
            Hide(SavingPage);
            lastCameraState = CameraState.Subway;
            ChangeToSubway();
        }
    }





    IEnumerator ScrollToTop(ScrollRect scrollRect)
    {
        yield return null;
        scrollRect.normalizedPosition= new Vector2(0, 1);
        Debug.Log("scroll" + scrollRect.normalizedPosition);
    }

    public void ChangeToSubway()
    {
        //if (LevelManager.isInstruction) return;
        //if(LevelManager.stageTransiting)
        //{
        //    LevelManager.UpdateStage();
        //}

        // phone.SetActive(true);
        CameraMovement.atInventoryOrAd = false;
        CameraMovement.JumpToPreviousPage();
        if (!isTutorial) CheckInstructionButton.SetActive(false);
        // Mask.alpha = 1;





        Hide(TakePhoto);
        Posture.SetActive(false);
        //transform.position = new Vector3(0, 0, -10);
        if (myCameraState == CameraState.Closet || myCameraState == CameraState.Map ||
            myCameraState == CameraState.App || myCameraState == CameraState.Ad)
        {

            if (lastCameraState != CameraState.Closet && lastCameraState != CameraState.Map &&
                lastCameraState != CameraState.App && myCameraState != CameraState.Ad)
            {
                myCameraState = lastCameraState;
                RatingSys.GoBackToSubway();
                phoneFrame.SetActive(false);
                //todo: kind of connfused
            }
            else
            {   if(myCameraState == CameraState.App && LevelManager.ending && !LevelManager.vibrationHold)
                {
                    LevelManager.endingPopupWindow.SetActive(true);
                    return;
                }

                if(LevelManager.vibrationHold)
                {
                    LevelManager.StartEndingProcess();
                    LevelManager.vibrationHold = false;
                }

                
                myCameraState = CameraState.Subway;
                RatingSys.GoBackToSubway();
                phoneFrame.SetActive(false);
            }

            
        }


        //hide everything 
        Hide(Inventory);
        //Show(subwayBackground);
        Hide(frontPage);
        Hide(appBackground);
        //Hide(NPCPage);
        Hide(SubwayMap);
        //Hide(postpage);


        if (InstagramController.waitingForRefresh && LevelManager.stage > 2) InstagramController.RefreshPost();

    }

    public GameObject DemoEndButton;
    public void ChangeToApp()
    {   
        
        if(LevelManager.ending) {
            LevelManager.MoveParticleEffect();
        }

        
        if (myCameraState != CameraState.Subway && myCameraState != CameraState.App)
        {
            return;
        }



        CancelAllUI(false);

        // phone.SetActive(false);
        Hide(fishShoutCG);
        CheckInstructionButton.SetActive(false);
 
        StartCoroutine(ScrollToTop(mainpageScrollRect));
        StartCoroutine(ScrollToTop(albumScrollRect));


        InstagramController.redDot.SetActive(false);

        Hide(SavingPage);
        Hide(TakePhoto);

        lastCameraState = myCameraState;
        myCameraState = CameraState.App;
        RatingSys.LeaveSubway(true);
        phoneFrame.SetActive(true); 
        myAppState = AppState.Mainpage;
        Show(frontPage);
        Show(appBackground);
              
        Debug.Log("change to app0");
        
        
    }



    public void MainPageToAlbum()
    {
        StartCoroutine(ScrollToTop(albumScrollRect));
        Show(albumBackground);
        Show(albumCG);
        Hide(appBackground);
        Hide(frontPage);
        myAppState = AppState.Album;
    }

    private bool FromMatch2Sub = false;
    public void ReturnFromSub()
    {
        if(FromMatch2Sub) SubToMatch();
        else SubToMainPage(); 
    }

    public void MatchToSub()
    {
        StartCoroutine(ScrollToTop(MPsubScrollRect));
        Show(subPage);
        Show(appBackground);
        Hide(frontPage);
        Hide(SubwayMap);
        lastCameraState = myCameraState;
        myCameraState = CameraState.App;
        myAppState = AppState.MainpageSub;
        FromMatch2Sub = true;
    }

    public void SubToMatch()
    {

        Hide(subPage);
        Hide(appBackground);
        Show(SubwayMap);
        lastCameraState = myCameraState;
        myCameraState = CameraState.Map;
        FromMatch2Sub = false;
    }

    public void MainPageToSub()
    {
        StartCoroutine(ScrollToTop(MPsubScrollRect));
        Show(subPage);
        Hide(frontPage);
        myAppState = AppState.MainpageSub;
    }


    public void SubToMainPage()
    {
        StartCoroutine(ScrollToTop(mainpageScrollRect));
        Show(frontPage);
        Hide(subPage);
        myAppState = AppState.Mainpage;
    }
    public void AlbumToMainPage()
    {
        StartCoroutine(ScrollToTop(mainpageScrollRect));
        Hide(albumBackground);
        Hide(albumCG);
        Hide(albumDetailCG);
        Show(appBackground);
        Show(frontPage);
        myAppState = AppState.Mainpage;
    }


    public void AlbumToLargePicture()
    {
        Hide(albumCG);
        Show(albumDetailCG);
    }

    public void LargePictureToAlbum()
    {
        Hide(albumDetailCG);
        Show(albumCG);
    }


    public void ChangeToSavingPage()
    {
        lastCameraState = myCameraState;
        myCameraState = CameraState.App;
        myAppState = AppState.SavingPage;
        Show(SavingPage);
    }

    public void ChangeToMap()
    {
        if (myCameraState != CameraState.Subway)
        {
            return;
        }

        CancelAllUI(false);

        if(LevelManager.stage > 1) CheckInstructionButton.SetActive(true);
        if(LevelManager.stageTransiting &&  LevelManager.stage == 1)
        {
            StartCoroutine(LevelManager.EnterMap_Tut_S2());
            
        }

        if (LevelManager.stageTransiting && LevelManager.stage == 2)
        {
            StartCoroutine(LevelManager.EnterMap_Tut_S3());
        }


        lastCameraState = myCameraState;
        myCameraState = CameraState.Map;
        RatingSys.LeaveSubway(true);
        phoneFrame.SetActive(true);
        // phone.SetActive(false);
        Show(SubwayMap);


    }




    public void Hide(CanvasGroup UIGroup)
    {
        if(UIGroup == fishShoutCG) fishShouting = false;
        UIGroup.alpha = 0f; //this makes everything transparent
        UIGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
        UIGroup.interactable = false;
    }

    public void Show(CanvasGroup UIGroup)
    {
        if(UIGroup == fishShoutCG) fishShouting = true;
        UIGroup.alpha = 1f;
        UIGroup.blocksRaycasts = true;
        UIGroup.interactable = true;
    }

    public void ChangeToAdvertisement()
    {
        //Hide(subwayBackground);
        
        if (myCameraState != CameraState.Subway)
        {
            return;
        }
        
        myCameraState = CameraState.Ad;
        if (!AdInstructionShown)
        {
            InstructionShow();
            AdInstructionShown = true;
        }
        Show(TakePhoto);
        // phone.SetActive(false);
        Posture.SetActive(true);
        lastCameraState = myCameraState;
        CameraMovement.previousPage = CameraMovement.currentPage;
        //transform.position = new Vector3(24, 0, -10);
        RatingSys.LeaveSubway(true);
        phoneFrame.SetActive(true);
        // transform.position = PosturePos;
        CameraMovement.atInventoryOrAd = true;
    }



    public void HideAllPersonalPages()
    {
        for (var i = 0; i < pageList.Count; i++)
        {
            Hide(pageList[i]);
        }
    }

    private bool isSetting;
    public void clickSetting()
    {
        CancelAllUI(false);
        if (isSetting)
        {
            setting.SetActive(false);
            isSetting = false;
        }
        else
        {
            setting.SetActive(true);
            isSetting = true;
        }
    }

    public void DisableInput(bool temp)
    {
        // false --> 不block interactable
        disableInputCG.blocksRaycasts = temp;
    }

    // hide inventory
    public void InstructionDismiss()
    {
        if (LevelManager.isInstruction) return;

        instruction.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 1f;
        if (activeInstruction != null) activeInstruction.SetActive(false);
        instruction.SetActive(false);
    }

    public void InstructionShow()
    {
        
        instruction.GetComponentInChildren<ScrollRect>().enabled = true;
        if (myCameraState == CameraState.Closet)
        {
            inventoryInstruction.SetActive(true);
            activeInstruction = inventoryInstruction;
        }
        else if (myCameraState == CameraState.Ad)
        {
            cameraInstruction.SetActive(true);
            activeInstruction = cameraInstruction;
            instruction.GetComponentInChildren<ScrollRect>().StopMovement();
            instruction.GetComponentInChildren<ScrollRect>().enabled = false;
        }
        else
        {
            mapInstruction.SetActive(true);
            activeInstruction = mapInstruction;
        }


        


        //Debug.Log("showInstruction");
        instruction.SetActive(true);
    }

    IEnumerator WaitingInstruction()
    {

        yield return new WaitForSeconds(0.5f);
        InstructionShow();
        InventoryInstructionShown = true;

    }


    private void ChapterEndChecker()
    {
        if (ChapterOneEnd)
        {
            int followerNum = System.Convert.ToInt32(InstagramController.followerNum_UI.text);

            if (followerNum >= 30)
            {
                chapterOneSucceed = true;
                Show(ChapterOneEndComic); //漫画
                PlayerPrefs.SetInt("ongoingChapter", 2);
                PlayerPrefs.SetInt("skipInstruction", 1);
                PlayerPrefs.Save();
            }
            else
            {
                StartCoroutine(ChapterOneFailComic());
                PlayerPrefs.SetInt("ongoingChapter", 1);
                PlayerPrefs.SetInt("skipInstruction", 0);
                PlayerPrefs.Save();
            }
        }
    }

    public void GotoPage(int pageNum)
    {

        CameraMovement.Go2Page(pageNum);


    }

    public void ChangeCameraSpeed(float speed)

    {
        if (speed < 0) CameraMovement.smoothSpeed = CameraMovement.defaultSpeed;
        CameraMovement.smoothSpeed = speed;
    }

    public void BagOverFlow()
    {
        Debug.Log("有很多包没洗");
        string toSay;

        if(AllMachines.FindSuitableMachine(AllMachines.MachineState.empty)>=0)
        {
            toSay = "Wash the bags!";

        }
        else toSay = "Return bags have been washed!";  

        if(myCameraState == CameraState.Subway)
        {
            if (mySubwayState == SubwayState.One) fishTalkText.text = toSay;
            else FishBossNotification.ShowFish(toSay);
        }
        
    }

   
}

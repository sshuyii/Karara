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
    private InstagramController InstagramController;

    public CanvasGroup disableInputCG, ChapterOneEndComic, Mask, TakePhoto, fishShoutCG,
        ChapterOneFailCG, Inventory, SubwayMap,inventory, appBackground, albumBackground, albumCG, albumDetailCG, frontPage, SavingPage;
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
    private bool isfishTalking = true;

    [HideInInspector]
    public CameraMovement CameraMovement;

    //public CameraMovement myHSS;

    //a dictionary of all the clothes that comes from station 0 and are currently in the machines 
    public Dictionary<string, List<Sprite>> AllStationClothList = new Dictionary<string, List<Sprite>>();

    public Sprite startSprite;


    public GameObject instruction, instructionContent, inventoryInstruction, mapInstruction, cameraInstruction, Posture,
        phone,CheckInstructionButton;
    private GameObject activeInstruction;
    public InventorySlotMgt inventorySlotMgt;



    public bool lateReturnComic;
    public GameObject setting;

    public ScrollRect mainpageScrollRect,albumScrollRect;
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
        AlbumDetail
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

    // Start is called before the first frame update
    void Start()
    {

        if (!isTutorial)
        {
            setting.SetActive(false);
            instruction.SetActive(false);
        }
        //        myCameraState = CameraState.Subway;
        myAppState = AppState.Mainpage;

        CameraMovement = transform.gameObject.GetComponent<CameraMovement>();

        //myHSS = GameObject.Find("Horizontal Scroll Snap").GetComponent<CameraMovement>();
        //subwayScrollRect = GameObject.Find("Horizontal Scroll Snap").GetComponent<ScrollRect>();
        InstagramController = GameObject.Find("---InstagramController").GetComponent<InstagramController>();
        RatingSys = GameObject.Find("FloatingUI").GetComponent<RatingSystem>();
        inventorySlotMgt = GameObject.Find("---InventoryController").GetComponent<InventorySlotMgt>();
        FishBossNotification = GameObject.Find("FishBossUI").GetComponent<FishBossNotification>();
        BagsController = GameObject.Find("---BagsController").GetComponent<BagsController>();
        FishTextManager = GameObject.Find("---FishTextManager").GetComponent<FishTextManager>();
        FishBossNotification.HideFish();
        //pageList.Add(RetroPage);
        //pageList.Add(KararaPage);
        //pageList.Add(DesignerPage);


        // Hide(fishTalk);
        fishTalk.SetActive(false);
        Hide(frontPage);
        //Hide(postpage);
        //HideAllPersonalPages();
        Hide(TakePhoto);
        //Hide(Posture);
        Posture.SetActive(false);



        fishTalkText = fishTalk.gameObject.GetComponentInChildren<TextMeshPro>();
        //hide the UIs when click Karara
        //clothCG.SetActive(false);
        //messageCG.SetActive(false);
        Hide(Inventory);

        ChapterOneEnd = false;

        alreadyClothUI = false;
    }


    public void CancelAllUI(bool clickMachine)
    {
        //print("cancelallui");

        


        if (!isTutorial)
        {
            //touch anywhere on screen, close Karara UI
            // Hide(clothCG);
            //clothCG.SetActive(false);
            //messageCG.SetActive(false);
            Hide(fishShoutCG);
            isShown = false;
            //close fish talking
            //            Hide(fishTalk);
            if (alreadyNotice)
            {
                //generatedNotice.SetActive(false);

            }

            AllMachines.CloseAllMachines(clickMachine);

        }

    }


    public void CloseAllUI()
    {

        //    //
        //public CanvasGroup disableInputCG, ChapterOneEndComic, Mask, TakePhoto, fishShoutCG,
        //    ChapterOneFailCG, Inventory, SubwayMap, inventory, appBackground, albumBackground, albumCG, albumDetailCG, frontPage, SavingPage;

        inventorySlotMgt.CloseAllUI();
        AllMachines.CloseAllMachines(machineOpen);
        BagsController.ClickReturnNo();
        InstructionDismiss();

        Hide(TakePhoto);
        Hide(fishShoutCG);
        Hide(SubwayMap);
        Hide(Inventory);
        Hide(frontPage);
        Hide(appBackground);
        Hide(albumBackground);
        Hide(albumCG);
        Hide(albumDetailCG);
        Hide(frontPage);
        Hide(SavingPage);
    }




    // Update is called once per frame
    void Update()
    {

        ChapterEndChecker();


        //hide shout
        if (!isTutorial && LevelManager.isInstruction)
        {
            if (mySubwayState == SubwayState.One)
            {

                Hide(fishShoutCG);
            }
            else
            {
                //Debug.Log("show shout2");
                Show(fishShoutCG);
            }
        }




        if (TouchController.isSwiping == true)
        {
            isSwipping = true;
        }
        else
        {
            isSwipping = false;
        }

        if (isSwipping && !machineOpen)
        {
            CancelAllUI(false);
        }

        ////print(CameraMovement.CurrentPage);
        //change camera state to page number
        if (myCameraState == CameraState.Subway && !isSwipping)
        {
            CheckScreenNum();

            if (transform.position.x < 5 && transform.position.x > -5 && FishBossNotification.isActive)
            {
                FishBossNotification.HideFish();
            }
            else if (transform.position.x > 5 & FishBossNotification.isActive)
            {
                FishBossNotification.ShowFish();
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
        CancelAllUI(false);
        if (isfishTalking == false)
        {
            fishTalk.SetActive(true);
            FishTalk("Concentrate");
        }
        else fishTalk.SetActive(false);
        isfishTalking = !isfishTalking;
    }

    public void FishTalkAccessFromScript(string keyWord)
    {
        CancelAllUI(false);
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
        if (!isShown)
        {


            CancelAllUI(false);//进行完了isshown一定是false?

            Debug.Log("show cloth CG");
            isShown = true;
        }
        else
        {
            isShown = false;
        }
    }

    public void ChangeToCloth()
    {
        myCameraState = CameraState.Closet;
        CameraMovement.atInventory = true;
        phone.SetActive(false);
        CameraMovement.previousPage = CameraMovement.currentPage;
        Show(Inventory);
        Hide(fishShoutCG);
        Mask.alpha = 0;
        if (!isTutorial) CheckInstructionButton.SetActive(true);



        if (alreadyClothUI == false)
        {
            if (isSwipping == false)
            {
                lastCameraState = myCameraState;
                transform.position = new Vector3(-25, 0, -10);
            }

            if (!InventoryInstructionShown)
            {

                StartCoroutine(WaitingInstruction());

            }
        }
        else
        {
            currentClothUI.SetActive(false);
            alreadyClothUI = false;
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
        else if (myAppState == AppState.RetroPage || myAppState == AppState.KararaPage || myAppState == AppState.DesignerPage || myAppState == AppState.NPCPage)
        {
            Show(frontPage);

            myAppState = AppState.Mainpage;
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
        phone.SetActive(true);
        CameraMovement.atInventory = false;
        CameraMovement.JumpToPreviousPage();
        if (!isTutorial) CheckInstructionButton.SetActive(false);
        Mask.alpha = 1;



        if (LevelManager.isInstruction)//换到鱼界面
        {
            //print("Final camera controller clicktime = 7");
            //myHSS.GoToScreen(3);
            LevelManager.clicktime = 7;
            LevelManager.isInstruction = false;
            //Debug.Log("show shout1");
            Show(fishShoutCG);
        }

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
                //todo: kind of connfused
            }
            else
            {
                myCameraState = CameraState.Subway;
                RatingSys.GoBackToSubway();
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


        if (InstagramController.waitingForRefresh) InstagramController.RefreshPost();

    }

    public void ChangeToApp()
    {


        phone.SetActive(false);
        Hide(fishShoutCG);
        CheckInstructionButton.SetActive(false);
        Debug.Log("change to app0");
        StartCoroutine(ScrollToTop(mainpageScrollRect));
        StartCoroutine(ScrollToTop(albumScrollRect));


        InstagramController.redDot.SetActive(false);


        Hide(SavingPage);
        Hide(TakePhoto);

        Debug.Log("change to app");

        if (alreadyClothUI == false)
        {

            if (isSwipping == false)
            {
                //transform.position = new Vector3(0, 0, -10);
                lastCameraState = myCameraState;
                myCameraState = CameraState.App;
                RatingSys.LeaveSubway();
                myAppState = AppState.Mainpage;
                Show(frontPage);
            }
        }
        else
        {
            //print("alreadyClothUi = true");

            //generatedNotice.SetActive(false);
            // Hide(currentClothUI);
            currentClothUI.SetActive(false);
            alreadyClothUI = false;
        }
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
        //print("mappppppp");
        if (!isTutorial) CheckInstructionButton.SetActive(true);

        if (alreadyClothUI == false)
        {
            //Hide(subwayBackground);
            if (isSwipping == false)
            {
                lastCameraState = myCameraState;
                myCameraState = CameraState.Map;
                RatingSys.LeaveSubway();
                phone.SetActive(false);
                Show(SubwayMap);
            }
        }
        else
        {
            //generatedNotice.SetActive(false);
            // Hide(currentClothUI);
            currentClothUI.SetActive(false);
            alreadyClothUI = false;
        }

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

    public void GoAdvertisement()
    {
        //Hide(subwayBackground);
        myCameraState = CameraState.Ad;
        if (!AdInstructionShown)
        {
            InstructionShow();
            AdInstructionShown = true;
        }
        Show(TakePhoto);
        phone.SetActive(false);
        Posture.SetActive(true);
        lastCameraState = myCameraState;
        //transform.position = new Vector3(24, 0, -10);
        RatingSys.LeaveSubway();
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
        disableInputCG.blocksRaycasts = temp;
    }

    // hide inventory
    public void InstructionDismiss()
    {
        instruction.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 1f;
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

        if (myCameraState == CameraState.Map)
        {
            mapInstruction.SetActive(true);
            activeInstruction = mapInstruction;
        }


        if (myCameraState == CameraState.Ad)
        {
            cameraInstruction.SetActive(true);
            activeInstruction = cameraInstruction;
            instruction.GetComponentInChildren<ScrollRect>().StopMovement();
            instruction.GetComponentInChildren<ScrollRect>().enabled = false;
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
}

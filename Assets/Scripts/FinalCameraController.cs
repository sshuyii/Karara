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

    public CanvasGroup disableInputCG;
    public CanvasGroup ChapterOneEndComic;
    public int entryTime = 1;
    public CanvasGroup TakePhoto;
    public GameObject Posture;
    public bool ChapterOneEnd;
    public CanvasGroup Mask;
    
    public CanvasGroup fishShoutCG;

    public CanvasGroup ChapterOneFailCG;
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
    public CameraMovement HorizontalScrollSnap;
    
    //public HorizontalScrollSnap myHSS;

    //a dictionary of all the clothes that comes from station 0 and are currently in the machines 
    public Dictionary<string, List<Sprite>> AllStationClothList = new Dictionary<string, List<Sprite>>();
    
    public Sprite startSprite;

    public CanvasGroup Inventory;
    public GameObject instruction,instructionContent,inventoryInstruction,mapInstruction,cameraInstruction;
    private GameObject activeInstruction;
    public InventorySlotMgt inventorySlotMgt;
    public GameObject CheckInstructionButton;



    public bool lateReturnComic;
    public GameObject setting;

    public ScrollRect mainpageScrollRect;

    public CanvasGroup SubwayMap;
  
    
    public enum AppState
    {
        Mainpage,
        KararaPage,
        RetroPage,
        DesignerPage,
        NPCPage,
        SavingPage,
        Post,
        Album
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
    public bool alreadyNotice = false;
    public bool alreadyClothUI = false;
    public GameObject currentClothUI;
    
    
    
    //public GameObject generatedNotice;



    private bool isLerp = false;

    public CameraState lastCameraState;

    public CameraState myCameraState;
    public AppState myAppState;
    private Vector3 movement;
    public CanvasGroup inventory;
    //public CanvasGroup basicUI;

    public CanvasGroup appBackground;
    public CanvasGroup albumBackground;
    public CanvasGroup albumCG,albumDetailCG;
    public CanvasGroup frontPage;
    public CanvasGroup SavingPage;
    //public CanvasGroup subwayBackground;


    public List<CanvasGroup> pageList = new List<CanvasGroup>();

    private RatingSystem RatingSys;

    private bool InventoryInstructionShown = false;
    private bool AdInstructionShown = false;



    // Start is called before the first frame update
    void Start()
    {
        
        if(!isTutorial)
        {
            setting.SetActive(false);
            instruction.SetActive(false);
        }
//        myCameraState = CameraState.Subway;
        myAppState = AppState.Mainpage;

        HorizontalScrollSnap = transform.gameObject.GetComponent<CameraMovement>();

        //myHSS = GameObject.Find("Horizontal Scroll Snap").GetComponent<HorizontalScrollSnap>();
        //subwayScrollRect = GameObject.Find("Horizontal Scroll Snap").GetComponent<ScrollRect>();
        InstagramController = GameObject.Find("---InstagramController").GetComponent<InstagramController>();
        RatingSys = GameObject.Find("FloatingUI").GetComponent<RatingSystem>();
        inventorySlotMgt = GameObject.Find("---InventoryController").GetComponent<InventorySlotMgt>();

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
    // Update is called once per frame
    void Update()
    {

        ChapterEndChecker();

 
        //hide shout
        if(!isTutorial && LevelManager.isInstruction)
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
        
        
        
        //disable swipe before player click the poster
        if (isTutorial)
        {
            if(TutorialManager.tutorialNumber == 0 && mySubwayState == SubwayState.Four)
            {
                //myHSS.GoToScreen(4);
                //myHSS.enabled = false;
            }        
            else if(TutorialManager.tutorialNumber == 2 && mySubwayState == SubwayState.One)
            {
                //myHSS.GoToScreen(1);
                //myHSS.enabled = false;
            }
        }
        

        
//        if (lateReturnComic)
//        {
////            GoSubwayPart();
////            lateReturnImage.enabled = true;
////            ChangeToSubway();
////            myHSS.GoToScreen(1);
////            Show(fishTalk);
////            fishTalkText.text = "Return your customers' clothes in time! How can you have such bad memory!";
////            lateReturnComic = false;

//        }
//        else
//        {
//        }
        
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
            
        ////print(HorizontalScrollSnap.CurrentPage);
        //change camera state to page number
        if(myCameraState == CameraState.Subway && !isSwipping)
        {
            CheckScreenNum();
            //Show(subwayBackground);
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

        else if(myCameraState == CameraState.Closet)
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
            PlayerPrefs.SetInt("ongoingChapter",1);
            PlayerPrefs.SetInt("skipInstruction",0);
            PlayerPrefs.Save();
        }
    }

    IEnumerator ChapterOneFailComic()
    {
        DisableInput(true);
        yield return new WaitForSeconds(1f);
        
        ChangeToSubway();
        HorizontalScrollSnap.currentPage = 1;


        Show(ChapterOneFailCG);
    }

    public void BossTalk()
    {
        CancelAllUI(false);
        if(isfishTalking == false)
        {
            // Show(fishTalk);
            fishTalk.SetActive(true);
            isfishTalking = true;
            fishTalkText.text = "Concentrate on your work! Do the laundry!";
        }
        else
        {
            // Hide(fishTalk);
            fishTalk.SetActive(false);
            isfishTalking = false;
        }
    }
    
    void CheckScreenNum()
    {

        switch (HorizontalScrollSnap.currentPage) {
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
            // Show(clothCG);
            //clothCG.SetActive(true);
            //messageCG.SetActive(true);
            isShown = true;
        }
        else
        {
            // Hide(clothCG);
            //clothCG.SetActive(false);
            //messageCG.SetActive(false);
            isShown = false;
        }
    }
    
    public void ChangeToCloth()
    {
        HorizontalScrollSnap.previousPage = HorizontalScrollSnap.currentPage;
        Show(Inventory);
        Hide(fishShoutCG);
        Mask.alpha = 0;
        if (!isTutorial) CheckInstructionButton.SetActive(true);


        
        if(alreadyClothUI == false)
        {
            //Hide(subwayBackground);

            //print("alreadyCLoth = false" + isSwipping);
           

            if (isSwipping == false)
            {
                ////print("myCameraState = " + myCameraState);
                lastCameraState = myCameraState;
                myCameraState = CameraState.Closet;
                //RatingSys.LeaveSubway();
                transform.position = new Vector3(-25, 0, -10);
                //print("actually change");
            }

            if (!InventoryInstructionShown)
            {

                StartCoroutine(WaitingInstruction());
                
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
            lastCameraState = CameraState.Subway;
            ChangeToSubway();
            //}
        }
       else if (myAppState == AppState.Mainpage)
       {
           if (ChapterOneEnd)//第一章结束，出现漫画？如果点了一下之后进入第二章
           {
               if(chapterOneSucceed)
               {
                   Show(ChapterOneEndComic); //漫画
                   PlayerPrefs.SetInt("ongoingChapter",2);
                   PlayerPrefs.SetInt("skipInstruction",1);
                   PlayerPrefs.Save();
               }
              
           }
           else
           {
               lastCameraState = CameraState.Subway;
               ChangeToSubway();
           }


       }
       else if(myAppState == AppState.RetroPage || myAppState == AppState.KararaPage || myAppState == AppState.DesignerPage || myAppState == AppState.NPCPage)
       {
           Show(frontPage);
           //HideAllPersonalPages();
           //Hide(postpage);
           //Hide(NPCPage);

           myAppState = AppState.Mainpage;
           resetPostOrder();

       }
    }


    public void resetPostOrder()
    {


        





        //print("resetttttttposterOrder");

        List<int> temp = new List<int>();

        Dictionary<int, GameObject> tempDic = new Dictionary<int, GameObject>();
        Dictionary<GameObject, int> tempDic1 = new Dictionary<GameObject, int>();

        
        //reset the order of all posts
        for (int i = 0; i < InstagramController.postList.Count; i++)
        {
            //temp.Add(InstagramController.postList[i].GetComponent<EntryTime>().time);
            //tempDic.Add(InstagramController.postList[i].GetComponent<EntryTime>().time, InstagramController.postList[i]);

        }   
        //时间是不会重复的，每个npc的个位数都不一样
        
        temp.Sort();
        temp.Reverse();
        
        for (int i = 0; i < InstagramController.postList.Count; i++)
        {
            tempDic1.Add(tempDic[temp[i]], i);

        }  
        //需要一个list，按顺序放着所有的gameobject
        
        
        
        for (int i = 0; i < InstagramController.postList.Count; i++)
        {
            //order是gameobject在list中排的位置
            //InstagramController.postList[i].GetComponent<EntryTime>().order = tempDic1[InstagramController.postList[i]];

            //InstagramController.postList[i].transform.SetSiblingIndex(InstagramController.postList[i].GetComponent<EntryTime>().order);
        }           
        
//        for (int i = 0; i < InstagramController.postList.Count; i++)
//        {
//            InstagramController.postList[i].transform.SetSiblingIndex(-InstagramController.postList[i].GetComponent<EntryTime>().time);
//        }           
        
        //清空所有内容
        temp.Clear();
        tempDic.Clear();
        tempDic1.Clear();
        
    }
    
    
    
    public static void ScrollToTop(ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }
    
    public void ChangeToSubway()
    {
        HorizontalScrollSnap.JumpToPreviousPage();
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


        if(InstagramController.waitingForRefresh) InstagramController.RefreshPost();
        
    }
    
    public void ChangeToApp()
    {
        Hide(fishShoutCG);
        CheckInstructionButton.SetActive(false);
        ScrollToTop(mainpageScrollRect);




        //reset the order of all posts

        //todo: yun 这里有问题
        //resetPostOrder();

        //go to main page top     
        //cancel all dialogues
        //print("click ChangeToAPP");
        
        //cancel red dot
        //InstagramController.redDot.SetActive(false);

        Hide(SavingPage);
        Hide(TakePhoto);

        Debug.Log("change to app");

       if(alreadyClothUI == false)        
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
        Show(albumBackground);
        Show(albumCG);
        Hide(appBackground);
        Hide(frontPage);
        myAppState = AppState.Album;
    }


    public void AlbumToMainPage()
    {
        Hide(albumBackground);
        Hide(albumCG);
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

        if (alreadyClothUI == false)        {
            //Hide(subwayBackground);
            if (isSwipping == false)
            {
                lastCameraState = myCameraState;
                myCameraState = CameraState.Map;
                RatingSys.LeaveSubway();
                //transform.position = new Vector3(0, 13, -10);
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
    
    public void GoAdvertisement()
    {
        //Hide(subwayBackground);
        myCameraState = CameraState.Ad;
        if (!AdInstructionShown) {
            InstructionShow();
            AdInstructionShown = true;
        }
        Show(TakePhoto);
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
    public void InstructionDismiss() {
        instruction.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 1f;
        activeInstruction.SetActive(false);
        instruction.SetActive(false);
    }

    public void InstructionShow()
    {

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
            

        if(myCameraState == CameraState.Ad)
        {
            cameraInstruction.SetActive(true);
            activeInstruction = cameraInstruction;
        }

       
        //Debug.Log("showInstruction");
        instruction.SetActive(true);
    }

    IEnumerator WaitingInstruction() {

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
}

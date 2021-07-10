using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using UnityEngine.Experimental.GlobalIllumination;
using TMPro.Examples;
using TreeEditor;

public class TutorialManagerNew : MonoBehaviour
{
    
    public int stepCounter,bagClick;
    public bool forwardOneStep;
    private bool machineOpen = false;
    private bool timerStop = false;
    private bool isFirstOpen = true;
    private bool scrollTeaching = false;

    [SerializeField]
    private GameObject BlackBlocks;
    [SerializeField]
    private CanvasGroup ProceedCG;

    private CanvasGroup BlackBlocksCG;
    private Animator blackBlockAnimator;

    [SerializeField]
    private bool deactiveButtons = false;
    private bool inLoop = false;
    private bool flashing = false;
    private bool isInitialPosture = true;
    private bool returning = false;

    private float realTimer;
    private float timer = 0;
    private TutorialCameraController TutorialCameraController;
    private CameraMovement CameraMovement;
    private CanvasGroup bottomDotsCG;
    private FishTextManager FishTextManager;


    [SerializeField]
    TextMeshPro fishText;
    TextMeshProUGUI fishTextUI;
    bool ftUI = false;

    [Header("Machine Related")]
    [SerializeField]
    private float washTotalTime;

    [SerializeField]
    private GameObject ClothUI, liquid, ReturnNotice, ReturnNoticeYes;

    [SerializeField]
    private GameObject Posture, Phone, HintUI, ScrollHint, clothBag, machineFull, machineEmpty, phoneAnimation, changePostureButton,
        machineOccupied, ClothFish, EmojiBubble, InventoryBackButton, Shutter, FishTalkButton, sweet;

    public GameObject Hint2D, HintScreen, inventoryFish;
    [SerializeField]
    private CanvasGroup CameraBackground, Notice, scream, Inventory, Flashlight, Comic;

    [SerializeField]
    private Vector3 HintPosPoster, HintPosCamera, HintPosBag, MachinePos, HintPosKarara, HintPosBackButton,
        HintPosShutter;

    private Vector3[] ClothUIPos = new Vector3[3];
    private Vector3[] InventoryUIPos = new Vector3[3];
    public GameObject[] ClothUIPosObject, InventoryUIPosObject;

    [SerializeField]
    private Sprite pos0Work, pos1Work, openBag, closeBag, fullImg, emptyImg,
        EmojiCloth, EmojiHappy, openDoor, closeDoor, postPose1, postPose2;

    [SerializeField]
    private SpriteRenderer body, everything, machineFront, emoji, MachineDoor;    
    
    [SerializeField]
    private Animator shoeAnimator, followerAnimator;

    [SerializeField]
    private List<Animator> AnimatorList = new List<Animator>();
    private Dictionary<string, Animator> AnimatorTable = new Dictionary<string, Animator>();

    public SpriteRenderer[] subwayCloth, inventoryCloth, adsCloth;

    [Header("Camera Related")]
    [SerializeField]
    private Animator shutterAnimator;


    [Header("Ins Contents")]
    [SerializeField]
    private GameObject TutorialPost;
    [SerializeField]
    private CanvasGroup InsPage, FollowerHint,phone_background,Memo;
    [SerializeField]
    private Image postKararaImage;

    [SerializeField]
    private CanvasGroup followerNum, followerAnimation, mobile;

    [SerializeField]
    private int followerNumReal, followerNumTime, followerNumGoal;
    [SerializeField]
    TextMeshProUGUI followerNumText;



    [Header("Work Sprites")]
    public Sprite workInventory;
    public Sprite workSubway;
    public Sprite workShoe;
    public Sprite SlipperInventory;
    
    public enum MachineState
    {
        Empty,
        Washing,
        Finish
    }
    MachineState myMachineState;

    //fish talk相关内容
    FishText currentFT;
    bool fishForward;
    int currentFS;
    Coroutine lastRoutine = null;
    AudioManager AudioManager;


    //穿上一件，另一件放回洗衣机
    public bool workClothOn = false;
    public bool workShoeOn = false;

    //偷了几件衣服
    int pickedClothNum = 0;
    public List<GameObject> ClothSlotList;
    public Dictionary <int, GameObject> clothSlotTable = new Dictionary <int, GameObject> ();

    //inventory里穿了哪几件衣服
    public int isWearingClothNum = 0;
    //inventory里karara
    public Animator kararaAnimator;
    
    [SerializeField]
    private Animator ClothUiAnimator;

    public List<GameObject> ReturnNoticeList;

    private GameObject MainCamera;

    public TutorialTransition TutorialTransition;
    public Image progressBar;

    [SerializeField]
    List<GameObject> KararaList = new List<GameObject>();

    [SerializeField]
    List<GameObject> fishTextObjList = new List<GameObject>();
    List<TextMeshPro> fishTextList = new List<TextMeshPro>();

    [SerializeField]
    List<TextMeshProUGUI> fishTextUIList = new List<TextMeshProUGUI>();
    
    [SerializeField]
    List<GameObject> worldHintList = new List<GameObject>();
    Dictionary<string, Vector3> worldHintPosTable = new Dictionary<string, Vector3>();

    [SerializeField]
    List<GameObject> kararaBubbleList = new List<GameObject>();

    List<TextMeshPro> kararaTextList = new List<TextMeshPro>();

    bool photoTaken, clothReturned;
    public bool putBack;


    
    void ResetTutorial()
    {
        foreach (GameObject k in KararaList)
        {
            k.SetActive(false);
        }

        ShowFishText(true, 99);
        ShowFishText(false, 99);

        foreach (GameObject h in worldHintList)
        {
            h.SetActive(false);
        }

        foreach (GameObject k in kararaBubbleList)
        {
            k.SetActive(false);
        }

        Hide(InsPage);
        Hide(Memo);
        Hide(FollowerHint);
        Hide(CameraBackground);
        Hide(Notice);
        Hide(phone_background);

        Hide(BlackBlocksCG);

        Hide(bottomDotsCG);
        Hide(ProceedCG);

        ClothUI.SetActive(false);
        liquid.SetActive(false);
        ReturnNotice.SetActive(false);
        ReturnNoticeYes.SetActive(false);

        ResetFloatingUI();
    }

    void ResetFloatingUI()
    {
        Hide(mobile);
        Hide(followerNum);
    }

    void Start()
    {
        deactiveButtons = true;

        for(int i = 0; i < fishTextObjList.Count; i++ )
        {
            fishTextList.Add(fishTextObjList[i].GetComponentInChildren<TextMeshPro>());
        }

        for(int i = 0; i < worldHintList.Count; i++ )
        {
            worldHintPosTable.Add(worldHintList[i].name, worldHintList[i].transform.position);
        }

        for(int i = 0; i < kararaBubbleList.Count; i++ )
        {
            kararaTextList.Add(kararaBubbleList[i].GetComponentInChildren<TextMeshPro>());
        }

        BlackBlocksCG = BlackBlocks.GetComponent<CanvasGroup>();
        blackBlockAnimator = BlackBlocks.GetComponent<Animator>();


        // clothBag.SetActive(false);//不然会显示在地铁外面

        //Set Camera position and size in stage 3  
        MainCamera = GameObject.Find("Main Camera"); 
        TutorialCameraController = MainCamera.GetComponent<TutorialCameraController>();
        CameraMovement = MainCamera.GetComponent<CameraMovement>();
        bottomDotsCG = CameraMovement.Dots.GetComponent<CanvasGroup>();

        AudioManager = GameObject.Find("---AudioManager").GetComponent<AudioManager>();
        FishTextManager = GameObject.Find("---FishTextManager").GetComponent<FishTextManager>();
        

        //step1
        bagClick = 0;
        myMachineState = MachineState.Empty;

        followerNumText.text = followerNumReal.ToString();

        workInventory = Resources.Load<Sprite>("Images/Karara/Cloth/Inventory/work");
        workSubway = Resources.Load<Sprite>("Images/Karara/Cloth/Subway/work");


        //disable a lot of things when tutorial starts
        phoneAnimation.SetActive(false);

        //所有洗衣机里的衣服
        for(int i = 0; i < ClothSlotList.Count; i++)
        {
            clothSlotTable.Add(i, ClothSlotList[i]);
        }

        //所有inventory里的hint位置
        for(int i = 0; i < ClothUIPosObject.Length; i++)
        {   
            ClothUIPos[i] = ClothUIPosObject[i].transform.position;

            InventoryUIPos[i] = InventoryUIPosObject[i].GetComponent<RectTransform>().position;
        }
        
        //所有animator
        for(int i = 0; i < AnimatorList.Count; i++)
        {
            AnimatorTable.Add(AnimatorList[i].gameObject.name, AnimatorList[i]);
        }        

        //放在最后防止进行到这一步时，有些变量还未赋值
        ResetTutorial();
    }


    IEnumerator LongTap()
    {
        //出现鱼教学长按还衣服
        yield return new WaitForSeconds(0.5f);

        FishTalk("ReturnCloth", true, 0, true, false);
        
        // int bubbleCount = 10;
        // for(int i = 0; i < bubbleCount; i ++)
        // {
        //     int random = UnityEngine.Random.Range(0,5);
        //     AudioManager.PlayAudio(fishBubbles[random]);
        //     yield return new WaitForSeconds(0.1f);
        // }
        
        shoeAnimator.SetBool("isShining", true);

        //箭头出现在
        HintScreen.SetActive(true);
        HintScreen.transform.position = InventoryUIPos[1];

    }

    // Update is called once per frame
    public void ClickHiring()
    {
        TutorialTransition.TransitionStage++;
    }

    private void ShowBlackBlocks(bool show)
    {
        if(show)
        {
            Show(BlackBlocksCG);

            blackBlockAnimator.SetTrigger("in");
        }
        else{
            blackBlockAnimator.SetTrigger("out");

        }
    }

    void Update()
    {
        followerNumText.text = followerNumReal.ToString();

        if(TutorialTransition.TransitionStage == 4)
        {
            print("TutorialTransition =" + TutorialTransition.TransitionStage );

            MainCamera.GetComponent<Camera>().orthographicSize = 5;
            MainCamera.GetComponent<RectTransform>().position = new Vector3(17.5f, 0, -10f);
            
            TutorialTransition.TransitionStage ++;
            //正式开始，进入车厢
            StartCoroutine(GetOnTrain());

            // Show(ProceedCG);
        }

        if (!timerStop)
        {
            if (myMachineState == MachineState.Washing)
            {
                CalculateTime();
                UpdateProgressBar();
            }

            timer += Time.deltaTime;
        }

        //如果拿了三件衣服，karara闪光
        if(pickedClothNum == 3)
        {
            ShowAnimation("PlayerSubwayFolder");
            ShowKararaBubble(1, "put on", false);

            pickedClothNum = 99;
        }

        if(isWearingClothNum == 3)
        {
            StartCoroutine(ShowBackButton());
            isWearingClothNum ++;

        }

        if(putBack && TutorialCameraController.myCameraState == TutorialCameraController.CameraState.Closet)
        {
            ShowFishText(true, 99);
        }


        if(flashing)
        {
            Flashlight.alpha = Flashlight.alpha - Time.deltaTime;
            if(Flashlight.alpha < 0)
            {
                Flashlight.alpha = 0;
                flashing = false;
            }
        }

        //在screen1永远不显示fish scream
        if(TutorialCameraController.currentPage == 1)
        {
            Hide(scream);
        }

        if (TutorialCameraController.reachTarget)
        {
            Debug.Log("reach! manager");
            Hide(scream);
            ScrollHint.SetActive(false);
            TutorialCameraController.reachTarget = false;
            

            if (scrollTeaching)
            {
                TutorialCameraController.allowScroll = false;

                forwardOneStep = true;
            }
            
            else if(inLoop && TutorialCameraController.targetPage == 2)
            {   // 前提
                Debug.Log("前提");

                if (!machineOpen)
                {
                    Debug.Log("前提");
                    HintUI.SetActive(true);
                    ShowKararaBubble(1, "open", false);
                }

                TutorialCameraController.targetPage = 1;
            }
            else if (inLoop && TutorialCameraController.targetPage == 1)
            {
               
                TutorialCameraController.targetPage = 2;

                if (machineOpen) FishTalk("CloseDoor",true, 0, false, false);
                else FishTalk("Right",true, 0, false, false);
            }
            else if(inLoop && TutorialCameraController.currentPage == 3)
            {
                TutorialCameraController.JumpToPage(2);
            }
            //如果照完照片，回到screen2
            else if(stepCounter == 7)
            {
                //鱼说话，要玩家脱衣服or还包？
                Hide(scream);
                FishTalk("ReturnBagPre", true, 0, true, true);
                ShowAnimation("ClothBag");                
            }
            
            TutorialCameraController.targetPage = -1;
        }
        


        //go to next step
        if (forwardOneStep)
        {
            forwardOneStep = false;
            stepCounter++;
            switch (stepCounter)
            {
                case 1:
                    KararaGetIn();
                    break;

                case 2:
                    KararaPoseOne();
                    break;

                case 3:
                    KararaPoseTwo();
                    break;

                case 4:
                    StartCoroutine(KararaAnswer());
                    break;

                case 5:
                    PlayerControl();
                    break;

                case 6:
                    scrollTeaching = false;
                    FishTalk("Late", true, 0, false, true);
                    break;

                case 7:
                    BagOccur();
                    break;

                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    //还了鞋之后就可以显示返回地铁的按钮
                    StartCoroutine(ShowBackButton());
                    //一定是还了一件衣服才能返回的，所以洗衣机里又有衣服了
                    machineFront.sprite = fullImg;

                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;

                case 16:
                    //鱼说完还包之后从screen1跳转到screen2
                    StartCoroutine(ReturnPrep());
                    //ReturnPrep();
                    break;

                case 17:
                    FishTalkLast();
                    break;
                case 18:
                    // StartCoroutine(ShowComic());
                    // StartCoroutine(AddFans());
                    Handheld.Vibrate();
                    Show(mobile);
                    break;
            }

            if(clothReturned)
            {
                TutorialCameraController.GotoPage(1);
                FishTalk("TutorialEnd", true, 0, false, false);
            }
        }
    }

    private void KararaGetIn()
    {
        Debug.Log("karara step 1");
        ShowKarara(4, true,  "Poster", true);
    }

    private void KararaPoseOne()
    {
        Debug.Log("karara step 2");
        ShowKarara(3, true, "cool", true);
    }

    private void KararaPoseTwo()
    {
        Debug.Log("karara step 3");
        ShowKarara(2, true, "Hey", true);
    }

    IEnumerator KararaAnswer()
    {
        FishTalk("come", true, 0, true, false);
        ShowKararaBubble(99, "", false);
        yield return new WaitForSeconds(1.5f);

        ShowKararaBubble(2, "karara?", true);
        yield return new WaitForSeconds(.5f);
    }

    private void PlayerControl()
    {
        Hide(ProceedCG);
        ShowFishText(true, 99);

        StartCoroutine(ScrollTeaching(1));

    }


    private void CalculateTime()
    {
        realTimer = washTotalTime - timer;
        
        // timerNum.text = "0:0" + Mathf.RoundToInt(realTimer).ToString();

        if(realTimer < 0)
        {
            AudioManager.PlayAudio(AudioType.Machine_Finished);
            myMachineState = MachineState.Finish;
            // forwardOneStep = true;
            AfterWash();
            returning = true;
        }
    }

    private void ShowKarara(int index, bool showTalk, string text, bool click)
    {
        foreach (GameObject karara in KararaList)
        {
            karara.SetActive(false);
        }
        if(index > KararaList.Count) return;//如果i=99，就把所有的都deactive

        KararaList[index].SetActive(true);

        if(showTalk) ShowKararaBubble(index, text, click);
    }

    public void ShowKararaBubble(int index, string text, bool click)
    {
        foreach (GameObject b in kararaBubbleList)
        {
            b.SetActive(false);
        }
    
        if(index > kararaBubbleList.Count) return;

        StartCoroutine(ShowKararaBubble(index, click));
        kararaTextList[index].text = text;

    }

    IEnumerator ShowKararaBubble(int index, bool click)
    {
        yield return new WaitForSeconds(0.6f);
        kararaBubbleList[index].SetActive(true); 
        if(click) Show(ProceedCG);

    }
    
    private void ShowFishText(bool isUI, int i)
    {   
        if(!isUI)
        {
            foreach (TextMeshPro ft in fishTextList)
            {
                ft.gameObject.transform.parent.gameObject.SetActive(false);
                Debug.Log(ft.gameObject.transform.parent.gameObject.name + "is deactivated");
            }

            if(i > fishTextList.Count) return;
            
            fishTextList[i].gameObject.transform.parent.gameObject.SetActive(true);

        }
        else
        {
            foreach (TextMeshProUGUI ft in fishTextUIList)
            {
                ft.gameObject.transform.parent.gameObject.SetActive(false);
                Debug.Log(ft.gameObject.transform.parent.gameObject.name + "is deactivated");

            }

            if(i > fishTextUIList.Count) return;

            fishTextUIList[i].gameObject.transform.parent.gameObject.SetActive(true);

        }

    }

    private void ShowAnimation(string animatorName)
    {
        if(AnimatorTable.ContainsKey(animatorName) == false)//如果给的名字不存在，把所有animator都停掉
        {
            Debug.Log(animatorName + "does not exist!");
            if(animatorName == "none")
            {
                foreach(Animator a in AnimatorList)
                {
                    a.SetBool("hint", false);
                    Debug.Log(a.gameObject.name + "is disabled");
                }
            }
            return;
        }

        List<Animator> AnimatorListTemp = new List<Animator>();

        foreach (Animator a in AnimatorList)
        {
            AnimatorListTemp.Add(a);
        }
        AnimatorListTemp.Remove(AnimatorTable[animatorName]);

        foreach (Animator a in AnimatorListTemp)
        {
            a.SetBool("hint", false);
        }
        AnimatorTable[animatorName].SetBool("hint", true);
        Debug.Log(AnimatorTable[animatorName].gameObject.name + "is enabled");
        
    }
    public void Proceed()
    {
        forwardOneStep = true;
        Hide(ProceedCG);

    }

    IEnumerator GetOnTrain()
    {
        yield return new WaitForSeconds(.5f);
        //门打开
        AnimatorTable["doors"].SetTrigger("doorOpen");
        
        float Twait = (float)AnimatorTable["doors"].GetCurrentAnimatorClipInfo(0).Length;
        yield return new WaitForSeconds(Twait + .5f);

        ShowKarara(4, true, "Laundry?", true);

        if(stepCounter == 0)
        {
            Hint2D.SetActive(true);
        }
    }


    IEnumerator ShowHintInAd()
    {
        yield return new WaitForSeconds(1.5f);
        if(stepCounter == 1)
        {
            Hint2D.SetActive(true);
        }
    }

    public void ClickPoster()
    {
        ShowAnimation("none");

        if (deactiveButtons) return;
        if(!posterEnabled) return;

        posterEnabled = false;
        
        AudioManager.PlayAudio(AudioType.Poster_Enter);

        Debug.Log("ClickAd");

            //穿上工作服了
            body.sprite = pos1Work;
            
            Shutter.SetActive(true);
            Hint2D.SetActive(false);

            //照相界面下面有个黑条，改变姿势的透明图层需要变小一点
            RectTransform rt = changePostureButton.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 20);
            rt.sizeDelta = new Vector2 (700, 500);

            ShowCameraBG();

    }

    IEnumerator ShowShutterHint()
    {
        yield return new WaitForSeconds(8f);
        if(!flashing)
        {           
            HintScreen.SetActive(true);
        }
    }

    private void ChangeHintPos(Vector3 pos,int orderChange)
    {
        Hint2D.transform.position = pos;
        if (orderChange == 0) return;
        SpriteRenderer[] SRs = Hint2D.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in SRs)
        {
            sr.sortingOrder = sr.sortingOrder + orderChange;
        }
    }

   
    private void ShowCameraBG()
    {
        Debug.Log("ShowCameraBG");

        Posture.SetActive(true);
        Show(CameraBackground);

        everything.sprite = null;
        HintScreen.transform.localPosition = HintPosShutter;
    }


    [SerializeField]
    int clickTime = 0;

    public void ClickPosture()
    {
        if (deactiveButtons) return;

        AudioManager.PlayAudio(AudioType.Photo_Pose);
        Debug.Log("ClickPosture");

        clickTime ++;

            if(clickTime == 1)
            {
                //身上穿的衣服
                body.sprite = pos0Work;

                //第一次发的post
                isInitialPosture = false;
            
                postKararaImage.sprite = postPose1;
            }
            else if(clickTime == 2)
            {   
                //身上穿的衣服
                body.sprite = pos1Work;

                clickTime = 0;//reset

                //第一次发的post
                isInitialPosture = true;
                
                postKararaImage.sprite = postPose2;
            }        
    }


    IEnumerator ScrollTeaching(int targetPage)
    {
        yield return new WaitForSeconds(.5f);
        Show(scream);
        TutorialCameraController.allowScroll = true;
        TutorialCameraController.targetPage = targetPage;

        yield return new WaitForSeconds(.5f);

        scrollTeaching = true;
        ScrollHint.SetActive(true);
        deactiveButtons = false;
    }

    private void FishTalk(string keyWord, bool animateOrNot, int screen, bool isUI, bool forward)
    {
        ShowFishText(isUI, screen);

        FishTalkButton.SetActive(true);
        currentFT = FishTextManager.GetText(keyWord);
        print("fish talk keyword = " + keyWord);
        fishForward = forward;
        currentFS = screen;
        ftUI = isUI;
<<<<<<< HEAD

        FishTalk(animateOrNot);
=======
        // Debug.Log(currentFT.key + " fish text length " +currentFT.content.Count + " playing " + currentFT.playingIdx);
        FishTalk(animateOrNot, forward);
>>>>>>> 7541bdfae2f265aa5b054b8c8fbeaee999e3bd5d
    }

    private void FishTalk(bool animateOrNot)
    {
        if(ftUI) fishTextUI = fishTextUIList[currentFS];
        else fishText = fishTextList[currentFS];

        int idx = currentFT.playingIdx;
        if (animateOrNot) 
        {
            lastRoutine = StartCoroutine(AnimateText(fishText, fishTextUI, currentFT.content[idx]));
        }
        else
        {
            //when last sentence
            if (currentFT.playingIdx > currentFT.content.Count - 2)
            {
                FishTalkButton.SetActive(false);
                currentFT.isPlaying = false;
                forwardOneStep = fishForward;
            }

            //直接显示所有内容
            if(ftUI) 
            {
                fishTextUI.text = currentFT.content[idx];
            }
            else {
                fishText.text = currentFT.content[idx];   
            }
        }

        Debug.Log("idx " + idx);
        Debug.Log("fishTalking " + currentFT.content[idx]);
    }

    private List<AudioType> fishBubbles = new List<AudioType> {AudioType.FishBubble2,
        AudioType.FishBubble3, AudioType.FishBubble4,AudioType.FishBubble5, AudioType.FishBubble6, AudioType.FishBubble7};

    IEnumerator AnimateText(TextMeshPro text, TextMeshProUGUI textUI, string textContent)
    {
        currentFT.isPlaying = true;
        for (int i = 0; i < (textContent.Length + 1); i++)
        {
            if (!currentFT.isPlaying) break;

            int random = UnityEngine.Random.Range(0,5);
            AudioManager.PlayAudio(fishBubbles[random]);
            if(ftUI) textUI.text = textContent.Substring(0, i);
            else text.text = textContent.Substring(0, i);
            yield return new WaitForSeconds(.05f);

            //如果进行到最后一句话的最后一个字母，FishTalkButton可以被拿掉
            if(i == textContent.Length && currentFT.playingIdx > currentFT.content.Count - 2) 
            {
                FishTalkButton.SetActive(false);
                forwardOneStep = fishForward;
            }
        }
    }


    public void ClickWhileFishTalking()
    {
        Debug.Log("Click While Fish Talking");
        if(currentFT.isPlaying)
        {
            ShowWholeSentence();
        }
        else if (!currentFT.isPlaying)
        {
            FishTalkNextSentence();
        }
    }

     void ShowWholeSentence()
    {
        currentFT.isPlaying = false;
        StopCoroutine(lastRoutine);
        FishTalk(false);
    }

    private void FishTalkNextSentence()
    {
        Debug.Log("reset " + currentFT.playingIdx);
        currentFT.playingIdx = currentFT.playingIdx+1;
        Debug.Log("reset " + currentFT.playingIdx);

        if (currentFT.playingIdx < currentFT.content.Count) FishTalk(true);
        else
        {
            Debug.Log("reset 0 ");
            currentFT.playingIdx = currentFT.playingIdx - 1;
            FishTalk(false);
            currentFT.playingIdx = 0;
        }
    }



    private void BagOccur()
    {
        HintUI.SetActive(true);
        clothBag.SetActive(true);
        ShowAnimation("ClothBag");
    }

    public void ClickBag()
    {
        //called when click bag
        if (deactiveButtons) return;
        
        bagClick++;
        ShowAnimation("none");
        Debug.Log("Bag click " + bagClick.ToString());

        if(bagClick == 1)//从screen1到洗衣机下方
        {
            AudioManager.PlayAudio(AudioType.Bag_Phase1);
            StartCoroutine(BeforeWash());
        }

        else if(bagClick == 2)//洗衣机下方，衣服进洗衣机
        {
            AudioManager.PlayAudio(AudioType.Bag_Phase1);
            HintUI.SetActive(false);//点完包后提示消失一会儿，等洗衣机开门动画播完之后再出现在洗衣机上

            ThrowClothToMachine();
        }
        else
        {
            if (inLoop)
            {
                //du地一声
                //出现return UI，但是不能点
                StartCoroutine(ShowReturnNotice());
                AudioManager.PlayAudio(AudioType.No);
                bagClick = 2;
            }
            else if(returning)
            {
                //todo:还包的声音
                AudioManager.PlayAudio(AudioType.Bag_Phase1);
                HintUI.SetActive(false);
                StartCoroutine(ShowReturnNotice());
            }
            else
            {
                bagClick = 2;

            }
        }
    }

    public void ClickLiquid()
    {
        // if(returning)
        // {
        //     //todo:还包的声音
        //     AudioManager.PlayAudio(AudioType.Bag_Phase1);
        //     HintUI.SetActive(false);
        //     StartCoroutine(ShowReturnNotice());
        // }
    }

    bool returnBagFirstTime = true;

    IEnumerator ShowReturnNotice()
    {   
        // liquid.SetActive(false);
        // yield return new WaitForSeconds(0.2f);
        // MachineDoor.sprite = openDoor;

        yield return new WaitForSeconds(0.2f);

        ReturnNotice.SetActive(true);
        ReturnNoticeYes.SetActive(true);
    }
    private IEnumerator BeforeWash()
    {
        deactiveButtons = true;
        //HintUI.SetActive(false);
        TutorialCameraController.GotoPage(2);

        //localPosition!!!!
        Vector3 newPos = new Vector3(MachinePos.x - 0.5f, clothBag.transform.localPosition.y, 0);
        clothBag.transform.localPosition = newPos;
        
        yield return new WaitForSeconds(0.1f);

        FishTalk("ClickBag", true, 0, true, false);
        
        HintUI.transform.localPosition = newPos;
        
        deactiveButtons = false;
        ShowAnimation("ClothBag");
    }



    private void ThrowClothToMachine()
    {
        Debug.Log("ThrowClothToMachine");

        
        clothBag.GetComponent<Image>().sprite = openBag;
        StartCoroutine(PreWash());
        
    }

    IEnumerator PreWash()
    {
        //洗衣机开门，衣服进去，又关门
        yield return new WaitForSeconds(0.2f);

        MachineDoor.sprite = openDoor;

        yield return new WaitForSeconds(0.2f);
        machineFront.sprite = fullImg;

        yield return new WaitForSeconds(0.2f);

        MachineDoor.sprite = closeDoor;
        AudioManager.PlayAudio(AudioType.Machine_CloseDoor);

        FishTalk("StartWashing", true, 0, true, false);
        ShowAnimation("Machine0");

        yield return new WaitForSeconds(0.2f);//等一会儿再出现提示ui
        HintUI.transform.localPosition = MachinePos;
        HintUI.SetActive(true);

        // forwardOneStep = true;
    }

    public void ClickMachine()
    {
        if (deactiveButtons||stepCounter < 5) //确保在没进行到洗衣机的时候洗衣机不能点
        {
            //播放“du”声音
            AudioManager.PlayAudio(AudioType.No);
            return;
        }
        ShowAnimation("none");
        switch (myMachineState)
        {
            case MachineState.Empty:
                AudioManager.PlayAudio(AudioType.Machine_OnOff);
                AudioManager.PlayAudio(AudioType.Machine_Washing);
                Debug.Log("start washing");

                ShowFishText(true, 99);

                machineFull.SetActive(true);
                machineEmpty.SetActive(false);
                HintUI.SetActive(false);
                timer = 0;
                myMachineState = MachineState.Washing;

                StartCoroutine(StopTimerAndForward());
                break;

            case MachineState.Finish:
                if(deactiveButtons) return;
                ShowAnimation("none");
                liquid.SetActive(true); 
                if (!machineOpen) OpenMachine();
                else CloseMachine();

                //如果第二次打开了洗衣机门，鱼要阻止
                // if (stepCounter == 8)
                // {
                //     Show(scream);
                //     FishTalk("Open2ndTime", true, 0, false, false);

                // }
                break;
        }
    }


    IEnumerator StopTimerAndForward()
    {
        yield return new WaitForSeconds(1.5f);
        AnimatorTable["phone_lowBattery"].SetTrigger("isCharged");

        //todo：直接按clip长度走，不用每次都改具体时间？
        yield return new WaitForSeconds(2f);
        //提示手机动画
        //考虑一下出现手机动画的时机，目前是洗衣机转一会儿之后才出现
        phoneAnimation.SetActive(true);

        timerStop = true;

        //鱼说话：谁丢手机了？
        FishTalk("LostPhone", true, 0, true, false);
        // forwardOneStep = true;
    }

    public void OpenMachine()
    {
        machineOpen = true;
        AudioManager.PlayAudio(AudioType.Machine_OpenDoor);
        
        machineOccupied.SetActive(false);
        MachineDoor.sprite = openDoor;

        if(pickedClothNum != 3)
        {
             Hint2D.SetActive(false);
        }

        if (isFirstOpen)
        {
            Debug.Log("isFirstOpen and open machine");

            Hint2D.SetActive(false);
            HintUI.SetActive(false);

            StartCoroutine(OpenCloseDoor(0.2f));
        }
        else{
            ClothUI.SetActive(true);
        }
    }

    IEnumerator OpenCloseDoor(float time)
    {
        ///门打开后出现一下衣服，立即关上

        //开门
        machineOccupied.SetActive(false);
        MachineDoor.sprite = openDoor;
        AudioManager.PlayAudio(AudioType.Machine_OpenDoor);

        ClothUI.SetActive(true);

        yield return new WaitForSeconds(time);

        ClothUI.SetActive(false);

        // ClothUiAnimator.SetTrigger("StartShowing");
        
        // float Twait = ClothUiAnimator.GetCurrentAnimatorClipInfo(0).Length;
        // Debug.Log("TWait: " + Twait);
        // yield return new WaitForSeconds(Twait);

        //关门 
        MachineDoor.sprite = closeDoor;
        AudioManager.PlayAudio(AudioType.Machine_CloseDoor);
        machineOpen = false;
        
        ClothUiAnimator.SetTrigger("CloseImmediantly");
        HintScreen.SetActive(false);
        HintUI.SetActive(false);

        isFirstOpen = false;
        deactiveButtons = true;
        inLoop = true;
        yield return new WaitForSeconds(0.3f);

        Show(scream);
        yield return new WaitForSeconds(0.3f);

        //切换到screen1
        TutorialCameraController.ChangeCameraSpeed(5f);
        TutorialCameraController.GotoPage(1);
        FishTalk("CloseDoor",true, 0, false, false);

        //reset screen2
        Hide(scream);
        ShowAnimation("Machine0");
        ShowKararaBubble(99 ,"", false);

        deactiveButtons = false;

        TutorialCameraController.allowScroll = true;
        TutorialCameraController.targetPage = 2;
    }

     IEnumerator OpenDoor(float time)
    {
        ///播放开门动画，出现对话框
        machineOccupied.SetActive(false);
        MachineDoor.sprite = openDoor;

        HintUI.SetActive(false);

        if(pickedClothNum != 3)
        {
             Hint2D.SetActive(false);
        }
        yield return new WaitForSeconds(time);

        // ClothUI.SetActive(true);
        ClothUiAnimator.SetTrigger("StartShowing");
        
        yield return new WaitForSeconds(1f);//等动画播完
        
        Hint2D.SetActive(true);
        deactiveButtons = false;//打开过程有一定时间，到时间前不可再次点击
    }
    public void CloseMachine()
    {
        machineOpen = false;
        AudioManager.PlayAudio(AudioType.Machine_CloseDoor);

        if (inLoop)
        {
            if (Hint2D.active) Hint2D.SetActive(false);
        }

        // StartCoroutine(CloseDoor(0.2f));

        machineOccupied.SetActive(true);
        ClothUI.SetActive(false);

        MachineDoor.sprite = closeDoor;

        if(pickedClothNum != 3) 
        {
            Hint2D.SetActive(false);

            HintUI.SetActive(true);
        }
    }

    IEnumerator CloseDoor(float time)
    {
        ///播放关门动画，出现对话框
        //直接关上，窗口没有动画
        deactiveButtons = true;
        AudioManager.PlayAudio(AudioType.Machine_CloseDoor);

        machineOccupied.SetActive(true);
        ClothUiAnimator.SetTrigger("StartClosing");
        // ClothUI.SetActive(false);
        float t =  ClothUiAnimator.GetCurrentAnimatorClipInfo(0).Length;
    
        // yield return new WaitForSeconds(t);

        MachineDoor.sprite = closeDoor;

        yield return new WaitForSeconds(time);

        if(pickedClothNum != 3) 
        {
            Hint2D.SetActive(false);

            HintUI.SetActive(true);
        }
        deactiveButtons = false;
        
    }


    public void PickChargingPhone()
    {
        if (!timerStop) return;
        if (deactiveButtons) return;

        Debug.Log("PickChargingPhone");
        Phone.SetActive(false);

        ShowFishText(true, 99);//close fish text on screen2

        Show(mobile);
        ShowKarara(1, false, "", false);
        timerStop = false;
    }

    IEnumerator ShowKararaAndPhone()
    {
        ShowKarara(1, false, "", false);
        EmojiBubble.SetActive(false);
       
        yield return new WaitForSeconds(1f);

        Show(mobile);
        timerStop = false;

    }


    void AfterWash()
    {
        machineFull.SetActive(false);
        machineEmpty.SetActive(true);
        machineOccupied.SetActive(true);

        ShowKararaBubble(1, "Open",false);
        ShowAnimation("Machine0");

        HintUI.SetActive(true);
        HintUI.transform.localPosition = MachinePos;
    }

    bool[] clothUIAvailable = {true, true, true};
    
    public void ClickClothUI(int slotNum)
    {
        if (deactiveButtons) return;
        Color tmp;

        //已经穿上工作服了之后，针对放回洗衣机的那件衣服，不可以再拿了
        if(photoTaken)//教学可以从洗衣机这里还衣服
        {
            if(slotNum == 1 && !clothUIAvailable[1])
            {
                clothUIAvailable[slotNum] = true;

                tmp = new Color(1f, 1f, 1f, 1f);
                clothSlotTable[slotNum].GetComponent<SpriteRenderer>().color = tmp;

                //接下来可以教还包了
                FishTalk("ReturnBag", true, 0, true, false);
                ReturnNoticeYes.SetActive(true);
                // StartCoroutine(PickedAllCloth(1f));
            }

            return;
        }
        else
        {
            pickedClothNum ++;
            //点的衣服的发光层取消
            clothSlotTable[slotNum].transform.GetChild (0).gameObject.SetActive(false);
            clothUIAvailable[slotNum] = false; 

            tmp = new Color(0.4f, 0.4f, 0.4f, 0.2f);
            clothSlotTable[slotNum].GetComponent<SpriteRenderer>().color = tmp;
            print("color = " + clothSlotTable[slotNum].GetComponent<SpriteRenderer>().color);

            for(int i = 0; i < clothUIAvailable.Length; i++)
            {
                if(clothUIAvailable[i])
                {
                    ChangeHintPos(ClothUIPos[i], 0);
                    break;
                }
            }

            if(pickedClothNum == 3)
            {
                machineOccupied.SetActive(false);
                machineFront.sprite = emptyImg;

                Hint2D.SetActive(false);
                HintUI.SetActive(false);

                inLoop = false;

                StartCoroutine(PickedAllCloth(1f));
            }
        }
    }

    IEnumerator PickedAllCloth(float time)
    {
        deactiveButtons = true;

        yield return new WaitForSeconds(time);

        machineOpen = false;
        AudioManager.PlayAudio(AudioType.Machine_CloseDoor);
        MachineDoor.sprite = closeDoor;
        machineOccupied.SetActive(true);

        ClothUI.SetActive(false);
        
        // ClothUI.GetComponent<Animator>().SetTrigger("CloseImmediantly");
        // ClothUI.SetActive(false);

        deactiveButtons = false;
    }


    IEnumerator JumpToFish()
    {
        deactiveButtons = true;
        inLoop = true;

        yield return new WaitForSeconds(0.3f);

        Show(scream);
        // CloseMachine();

        HintUI.SetActive(false);
        yield return new WaitForSeconds(0.3f);

        TutorialCameraController.ChangeCameraSpeed(5f);
        TutorialCameraController.GotoPage(1);
        Hide(scream);

        FishTalk("CloseDoor",true, 0, false, false);

        deactiveButtons = false;

        TutorialCameraController.allowScroll = true;
        TutorialCameraController.targetPage = 2;
    }

    public void ClickKarara()
    {
        if(pickedClothNum != 99) return;
        if(stepCounter > 10) return;//过了阶段再点击要给个提示，告诉玩家可以点，但是现在不要点
        if (deactiveButtons) return;

        //disable a lot of things
        Hint2D.SetActive(false);
        Hide(scream);
        ShowFishText(true, 99);

        if(photoTaken)
        {
            StartCoroutine(LongTap());
        }
        
        ShowInventory();
    }


    private void ShowInventory()
    {
        TutorialCameraController.GoToCloset();
        // HintScreen.SetActive(true);
        TutorialCameraController.myCameraState = TutorialCameraController.CameraState.Closet;
        Show(Inventory);
        
        ResetFloatingUI();
    }

    

    IEnumerator ShowBackButton()
    {
        yield return new WaitForSeconds(0.5f);
        InventoryBackButton.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        HintScreen.transform.localPosition = HintPosBackButton;
    }
    

    public void ClickBackButton()
    {
        //if (deactiveButtons) return;

        if(!putBack && photoTaken) return;

        //真正回到地铁
        TutorialCameraController.ReturnToApp();
        TutorialCameraController.myCameraState = TutorialCameraController.CameraState.Subway;

        Hide(Inventory);

        //当karara照完相回来开始还衣服
        if(putBack)
        {
            FishTalk("ReturnBag", true, 0, true, false);
            ShowAnimation("ClothBag");
        }
        else
        {
             HintScreen.SetActive(false);

            //karara现在穿着工作服和拖鞋啦
            GameObject.Find("PlayerEverythingSubway").GetComponent<SpriteRenderer>().enabled = true;

            TutorialCameraController.GotoPage(4);
            posterEnabled = true;

            //准备提示和教学场景
            ShowKarara(2, false, "", false);
            ShowKararaBubble(2, "photo", false);
            ShowAnimation("Poster");

        }

    }
    private bool posterEnabled = true;

    public void ClickShutter()
    {
        AudioManager.PlayAudio(AudioType.Photo_Shutter);
        shutterAnimator.SetTrigger("click");

        HintScreen.SetActive(false);
        Hide(CameraBackground);

        Flashlight.alpha = 1;
        flashing = true;
        
        Debug.Log("click shutter");
        
        string pose = postKararaImage.sprite.name;
        PlayerPrefs.SetInt("KararaPose", (int)Char.GetNumericValue(pose[pose.Length -1]));
	    PlayerPrefs.Save();

        photoTaken = true;

        GoToIns();
    }

    private void GoToIns()
    {
        Show(InsPage);
        Show(phone_background);
        phone_background.blocksRaycasts = false;
        Posture.SetActive(false);
        TutorialCameraController.myCameraState = TutorialCameraController.CameraState.App;

        //照完相后展示tutorial post
        TutorialPost.SetActive(true);

        StartCoroutine(ShowFollowerHint());
    }

    IEnumerator ShowFollowerHint()
    {
        yield return new WaitForSeconds(0.2f);

        Animator followerHintAnimator = FollowerHint.gameObject.GetComponent<Animator>();
        followerHintAnimator.SetTrigger("go");

        float Twait = (float)followerHintAnimator.GetCurrentAnimatorClipInfo(1).Length;

        yield return new WaitForSeconds(Twait);

        Show(followerNum);
        
        for(int i = 0; i < followerNumGoal; i ++)
        {
            followerNumReal ++;
            yield return new WaitForSeconds(followerNumTime / followerNumGoal);

        }

        yield return new WaitForSeconds(1f);

        // Hide(InsPage);
        // Show(Memo);
    }

    public void ClickBackButtonIns()
    {
        Debug.Log("ClickInsBack");
        if (deactiveButtons) return;

        HintScreen.SetActive(false);
        Hide(CameraBackground);
        Hide(phone_background);
        Hide(Memo);
        Hide(InsPage);
        Show(mobile);


        TutorialCameraController.myCameraState = TutorialCameraController.CameraState.Subway;

        //第一次发完照片从ins回到地铁，滑到screen2
        if(stepCounter == 7)
        {
            TutorialCameraController.targetPage = 2;
            Show(scream);
            ShowKarara(1, false, "", false);
            ShowAnimation("PlayerSubwayFolder");
        }
    }


    IEnumerator ReturnPrep()
    {
        yield return new WaitForSeconds(2f);//鱼说完话后等两秒

        TutorialCameraController.GotoPage(2);
        yield return new WaitForSeconds(1f);

        HintUI.SetActive(true);
        HintUI.transform.localPosition = HintPosBag;
        returning = true;
    }
 
    IEnumerator PreReturn()
    {
        yield return new WaitForSeconds(0.2f);

        //衣服消失
        ClothUI.SetActive(false);//有ui的话也消失
        ReturnNotice.SetActive(false);
        
        ClothUI.GetComponent<Animator>().SetTrigger("CloseImmediantly");
        machineFront.sprite = emptyImg;
        yield return new WaitForSeconds(0.2f);

        //关门
        MachineDoor.sprite = closeDoor;
        AudioManager.PlayAudio(AudioType.Machine_CloseDoor);


        yield return new WaitForSeconds(0.2f);

        //包合上
        clothBag.GetComponent<Image>().sprite = closeBag;

        yield return new WaitForSeconds(0.2f);

        //包消失
        clothBag.SetActive(false);
        AudioManager.PlayAudio(AudioType.Bag_Vanish);
        ShowFishText(true, 99);


        TutorialCameraController.allowScroll = false;

        clothReturned = true;
        forwardOneStep = true;
    }

    public void ReturnYes()
    {
        if(!putBack)
        {
            OpenMachine();
            FishTalk("ReturnCloth", true, 0, true, false);
        }
        else{
            StartCoroutine(PreReturn());
        }
    }

    public void ReturnNo()
    {
        //ui消失
        Hide(Notice);   
        ShowFishText(false, 99);
        StartCoroutine(ReturnNoAfter());
        bagClick = 2;
    }

    IEnumerator ReturnNoAfter()
    {
        yield return new WaitForSeconds(0.3f);

        //关门
        HintUI.SetActive(true);
        MachineDoor.sprite = closeDoor;
        returnBagFirstTime = false;
        AudioManager.PlayAudio(AudioType.Machine_CloseDoor);
    }


    private void FishTalkLast()
    {
        Debug.Log("添加follower");

        TutorialCameraController.GotoPage(1);

        FishTalk("MultipleSentence", true, 0, false, true);
    }

    IEnumerator AddFans()
    {
        ///出现followerUI之后，followerNum上升，出现动画，然后进漫画阶段
        //todo: 需要等鱼说完话之后再显示
        yield return new WaitForSeconds(0.5f);

        Show(followerAnimation);
        followerAnimator.SetTrigger("Start");
        followerNumText.text = "17";

        yield return new WaitForSeconds(1f);

        ShowComic();
    }
    
    public void ShowComic()
    {
        Show(Comic);
        Hide(InsPage);
        Hide(followerNum);

        ResetFloatingUI();
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

    public void ProceedToChapterOne()
    {
        SceneManager.LoadScene("StreetStyle");
    }

    public void ClickSweet()
    {
        sweet.SetActive(false);
        ClothFish.SetActive(true);
        isWearingClothNum ++;
        // StartCoroutine(KararaEatSweet());
    }
    private void UpdateProgressBar()
    {
        progressBar.fillAmount = timer/5f;
        
    }
}

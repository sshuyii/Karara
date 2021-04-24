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





public class TutorialManagerNew : MonoBehaviour
{

    public int stepCounter,bagClick;
    public bool forwardOneStep;
    private bool AdClick1stTime = true;
    private bool allowScroll = false;
    [SerializeField]
    private bool machineOpen = false;
    private bool timerStop = false;
    private bool isFirstOpen = true;
    private bool scrollTeaching = false;
    private bool forwardLater = false;

    [SerializeField]
    private bool deactiveButtons = false;
    private bool inLoop = false;
    private bool flashing = false;
    private bool isInitialPosture = true;
    private bool returning = false;

    private float realTimer;
    private float timer = 0;
    public TextMeshPro timerNum;
    private TutorialCameraController TutorialCameraController;
    private FishTextManager FishTextManager;


    [SerializeField]
    TextMeshPro fishText;
    [SerializeField]
    TextMeshProUGUI followerNumText;

    [SerializeField]
    private GameObject Exclamation, Posture, Phone,Hint2D, HintUI, HintScreen, ScrollHint, clothBag,machineFull, machineEmpty, phoneAnimation, changePostureButton,
        machineOccupied, ClothUI, KararaC, KararaB, KararaA, EmojiBubble, InventoryBackButton, Ins, Shutter, Notice, FishTalkButton, inventoryBubble, inventoryFish;

    [SerializeField]
    private CanvasGroup CameraBackground, scream, Inventory, Flashlight, FloatingUI, RedDot, Comic;

    [SerializeField]
    private Vector3 HintPosPoster, HintPosCamera, HintPosBag, MachinePos, ClothUIPos, HintPosKarara, HintPosBackButton,
        HintPosShutter, HintPosInsBack, ParticlePos1, KararaAInScreen1Pos;

    [SerializeField]
    private Sprite initialBody, pos0Body, pos1Body, pos0Under, pos1Under, pos0Work, pos1Work, pos0WorkAlter, pos1WorkAlter,openBag, closeBag, fullImg, emptyImg, EmojiOpenDoor, unhappyFace, happyFace,
        EmojiCloth, EmojiHappy, EmojiUnhappy, openDoor, closeDoor, cameraBGFull, postPose1, postPose2, postPose1Alter, postPose2Alter;

    [SerializeField]
    private SpriteRenderer body, everything, machineFront, emoji, KararaFace, MachineDoor, inventoryBubbleSR, inventoryFishSR;


    [SerializeField]
    private CanvasGroup followerNum, followerAnimation, mobile;

    [SerializeField]
    private Image postKararaImage;

    [SerializeField]
    private Animator shoeAnimator, followerAnimator, phoneAnimator;

    public SpriteRenderer[] subwayCloth, inventoryCloth, adsCloth;


    public Sprite workInventory, workSubway, workAds0, workAds1,workAlterInventory, workAlterSubway, workAds2, workShoe, workShoeSubway,
        hoodie, barefoot, transparent;

    private float KararaPosX = -39f;

    public enum MachineState
    {
        Empty,
        Washing,
        Finish
    }

    MachineState myMachineState;
    FishText currentFT;
    Coroutine lastRoutine = null;
    AudioManager AudioManager;

    //穿上一件，另一件放回洗衣机
    public bool workClothOn = false;
    public bool putClothBack = false;//需要放回去一件衣服
    public bool workShoeOn = false;

    //偷了几件衣服
    int pickedClothNum = 0;
    public List<GameObject> ClothSlotList;
    public Dictionary <int, GameObject> clothSlotTable = new Dictionary <int, GameObject> ();

    public bool isAlter;//用于判断穿的是哪一件工作服

    //inventory里穿了哪几件衣服
    public int isWearingClothNum = 0;
    public bool isWearingShoe = false;

    //inventory里karara
    public Animator kararaAnimator;
    
    [SerializeField]
    private Animator ClothUiAnimator;


    public List<GameObject> ReturnNoticeList;


    private GameObject MainCamera;

    public TutorialTransition TutorialTransition;
    void Start()
    {
        //Set Camera position and size in stage 3  
        TutorialCameraController = GameObject.Find("Main Camera").GetComponent<TutorialCameraController>();
        MainCamera = GameObject.Find("Main Camera");
                    

        AudioManager = GameObject.Find("---AudioManager").GetComponent<AudioManager>();
        //step1
        stepCounter = 0;
        bagClick = 0;

        myMachineState = MachineState.Empty;

        workInventory = Resources.Load<Sprite>("Images/Karara/Cloth/Inventory/work");
        workSubway = Resources.Load<Sprite>("Images/Karara/Cloth/Subway/work");
        workAds0 = Resources.Load<Sprite>("Images/Karara/Cloth/Pose/Pose1_1/work");
        workAds1 = Resources.Load<Sprite>("Images/Karara/Cloth/Pose/Pose1_3/work");

        FishTextManager = GameObject.Find("---FishTextManager").GetComponent<FishTextManager>();

        //disable a lot of things when tutorial starts
        KararaB.SetActive(false);
        phoneAnimation.SetActive(false);

        //所有洗衣机里的衣服
        for(int i = 0; i < ClothSlotList.Count; i++)
        {
            clothSlotTable.Add(i+1, ClothSlotList[i]);
        }

        inventoryBubbleSR = inventoryBubble.GetComponentsInChildren<SpriteRenderer>()[1];

    }

    IEnumerator LongTap()
    {
        //出现鱼教学长按还衣服
        yield return new WaitForSeconds(1.5f);
        inventoryFish.SetActive(true);//todo: 说话的打字机效果

        yield return new WaitForSeconds(1f);

        shoeAnimator.SetBool("isShining", true);

        //对话框出现3s后消失（暂时
        yield return new WaitForSeconds(3f);
        inventoryFish.SetActive(false);    
    }

    bool longTap = false;
    // Update is called once per frame
    void Update()
    {

        if(TutorialTransition.TransitionStage == 3)
        {
            Hide(TutorialTransition.TransparentButton);
            MainCamera.GetComponent<Camera>().orthographicSize = 5;
            MainCamera.GetComponent<RectTransform>().position = new Vector3(17.5f, 0, -10f);
            TutorialTransition.trainWithWindow.SetActive(false);
            
            TutorialTransition.TransitionStage = 4;
            //正式开始，进入车厢
            StartCoroutine(ShowExclamation());
        }

        if (!timerStop)
        {
            if (myMachineState == MachineState.Washing)
            {
                CalculateTime();
            }

            timer += Time.deltaTime;
        }

        //在inventory里穿衣服
        //穿上工作服>>穿上鞋>>>鱼开始让人还衣服
        if(isWearingClothNum == 3 && !longTap)
        {           
            longTap = true;
            StartCoroutine(LongTap());
        }

        //还掉鞋之后出现返回地铁按钮
        if(putClothBack && stepCounter == 10)
        {
            forwardOneStep = true;
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


        if (inLoop && TutorialCameraController.currentPage == 4)
        {
            TutorialCameraController.JumpToPage(3);
            Debug.Log("what's going on????");

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
                    EmojiBubble.SetActive(true);
                    KararaTalk(EmojiOpenDoor);
                    HintUI.SetActive(true);
                }

                TutorialCameraController.targetPage = 1;
            }
            else if (inLoop && TutorialCameraController.targetPage == 1)
            {
               
                TutorialCameraController.targetPage = 2;

                forwardLater = false;
                if (machineOpen) FishTalk("CloseDoor",true);
                else FishTalk("Right",true);
            }
            else if(inLoop && TutorialCameraController.currentPage == 3)
            {
                TutorialCameraController.JumpToPage(2);

            }
            else if (TutorialCameraController.targetPage == 1)
            {

                //FishTalk("Return this bag!",true);
                // after ins ->return prep
            }
            
                TutorialCameraController.targetPage = -1;
        }

        //if(currentFT.isPlaying)
        //{
        //    FishTalkNextSentence();
        //}

        //go to next step
        if (forwardOneStep)
        {
            forwardOneStep = false;
            stepCounter++;
            switch (stepCounter)
            {
                case 1:
                    ShowCameraBG();
                    break;

                case 2:
                    StartCoroutine(BackToSubway());
                    StartCoroutine(ScrollTeaching(1));
                    KararaB.SetActive(true);
                    break;
                case 3:
                    scrollTeaching = false;

                    forwardLater = true;
                    FishTalk("Late", true);

                    break;
                case 4:
                    //鱼讲完话前进
                    BagOccur();
                    //包出现在screen1
                    //forward的时机：衣服进了洗衣机
                    break;
                case 5:
                    //衣服进了洗衣机
                    // PhoneOccur();
                    break;
                case 6:
                    BackToWashing();//时间开始前进
                    break;
                case 7:
                    AfterWash();
                    //从洗完到小循环前提
                    break;
                case 8:
                    StartCoroutine(JumpToFish());
                    //小循环-小循环跳出
                    break;
                case 9:
                    AfterPickUpCloth();
                    break;
                case 10:
                    ShowInventory();
                    break;
                case 11:
                    //还了鞋之后就可以显示返回地铁的按钮
                    StartCoroutine(ShowBackButton());
                    //一定是还了一件衣服才能返回的，所以洗衣机里又有衣服了
                    machineFront.sprite = fullImg;

                    break;
                case 12:
                    ReadyForPhoto();
                    break;
                case 13:
                    //照相界面下面有个黑条，改变姿势的透明图层需要变小一点
                    RectTransform rt = changePostureButton.GetComponent<RectTransform>();
                    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 20);
                    rt.sizeDelta = new Vector2 (700, 500);

                    ShowCameraBG();
                    break;
                case 14:
                    GoToIns();
                    break;
                case 15:
                    //从ins直接回到screen2
                    StartCoroutine(AfterIns());
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
        }
    }

    private void CalculateTime()
    {
       
        realTimer = 5f - timer;
        
        timerNum.text = "0:0" + Mathf.RoundToInt(realTimer).ToString();


        if(realTimer<0)
        {
            AudioManager.PlayAudio(AudioType.Machine_Finished);
            myMachineState = MachineState.Finish;
            forwardOneStep = true;
        }

    }


    IEnumerator ShowExclamation()
    {
        yield return new WaitForSeconds(1f);
        KararaA.SetActive(true);

        yield return new WaitForSeconds(1f);

        Exclamation.SetActive(true);

        yield return new WaitForSeconds(1f);

        Hint2D.SetActive(true);
    }



    public void ClickAd()
    {
        if (deactiveButtons) return;
        
        AudioManager.PlayAudio(AudioType.Poster_Enter);

        Debug.Log("ClickAd");
        if (AdClick1stTime)
        {
            Exclamation.SetActive(false);
            forwardOneStep = true;
        }
        else
        {
            if(stepCounter != 12) 
            {
                //todo:可能要替换成其他的，比如震动一下，鱼说一句话
                //这几个能到screen 4的阶段都在显示scream
                return;
            }
            Shutter.SetActive(true);
            Hint2D.SetActive(false);
            forwardOneStep = true;
        }
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
        KararaA.SetActive(false);
        Debug.Log("ShowCameraBG");
        if (AdClick1stTime)
        {
            Posture.SetActive(true);
            Show(CameraBackground);
            ChangeHintPos(HintPosCamera, 3);
            body.sprite = initialBody;

        }
        else
        {
            // set all children
            Posture.SetActive(true);
            Show(CameraBackground);
            CameraBackground.GetComponent<Image>().sprite = cameraBGFull;

            StartCoroutine(ShowShutterHint());


            //身上穿的衣服
            // if(isAlter) {body.sprite = pos0WorkAlter;}
            // else {body.sprite = pos0Work;}            
            
            everything.sprite = null;
            HintScreen.transform.localPosition = HintPosShutter;
            
        }

    }


    [SerializeField]
    int clickTime = 0;

    public void ClickChangePosture()
    {
        if (deactiveButtons) return;


        AudioManager.PlayAudio(AudioType.Photo_Pose);
        Debug.Log("ClickChangePosture");

        if (AdClick1stTime)
        {
            clickTime ++;

            if(clickTime == 1)
            {
                body.sprite = pos0Body;
                everything.sprite = pos0Under;            
            }
            else if(clickTime == 2)
            {   
                body.sprite = pos1Body;
                everything.sprite = pos1Under;
            }
            else if(clickTime == 3)
            {
                AdClick1stTime = false;
                Hint2D.SetActive(false);
                forwardOneStep = true;

               
                //reset clickTime for 2nd time
                clickTime = 0;
            }
        }
        else if(stepCounter == 13)
        {
            clickTime ++;

            if(clickTime == 1)
            {
                // //身上穿的衣服
                // if(isAlter) {body.sprite = pos0WorkAlter;}
                // else {body.sprite = pos0Work;}
                body.sprite = pos0Work;

                //第一次发的post
                isInitialPosture = false;
                // if(isAlter) {postKararaImage.sprite = postPose1Alter;}
                // else {postKararaImage.sprite = postPose1;}

                postKararaImage.sprite = postPose1;
       
            }
            else if(clickTime == 2)
            {   
                // //身上穿的衣服
                // if(isAlter) {body.sprite = pos1WorkAlter;}
                // else {body.sprite = pos1Work;}
                body.sprite = pos1Work;


                clickTime = 0;

                //第一次发的post
                isInitialPosture = true;
                // if(isAlter) {postKararaImage.sprite = postPose2Alter;}
                // else {postKararaImage.sprite = postPose2;}
                postKararaImage.sprite = postPose2;
            }

            Debug.Log("ClickChangePosture 2nd");
        }

    }

    IEnumerator BackToSubway()
    {
        //出现fish喊话
        Show(scream);
        yield return new WaitForSeconds(0.5f);

        KararaA.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Hide(CameraBackground);
        Posture.SetActive(false);
    }


    IEnumerator ScrollTeaching(int targetPage)
    {
        yield return new WaitForSeconds(1.5f);
        Show(scream);
        TutorialCameraController.allowScroll = true;
        TutorialCameraController.targetPage = targetPage;

        yield return new WaitForSeconds(1f);

        scrollTeaching = true;
        ScrollHint.SetActive(true);

    }

    private void FishTalk(string keyWord, bool animateOrNot)
    {
        FishTalkButton.SetActive(true);
        currentFT = FishTextManager.GetText(keyWord);

        FishTalk(animateOrNot);
    }

    private void FishTalk(bool animateOrNot)
    {
        int idx = currentFT.playingIdx;
        if (animateOrNot) 
        {
            lastRoutine = StartCoroutine(AnimateText(fishText, currentFT.content[idx]));
        }
        else
        {
            //when last sentence
            if (currentFT.playingIdx > currentFT.content.Count - 2)
            {
                FishTalkButton.SetActive(false);
                currentFT.isPlaying = false;
                forwardOneStep = forwardLater;
            }

            fishText.text = currentFT.content[idx];
        }

        Debug.Log("idx " + idx);
        Debug.Log("fishTalking " + currentFT.content[idx]);
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

        if (currentFT.playingIdx < currentFT.content.Count) FishTalk( true);
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
    }

    public void ClickBag()
    {
        if (deactiveButtons) return;
        
        bagClick++;
        Debug.Log("Bag click " + bagClick.ToString());

        if(bagClick == 1)
        {
            // AudioManager.AdjustPitch(AudioType.Bag_Phase1, 0.5f);
            AudioManager.PlayAudio(AudioType.Bag_Phase1);
            StartCoroutine(BeforeWash());
        }

        else if(bagClick == 2)
        {
            // AudioManager.AdjustPitch(AudioType.Bag_Phase1, 0.6f);
            AudioManager.PlayAudio(AudioType.Bag_Phase1);
            HintUI.SetActive(false);//点完包后提示消失一会儿，等洗衣机开门动画播完之后再出现在洗衣机上

            ThrowClothToMachine();
        }
        else
        {
            if (inLoop)
            {
                //du地一声
                AudioManager.PlayAudio(AudioType.No);
                bagClick = 2;
                StartCoroutine(ChangeFace());
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

    IEnumerator ShowReturnNotice()
    {
        yield return new WaitForSeconds(0.2f);
        MachineDoor.sprite = openDoor;

        yield return new WaitForSeconds(0.2f);

        Notice.SetActive(true);    
    }


    private IEnumerator ChangeFace()
    {
        Debug.Log("change face");
        KararaTalk(EmojiOpenDoor);
        KararaFace.sprite = unhappyFace;
        // KararaTalk(EmojiUnhappy);

        yield return new WaitForSeconds(2f);

        HintUI.transform.localPosition = MachinePos;

        //KararaFace.sprite = happyFace;//维持不开心表情

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

        //FishTalk("Click Bag",false);
        //TutorialCameraController.GotoPage(1);


        //yield return new WaitForSeconds(2f);
        //HintUI.SetActive(true);
        HintUI.transform.localPosition = newPos;
        //TutorialCameraController.GotoPage(2);

        deactiveButtons = false;
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

        yield return new WaitForSeconds(0.2f);//等一会儿再出现提示ui
        HintUI.transform.localPosition = MachinePos;
        HintUI.SetActive(true);

        forwardOneStep = true;
    }

    public void ClickMachine()
    {
        if (deactiveButtons||stepCounter < 5) //确保在没进行到洗衣机的时候洗衣机不能点
        {
            //播放“du”声音
            AudioManager.PlayAudio(AudioType.No);
            return;
        }
        
        switch (myMachineState)
        {
            case MachineState.Empty:
                AudioManager.PlayAudio(AudioType.Machine_OnOff);
                AudioManager.PlayAudio(AudioType.Machine_Washing);
                Debug.Log("start washing");

                machineFull.SetActive(true);
                machineEmpty.SetActive(false);
                HintUI.SetActive(false);
                timer = 0;
                myMachineState = MachineState.Washing;

                StartCoroutine(StopTimerAndForward());
                //进入找到手机的部分
                // forwardOneStep = true;

                break;
            case MachineState.Finish:
                if(deactiveButtons) return;
                if (!machineOpen) OpenMachine();
                else CloseMachine();

                //如果第二次打开了洗衣机门，鱼要阻止
                if (stepCounter == 8)
                {
                    Show(scream);
                    FishTalk("Open2ndTime", true);

                }
                break;
        }
    }


    IEnumerator StopTimerAndForward()
    {
        yield return new WaitForSeconds(1.5f);
        phoneAnimator.SetTrigger("isCharged");

        //todo：直接按clip长度走，不用每次都改具体时间？
        yield return new WaitForSeconds(2f);
        //提示手机动画
        //考虑一下出现手机动画的时机，目前是洗衣机转一会儿之后才出现
        phoneAnimation.SetActive(true);

        timerStop = true;
        // forwardOneStep = true;
    }

    public void OpenMachine()
    {
        machineOpen = true;
        deactiveButtons = true;//打开过程有一定时间，到时间前不可再次点击

        AudioManager.PlayAudio(AudioType.Machine_OpenDoor);

        if (isFirstOpen)
        {
            Debug.Log("isFirstOpen and open machine");

            StartCoroutine(OpenCloseDoor(0.2f));
        }
        else if(inLoop)
        {
            Debug.Log("in loop and open machine");
            StartCoroutine(OpenDoor(0.2f));

            // Hint2D.SetActive(true);
            KararaTalk(EmojiCloth);
            // ChangeHintPos(ClothUIPos, 0);
            //forwardOneStep = true;

        }
        else {
            // MachineDoor.sprite = openDoor;
            StartCoroutine(OpenDoor(0.3f));

        }
    }

    IEnumerator OpenCloseDoor(float time)
    {
         ///门打开后出现一下衣服，立即关上
        machineOccupied.SetActive(false);
        MachineDoor.sprite = openDoor;
        AudioManager.PlayAudio(AudioType.Machine_OpenDoor);

        yield return new WaitForSeconds(time);

        ClothUI.SetActive(true);
        ClothUiAnimator.SetTrigger("StartClosing");
        
        float Twait = ClothUiAnimator.GetCurrentAnimatorClipInfo(0).Length;
        Debug.Log("TWait: " + Twait);
        yield return new WaitForSeconds(Twait);
   

        MachineDoor.sprite = closeDoor;
        AudioManager.PlayAudio(AudioType.Machine_CloseDoor);

        machineOpen = false;
        ClothUI.SetActive(false);

        deactiveButtons = false;//打开过程有一定时间，到时间前不可再次点击
        isFirstOpen = false;
        forwardOneStep = true;

    }

     IEnumerator OpenDoor(float time)
    {
        ///播放开门动画，出现对话框
        machineOccupied.SetActive(false);
        MachineDoor.sprite = openDoor;

        HintUI.SetActive(false);
        // if (deactiveButtons)
        // {
        //     Hint2D.SetActive(false);
        // }

        if(pickedClothNum != 3)
        {
             Hint2D.SetActive(false);
        }
        yield return new WaitForSeconds(time);

        ClothUI.SetActive(true);
        ClothUiAnimator.SetTrigger("StartShowing");
        
        yield return new WaitForSeconds(time);//等动画播完

        deactiveButtons = false;//打开过程有一定时间，到时间前不可再次点击
    }



    IEnumerator CloseDoor(float time)
    {
        ///播放关门动画，出现对话框
        //直接关上，窗口没有动画
        deactiveButtons = true;
        AudioManager.PlayAudio(AudioType.Machine_CloseDoor);


        machineOccupied.SetActive(true);
        KararaTalk(EmojiOpenDoor);
        // ClothUiAnimator.SetTrigger("StartClose");
        ClothUI.SetActive(false);

    
        yield return new WaitForSeconds(time);

        MachineDoor.sprite = closeDoor;

        yield return new WaitForSeconds(time);

        if(pickedClothNum != 3) 
        {
            Hint2D.SetActive(false);

            HintUI.SetActive(true);
        }
        deactiveButtons = false;
        
    }


    public void CloseMachine()
    {
        machineOpen = false;

        // AudioManager.PlayAudio(AudioType.Machine_OpenDoor);
        if (inLoop)
        {
            if (Hint2D.active) Hint2D.SetActive(false);
            StartCoroutine(CloseDoor(0.1f));
        }
        else
        {
            StartCoroutine(CloseDoor(0.1f));

        }
    }



    void PhoneOccur()
    {
        //TutorialCameraController.GotoPage(1);
        Debug.Log("PhoneOccur");
    }

    public void PickChargingPhone()
    {
        if (!timerStop) return;
        if (deactiveButtons) return;

        Debug.Log("PickChargingPhone");
        Phone.SetActive(false);
        //TutorialCameraController.GotoPage(2);
        forwardOneStep = true;//从4到5
    }


    void BackToWashing()
    {
        KararaC.SetActive(true);
        KararaB.SetActive(false);

        KararaA.SetActive(false);

       
        timerStop = false;
    }


    void AfterWash()
    {
        
        //myMachineState = MachineState.Finish;
        machineFull.SetActive(false);
        machineEmpty.SetActive(true);
        machineOccupied.SetActive(true);

        EmojiBubble.SetActive(true);
        KararaTalk(EmojiOpenDoor);

        HintUI.SetActive(true);
        HintUI.transform.localPosition = MachinePos;
    }

  
    public void ClickClothUI(int slotNum)
    {
        if (deactiveButtons) return;

        //已经穿上工作服了之后，针对放回洗衣机的那件衣服，不可以再拿了
        if(stepCounter > 10) return;
        //todo: 有个震动或者其他提示

        pickedClothNum ++;
        //点的衣服
        clothSlotTable[slotNum].SetActive(false);

        if(pickedClothNum == 3)
        {
            machineOccupied.SetActive(false);
            machineFront.sprite = emptyImg;

            Hint2D.SetActive(false);
            HintUI.SetActive(false);

            inLoop = false;

            StartCoroutine(PickedAllCloth(0.2f));
        }
    }

    IEnumerator PickedAllCloth(float time)
    {
        ///衣服UI和背景直接消失，洗衣机里的衣服直接消失，提示直接消失，等一段时间后门关上
        deactiveButtons = true;

        // KararaTalk(EmojiOpenDoor);
        ClothUI.SetActive(false);

        yield return new WaitForSeconds(time);

        MachineDoor.sprite = closeDoor;
        AudioManager.PlayAudio(AudioType.Machine_CloseDoor);
        machineOpen = false;


        deactiveButtons = false;
        forwardOneStep = true;
        //fish continue talking
        Show(scream);
        FishTalk("NoTouchCloth", true);

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

        forwardLater = false;
        FishTalk("CloseDoor",true);

        deactiveButtons = false;

        TutorialCameraController.allowScroll = true;
        TutorialCameraController.targetPage = 2;
    }

    private void AfterPickUpCloth()
    {
        Debug.Log("AfterPickUp");
        KararaTalk(EmojiHappy);

        Hint2D.SetActive(true);
        ChangeHintPos(HintPosKarara,0);
    }

    public void ClickKarara()
    {
        if(stepCounter < 9) return;
        if (deactiveButtons) return;
        Hint2D.SetActive(false);
        forwardOneStep = true;
        Hide(scream);
    }


    private void ShowInventory()
    {
        TutorialCameraController.GoToCloset();
        // HintScreen.SetActive(true);
        Show(Inventory);
    }

    

    IEnumerator ShowBackButton()
    {
        yield return new WaitForSeconds(0.5f);
        InventoryBackButton.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        HintScreen.transform.localPosition = HintPosBackButton;
    }
    
    IEnumerator KararaTalkInventory(Sprite emoji)
    {
        //出现karara对话框，需要有个动画吸引玩家注意力
        yield return new WaitForSeconds(0.1f);
        kararaAnimator.SetTrigger("isShaking");

        yield return new WaitForSeconds(1f);

        inventoryBubble.SetActive(true);
        inventoryBubbleSR.sprite = emoji;

        //对话框出现3s后消失（暂时
        yield return new WaitForSeconds(3f);
        inventoryBubble.SetActive(false);
    }

    public void ClickBackButton()
    {
        if (deactiveButtons) return;
        if(!putClothBack) return;

        // if(inventoryCloth[3].sprite != workInventory &&
        //     inventoryCloth[3].sprite != workAlterInventory) 
        // {
        //     //如果两件工作服都没穿的话
        //     //todo: 手机震动
        //     Handheld.Vibrate();

        //     //todo: 提示不想只穿白帽衫
        //     StartCoroutine(KararaTalkInventory(hoodie));
            
        //     return;
        // }
        // else{
        //     //如果穿上了其中一件
        //     if(inventoryCloth[2].sprite != workShoe)//但是没穿鞋
        //     {
        //         //todo: 手机震动
        //         Handheld.Vibrate();


        //         //todo: 提示不想光脚
        //         StartCoroutine(KararaTalkInventory(barefoot));

        //         return;
        //     }
        // }

        // //如果有衣服没有还，也不能回到地铁
        // if(!putClothBack) 
        // {
        //     //todo: 鱼告诉karara要把衣服都还了
        //     StartCoroutine(FishTalkInventory());
            
        //     return;
        // }

        //真正回到地铁
        TutorialCameraController.ReturnToApp();

        Hide(Inventory);

        HintScreen.SetActive(false);

        TutorialCameraController.JumpToPage(4);

        //karara现在穿着工作服啦
        GameObject.Find("PlayerEverythingSubway").GetComponent<SpriteRenderer>().enabled = true;
        KararaC.SetActive(true);
        forwardOneStep = true;
    }

    IEnumerator FishTalkInventory()
    {
        //出现karara对话框，需要有个动画吸引玩家注意力
        yield return new WaitForSeconds(0.1f);

        inventoryFish.SetActive(true);

        //对话框出现3s后消失（暂时
        yield return new WaitForSeconds(3f);
        inventoryFish.SetActive(false);    
    }


    private void ReadyForPhoto()
    {
        // KararaC.SetActive(false);
        // KararaA.SetActive(true);

        Hint2D.SetActive(true);
        ChangeHintPos(HintPosPoster, 0);

        TutorialCameraController.allowScroll = true;
        TutorialCameraController.targetPage = 4;

        //暂时不用教第二次滑动
        // ScrollHint.SetActive(true);
        // ScrollHint.transform.localRotation = new Quaternion(0,0,180,0);
        // foreach (Transform child in ScrollHint.transform)
        // {
        //     child.localRotation = new Quaternion(0, 0, 180, 0);
        // }
    }


    public void ClickShutter()
    {
        AudioManager.PlayAudio(AudioType.Photo_Shutter);
        HintScreen.SetActive(false);
        Hide(CameraBackground);

        Flashlight.alpha = 1;

        flashing = true;
        
        Debug.Log("click shutter");
        forwardOneStep = true;
        
    }




    private void GoToIns()
    {
        Ins.SetActive(true);
        Posture.SetActive(false);
        TutorialCameraController.myCameraState = TutorialCameraController.CameraState.App;
        //HintPosInsBack 暂时不用
    }

    public void ClickBackButtonIns()
    {
        Debug.Log("ClickInsBack");
        if (deactiveButtons) return;
        HintScreen.SetActive(false);
        Ins.SetActive(false);
        Hide(CameraBackground);
        

        TutorialCameraController.myCameraState = TutorialCameraController.CameraState.Subway;

        forwardOneStep = true;

    }


    IEnumerator AfterIns()
    {
        // only phone
        // Show(FloatingUI);
        // Particle.SetActive(true);

        TutorialCameraController.JumpToPage(1);//从ins回到subway scene直接跳转到包在的页面
        yield return new WaitForSeconds(0.6f);

        //show hint arrow
        // ReturnPrep();

        // Particle.SetActive(false);
        
        // Show(scream);
        //ScrollHint.transform.localRotation = new Quaternion(0, 0, 0, 0);
        //ScrollHint.SetActive(true);

        forwardLater = true;//为true则说完话之后stepCounter向前进1
        FishTalk("ReturnBag", true);
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


        TutorialCameraController.allowScroll = false;
        yield return new WaitForSeconds(0.5f);

        forwardOneStep = true;
    }

    public void ReturnYes()
    {
        //ui消失
        Notice.SetActive(false);
        HintUI.SetActive(false);

        StartCoroutine(PreReturn());
    }

    public void ReturnNo()
    {
        //ui消失
        Notice.SetActive(false);

        StartCoroutine(ReturnNoAfter());

        bagClick = 2;
    }
    IEnumerator ReturnNoAfter()
    {
        yield return new WaitForSeconds(0.3f);

        //关门
        HintUI.SetActive(true);
        MachineDoor.sprite = closeDoor;
        AudioManager.PlayAudio(AudioType.Machine_CloseDoor);
    }


    private void FishTalkLast()
    {
        Debug.Log("添加follower");

        TutorialCameraController.GotoPage(1);

        //把kararaA换到screen1
        // Vector3 pos = KararaA.transform.localPosition;
        // pos.x = KararaPosX;
        // KararaA.transform.localPosition = pos;

        //screen1用kararaC
        // KararaB.SetActive(true);

        forwardLater = true;
        FishTalk("MultipleSentence", true);
    }

    public void ClickMobile()
    {
        ///点击mobile按钮，出现followerNum
        Show(followerNum);
        Hide(RedDot);

        StartCoroutine(AddFans());
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
        
        // Particle.transform.localPosition = ParticlePos1;
        // Particle.transform.localScale = new Vector3(2, 2, 1);

        // foreach (Transform child in FloatingUI.transform)
        // {
        //     child.gameObject.SetActive(true);
        // }
        // KararaA.transform.localRotation = new Quaternion(0, 180, 0,0);
        // KararaA.transform.localPosition = KararaAInScreen1Pos;
        // KararaA.SetActive(true);
        // Exclamation.transform.localPosition = Exclamation.transform.localPosition;
        // Exclamation.SetActive(true);
        // Hint2D.SetActive(true);
        // ChangeHintPos(Exclamation.transform.position,0);

        // yield return new WaitForSeconds(1f);
    }


    public void ClickExclamation()
    {
        Hint2D.SetActive(false);
        forwardOneStep = true;
    }


    // IEnumerator ShowComic()
    // {
        
    //     Show(Comic);

    //     yield return new WaitForSeconds(2f);

    //     SceneManager.LoadScene("StreetStyle", LoadSceneMode.Single);

    // }

    
    public void ShowComic()
    {
        Show(Comic);
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



    IEnumerator AnimateText(TextMeshPro text, string textContent)
    {
        //TutorialCameraController.

        currentFT.isPlaying = true;
        for (int i = 0; i < (textContent.Length + 1); i++)
        {
            if (!currentFT.isPlaying) break;

            text.text = textContent.Substring(0, i);
            yield return new WaitForSeconds(.1f);
        }

  

        FishTalkNextSentence();
    }

    void KararaTalk(Sprite sprite)
    {
        emoji.sprite = sprite;
    }

    public void ProceedToChapterOne()
    {
        SceneManager.LoadScene("StreetStyle");

    }
}

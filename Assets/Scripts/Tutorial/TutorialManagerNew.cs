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
    private bool forwardOneStep;
    private bool AdClick1stTime = true;
    private bool allowScroll = false;
    private bool machineOpen = false;
    private bool timerStop = false;
    private bool isFirstOpen = true;
    private bool scrollTeaching = false;

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


    [SerializeField]
    TextMeshPro fishText;

    [SerializeField]
    private GameObject Exclamation, Posture, Phone,Hint2D, HintUI, HintScreen,ScrollHint, clothBag,machineFull, machineEmpty, phoneAnimation, changePostureButton,
        machineOccupied,ClothUI, KararaC,KararaB,KararaA,EmojiBubble,InventoryBackButton, Ins, Particle,Shutter,Notice;

    [SerializeField]
    private CanvasGroup CameraBackground,scream, Inventory, Flashlight, FloatingUI,Comic;

    [SerializeField]
    private Vector3 HintPosPoster, HintPosCamera, HintPosBag,MachinePos, ClothUIPos,HintPosKarara,HintPosBackButton,
        HintPosShutter,HintPosInsBack, ParticlePos1, KararaAInScreen1Pos;

    [SerializeField]
    private Sprite initialBody,pos0Body, pos1Body, pos0Under, pos1Under, pos0Work, pos1Work,openBag,fullImg,emptyImg,EmojiOpenDoor, unhappyFace,happyFace,
        EmojiCloth, EmojiHappy, EmojiUnhappy, openDoor, closeDoor, transparet,cameraBGFull;

    [SerializeField]
    private SpriteRenderer body, everything,machineFront, emoji,KararaFace,MachineDoor;


    [SerializeField]
    SpriteRenderer[] subwayCloth, inventoryCloth, adsCloth;


    private Sprite workInventory, workSubway, workAds0, workAds1;

    private float KararaPosX = -39f;

    public enum MachineState
    {
        Empty,
        Washing,
        Finish
    }

    MachineState myMachineState;


    void Start()
    {
        TutorialCameraController = GameObject.Find("Main Camera").GetComponent<TutorialCameraController>();

        //step1
        stepCounter = 0;
        bagClick = 0;
        StartCoroutine(ShowExclamation());

        myMachineState = MachineState.Empty;

        workInventory = Resources.Load<Sprite>("Images/Karara/Cloth/Inventory/work");
        workSubway = Resources.Load<Sprite>("Images/Karara/Cloth/Subway/work");
        workAds0 = Resources.Load<Sprite>("Images/Karara/Cloth/Pose/Pose1_1/work");
        workAds1 = Resources.Load<Sprite>("Images/Karara/Cloth/Pose/Pose1_3/work");

        //disable a lot of things when tutorial starts
        KararaB.SetActive(false);
        phoneAnimation.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if(!timerStop) timer += Time.deltaTime;
        if (myMachineState == MachineState.Washing)
        {
           
            CalculateTime();
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
                    EmojiBubble.SetActive(true);
                    KararaTalk(EmojiOpenDoor);
                    HintUI.SetActive(true);
                }


                TutorialCameraController.targetPage = 1;
            }
            else if (inLoop && TutorialCameraController.targetPage == 1)
            {
               
                TutorialCameraController.targetPage = 2;
                if (machineOpen) FishTalk("Close the door!!",false);
                else FishTalk("Right!", false);
            }
            else if (TutorialCameraController.targetPage == 1)
            {

                //FishTalk("Return this bag!",true);
                // after ins ->return prep
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
                    ShowCameraBG();
                    break;

                case 2:
                    StartCoroutine(BackToSubway());
                    StartCoroutine(ScrollTeaching(1));
                    KararaB.SetActive(true);
                    break;
                case 3:
                    scrollTeaching = false;
                    FishTalk("too late",true);
                    break;
                case 4:
                    BagOccur();
                    //从包出现到看到手机之前
                    break;
                case 5:
                    PhoneOccur();
                    break;
                case 6:
                    BackToWashing();
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
                    ShowBackButton();
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
                    //
                    ReturnPrep();
                    break;

                case 17:
                    StartCoroutine(AddFans());
                    break;
                case 18:
                    StartCoroutine(ShowComic());
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
            forwardOneStep = true;
        }

    }


    IEnumerator ShowExclamation()
    {
        yield return new WaitForSeconds(1f);

        Exclamation.SetActive(true);
        Hint2D.SetActive(true);
    }



    public void ClickAd()
    {
        if (deactiveButtons) return;


        Debug.Log("ClickAd");
        if (AdClick1stTime)
        {
            Exclamation.SetActive(false);
            forwardOneStep = true;
        }
        else
        {
            Shutter.SetActive(true);
            Hint2D.SetActive(false);
            forwardOneStep = true;
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
            HintScreen.SetActive(true);
            body.sprite = pos0Work;
            everything.sprite = null;
            HintScreen.transform.localPosition = HintPosShutter;
            
        }

    }


    [SerializeField]
    int clickTime = 0;

    public void ClickChangePosture()
    {
        if (deactiveButtons) return;

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
        else
        {
            clickTime ++;

            if(clickTime == 1)
            {
                body.sprite = pos0Work;
                // everything.sprite = pos0Work;     
                isInitialPosture = false;
       
            }
            else if(clickTime == 2)
            {   
                body.sprite = pos1Work;
                // everything.sprite = pos1Work;
                clickTime = 0;
                isInitialPosture = true;

            }

            Debug.Log("ClickChangePosture 2nd");
        }

    }

    IEnumerator BackToSubway()
    {
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
        scrollTeaching = true;
        ScrollHint.SetActive(true);

    }

    private void FishTalk(string content, bool forwardOrNot)
    {
        StartCoroutine(AnimateText(fishText, content,forwardOrNot));
        Debug.Log("fishTalking " + content);
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
            StartCoroutine(BeforeWash());

        }

        else if(bagClick == 2)
        {
            ThrowClothToMachine();
        }
        else
        {
            if (inLoop)
            {
                bagClick = 2;
                StartCoroutine(ChangeFace());
            }

            else if(returning)
            {
                ShowReturnNotice();
            }
            else
            {
                bagClick = 2;
            }

        }

        


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
        Vector3 newPos = new Vector3(MachinePos.x, clothBag.transform.localPosition.y, 0);
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
        machineFront.sprite = fullImg;
        HintUI.transform.localPosition = MachinePos;
    }


    public void ClickMachine()
    {
        if (deactiveButtons) return;
        if (stepCounter < 4) return;//确保在没进行到洗衣机的时候洗衣机不能点

        switch (myMachineState)
        {
            case MachineState.Empty:
                Debug.Log("start washing");
                machineFull.SetActive(true);
                machineEmpty.SetActive(false);
                HintUI.SetActive(false);
                timer = 0;
                myMachineState = MachineState.Washing;

                StartCoroutine(StopTimerAndForward());
                //进入找到手机的部分
                //forwardOneStep = true;

                break;
            case MachineState.Finish:
                
                if (!machineOpen) OpenMachine();
                else CloseMachine();
                break;
        }
    }


    IEnumerator StopTimerAndForward()
    {
        yield return new WaitForSeconds(.5f);

        //提示手机动画
        //考虑一下出现手机动画的时机，目前是洗衣机转一会儿之后才出现
        phoneAnimation.SetActive(true);

        yield return new WaitForSeconds(2.5f);

        timerStop = true;
        forwardOneStep = true;

    }

    public void OpenMachine()
    {
        machineOpen = !machineOpen;

        if (isFirstOpen)
        {
            Debug.Log("isFirstOpen and open machine");
            deactiveButtons = true;

            OpenDoor();

            isFirstOpen = false;
            forwardOneStep = true;
        }
        else if(inLoop)
        {
            Debug.Log("in loop and open machine");
            OpenDoor();

            Hint2D.SetActive(true);
            KararaTalk(EmojiCloth);
            ChangeHintPos(ClothUIPos, 0);
            //forwardOneStep = true;

        }

        else {

            MachineDoor.sprite = openDoor;

        }

        
    }

    public void CloseMachine()
    {
        if (inLoop)
        {
            if (Hint2D.active) Hint2D.SetActive(false);
            machineOpen = !machineOpen;
            CloseDoor();
        }
        else
        {
            MachineDoor.sprite = closeDoor;
        }
    }



    void PhoneOccur()
    {
        //TutorialCameraController.GotoPage(1);
        Debug.Log("PhoneOccur");
    }

    public void PickChargingPhone()
    {
        Debug.Log("PickChargingPhone");
        Phone.SetActive(false);
        //TutorialCameraController.GotoPage(2);
        forwardOneStep = true;
    }


    void BackToWashing()
    {
        KararaC.SetActive(true);
        KararaB.SetActive(false);

        KararaA.SetActive(false);

        timer = 4;
        timerStop = false;
    }


    void AfterWash()
    {
        
        myMachineState = MachineState.Finish;
        machineFull.SetActive(false);
        machineEmpty.SetActive(true);
        machineOccupied.SetActive(true);

        EmojiBubble.SetActive(true);
        KararaTalk(EmojiOpenDoor);

        HintUI.SetActive(true);
        HintUI.transform.localPosition = MachinePos;


    }

    
    public void ClickClothUI()
    {
        if (deactiveButtons) return;

        CloseMachine();

        machineOccupied.SetActive(false);
        machineFront.sprite = emptyImg;

        Hint2D.SetActive(false);
        HintUI.SetActive(false);

        inLoop = false;
        forwardOneStep = true;
    }




    IEnumerator JumpToFish()
    {
        deactiveButtons = true;
        inLoop = true;

        yield return new WaitForSeconds(0.5f);

        Show(scream);
        CloseMachine();
        HintUI.SetActive(false);
        yield return new WaitForSeconds(1f);
        TutorialCameraController.JumpToPage(1);
        Hide(scream);
        FishTalk("Close the door", false);

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

    }


    private void ShowInventory()
    {
        TutorialCameraController.GoToCloset();
        HintScreen.SetActive(true);
        Show(Inventory);
    }

    
    public void ClickClothInventory()
    {
        if (deactiveButtons) return;
        Debug.Log("ClickClothInventory");
        //change Cloth
        for (int i = 0; i<2; i++)
        {
            inventoryCloth[i].sprite = transparet;
            subwayCloth[i].sprite = transparet;
            adsCloth[i].sprite = transparet;
        }

        inventoryCloth[3].sprite = workInventory;
        subwayCloth[3].sprite = workSubway;
        adsCloth[3].sprite = workAds0;

        forwardOneStep = true;

    }




    private void ShowBackButton()
    {
        InventoryBackButton.SetActive(true);
        HintScreen.transform.localPosition = HintPosBackButton;
    }



    public void ClickBackButton()
    {
        if (deactiveButtons) return;
        TutorialCameraController.ReturnToApp();
        Hide(Inventory);

        HintScreen.SetActive(false);

        forwardOneStep = true;
    }



    private void ReadyForPhoto()
    {
        KararaC.SetActive(false);
        // KararaA.SetActive(true);

        Hint2D.SetActive(true);
        ChangeHintPos(HintPosPoster, 0);

        TutorialCameraController.allowScroll = true;
        TutorialCameraController.targetPage = 4;

        ScrollHint.SetActive(true);
        ScrollHint.transform.localRotation = new Quaternion(0,0,180,0);
        foreach (Transform child in ScrollHint.transform)
        {
            child.localRotation = new Quaternion(0, 0, 180, 0);
        }
    }




    public void ClickShutter()
    {
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

        TutorialCameraController.JumpToPage(2);//从ins回到subway scene直接跳转到包在的页面
        yield return new WaitForSeconds(0.6f);

        //show hint arrow
        ReturnPrep();

        // Particle.SetActive(false);
        
        Show(scream);
        //ScrollHint.transform.localRotation = new Quaternion(0, 0, 0, 0);
        //ScrollHint.SetActive(true);

        FishTalk("Return this bag!", true);
    }


    private void ReturnPrep()
    {
        HintUI.SetActive(true);
        HintUI.transform.localPosition = HintPosBag;
        returning = true;
        
    }

    private void ShowReturnNotice()
    {
        Notice.SetActive(true);
        HintUI.SetActive(false);

    }


    public void ReturnYes()
    {
        Notice.SetActive(false);
        HintUI.SetActive(false);

        clothBag.SetActive(false);

        TutorialCameraController.allowScroll = false;
        forwardOneStep = true;


    }

    public void ReturnNo()
    {
        Notice.SetActive(false);
        HintUI.SetActive(true);

        bagClick = 2;
    }


    IEnumerator AddFans()
    {
        Debug.Log("添加follower");

        TutorialCameraController.GotoPage(1);

        //把kararaA换到screen1
        // Vector3 pos = KararaA.transform.localPosition;
        // pos.x = KararaPosX;
        // KararaA.transform.localPosition = pos;

        //screen1用kararaC
        KararaB.SetActive(true);


        FishTalk("say something say something say something ", false);
        yield return new WaitForSeconds(1f);
        
        Particle.transform.localPosition = ParticlePos1;
        Particle.transform.localScale = new Vector3(2, 2, 1);

        foreach (Transform child in FloatingUI.transform)
        {
            child.gameObject.SetActive(true);
        }
        // KararaA.transform.localRotation = new Quaternion(0, 180, 0,0);
        // KararaA.transform.localPosition = KararaAInScreen1Pos;
        // KararaA.SetActive(true);
        Exclamation.transform.localPosition = Exclamation.transform.localPosition;
        Exclamation.SetActive(true);
        Hint2D.SetActive(true);
        ChangeHintPos(Exclamation.transform.position,0);

        yield return new WaitForSeconds(1f);
        Particle.SetActive(false);
    }


    public void ClickExclamation()
    {
        Hint2D.SetActive(false);
        forwardOneStep = true;
    }


    IEnumerator ShowComic()
    {
        
        Show(Comic);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("StreetStyle", LoadSceneMode.Single);

    }







    private void OpenDoor()
    {
        machineOccupied.SetActive(false);
        MachineDoor.sprite = openDoor;
        ClothUI.SetActive(true);

        HintUI.SetActive(false);

        if (deactiveButtons)
        {
            Hint2D.SetActive(false);
        }
    }



    private void CloseDoor()
    {
        machineOccupied.SetActive(true);
        MachineDoor.sprite = closeDoor;
        KararaTalk(EmojiOpenDoor);
        ClothUI.SetActive(false);


        HintUI.SetActive(true);
        Hint2D.SetActive(false);
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



    IEnumerator AnimateText(TextMeshPro text, string textContent,bool forwardOrNot)
    {
        for (int i = 0; i < (textContent.Length + 1); i++)
        {
            text.text = textContent.Substring(0, i);
            yield return new WaitForSeconds(.03f);
            if (i == textContent.Length)
            {
                forwardOneStep = forwardOrNot;
            }
        }
    }

    void KararaTalk(Sprite sprite)
    {
        emoji.sprite = sprite;
    }


}

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


public class TutorialManager : MonoBehaviour
{
    public GameObject DialogueBubble;

    public CanvasGroup hintArrowInventory;
    public CanvasGroup ChapterOne;
    public CanvasGroup closetSubway;
    public GameObject screenshot;
    public GameObject hintArrow;
    public CanvasGroup finalComic;
    public CanvasGroup anotherApp;
    public GameObject closet;
    public GameObject Inventory;

    public GameObject startButton;
    
    public Image hint;
    public Image screamImage;
    public GameObject KararaDialogueBubble;

    private Animator DialogueBubbleAC;

    private RectTransform kararaDialogueRT;
    public RectTransform kararaTextRT;
    
    public CanvasGroup GestureCG;
    public FinalCameraController FinalCameraController;

    public GameObject ClothUI;

//    private Button[] ClothUIButtons;
//    public Image[] ClothUIImages;


    public bool chooseBag = false;
    public GameObject ScreenZero;
    public GameObject ScreenFour;
    
    private HorizontalScrollSnap myHSS;
    public Image[] DialogueImageList;
    public TextMeshProUGUI fishText;
    public Image arrow;
    public CanvasGroup arrowButton;
    
    public Image[] KararaDialogueImageList;
    public TextMeshProUGUI kararaText;

    
    public GameObject poster;

    public bool wearNothing;
   


    public GameObject KararaStanding;
    public GameObject KararaSitting;
    
    public Image KararaStandingImage;
    private Sprite KararaWorkCloth;
    private CanvasGroup KararaSittingCanvasGroup;


    
    public Image ProfileImage;
    public Sprite KararaProfile;
    public Sprite FishProfile;

    public TextMeshProUGUI nameTag;
    public Sprite DialogueKarara;
    public Sprite DialogueFish;

    public Image DialogueBackground;

    public bool pressScreenshot;
    public CanvasGroup myFlash;
    public bool isFishTalking;
    public bool isPrewash;

    public int tutorialNumber;

    public GameObject clothBag;
    public Vector3 bagPos;
    public GameObject clothBagGroup;

    private RectTransform KararaRectT;
    private Image[] KararaAllImage;

    public GameObject bag;
    public GameObject door;
    public GameObject cloth;
    public Button SubwayMap;

    private Image bagImage;
    private Image doorImage;
    private Image clothImage;


    public GameObject gobackButton;
    public Button KararaButton;

    private Color fishColor;
    private Color kararaColor;
    public Sprite KararaDisco;
    
    //use this to decide whether dialogues should disappear when swipe
    public bool stopDisappear = false;

    private Vector2 lowerPosition;
    private Vector2 higherPosition;
    private RectTransform DialogueRT;

    private Vector2 startPosition;
    private Vector2 leftPosition;
    private Vector2 rightPosition = new Vector2(175, 289);

    public Image NameTagImage;

    private Vector2 posterV = new Vector2(-3, 37);
    private Vector2 bagV = new Vector2(-6, -45);
    private Vector2 machineV = new Vector2(-40, 50);
    private Vector2 doorV = new Vector2(0, -0.7f);
    private Vector2 clothV = new Vector2(926, 182f);
    private Vector2 clothV1 = new Vector2(940, 340f);

    private Vector2 closetV = new Vector2(-81, 37);
    private Vector2 inventoryV = new Vector2(-2134,54);
    private Vector2 subwayV = new Vector2(-2542, -299);
    private Vector2 bagdoorV = new Vector2(0, -3);
    private Vector2 messageV = new Vector2(-16, 108);


    private bool tempn = false;

    private AudioSource audioClick;
    
    //which dialogue should be shown on screen
    public enum DialogueState
    {
        fish,
        karara,
        fishElse,
        none,
        all
    }
    
    public DialogueState tutorialDialogueState;
    private CanvasGroup closetCG;
    private CameraMovement cameraMovement;
     
    // Start is called before the first frame update
    void Start()
    {


        closetCG = closet.GetComponent<CanvasGroup>();
        Resources.UnloadUnusedAssets();
        audioClick = GetComponent<AudioSource>();
        cameraMovement = GameObject.Find("Main Camera").GetComponent<CameraMovement>();

        Hide(GestureCG);
        Hide(hintArrow.GetComponent<CanvasGroup>());
        
        fishColor = new Color(176f/255f, 140f/255f, 84f/255f);
        
        //myHSS = GameObject.Find("Horizontal Scroll Snap").GetComponent<HorizontalScrollSnap>();
        
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();

        KararaStandingImage = KararaStanding.GetComponent<Image>();

        KararaSittingCanvasGroup = KararaSitting.GetComponent<CanvasGroup>();
        kararaDialogueRT = KararaDialogueBubble.GetComponent<RectTransform>();
        KararaWorkCloth = KararaStandingImage.sprite;
        startPosition = kararaDialogueRT.anchoredPosition;
        
        StartCoroutine("TrainMoveIn");

        KararaRectT = KararaStanding.GetComponent<RectTransform>();
        
        DialogueImageList = DialogueBubble.GetComponentsInChildren<Image>();
        fishText = DialogueBubble.GetComponentInChildren<TextMeshProUGUI>();
        
        KararaDialogueImageList = KararaDialogueBubble.GetComponentsInChildren<Image>();
        kararaText = KararaDialogueBubble.GetComponentInChildren<TextMeshProUGUI>();



        DialogueRT = DialogueBubble.GetComponent<RectTransform>();
        lowerPosition = DialogueRT.anchoredPosition - new Vector2(0, 200);
        higherPosition = DialogueRT.anchoredPosition;
        //disable the dialogues
        DoFishDialogue(false);
        

        //disable karara standing in the train
//        KararaStandingImage.enabled = false;
        KararaDisappear(false);

//        ClothUIButtons = ClothUI.GetComponentsInChildren<Button>();
//        ClothUIImages = ClothUI.GetComponentsInChildren<Image>();
        
        doorImage = door.GetComponent<Image>();
        clothImage = cloth.GetComponent<Image>();

        ProfileImage.enabled = false;

        //disable scream
        screamImage.enabled = false;


        KararaAllImage = KararaSitting.gameObject.GetComponentsInChildren<Image>();

        //disable all shader effect
        //bag.GetComponent<Image>().material.EnableKeyword("SHAKEUV_OFF");
//        cloth.GetComponent<Image>().material.DisableKeyword("SHAKEUV_ON");
//        door.GetComponent<Image>().material.DisableKeyword("SHAKEUV_ON");

//        foreach (var button in ClothUIButtons)
//        {
//            button.GetComponent<Image>().material.DisableKeyword("SHAKEUV_ON");
//        }

        foreach (var image in KararaAllImage)
        {
            Material mat = image.material;
//            mat.DisableKeyword("SHAKEUV_ON");
//            mat.DisableKeyword("DOODLE_ON");
        }
        //disable map before it's in the tutorial
        SubwayMap.enabled = false;

        leftPosition = kararaDialogueRT.anchoredPosition;

        //set dialogue state
        tutorialDialogueState = DialogueState.none;

        DialogueBubbleAC = DialogueBubble.GetComponentInChildren<Animator>();

        hintArrowCG = hintArrow.GetComponent<CanvasGroup>();
    }

    private bool temp1;

    private int time;
    public bool pause;

    public bool lastTextFinish;

    private bool showHintArrow;

    public CanvasGroup hintArrowCG;
    //typewriter
    public IEnumerator AnimateText(TextMeshProUGUI text, string textContent, bool showArrow, GameObject hintObject, Vector2 arrowPosition)
    {
        clicktime++;
        FinalCameraController.DisableInput(true);
        lastTextFinish = false;
        Hide(hintArrowCG);

//        if(startClicktime == clicktime && startTutorialNum == tutorialNumber)
//        {
            for (int i = 0; i < (textContent.Length + 1); i++)
            {
                text.text = textContent.Substring(0, i);
                yield return new WaitForSeconds(.001f);

                showHintArrow = false;
                if (i == textContent.Length)
                {
                    FinalCameraController.DisableInput(false);
                    yield return new WaitForSeconds(.005f);

                    if (tutorialNumber == 3)//是在说话之后才disable的，会导致如果在说话中途滑动，可能会出现对话框消失的情况
                    {
                        scrollControl(false);
                        FinalCameraController.DisableInput(false);
                    }
                    else if (tutorialNumber == 15)
                    {
                        FinalCameraController.DisableInput(true);

                    }
                    else if (tutorialNumber == 16)
                    {
//                        FinalCameraController.DisableInput(true);

                    }
                   
                    lastTextFinish = true;
                    showHintArrow = showArrow;
                    if (showArrow)
                    {
                        Show(hintArrowCG);
                        hintArrow.transform.SetParent(hintObject.transform);
                        hintArrow.GetComponent<RectTransform>().anchoredPosition = arrowPosition;
                    }
                    else
                    {
                        Hide(hintArrowCG);
                    }
                }
            }
//        }
//        else
//        {
//            text.text = textContent;
//            lastTextFinish = true;
//            if(showArrow)
//            {
//                Show(hintArrowCG);
//                hintArrow.transform.SetParent(hintObject.transform);
//                hintArrow.GetComponent<RectTransform>().anchoredPosition = arrowPosition;
//            }
//            else{Hide(hintArrowCG);}
//        }
    }


    private bool temp2 = false;
    // Update is called once per frame
    void Update()
    {
//        //set hint arrow visibility
//        if (showHintArrow)
//        {
//            Show(hintArrowCG);
//        }
//        else
//        {
//            Hide(hintArrowCG);
//        }
        //decide which dialogue should be shown on screen using the state machine
//        if(stopDisappear && tutorialNumber != 9 && tutorialNumber != 10 && tutorialNumber != 11 && tutorialNumber != 12)
//        {
            if (tutorialDialogueState == DialogueState.fish)
            {
                DialogueBubbleAC.SetBool("isOut", false);
                DoFishDialogue(true);
                DoKararaDialogue(false);
            }
            else if (tutorialDialogueState == DialogueState.karara)
            {
                DoFishDialogue(false);
                DoKararaDialogue(true);
            }
            else if (tutorialDialogueState == DialogueState.fishElse)
            {
                DialogueBubbleAC.SetBool("isOut", true);
                NameTagImage.enabled = false;

                DoFishDialogue(true);
                DoKararaDialogue(false);
            }
            else if (tutorialDialogueState == DialogueState.none)
            {
                DoFishDialogue(false);
                DoKararaDialogue(false);
            }
            else if (tutorialDialogueState == DialogueState.all)
            {
                DoFishDialogue(true);
                DoKararaDialogue(true);
            }
//        }
        
        
        //don't show fish dialogue if not in fish page
        if (FinalCameraController.myCameraState == CameraState.Subway)
        {
            if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.Two)
            {
                kararaDialogueRT.anchoredPosition = new Vector3(0, -570);
                kararaTextRT.localScale = new Vector3(2f, -2f, 2f);
                kararaDialogueRT.localScale = new Vector3(0.5f, -0.5f, 0.5f);
                
            }
            else if(FinalCameraController.mySubwayState == FinalCameraController.SubwayState.Four ||
                    FinalCameraController.mySubwayState == FinalCameraController.SubwayState.Three)
            {
//                if (tutorialNumber == 0)
//                {
//                    kararaDialogueRT.anchoredPosition = leftPosition - new Vector2(100, 0);
//
//                }
//                else { kararaDialogueRT.anchoredPosition = leftPosition;}
                kararaDialogueRT.anchoredPosition = leftPosition;
                kararaTextRT.localScale = new Vector3(2f, 2f, 2f);

                kararaDialogueRT.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            }
            else if(FinalCameraController.mySubwayState == FinalCameraController.SubwayState.One || cameraMovement.currentPage == 1 )//在第一个画面里永远出现鱼的对话框
            {
                //tutorialDialogueState = DialogueState.fish;//always show fish dialogue in scene1
                screamImage.enabled = false;//no noise image
                Hide(GestureCG);
                //karara dialogue position needs to be adjusted
                kararaDialogueRT.anchoredPosition = rightPosition;
                kararaTextRT.localScale = new Vector3(2f, 2f, 2f);
                kararaDialogueRT.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            }
            else if(FinalCameraController.mySubwayState == FinalCameraController.SubwayState.None)
            {
                if (tutorialNumber == 0)
                {
                    kararaDialogueRT.anchoredPosition = leftPosition;

                }
            }
        }
        else if(FinalCameraController.myCameraState == CameraState.Ad)
        {
            //exception is when taking photo in the beginning of the tutorial
            if(!pause )
            {
                tutorialDialogueState = DialogueState.none;
            }
        }
        else if(FinalCameraController.myCameraState == CameraState.Closet)
        {
            screamImage.enabled = false;
            //在换装界面set好位置
            kararaDialogueRT.anchoredPosition = new Vector2(-80, 600);
            kararaTextRT.localScale = new Vector3(2f, 2f, 2f);
            kararaDialogueRT.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        if (nameTag.text == "Goldfish" || nameTag.text == "?????")
        {
            ////print("goldfish");
            if(FinalCameraController.mySubwayState != FinalCameraController.SubwayState.One)
            {
                //DialogueRT.anchoredPosition = lowerPosition;
            }
            else
            {
                //DialogueRT.anchoredPosition = higherPosition;
            }
        }
        else
        {
            //DialogueRT.anchoredPosition = higherPosition;
        }
        

        //make sure that karara only appears at the end and start when player is taught to swipe
//        if (tutorialNumber == 1 && FinalCameraController.myCameraState == CameraState.Subway)
//        {
//            if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.Four ||
//                FinalCameraController.mySubwayState == FinalCameraController.SubwayState.One)
//            {
//                KararaImage.enabled = true;
//            }
//            else
//            {
//                KararaImage.enabled = false;
//            }
//        }
        
        //disable go back button when it is not needed
        if (tutorialNumber < 12)
        {
            gobackButton.SetActive(false);
        }

        if (tutorialNumber == 1)
        {
            Hide(arrowButton);
        }
        else if (tutorialNumber == 2)
        {
            if (clicktime == 7 && lastTextFinish && !temp2)
            {
                bag = Instantiate(clothBag, bagPos, Quaternion.identity) as GameObject;
                bag.transform.SetParent(clothBagGroup.transform, false);
                //bag.transform.SetParent(clothBagGroup.transform, false);
                //                arrow.enabled = false;
                Hide(arrowButton);//so the bag can be picked
                
                //show the arrow after the bag has been instantiated
                Show(hintArrowCG);
                hintArrow.transform.SetParent(bag.transform);
                hintArrow.GetComponent<RectTransform>().anchoredPosition = bagV;
                lastTextFinish = false;
                temp2 = true;
            }
        }
        else if (tutorialNumber == 3)
        {
            //stop moving
//            myHSS.GoToScreen(2);
            if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.Two)
            {
                scrollControl(false);
                //print("number == 3, stop screen");
            }
            if(clicktime == 7)
            {
                //包已经在洗衣机下面
                tutorialDialogueState = DialogueState.fishElse;
                StartCoroutine(AnimateText(fishText, "Tap the bag! Put the clothes into the machine!",true, bag, bagV));
                //now clicktime = 8;
//                fishText.text = "Click the bag! Put the cloth into the machine!";
            }
            

        }
        else if (tutorialNumber == 4)
        {
            cameraMovement.currentPage = 2;
            
            if(clicktime == 8)
            {
                //衣服已经进了洗衣机，还没开始洗
                tutorialDialogueState = DialogueState.fishElse;
                StartCoroutine(AnimateText(fishText, "Tap the button to start!",true, startButton, machineV));
                tutorialNumber = 5;
                FinalCameraController.mySubwayState = FinalCameraController.SubwayState.Two;
            }
        }
        else if (tutorialNumber == 6)//此时衣服开始洗
        {
            //myHSS.GoToScreen(2);
            cameraMovement.currentPage = 2;
            tutorialDialogueState = DialogueState.none;
            Hide(hintArrowCG);
            
            
            //enable sitting
            Show(KararaSittingCanvasGroup);
        }
        else if(tutorialNumber == 8)
        {


            //洗衣机洗好了之后自然出现了这个
            if(clicktime == 9)
            {
                tutorialDialogueState = DialogueState.karara;
                StartCoroutine(AnimateText(kararaText, "Open Door",true, door, doorV));//clicktime == 10

            }
        }
        //如果打开了门
        else if(tutorialNumber == 9)//如果karara把门第一次打开了
        {
            stopDisappear = false;

            if(clicktime == 10 && lastTextFinish)
            {
                if(!temp)
                {
                    temp = true;

                    StartCoroutine(FishCloseDoor());
                }
                else 
                {
                    if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.One)
                    {
                        tutorialDialogueState = DialogueState.fishElse;
//                        StartCoroutine(AnimateText(fishText, "What are you doing? Close the door!",false, null, Vector2.zero));
                        fishText.text = "What are you doing? Close the door!";
                        screamImage.enabled = false;
                        Hide(arrowButton);
                        tempn = false;
                    }
                    else if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.Two)
                    {
                        tempn = true;
                        scrollControl(true);

                        //出现鱼对话制止karara开门，karara对话框消失
                        tutorialDialogueState = DialogueState.karara;
                        fishText.text = "What are you doing? Close the door!";
//                        StartCoroutine(AnimateText(kararaText, "Get cloth",true, ClothUI, clothV));

                        kararaText.text = "Get cloth";
                        screamImage.enabled = true;
                        Hide(arrowButton);
                        
                        //show hint arrow
                        Show(hintArrowCG);
                        hintArrow.transform.SetParent(ClothUI.transform);
                        hintArrow.GetComponent<RectTransform>().anchoredPosition = clothV;
                    }
                    else if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.None)
                    {
                        tutorialDialogueState = DialogueState.none;
                    }
                }
                    
                    
            }
        }
        else if(tutorialNumber == 10)//如果karara把门关上了
        {
            if(clicktime == 10)
            {
                tutorialDialogueState = DialogueState.karara;
                StartCoroutine(AnimateText(kararaText, "Open door", true, door, doorV));   //clicktime = 11
//                fishText.text = "open door";
            }
            
            if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.One)
            {
                tutorialDialogueState = DialogueState.fishElse;
                fishText.text = "Don't open the door!";
                screamImage.enabled = false;
                Hide(arrowButton);
            }
            else if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.Two)
            {
                //出现鱼对话制止karara开门，karara对话框消失
                tutorialDialogueState = DialogueState.karara;
                kararaText.text = "open door";
                screamImage.enabled = true;
                Hide(arrowButton);
                
                //show hint arrow
                Show(hintArrowCG);
                hintArrow.transform.SetParent(door.transform);
                hintArrow.GetComponent<RectTransform>().anchoredPosition = doorV;

            }
            else if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.None)
            {
                tutorialDialogueState = DialogueState.none;
            }
        }
        else if(tutorialNumber == 11)//如果又把门打开了
        {
            kararaText.text = "Get cloth";
            
            if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.One)
            {
                tutorialDialogueState = DialogueState.fishElse;
                fishText.text = "Don't you dare touch your customer's cloth!";
                screamImage.enabled = false;
                Hide(arrowButton);
            }
            else if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.Two)
            {
                //出现鱼对话制止karara开门，karara对话框消失
                tutorialDialogueState = DialogueState.karara;
                screamImage.enabled = true;
                Hide(arrowButton);
                
                //show hint arrow
                Show(hintArrowCG);
                hintArrow.transform.SetParent(ClothUI.transform);
                hintArrow.GetComponent<RectTransform>().anchoredPosition = clothV;
            }
            else if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.None)
            {
                tutorialDialogueState = DialogueState.none;
            }
        }
//        else if(tutorialNumber == 12)//如果点了衣服
//        { 
//            if (clicktime == 12 || clicktime == 11)
//            {
//                StartCoroutine(AnimateText(kararaText, "One more", true, ClothUI, clothV));   //clicktime = 13
//            }
//        }
        else if(tutorialNumber == 12)//如果点了第二件衣服
        {
            clicktime = 14;

//            if (clicktime == 11|| clicktime == 12 || clicktime == 10)
//            {
//                StartCoroutine(AnimateText(kararaText, "Put on", true, closet, closetV));   //clicktime = 13
////                //箭头给到换衣界面的衣服UI
////                Show(hintArrowCG);
////                tutorialDialogueState = DialogueState.none;
////                hintArrow.transform.SetParent(closet.transform f11);
////                hintArrow.GetComponent<RectTransform>().anchoredPosition = closetV;
//            }
            if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.One)
            {
                tutorialDialogueState = DialogueState.fishElse;
                fishText.text = "Karara! Put that cloth back into the machine Now!";
                screamImage.enabled = false;
                Hide(arrowButton);
            }
            else if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.Two)
            {
                //出现鱼对话制止karara开门，karara对话框消失
                tutorialDialogueState = DialogueState.karara;
                screamImage.enabled = true;
                Hide(arrowButton);
                Show(closetCG);
                //show closet button
//                Show(closet.GetComponent<CanvasGroup>());
            }
            else
            {
                tutorialDialogueState = DialogueState.none;
            }
        }
        else if(tutorialNumber == 13)
        {
            //点进了换衣服的界面
            
//            kararaText.text = "Disco";
            //箭头给到换衣界面的衣服UI
            Show(hintArrowCG);
            //新方式给箭头，有两个箭头，其中一个专门给closet
            Show(hintArrowInventory);
            tutorialDialogueState = DialogueState.none;
            hintArrow.transform.SetParent(Inventory.transform);
            hintArrow.GetComponent<RectTransform>().anchoredPosition = inventoryV;
        }
        else if(tutorialNumber == 14 && clicktime == 14)//穿上disco衣服
        {
//            kararaText.text = "Cool";
            Hide(hintArrowInventory);

            StartCoroutine(AnimateText(kararaText, "Cool", true, Inventory, subwayV));   //clicktime = 15

            tutorialDialogueState = DialogueState.karara;
//            arrow.enabled = true;
            Hide(arrowButton);
        }
        else if(tutorialNumber == 15)//回到地铁scene
        {
//            kararaText.text = "Poster";
            if(clicktime == 15)
            {
                FinalCameraController.DisableInput(true);
                StartCoroutine(AnimateText(kararaText, "Disco", true, poster, posterV)); //clicktime = 16

                //myHSS.GoToScreen(4);
                cameraMovement.currentPage = 4;
                Hide(arrowButton);
                Hide(KararaSittingCanvasGroup);
                KararaStandingImage.enabled = true;
                KararaStandingImage.sprite = KararaDisco;
                //show karara
                KararaStanding.transform.SetParent(ScreenFour.transform);

                KararaRectT.anchoredPosition =
                    new Vector3(-72, KararaRectT.anchoredPosition.y);
//            var RectTransform = KararaStanding.GetComponent<RectTransform>();
//            RectTransform.anchoredPosition = new Vector3(85, RectTransform.anchoredPosition.y);
                KararaStandingImage.enabled = true;
                KararaDisappear(false);
            }

            else if (clicktime == 16)
            {
                if(FinalCameraController.mySubwayState == FinalCameraController.SubwayState.Four)
                {
                            scrollControl(false);
                    FinalCameraController.DisableInput(false);

                }      
            }
            //disable swipping screen
           
            
        }
        else if(tutorialNumber == 16)//从海报出来
        {
            if(clicktime == 16)
            {
                scrollControl(true);
                //show karara
                KararaRectT.anchoredPosition =
                    new Vector3(85, KararaRectT.anchoredPosition.y);
                KararaStanding.transform.SetParent(ScreenZero.transform);
            var RectTransform = KararaStanding.GetComponent<RectTransform>();
            RectTransform.anchoredPosition = new Vector3(85, RectTransform.anchoredPosition.y);
                KararaStandingImage.enabled = true;
                KararaDisappear(false);

                //fish talking
                tutorialDialogueState = DialogueState.fish;
//                fishText.text = "What are you doing with your customer's clothes?";
                StartCoroutine(AnimateText(fishText,  "What are you doing with your customer's clothes?", false, null, Vector2.zero));//clicktime = 17
                nameTag.text = "GoldFish";//clicktime = 17

                Show(arrowButton);
            }
            else if (clicktime == 18 && FinalCameraController.mySubwayState == FinalCameraController.SubwayState.Two)
            {
                StartCoroutine(AnimateText(fishText, "Tap the bag to return it!", true, door,
                    bagdoorV)); //clicktime = 19
                scrollControl(false);

            }
        }
        else if(tutorialNumber == 17)
        {
            
            //还好包了
            KararaStandingImage.sprite = KararaWorkCloth;


            if(clicktime == 19)
            {
                Hide(hintArrowCG);

                //print("clicktime = 19, tutorialNumber = 17");
                //如果转到第一个页面，那么鱼老板讲话
                if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.One)
                {
                    tutorialDialogueState = DialogueState.fish;
                    scrollControl(false);
//                    fishText.text = "Lucky you! All clothes are automatically returned!";
                    StartCoroutine(AnimateText(fishText,  "Lucky you! All clothes are automatically returned!", false, null, Vector2.zero));
                    //clicktime = 20

                    screamImage.enabled = false;
                    Show(arrowButton);
                }
                else
                {
                    screamImage.enabled = true;
                    tutorialDialogueState = DialogueState.none;
                }
            }
          
        }
        
        
        
        //only show Karara when she's in the subway scene
        if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.One ||
            FinalCameraController.mySubwayState == FinalCameraController.SubwayState.Two ||
            FinalCameraController.mySubwayState == FinalCameraController.SubwayState.Three ||
            FinalCameraController.mySubwayState == FinalCameraController.SubwayState.Four ||
            FinalCameraController.mySubwayState == FinalCameraController.SubwayState.None

            )
        {
        }
        else
        {
            Hide(KararaSittingCanvasGroup);
            KararaStandingImage.enabled = false;
        }
        
        
        
        //take a photo scene
        if (FinalCameraController.myCameraState == CameraState.Ad)
        {
//            touchImage.enabled = true;
//
//            touch.transform.position = TakeScreenshot.transform.position;
        }
        
        //if the player has already pressed screenshot button
        //only need to do this once?
        if (pressScreenshot)
        {
            pause = true;
            if(tutorialNumber == 0)
            {
//                StartCoroutine(FishCallout());
                //pressScreenshot = false;
                if (temp)
                {
                    FinalCameraController.ChangeToSubway();
                    DialogueRT.anchoredPosition -= new Vector2(0, 300);
                    temp = false;
                    tutorialDialogueState = DialogueState.fish;
                    
                    //clicktime = 1
                    StartCoroutine(AnimateText(fishText, "Hey you! Over here!",false, null, new Vector2(0,0)));

                }
                //print("pressscreenshot is true, should go a lot of times");

                //this is specifically for the taking picture process, some images should be cancelled
                screamImage.enabled = true;
                NameTagImage.enabled = false;

                //fishText.text = "Hey you! Over here!";

                //scrollControl(true);//enable player to swipe

//                arrow.enabled = false;
//                clicktime = 1;
                backToSubway = true;
                Show(arrowButton);
            }
            else if(tutorialNumber > 12)
            {
                DoFishDialogue(false);
                fishText.text = "";
                nameTag.text = "";
                myFlash.alpha = myFlash.alpha - Time.deltaTime;
     
                if (myFlash.alpha <= 0)
                {
                    myFlash.alpha = 0;
                    pressScreenshot = false;
                    KararaDisappear(false);
                }
            }
        }

        if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.None && tutorialNumber == 1)
        {
            //show karara
            KararaRectT.anchoredPosition = 
                new Vector3(205, KararaRectT.anchoredPosition.y);
            KararaStanding.transform.SetParent(ScreenZero.transform);
            var RectTransform = KararaStanding.GetComponent<RectTransform>();
            RectTransform.anchoredPosition = new Vector3(85, RectTransform.anchoredPosition.y);
            KararaStandingImage.enabled = true;
            KararaDisappear(false);
            Hide(arrowButton);
        }
        else if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.One && tutorialNumber == 1)
        {
            //don't let the dialogue disappear if swipe
            stopDisappear = true;

            //show fish dialogue
            tutorialDialogueState = DialogueState.fish;
            StartCoroutine(AnimateText(fishText, "Karara! You're finally here!",false, null, Vector2.zero));//结束之后clicktime = 3

//            arrow.enabled = true;

            
            //enable click the screen
            Show(arrowButton);
            Hide(GestureCG);
            tutorialNumber = 2;            

            ChangeDialogue("Goldfish");
        }
    }

    public void ChangeDialogue(string Character)
    {
        nameTag.text = Character;
        if (Character == "Karara")
        {
            DialogueBackground.sprite = DialogueKarara;
        }
        else if (Character == "Goldfish" || Character == "?????")
        {
            DialogueBackground.sprite = DialogueFish;

        }
    }
    void Hide(CanvasGroup UIGroup) {
        UIGroup.alpha = 0f; //this makes everything transparent
        UIGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
        UIGroup.interactable = false;
    }
    
    void Show(CanvasGroup UIGroup) {
        UIGroup.alpha = 1f;
        UIGroup.blocksRaycasts = true;
        UIGroup.interactable = true;
    }


    IEnumerator FishCloseDoor()
    {

        FinalCameraController.DisableInput(true);
        scrollControl(true);
        yield return new WaitForSeconds(0.4f);
        //myHSS.GoToScreen(1);
        cameraMovement.currentPage = 1;
        yield return new WaitForSeconds(0.4f);
        FinalCameraController.DisableInput(false);


    }

    IEnumerator StoryStart()
    {
        yield return new WaitForSeconds(0.4f);

        Show(ChapterOne);
        SceneManager.LoadScene("StreetStyle", LoadSceneMode.Single);
        //print("loadScene");
    }
    

    public void DoFishDialogue(bool trueOrFalse)
    {
        //disable the dialogues
        for (int a = 0; a < DialogueImageList.Length; a++)
        {
            if(trueOrFalse)
            {
                //if true, enable fish dialogue
                DialogueImageList[a].enabled = true;
                fishText.enabled = true;
                nameTag.enabled = true;
            }
            else
            {
                //if false, disable fish dialogue
                DialogueImageList[a].enabled = false;
                fishText.enabled = false;
                nameTag.enabled = false;
            }
           
        }
    }
    
    public void DoKararaDialogue(bool trueOrFalse)
    {
        //disable the dialogues
        for (int a = 0; a < KararaDialogueImageList.Length; a++)
        {
            if(trueOrFalse)
            {
                //if true, enable karara dialogue
                KararaDialogueImageList[a].enabled = true;
                kararaText.enabled = true;
            }
            else
            {
                //if false, disable karara dialogue
                KararaDialogueImageList[a].enabled = false;
                kararaText.enabled = false;
            }
        }
    }
    IEnumerator TrainMoveIn()
    {
        FinalCameraController.DisableInput(true);

        yield return new WaitForSeconds(0.5f);
        //todo: transitionspeed???
        //myHSS.transitionSpeed = 2f;
        for(int i = 0; i < 5; i ++)
        {
            yield return new WaitForSeconds(0);
            //myHSS.GoToScreen(i);
            cameraMovement.currentPage = i;
            if (i == 4)
            {
                //karara appears
                yield return new WaitForSeconds(1);

                KararaDisappear(false);                
                yield return new WaitForSeconds(2);

             
                //todo: transitionspeed
                //myHSS.transitionSpeed = 7.5f;
                //show karara dialogue
                tutorialDialogueState = DialogueState.karara;
                StartCoroutine(AnimateText(kararaText, "poster", true, poster, posterV));
                Hide(arrowButton);//player should click poster
                scrollControl(false);
            }
        }
    }

    public void scrollControl(bool trueOrFalse)
    {
        //if true, then swipe is enabled
        stopDisappear = !trueOrFalse;
        //myHSS.enabled = trueOrFalse;
        //FinalCameraController.subwayScrollRect.enabled = trueOrFalse;
    }


  
    //true means disappear, like literally
    private void KararaDisappear(bool trueOrFalse)
    {
        var tempColor = KararaStandingImage.color;
        if (trueOrFalse)
        {
            tempColor.a = 0f;
        }
        else if (!trueOrFalse)
        {
            tempColor.a = 1f;
        }
        KararaStandingImage.color = tempColor;
    }

  

   

 

    bool temp = true;

    private bool backToSubway;
    private bool startFadeOut;

    IEnumerator WaitAndChange()
    {
        yield return new WaitForSeconds(0.8f);
        clicktime = 8;

    }
    
    IEnumerator FishCallout()
    {
        yield return new WaitForSeconds(1);
        tutorialDialogueState = DialogueState.fishElse;

        //this is specifically for the taking picture process
        screamImage.enabled = true;
        fishText.text = "Hey you! Over here!";
        //scrollControl(true);

        //swipe is enabled
        //myHSS.enabled = true;
        //FinalCameraController.subwayScrollRect.enabled = true;

//        arrow.enabled = false;

        if (temp)
        {
            DialogueRT.anchoredPosition -= new Vector2(0, 300);
            temp = false;
        }

        yield return new WaitForSeconds(1);

        backToSubway = true;
        Show(arrowButton);
    }


    IEnumerator MessageAppear()
    {
        tutorialDialogueState = DialogueState.fish;
        StartCoroutine(AnimateText(fishText, "Stop talking! listen, never dare you...", true, KararaStanding, messageV));
//        fishText.text = "Stop talking! listen, never dare you...";
        yield return new WaitForSeconds(1.5f);
        Show(anotherApp);
        
    }

    


//        //cancel flash after a few seconds
//        myFlash.alpha = myFlash.alpha - Time.deltaTime;
//     
//        if (myFlash.alpha <= 0)
//        {
////            myFlash.alpha = 0;
////            pressScreenshot = false;
////            Show(GestureCG);
//            backToSubway = true;
//            Show(arrowButton);
//
//
//        }
        
        //change back to subway after the flashlight
        //KararaImage.enabled = true;
//        FinalCameraController.ChangeToSubway();
//        tutorialNumber = 1;
        
        //change dialogue background and name
//        ChangeDialogue("?????");

        //KararaDisappear(true);

   
    public int clicktime = 0;
    private bool clickBool = false;

    private bool tempClick = true;

    public void startChapter1()
    {
        StartCoroutine(StoryStart());

    }
    
    public void DialogueArrowButton()
    {
        if (!FinalCameraController.isSwipping)
        {
            audioClick.Play();
            Hide(hintArrowCG);
            //print("clickedddddddddddddd");
            if (tutorialNumber == 0 && backToSubway)
            {
                if (clicktime == 2)
                {
                    backToSubway = false;

                    myFlash.alpha = 0;
                    pressScreenshot = false;

                    scrollControl(true);

                    Show(GestureCG);
                    //screamImage.enabled = true;

                    //print("temptemp");
                    DialogueRT.anchoredPosition += new Vector2(0, 250);
                    NameTagImage.enabled = true;

                    //disable fish dialogue
                    tutorialDialogueState = DialogueState.none;
                    //print("pressscreenshot is true, should go only one time");


                    //change back to subway after the flashlight
                    FinalCameraController.ChangeToSubway();
                    Hide(arrowButton);

                    tutorialNumber = 1;
                    pause = false;
                }
            }
            else if (tutorialNumber == 2)
            {
                stopDisappear = true;
                if (clicktime == 3)
                {
                    scrollControl(true);

                    if(FinalCameraController.mySubwayState == FinalCameraController.SubwayState.One)
                    {
                        scrollControl(false);
                    }                    tutorialDialogueState = DialogueState.karara;
                    StartCoroutine(AnimateText(kararaText, "Karara?", false, null, Vector2.zero));

//              kararaText.text = "Karara?";

                    Show(arrowButton);
                }

                else if (clicktime == 4 && lastTextFinish)
                {
                    tutorialDialogueState = DialogueState.fish;
                    StartCoroutine(AnimateText(fishText, "You are already late! Stop talking and do the laundry!",
                        false, null, Vector2.zero));
//              fishText.text = "You are already late! Stop talking and do the laundry!";
                }
                else if (clicktime == 5 && lastTextFinish)
                {
                    tutorialDialogueState = DialogueState.karara;
                    StartCoroutine(AnimateText(kararaText, "laundry?", false, null, Vector2.zero));

//              kararaText.text = "laundry?";
                }
                else if (clicktime == 6 && lastTextFinish)
                {
                    tutorialDialogueState = DialogueState.fish;

//              fishText.text = "How come you have such a bad memory! Do I need to teach you every time! Now pick up the bag!";

                    //instantiate the bag


                    StartCoroutine(AnimateText(fishText, "Now Pick up the bag!", false, null, Vector2.zero));
                    //结束之后clicktime = 7
                }
            }
//            else if (tutorialNumber == 8)//衣服洗好了
//            {
//                if (clicktime == 10 && lastTextFinish)
//                {
//                    StartCoroutine(AnimateText(kararaText, "Open door", true, door, doorV)); //clicktime = 11
//                    Hide(arrowButton);
//                }
//
//            }
//           
            else if (tutorialNumber == 16) //照完相出来
            {
                if (clicktime == 17)
                {
                    StartCoroutine(AnimateText(fishText, "Now return the clothes immediately!", false, null,
                        Vector2.zero)); //clicktime = 18

//                fishText.text = "Now return the clothes! Immediately!";
//                    arrow.enabled = false;
                    Show(arrowButton);
                    KararaStandingImage.enabled = true;
                    tutorialDialogueState = DialogueState.fish;
                }
                else if (clicktime == 18 && lastTextFinish)
                {
                    scrollControl(true);
                    //myHSS.GoToScreen(2);
                    cameraMovement.currentPage = 2;
                    if (FinalCameraController.mySubwayState == FinalCameraController.SubwayState.Two)
                    {
                        scrollControl(false);

                    }
                  
                    //FinalCameraController.mySubwayState = FinalCameraController.SubwayState.Two;
                    Hide(arrowButton);
                    //clicktime++;
                }

//            else if (clicktime == 10)
//            {
//                fishText.text = "What are you murmuring? Quiet! Always remember time! Check the map if you need to!";
//                nameTag.text = "Goldfish";
//                
//                //enable map button
//                SubwayMap.enabled = true;
//                Hide(arrowButton);
//
//                clicktime++;
//            }
//            else if (clicktime == 11)
//            {
//                //镜头转到map
//                
//                FinalCameraController.mySubwayState = FinalCameraController.SubwayState.Three;

//                //stopDisappear = true;
//                
//                clicktime++;
//            }
            }
            else if (tutorialNumber == 17)
            {
                if (clicktime == 20)
                {
                    tutorialDialogueState = DialogueState.karara;
                    StartCoroutine(AnimateText(kararaText, "Pity", false, null, Vector2.zero)); //clicktime = 20
                }
                else if (clicktime == 21)
                {
                    StartCoroutine(MessageAppear());
                    Hide(arrowButton);
                }
            }
            else if (tutorialNumber == 18)//在展示漫画
            {
                //print("startSceneeeee");
                StartCoroutine(StoryStart());
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using TMPro;
using System;
public class WasherController : MonoBehaviour
{

    public int number;

    public GameObject empty;
    public GameObject full;

    public GameObject front;

    public GameObject Occupied;

    private SpriteRenderer emptyImage;
    private SpriteRenderer fullImage;
    public SpriteRenderer DoorImage;

    public AllMachines AllMachines;

    
    public bool alreadyNotice;
    
    private Animator myAnimator;

    public Animator lightAnimator;
    // public CanvasGroup ClothUI;

    public TextMeshPro timerNum;
    private float realTimer;
    
    public bool isFirstOpen = true;

   

    public AllMachines.MachineState myMachineState;
    
    private FinalCameraController FinalCameraController;
    private SpriteLoader SpriteLoader;

    public int clothNum;

    public int shut = 0;

    private float timer = 0;

    public GameObject[] buttons;


    public GameObject ClothUI;

    private bool fulltemp = false;
    private AudioSource washingSound;
    private TouchController TouchController;

    public string currentCustomer;
    private AudioManager AudioManager;
    
    // Start is called before the first frame update
    void Start()
    {
        clothNum = buttons.Length;
        myMachineState = AllMachines.MachineState.empty;

        myAnimator = GetComponentInChildren<Animator>();
        
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        SpriteLoader = GameObject.Find("---SpriteLoader").GetComponent<SpriteLoader>();
        AudioManager = GameObject.Find("---AudioManager").GetComponent<AudioManager>();


        washingSound = AllMachines.gameObject.GetComponent<AudioSource>();
        
        TouchController =  GameObject.Find("---TouchController").GetComponent<TouchController>();


        emptyImage  = empty.GetComponent<SpriteRenderer>();
        fullImage  = full.GetComponent<SpriteRenderer>();
        DoorImage  = front.GetComponent<SpriteRenderer>();
        
    }


    public void ClickStart()
    {
        // myMachineState = AllMachines.MachineState.full;
        if(myMachineState == AllMachines.MachineState.full)
        {
            myMachineState = AllMachines.MachineState.washing;
            //for tutorial
            if (FinalCameraController.isTutorial)
            {
                FinalCameraController.TutorialManager.tutorialNumber = 6;
            }
        }
    }
    
    
    // Update is called once per frame
    void Update()
    {

        
        realTimer = AllMachines.washTime - timer;
        if (Mathf.RoundToInt(timer / 60) < 10)
        {
            if (Mathf.RoundToInt(realTimer % 60) < 10)
            {
                timerNum.text = "0" + Mathf.RoundToInt(realTimer / 60).ToString() + ":" + "0" +
                                Mathf.RoundToInt(realTimer % 60).ToString();
            }
            else
            {
                timerNum.text = "0" + Mathf.RoundToInt(realTimer / 60).ToString() + ":" +
                                Mathf.RoundToInt(realTimer % 60).ToString();
            }
        }
        else
        {
            if (Mathf.RoundToInt(realTimer % 60) < 10)
            {
                timerNum.text = Mathf.RoundToInt(realTimer / 60).ToString() + ":" + "0" +
                                Mathf.RoundToInt(realTimer % 60).ToString();
            }
            else
            {
                timerNum.text = Mathf.RoundToInt(realTimer / 60).ToString() + ":" +
                                Mathf.RoundToInt(realTimer % 60).ToString();
            }
        }
       
                
        //close all ui if swipping the screen
        if (FinalCameraController.isSwipping && !FinalCameraController.isTutorial)
        {
            //@@@
            //Destroy(FinalCameraController.generatedNotice);
            FinalCameraController.alreadyNotice = false;

           
            
        }
        
        if (myMachineState == AllMachines.MachineState.empty)
        {
           
            fullImage.enabled = false;
            emptyImage.enabled = true;
            DoorImage.sprite = AllMachines.closedDoor;
            //@@@
            // Hide(Occupied);
            Occupied.SetActive(false);

        }
        else if (myMachineState == AllMachines.MachineState.full)
        {

            if(clothNum > 0)
            {
                emptyImage.enabled = false;
                fullImage.enabled = true;
            }
            
        }
        else if(myMachineState == AllMachines.MachineState.finished)
        {
           //todo: 滑动关闭ClothUI & cloth buttons
            

            if(clothNum > 0)
            {
                emptyImage.enabled = false;
                fullImage.enabled = true;
            }
            else if (clothNum == 0)
            {
                emptyImage.enabled = true;
                fullImage.enabled = false;
            }
        }
        
        //if click a bag of cloth, put them into the machine and start washing
        else if (myMachineState == AllMachines.MachineState.washing)
        {
            myAnimator.SetBool("isWashing", true);
            lightAnimator.SetBool("isWashing", true);
            lightAnimator.SetBool("Reset",false);
            
            emptyImage.enabled = false;
            
            timer += Time.deltaTime;
            
            
            if (timer > AllMachines.washTime)
            {
                //Debug.Log("this machine finished!");
                myMachineState = AllMachines.MachineState.finished;
                //本来在这里 generatecloth
                

                myAnimator.SetBool("isWashing", false);
                lightAnimator.SetBool("isWashing", false);
                washingSound.Stop();
                //@@@
                // Show(Occupied);
                Occupied.SetActive(true);
                     

                timer = 0;
                

            }
        }
              


        //@@@
        // handle click
        if(TouchController.myInputState == TouchController.InputState.Tap){
            if(TouchController.RaycastHitResult[0] == "washer" && TouchController.RaycastHitResult[1] == number.ToString()){
                clickMachineNewMethod();     
            }
        }
            
        

    }

    public void MachineUnfold()
    {
        
        pressOK = false;
        FinalCameraController.machineOpen = true;
        //disable any input
        FinalCameraController.DisableInput(true);
        if (FinalCameraController.isTutorial && FinalCameraController.TutorialManager.tutorialNumber > 12)
        {
            return;
        }

        //
        if (isFirstOpen) {
            //只有karara见到过的衣服才能进collection
            isFirstOpen = false;
            AddClothToCollection();
            
        }

        //@@@
        StartCoroutine(ShowClothUI());
               
        pressOK = true;
        FinalCameraController.machineOpen = false;
        FinalCameraController.DisableInput(false);

        if(FinalCameraController.isTutorial && FinalCameraController.TutorialManager.tutorialNumber == 8 && FinalCameraController.TutorialManager.clicktime == 10)
        {

            FinalCameraController.TutorialManager.tutorialNumber = 9;
        }
        else if (FinalCameraController.isTutorial && FinalCameraController.TutorialManager.tutorialNumber == 10)
        {
            FinalCameraController.TutorialManager.tutorialNumber = 11;//如果karara还是把门打开了
        }

    }
    
    public void MachineFold()
    {
        pressOK = false;
        FinalCameraController.machineOpen = true;
        FinalCameraController.DisableInput(true);

        StartCoroutine(HideClothUI());
      
        Occupied.SetActive(true);
        pressOK = true;
        FinalCameraController.machineOpen = false;
        FinalCameraController.DisableInput(false);
        FinalCameraController.clickKarara();

        if (FinalCameraController.isTutorial && FinalCameraController.TutorialManager.tutorialNumber == 9 || FinalCameraController.isTutorial && FinalCameraController.TutorialManager.tutorialNumber == 11)
        {
            FinalCameraController.TutorialManager.tutorialNumber = 10;
        }

        if(clothNum == 0){
            Occupied.SetActive(false);
        }
        // yield return new WaitForSeconds(0.1f);
    }
    
    public void CancelPanel()
    {
        //@@@
        // Hide(ClothUI);
        // Hide(backgroundUI);
        //@@@    
        shut = 0;
    }

    public void clickMachineNewMethod()
    {
//        if (clothNum == 0 && shut == 0) //if there's no cloth in the door, don't let it open
//        {
            if (myMachineState == AllMachines.MachineState.bagUnder)
            {

            }
            else if (myMachineState == AllMachines.MachineState.full)
            {
                
                washingSound.Play();
                ClickStart();
            }
            else if (myMachineState == AllMachines.MachineState.finished)
            {
                if (FinalCameraController.isTutorial)
                {
                    if(FinalCameraController.TutorialManager.tutorialNumber > 11)
                    {
                        return;
                    }                
                }
                if(pressOK)
                {
                    //Debug.Log("machine " + number.ToString() + " clickMachine()");
                    clickMachine();
                }            
            }

//        }
//        else
//        {
//            //todo: play a sound indicating there's no cloth in the machine
//        }
//        }
    }

    IEnumerator CannotClickFor1s()
    {
        yield return new WaitForSeconds(0.8f);

    }

    
    public bool pressOK = true;
    
    public void clickMachine()
    {

            FinalCameraController.CancelAllUI(true);
            //print("presssssssed");

                if (shut == 0)
                {
                    //Debug.Log("machine " + number.ToString() + " open the door");
                    shut = 1;
                    MachineUnfold();

                    //change door sprite to open
                    DoorImage.sprite = AllMachines.openedDoor;
                    //@@@
                    // Hide(Occupied);
                    Occupied.SetActive(false);

                    //ClothUiAnimator.SetBool("isUnfold",true);

//                    StartCoroutine("WaitFor2Seconds");

                }
                //if click machine again, close UI
                else if (shut == 1)
                {
                    //Debug.Log("machine " + number.ToString() + " close the door");
                    shut = 0;
                    // Hide(ClothUI);
                    MachineFold();
                    //ClothUiAnimator.SetBool("isUnfold",false);

                    //change door to closed sprite
                    DoorImage.sprite = AllMachines.closedDoor;
                    Occupied.SetActive(true);
                    //@@@
                    // Show(Occupied);
                    //如果是通过点洗衣机关门的话，展示karara头上的两个对话框

                    //show message and closet UI
//                    if(!FinalCameraController.isTutorial)
//                    {
//                        Show(FinalCameraController.clothCG);
//                        Show(FinalCameraController.messageCG);
//                        FinalCameraController.isShown = true;
//                    }
                }
    }

    void Hide(CanvasGroup UIGroup) {
        UIGroup.alpha = 0f; //this makes everything transparent
        UIGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
        UIGroup.interactable = false;


        if (UIGroup == ClothUI)
        {
            FinalCameraController.alreadyClothUI = false;
        }
//            foreach (Button btn in btns)
//            {
//                btn.enabled = false;
//            }            
    }
    
    void Show(CanvasGroup UIGroup) {
        UIGroup.alpha = 1f;
        UIGroup.blocksRaycasts = true;
        UIGroup.interactable = true;
        
        if (UIGroup == ClothUI)
        {
            FinalCameraController.alreadyClothUI = true;
            FinalCameraController.currentClothUI = ClothUI;
        }
    }

    IEnumerator ShowClothUI(){
        ClothUI.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        ClothUI.GetComponent<Animator>().SetTrigger("AfterPop");
        FinalCameraController.alreadyClothUI = true;
        FinalCameraController.currentClothUI = ClothUI;
    }
    IEnumerator HideClothUI(){
        ClothUI.GetComponent<Animator>().SetTrigger("StartClose");
        yield return new WaitForSeconds(0.5f);
        ClothUI.SetActive(false);
        FinalCameraController.alreadyClothUI = false;
    }

    

    private void GenerateClothAccordingToTag(string tagName, List<Sprite> MachineClothList)
    {
        int upperBound = SpriteLoader.NPCDic[tagName].myClothes.Count;
        for (int i = 0; i < buttons.Length; i++)
        {


            HashSet<int> usedIdx = SpriteLoader.NPCDic[tagName].usedIdx;
            int randomIdx;

            while (true) {
               
                randomIdx = UnityEngine.Random.Range(0, upperBound);
                if (!usedIdx.Contains(randomIdx))
                {
                    usedIdx.Add(randomIdx);
                    break;
                }
            }

            string clothName = SpriteLoader.NPCDic[tagName].myClothes[randomIdx];


            buttons[i].GetComponent<SpriteRenderer>().enabled = true;
            buttons[i].GetComponent<BoxCollider2D>().enabled = true;
            buttons[i].tag = tagName;
            buttons[i].GetComponent<SpriteRenderer>().sprite = SpriteLoader.ClothDic[clothName].spritesInScenes[2];
            Sprite buttonImage = buttons[i].GetComponent<SpriteRenderer>().sprite;
            MachineClothList.Add(buttonImage);


            //int randomIndex = UnityEngine.Random.Range(0, AllMachines.nameToTemp[tagName].Count);

            //                  Button ClothInMachine =
            //                  Instantiate(alexClothesTemp[randomIndex], buttonPositions[i], Quaternion.identity) as Button;
            // buttons[i].GetComponent<Button>().enabled = true;
            // @@@
            //buttons[i].GetComponent<SpriteRenderer>().enabled = true;
            //buttons[i].GetComponent<BoxCollider2D>().enabled = true;
            //buttons[i].tag = tagName;
            //buttons[i].GetComponent<SpriteRenderer>().sprite = AllMachines.nameToTemp[tagName][randomIndex];
            //Sprite buttonImage = buttons[i].GetComponent<SpriteRenderer>().sprite;


            //AllMachines.nameToTemp[tagName].Remove(AllMachines.nameToTemp[tagName][randomIndex]);
            //as child of the folder
            //doesn't work for some reason I don't understand
            //ClothInMachine.transform.SetParent(MachineGroup.transform, false);
            //ClothInMachine.transform.SetParent(MachineGroup.transform, false);

            //record into List
            //MachineClothList.Add(buttonImage);
        }
    }
    
    public void GenerateCloth(string tagName)
    {
        //need to get all button reset, because they are disabled when clicked last time
        
        List<Sprite> MachineClothList = new List<Sprite>();

        //randomly generate clothes when player opens the machine
        if(isFirstOpen)
        {

            if (FinalCameraController.isTutorial && FinalCameraController.TutorialManager.tutorialNumber < 12)
            {
                //cloth 1
                //buttons[0].GetComponent<Button>().enabled = true;

                buttons[0].GetComponent<SpriteRenderer>().enabled = true;
                buttons[0].GetComponent<BoxCollider2D>().enabled = true;
                buttons[0].GetComponent<SpriteRenderer>().sprite = AllMachines.TutorialCloth1;
                buttons[0].tag = "Tutorial";
                Sprite buttonImage = buttons[0].GetComponent<SpriteRenderer>().sprite;

                MachineClothList.Add(buttonImage);
                
                //cloth 2
                //buttons[1].GetComponent<Button>().enabled = true;
                //buttons[1].tag = "Tutorial";
                //Image buttonImage1 = buttons[1].GetComponent<Image>();
                //buttonImage1.enabled = true;
                //buttonImage1.sprite = AllMachines.TutorialCloth2;
                
                //MachineClothList.Add(buttonImage1.sprite);
            }
            else if (AllMachines.CustomerNameList[number].Contains(tagName))
            {
                //print("isGeneratingCloth");
                GenerateClothAccordingToTag(tagName, MachineClothList);
            }

        }

         isFirstOpen = false;
         if(!FinalCameraController.AllStationClothList.ContainsKey(tagName))
         {
            //Debug.Log(tagName);
             FinalCameraController.AllStationClothList.Add(tagName, MachineClothList);
         }
    }
        
       
    public void ResetMachine(){
        currentCustomer = "";
        transform.tag ="Untagged";
        myMachineState = AllMachines.MachineState.empty;
        isFirstOpen = true;
        clothNum = 4;
        lightAnimator.SetBool("Reset",true);
                
    }


    public void AddClothSprite(List<string> clothNames)
    {
        for(int i = 0; i < buttons.Length; i ++) {
            buttons[i].GetComponent<SpriteRenderer>().enabled = true;
            buttons[i].GetComponent<BoxCollider2D>().enabled = true;
            buttons[i].GetComponent<SpriteRenderer>().sprite = SpriteLoader.ClothDic[clothNames[i]].spritesInScenes[2];
        }
        
    }


    private void AddClothToCollection()
    {
        foreach (GameObject thisButton in buttons)
        {
            FinalCameraController.AllStationClothList[currentCustomer]
                .Add(thisButton.GetComponent<SpriteRenderer>().sprite);
        }
    }
}

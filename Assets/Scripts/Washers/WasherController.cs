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
    SubwayMovement SubwayMovement;

    [SerializeField]
    Image progressBar;

    [SerializeField]
    SpriteRenderer Light;

    [SerializeField]
    Sprite statusEmpty, statusFinished, statusWashing;
    // Start is called before the first frame update


    void Start()
    {
        clothNum = buttons.Length;
        myMachineState = AllMachines.MachineState.empty;

        myAnimator = GetComponentInChildren<Animator>();
        
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        SpriteLoader = GameObject.Find("---SpriteLoader").GetComponent<SpriteLoader>();
        AudioManager = GameObject.Find("---AudioManager").GetComponent<AudioManager>();
        SubwayMovement = GameObject.Find("---StationController").GetComponent<SubwayMovement>();

        washingSound = AllMachines.gameObject.GetComponent<AudioSource>();
        
        TouchController =  GameObject.Find("---TouchController").GetComponent<TouchController>();


        emptyImage  = empty.GetComponent<SpriteRenderer>();
        fullImage  = full.GetComponent<SpriteRenderer>();
        DoorImage  = front.GetComponent<SpriteRenderer>();

        Light.sprite = statusEmpty;
        
    }


    public void ClickStart()
    {
        // myMachineState = AllMachines.MachineState.full;
        if(myMachineState == AllMachines.MachineState.full)
        {
            myMachineState = AllMachines.MachineState.washing;
            Light.sprite = statusWashing;
        }
    }
    
    
    // Update is called once per frame
    void Update()
    {

        
        if(!SubwayMovement.pauseBeforeMove) realTimer = AllMachines.washTime - timer;

        UpdateProgressBar();
        
        //if (Mathf.RoundToInt(timer / 60) < 10)
        //{
        //    if (Mathf.RoundToInt(realTimer % 60) < 10)
        //    {
        //        timerNum.text = "0" + Mathf.RoundToInt(realTimer / 60).ToString() + ":" + "0" +
        //                        Mathf.RoundToInt(realTimer % 60).ToString();
        //    }
        //    else
        //    {
        //        timerNum.text = "0" + Mathf.RoundToInt(realTimer / 60).ToString() + ":" +
        //                        Mathf.RoundToInt(realTimer % 60).ToString();
        //    }
        //}
        //else
        //{
        //    if (Mathf.RoundToInt(realTimer % 60) < 10)
        //    {
        //        timerNum.text = Mathf.RoundToInt(realTimer / 60).ToString() + ":" + "0" +
        //                        Mathf.RoundToInt(realTimer % 60).ToString();
        //    }
        //    else
        //    {
        //        timerNum.text = Mathf.RoundToInt(realTimer / 60).ToString() + ":" +
        //                        Mathf.RoundToInt(realTimer % 60).ToString();
        //    }
        //}
       
                
        //close all ui if swipping the screen
        if (FinalCameraController.isSwipping && !FinalCameraController.isTutorial)
        {
            //@@@
            //Destroy(FinalCameraController.generatedNotice);
            FinalCameraController.alreadyNotice = false;

           
            
        }
        
     
        if(myMachineState == AllMachines.MachineState.finished)
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
        if (myMachineState == AllMachines.MachineState.washing)
        {
            myAnimator.SetBool("isWashing", true);
            //lightAnimator.SetBool("isWashing", true);
            //lightAnimator.SetBool("Reset",false);
            
            emptyImage.enabled = false;
            
            timer += Time.deltaTime;
            
            
            if (timer > AllMachines.washTime)
            {
                //Debug.Log("this machine finished!");

                AudioManager.PlayAudio(AudioType.Machine_Finished);


                myMachineState = AllMachines.MachineState.finished;
                //本来在这里 generatecloth
                //finish
                



                myAnimator.SetBool("isWashing", false);
                //lightAnimator.SetBool("isWashing", false);
                
                Occupied.SetActive(true);


                //timer = 0; //when the bag returned set the timer 0 and change image 
                Light.sprite = statusFinished;
                UpdateProgressBar();

                
                

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

    public void SetMachineAsFull()
    {
       
        StartCoroutine(DropClothes());
    }



    private void UpdateProgressBar()
    {
        progressBar.fillAmount = timer/AllMachines.washTime;
        
    }
    public void MachineUnfold()
    {
        
        pressOK = false;
        FinalCameraController.machineOpen = true;
        //disable any input
        //
        if (isFirstOpen) {
            //只有karara见到过的衣服才能进collection
            isFirstOpen = false;
            AddClothToCollection();
            
        }

        //@@@
        StartCoroutine(ShowClothUI());
        FinalCameraController.machineOpen = false;
        FinalCameraController.DisableInput(false);

        DoorImage.sprite = AllMachines.openedDoor;
        Occupied.SetActive(false);


    }
    
    public void MachineFold()
    {
        pressOK = false;
        FinalCameraController.machineOpen = true;
        FinalCameraController.DisableInput(true);

        StartCoroutine(HideClothUI());
      
        Occupied.SetActive(true);
        
        FinalCameraController.machineOpen = false;
        FinalCameraController.DisableInput(false);

        DoorImage.sprite = AllMachines.closedDoor;
        Occupied.SetActive(true);

        if (clothNum == 0){
            Occupied.SetActive(false);
        }
       
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
        if(FinalCameraController.myCameraState != FinalCameraController.CameraState.Subway)
        {
            return;
        }

        Debug.Log("machine " + number.ToString() + " clickMachine()");
        //        if (clothNum == 0 && shut == 0) //if there's no cloth in the door, don't let it open
        //        {
        if (myMachineState == AllMachines.MachineState.bagUnder)
            {

            }
            else if (myMachineState == AllMachines.MachineState.full)
            {

            //washingSound.Play();
            AudioManager.PlayAudio(AudioType.Machine_OnOff);
            AudioManager.PlayAudio(AudioType.Machine_Washing);
            ClickStart();
            }
            else if (myMachineState == AllMachines.MachineState.finished)
            {
                
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
            shut = 1;
            MachineUnfold();

        }
        else if (shut == 1)
        {
            shut = 0;
            MachineFold();

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
        Debug.Log("show cloth ui");
        ClothUI.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        ClothUI.GetComponent<Animator>().SetTrigger("AfterPop");
        FinalCameraController.alreadyClothUI = true;
        FinalCameraController.currentClothUI = ClothUI;
        pressOK = true;
    }
    IEnumerator HideClothUI(){
        ClothUI.GetComponent<Animator>().SetTrigger("StartClose");
        yield return new WaitForSeconds(0.5f);
        ClothUI.SetActive(false);
        FinalCameraController.alreadyClothUI = false;
        pressOK = true;
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

        Debug.Log("from " + transform.tag + " to untagge");
        currentCustomer = "";
        transform.tag ="Untagged";
        myMachineState = AllMachines.MachineState.empty;
        isFirstOpen = true;
        clothNum = 4;
        //lightAnimator.SetBool("Reset",true);
        
        timer = 0;

        StartCoroutine(CollectClothese());
        
                
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


    public IEnumerator DropClothes()
    {


        emptyImage.enabled = false;
        fullImage.enabled = true;
        DoorImage.sprite = AllMachines.openedDoor;


        yield return new WaitForSeconds(0.5f);

        
        DoorImage.sprite = AllMachines.closedDoor;

        myMachineState = AllMachines.MachineState.full;

    }

    public IEnumerator CollectClothese()
    {

        Occupied.SetActive(false);
        emptyImage.enabled = true;
        fullImage.enabled = false;

        DoorImage.sprite = AllMachines.openedDoor;
        yield return new WaitForSeconds(0.5f);

        DoorImage.sprite = AllMachines.closedDoor;
        Light.sprite = statusEmpty;
    }
}

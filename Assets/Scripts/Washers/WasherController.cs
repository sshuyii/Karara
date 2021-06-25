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
    private ValueEditor ValueEditor;
    public int number;

    public GameObject empty;
    public GameObject full;

    public GameObject front;

    public GameObject Occupied;

    [HideInInspector]
    public SpriteRenderer emptyImage;
    [HideInInspector]
    public SpriteRenderer fullImage;
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
    private InventorySlotMgt InventorySlotMgt;

    public int clothNum;

    public int shut = 0;

    private float timer = 0;

    public GameObject[] buttons;


    public GameObject ClothUI;

    private bool fulltemp = false;
    private AudioSource washingSound;
    private TouchController TouchController;


    public GameObject currentBag;
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
        ValueEditor = GameObject.Find("---ValueEditor").GetComponent<ValueEditor>();
        InventorySlotMgt = GameObject.Find("---InventoryController").GetComponent<InventorySlotMgt>();


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
            

            // if(clothNum > 0)
            // {
            //     emptyImage.enabled = false;
            //     fullImage.enabled = true;
            // }
            // else if (clothNum == 0)
            // {
            //     emptyImage.enabled = true;
            //     fullImage.enabled = false;
            // }
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
    public IEnumerator MachineUnfold()
    {
        // 开门
        pressOK = false;
        FinalCameraController.machineOpen = true;
        FinalCameraController.DisableInput(true);

        //disable any input
        //
        if (isFirstOpen) {
            //只有karara见到过的衣服才能进collection
            isFirstOpen = false;
            AddClothToCollection();
            
        }

        //@@@
        FinalCameraController.alreadyClothUI = true;
        FinalCameraController.currentClothUI = ClothUI;
        ClothUI.SetActive(true);

        DoorImage.sprite = AllMachines.openedDoor;
        Occupied.SetActive(false);


        Animator ac = ClothUI.GetComponent<Animator>();
        ac.SetTrigger("StartShowing");
        

        FinalCameraController.machineOpen = false;
        FinalCameraController.DisableInput(false);

        pressOK = true;

        yield return null;

    }
    //关门！
    public IEnumerator MachineFold(int close)
    {
        ///Shuyi:不知为何从外部call这个function的时候，pressOK有时候不会回归true
        //close = 1，是手动点关门,不能出现inventory is full
        //close = 0, 是被动关门，应该出现
        //close = 3，被动关门，不应该出现
        pressOK = false;
        FinalCameraController.machineOpen = true;
        FinalCameraController.DisableInput(true);

        // Animator ac = ClothUI.GetComponent<Animator>();
        // ac.SetTrigger("StartClosing");
 

        // float Twait = (float)ac.GetCurrentAnimatorClipInfo(0).Length;
        // yield return new WaitForSeconds(Twait);
        
        
        FinalCameraController.alreadyClothUI = false;
        FinalCameraController.machineOpen = false;
        FinalCameraController.DisableInput(false);

        //Shuyi: 关门暂定干脆一点，尽量避免点击之后过一段时间才关门+出声音
        yield return new WaitForSeconds(.2f);

        ClothUI.SetActive(false);

        DoorImage.sprite = AllMachines.closedDoor;
        AudioManager.PlayAudio(AudioType.Machine_CloseDoor);

        // yield return new WaitForSeconds(.2f);

        pressOK = true;

        //如果inventory is full
        //只有拿了衣服才出现，光关门不能出现
        if(InventorySlotMgt.InventoryIsFull == true && close == 0)
        {
            InventorySlotMgt.ShowInventoryFullNotice();

            // FinalCameraController.GotoPage(1);
            // FinalCameraController.FishTalkAccessFromScript("InventoryFull",true);
            // FinalCameraController.Show(FinalCameraController.fishShoutCG);
        }
        
        
        if (clothNum == 0 || myMachineState == AllMachines.MachineState.empty) 
        {
            emptyImage.enabled = true;
            fullImage.enabled = false;
            Occupied.SetActive(false);
        }
        
        else if(myMachineState == AllMachines.MachineState.finished || myMachineState == AllMachines.MachineState.noninteractable) {
            Occupied.SetActive(true);
            fullImage.enabled = true;
            emptyImage.enabled = false;
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
            return;
        }
        else if (myMachineState == AllMachines.MachineState.full)
        {

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
        else if ( myMachineState == AllMachines.MachineState.noninteractable)
        {   
            string talk = "Return bags on time!";
            FinalCameraController.fishTalkText.text = talk;
            FinalCameraController.FishBossNotification.ShowFish(talk);
        }


     
        


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
            StartCoroutine(MachineUnfold());

        }
        else if (shut == 1)
        {
            shut = 0;
            StartCoroutine(MachineFold(1));

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



    

    
       
    public void ResetMachine(){

        StartCoroutine(CollectClothese());
        transform.tag ="Untagged";
        currentBag = null;
        isFirstOpen = true;
        clothNum = 4;
        timer = 0;
                
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
        string bagOwnerName = currentBag.GetComponent<ClothToMachine>().owner.name;

        foreach (GameObject thisButton in buttons)
        {
            FinalCameraController.AllStationClothList[bagOwnerName]
                .Add(thisButton.GetComponent<SpriteRenderer>().sprite);
        }
    }


    public IEnumerator DropClothes()
    {


        myMachineState = AllMachines.MachineState.full;
        DoorImage.sprite = AllMachines.openedDoor;
        shut = 1;

        yield return new WaitForSeconds(ValueEditor.TimeRelated.openWasherDelay1);
        
        emptyImage.enabled = false;
        fullImage.enabled = true;
        
        yield return new WaitForSeconds(ValueEditor.TimeRelated.openWasherDelay2);

        DoorImage.sprite = AllMachines.closedDoor;
        shut = 0;
        AudioManager.PlayAudio(AudioType.Machine_CloseDoor);
        

    }

    public IEnumerator CollectClothese()
    {


        emptyImage.enabled = true;
        fullImage.enabled = false;

        yield return new WaitForSeconds(ValueEditor.TimeRelated.openWasherDelay2);

        DoorImage.sprite = AllMachines.closedDoor;
        shut = 0;
        
        Light.sprite = statusEmpty;
        myMachineState = AllMachines.MachineState.empty;
        Occupied.SetActive(false);
    }
}

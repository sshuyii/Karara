using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class ClothChanging : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int originSlotNum;
    public Text text;
    private Vector3 startPos;
    public Sprite transparent;

//    public Image checkImage;
//    public GameObject crossImage;

    public CanvasGroup Occupied;

    //used for lont tap
    private bool tapStart = false;
        
    public bool isWearing;

    private FinalCameraController FinalCameraController;
    private SubwayMovement SubwayMovement;
    

    private GameObject InventoryController;

    private bool hittingBody;
    //this dictionary is the player inventory

    //a list that stores UI location
    private Sprite currentSprite;
  
   

    private TouchController TouchController;

    private Sprite startSprite;
    private Image myImage;

    public Image halfTspImg;


    private AllMachines AllMachines;
    // Start is called before the first frame update

    private bool pointerDown;
	private float pointerDownTimer;
    private bool longPress = false;


    
    public Button returnConfirmButton;
    [SerializeField]
    private Button buttonPrefab;
    [SerializeField]
    private bool isWorkCloth;
    

	
	private float requiredHoldTime = 0.6f;

    public bool isOccupied = false;

    private LostAndFound LostAndFound;

    private float myFillAmount = 1f;

    private SpriteLoader SpriteLoader;
    private AdsController AdsController;
    private InventorySlotMgt InventorySlotMgt;

    private float timer = -1f;
    private float totalTime = -1f;


    void Start()
    {
        
        InventoryController = GameObject.Find("---InventoryController");
        TouchController = GameObject.Find("---TouchController").GetComponent<TouchController>();
        AllMachines = GameObject.Find("---ClothInMachineController").GetComponent<AllMachines>();
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        SubwayMovement = GameObject.Find("---StationController").GetComponent<SubwayMovement>();
        LostAndFound = GameObject.Find("Lost&Found_basket").GetComponent<LostAndFound>();
        SpriteLoader = GameObject.Find("---SpriteLoader").GetComponent<SpriteLoader>();
        AdsController = GameObject.Find("---AdsController").GetComponent<AdsController>();
        InventorySlotMgt = GameObject.Find("---InventoryController").GetComponent<InventorySlotMgt>();

        
        startPos = transform.position;



//        selfButton.onClick.AddListener(AddClothToInventory);

        //currentSprite = GetComponent<SpriteRenderer>().sprite;

        myImage = GetComponent<Image>();



        startSprite = GetComponent<Image>().sprite;


        if(!FinalCameraController.isTutorial)
            returnConfirmButton.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {


        if (pointerDown)
		{
			pointerDownTimer += Time.deltaTime;
			if (pointerDownTimer >= requiredHoldTime)
			{
                longPress = true;

                //Debug.Log("LongPress");
                if (!isWorkCloth) InventorySlotMgt.castLongPress();
                //if(!isWorkCloth) showReturnConfirm();
                Reset();
			}
	
		}



        if(isOccupied)
        {
            myFillAmount -= 1 / (3 * (SubwayMovement.stayTime + SubwayMovement.moveTime) + SubwayMovement.stayTime) *
                Time.deltaTime;

            timer -= Time.deltaTime;
            myImage.fillAmount = timer/totalTime;

        }

    }


    public void setFillAmount(float[] timers) {
        timer = timers[0];
        totalTime = timers[1];
    }
    public void OnPointerDown(PointerEventData eventData)
	{
		pointerDown = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Reset();
	}

    private void Reset()
	{
		pointerDown = false;
		pointerDownTimer = 0;
		
	}

    public void hideReturnConfirm()
    {
        returnConfirmButton.gameObject.SetActive(false);
    }
    public void showReturnConfirm()
    {

        if(isOccupied && !isWorkCloth)returnConfirmButton.gameObject.SetActive(true);

    }

    public void ReturnClothPre()
    {
        tapStart = true;
    }

    public void ReturnCloth()
    {
        returnConfirmButton.gameObject.SetActive(false);
        longPress = false;
        //todo: long press but later chose not to return?


        //print("return cloth is working");

        //print("return cloth is working and double touched");


        Debug.Log(this.tag);
        bool bagFoundInCar = AllMachines.PutClothBackToMachine(this.tag, originSlotNum);

        if (!bagFoundInCar) DropToLostAndFound();
        DropToLostAndFound();


        takeOffCloth();
        AfterTakeOff();

        
    }

    
   
    private void DropToLostAndFound(){
        currentSprite = GetComponent<Image>().sprite;
        Debug.Log("丢掉" +currentSprite.name);
        Cloth thisCloth = SpriteLoader.ClothDic[currentSprite.name];
        NPC owner = SpriteLoader.NPCDic[thisCloth.owner];
        LostAndFound.AddClothToList(thisCloth, owner);
        //takeOffCloth();
        //AfterTakeOff();
       
    }

    private void AfterTakeOff(){
        myImage.sprite = startSprite;
        halfTspImg.sprite = startSprite;
        InventorySlotMgt.occupiedNum -= 1;


        isOccupied = false;
        isWearing = false;
        myImage.fillAmount = 1;
        originSlotNum = -1;
    }



    private void takeOffCloth() 
    {
        if (!isWearing) return;
        currentSprite = GetComponent<Image>().sprite;

        Cloth thisCloth = SpriteLoader.ClothDic[currentSprite.name];


        
        int typeIdx;
        switch (thisCloth.type)
        {
            case Cloth.ClothType.Top:
                ShowDefaultTop();
                typeIdx = 0;
                break;
            case Cloth.ClothType.Bottom:
                ShowDefaultBottom();
                typeIdx = 1;
                break;
            case Cloth.ClothType.Shoe:
                ShowDefaultShoe();
                typeIdx = 2;
                break;
            case Cloth.ClothType.Everything:
                ShowDefaultTop();
                ShowDefaultBottom();
                typeIdx = 3;
                break;
            default:
                return;

        }



        InventorySlotMgt.wearingNonDefualtClothes[typeIdx] = false;

       
    }




    public void ChangeCloth()

    {


        if (longPress) return;
        if (FinalCameraController.isTutorial)
        {
            // do something
            return;
        }



        currentSprite = GetComponent<Image>().sprite;
        Cloth thisCloth = SpriteLoader.ClothDic[currentSprite.name];
       

        int typeIdx;
        SpriteRenderer[] ImagesToChange;
        switch (thisCloth.type)
        {
            case Cloth.ClothType.Top:
                ImagesToChange = InventorySlotMgt.TopImages;
                typeIdx = 0;
                
                break;
            case Cloth.ClothType.Bottom:
                ImagesToChange = InventorySlotMgt.BottomImages;
                typeIdx = 1;
                break;
            case Cloth.ClothType.Shoe:
                ImagesToChange = InventorySlotMgt.ShoeImages;
                typeIdx = 2;
                break;
            case Cloth.ClothType.Everything:
                ImagesToChange = InventorySlotMgt.EverythingImages;
                typeIdx = 3;
                break;
            default:
                return;

        }


        isWearing = !isWearing;

        if (isWearing)
        {
            if (typeIdx == 3)
            {
                HideTopBottom();
            }

            if(typeIdx <2)HideEverything(typeIdx);

            ChangeInThreeScenes(ImagesToChange, thisCloth, typeIdx);


            if (InventorySlotMgt.wearingNonDefualtClothes[typeIdx])
            {
                InventorySlotMgt.wearingSlots[typeIdx].GetComponent<ClothChanging>().isWearing = false;
            }

            InventorySlotMgt.wearingSlots[typeIdx] = this.transform.gameObject;
            InventorySlotMgt.wearingNonDefualtClothes[typeIdx] = true;
            Debug.Log(thisCloth.name);
            thisCloth.state = Cloth.ClothState.Wearing;
        }
        else
        {

            thisCloth.state = Cloth.ClothState.InventoryNotWear;


            if (typeIdx == 3)
            {
                ShowDefaultTop();
                ShowDefaultBottom();
            }
            else
            {
                Cloth defaultCloth = SpriteLoader.defaultClothes[typeIdx];
                ChangeInThreeScenes(ImagesToChange, defaultCloth, typeIdx);
            }

            InventorySlotMgt.wearingNonDefualtClothes[typeIdx] = false;

        }
    




    }

    private void ChangeInThreeScenes(SpriteRenderer[] ImagesToChange, Cloth cloth, int typeIdx)
    {

        ImagesToChange[0].sprite = cloth.spritesInScenes[0];
        ImagesToChange[1].sprite = cloth.spritesInScenes[1];
        AdsController.ChangeClothInAds(typeIdx, cloth);

    }

    private void HideTopBottom()
    {
        InventorySlotMgt.TopImages[0].sprite = transparent;
        InventorySlotMgt.TopImages[1].sprite = transparent;
        AdsController.TakeOffInAds(1);

        InventorySlotMgt.BottomImages[0].sprite = transparent;
        InventorySlotMgt.BottomImages[1].sprite = transparent;
        AdsController.TakeOffInAds(2);


        if (InventorySlotMgt.wearingNonDefualtClothes[0])
        {
            InventorySlotMgt.wearingSlots[0].GetComponent<ClothChanging>().isWearing = false;
            InventorySlotMgt.wearingNonDefualtClothes[0] = false;
        }

        if (InventorySlotMgt.wearingNonDefualtClothes[1])
        {
            InventorySlotMgt.wearingSlots[1].GetComponent<ClothChanging>().isWearing = false;
            InventorySlotMgt.wearingNonDefualtClothes[1] = false;
        }


    }

    private void HideEverything(int typeIdx)

    {
        

        if (InventorySlotMgt.wearingNonDefualtClothes[3]||InventorySlotMgt.isWorkingCloth)
        {
            
            InventorySlotMgt.EverythingImages[0].sprite = transparent;
            InventorySlotMgt.EverythingImages[1].sprite = transparent;
            AdsController.TakeOffInAds(3);


            if (InventorySlotMgt.wearingNonDefualtClothes[3])InventorySlotMgt.wearingSlots[3].GetComponent<ClothChanging>().isWearing = false;
            InventorySlotMgt.wearingNonDefualtClothes[3] = false;
            ShowDefaultBottom();
            ShowDefaultTop();
            InventorySlotMgt.isWorkingCloth = false;
        }


    }

    private void ShowDefaultTop()
    {
        Cloth defaultCloth = SpriteLoader.defaultClothes[0];
        ChangeInThreeScenes(InventorySlotMgt.TopImages, defaultCloth, 0);
        InventorySlotMgt.wearingNonDefualtClothes[0] = false;

    }

    private void ShowDefaultBottom()
    {
        Cloth defaultCloth = SpriteLoader.defaultClothes[1];
        ChangeInThreeScenes(InventorySlotMgt.BottomImages, defaultCloth, 1);
        InventorySlotMgt.wearingNonDefualtClothes[1] = false;

    }


    private void ShowDefaultShoe()
    {
        Cloth defaultCloth = SpriteLoader.defaultClothes[2];
        ChangeInThreeScenes(InventorySlotMgt.ShoeImages, defaultCloth, 2);
        InventorySlotMgt.wearingNonDefualtClothes[2] = false;
    }

    public void ChangeWorkCloth()
    {
        if (FinalCameraController.isTutorial)
        {
//            FinalCameraController.TutorialManager.tutorialNumber = 13;
            FinalCameraController.TutorialManager.kararaText.text = "Workwear No";
            return;
        }


        HideTopBottom();

        Cloth workingCloth = SpriteLoader.ClothDic["work"];
        ChangeInThreeScenes(InventorySlotMgt.EverythingImages, workingCloth, 3);

        if (InventorySlotMgt.wearingNonDefualtClothes[3])
        {
            InventorySlotMgt.wearingSlots[3].GetComponent<ClothChanging>().isWearing = false;
        }
        isWearing = true;
        InventorySlotMgt.wearingSlots[3] = this.transform.gameObject;

        InventorySlotMgt.isWorkingCloth = true;
    }



    



}


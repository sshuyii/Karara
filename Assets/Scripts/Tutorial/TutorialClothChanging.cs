using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class TutorialClothChanging : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int originSlotNum;
    public Text text;
    private Vector3 startPos;

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
    private GameObject returnBG,dropClothImage;
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

    private AudioManager AudioManager;

   
    public TutorialManagerNew TutorialManagerNew;

    [SerializeField]
    private int clothName;    //这件衣服是什么
    [SerializeField]
    private GameObject returnClothNotice;//还包的提示
    private Sprite top;
    private Sprite bottom;

    void Start()
    {
        currentSprite = GetComponent<Image>().sprite;
        top =  TutorialManagerNew.inventoryCloth[0].sprite;
        bottom = TutorialManagerNew.inventoryCloth[1].sprite;

    }

    // Update is called once per frame
    void Update()
    {
        if (pointerDown)
		{
            //取消已经生成的所有ui界面
            for(int i = 0; i < 4; i++)
            {            
                TutorialManagerNew.ReturnNoticeList[i].SetActive(false);
            }

            longPress = false;

			pointerDownTimer += Time.deltaTime;
			if (pointerDownTimer >= requiredHoldTime)
			{
                longPress = true;

                //Debug.Log("LongPress");
                //if (!isWorkCloth) InventorySlotMgt.castLongPress();
                //新功能
                if (!isWorkCloth) showReturnConfirm();
                //if(!isWorkCloth) showReturnConfirm();
                Reset();
			}
		}
    }


    public void ClickClothInventory()
    {
        //如果长按
        if(longPress) return;
        

        //点击inventory里的衣服UI
        // if (deactiveButtons) return;
        Debug.Log("ClickClothInventory");

        //change Cloth
        if(clothName == 1)
        {   
            if(TutorialManagerNew.isWearingClothNum != 1)
            {
                TutorialManagerNew.isWearingClothNum = 1;

                TutorialManagerNew.inventoryCloth[3].sprite = TutorialManagerNew.workInventory;
                TutorialManagerNew.subwayCloth[3].sprite = TutorialManagerNew.workSubway;
                // TutorialManagerNew.postKararaImage.sprite = postPose1;
                TutorialManagerNew.workClothOn = true;
                TutorialManagerNew.isAlter = false;
            }
            else{
                //把这件衣服脱了
                TutorialManagerNew.isWearingClothNum = 0;

                TutorialManagerNew.inventoryCloth[3].sprite = TutorialManagerNew.transparent;
                // TutorialManagerNew.inventoryCloth[0].sprite = top;
                // TutorialManagerNew.inventoryCloth[1].sprite = bottom;

            }
        }
        else if(clothName == 2)
        {
            if(TutorialManagerNew.isWearingClothNum != 2)
            {
                TutorialManagerNew.isWearingClothNum = 2;

                TutorialManagerNew.inventoryCloth[3].sprite = TutorialManagerNew.workAlterInventory;
                TutorialManagerNew.subwayCloth[3].sprite = TutorialManagerNew.workAlterSubway;
                // TutorialManagerNew.postKararaImage.sprite = postPose1Alter;
                TutorialManagerNew.workClothOn = true;
                TutorialManagerNew.isAlter = true;
            }
            else{
                //把这件衣服脱了
                TutorialManagerNew.isWearingClothNum = 0;

                TutorialManagerNew.inventoryCloth[3].sprite = TutorialManagerNew.transparent;
            }

        }
        else if(clothName == 3)
        {
            if(!TutorialManagerNew.isWearingShoe)
            {
                TutorialManagerNew.isWearingShoe = true;

                TutorialManagerNew.inventoryCloth[2].sprite = TutorialManagerNew.workShoe;
                TutorialManagerNew.subwayCloth[2].sprite = TutorialManagerNew.workShoeSubway;
                TutorialManagerNew.workShoeOn = true;
            }
            else{
                //把鞋脱了
                TutorialManagerNew.isWearingShoe = false;

                TutorialManagerNew.inventoryCloth[2].sprite = TutorialManagerNew.transparent;
            }
        } 
    }



    public void setFillAmount(float[] timers) {
        timer = timers[0];
        totalTime = timers[1];
    }
    public void OnPointerDown(PointerEventData eventData)
	{
        //AudioManager.PlayAudio(AudioType.UI_Dialogue);
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
        //cancelReturnButton.SetActive(false);
        returnClothNotice.SetActive(false);
        //returnConfirmButton.gameObject.SetActive(false);
    }

    IEnumerator KararaDeny()
    {
        returnClothNotice.SetActive(true);

        //karara摇头
        TutorialManagerNew.kararaAnimator.SetTrigger("isShaking");
        //等待两秒之后消失，这两秒内也不能点yes
        yield return new WaitForSeconds(1f);
        returnClothNotice.SetActive(false);

    }

    public void showReturnConfirm()
    {
        //出现了归还ui之后再禁止还衣服
        if(TutorialManagerNew.putClothBack)
        {
            if(clothName == 1|| clothName == 2)
            {
                //todo: 显示karara说一些话，工作服不能还


            }
            else if(clothName == 3){
                //鞋不可以还
                //todo: 显示karara说一些话，鞋不能还
                
            }
           
            StartCoroutine(KararaDeny());
            return;
        }

        //鞋无论如何都不能还
        if(clothName == 3)
        {
            //鞋不可以还
            //todo: 显示karara说一些话，鞋不能还

            TutorialManagerNew.kararaAnimator.SetTrigger("isShaking");
            return;
        }

        //如果是两件衣服中的一件，可以还
        //鞋不可以还
        //肯定回到洗衣机里
        returnClothNotice.SetActive(true);
    }

    public void ReturnClothPre()
    {
        tapStart = true;
    }


    public void ReturnCloth()
    {
        ///此时把衣服还到洗衣机里的ui已经出现了，玩家点了yes
        //如果是已经穿上了工作服和鞋，点yes也不能还了
        if(TutorialManagerNew.putClothBack)
        {
            return;
        }
        //鞋无论如何都不能还
        if(clothName == 3) return;

        //界面变化
        //这个script是挂在inventory的衣服UI上的
        returnClothNotice.SetActive(false);
        longPress = false;

        //inventory里的衣服消失
        GetComponent<Image>().sprite = TutorialManagerNew.transparent;

        //确认哪件衣服出现
        TutorialManagerNew.clothSlotTable[clothName].SetActive(true);

        //Karara身上的衣服消失
        if(clothName == 1)
        {   
            //还了任意一件工作服，就不能还另一件了
            TutorialManagerNew.putClothBack = true;
            if(TutorialManagerNew.isWearingClothNum == 1)
            {
                //karara身上的衣服消失
                TutorialManagerNew.inventoryCloth[3].sprite = TutorialManagerNew.transparent;
                TutorialManagerNew.subwayCloth[3].sprite = TutorialManagerNew.transparent;
                TutorialManagerNew.adsCloth[3].sprite = TutorialManagerNew.transparent;
            }
            //放回洗衣机里
            TutorialManagerNew.ClothSlotList[0].SetActive(true);
        }
        else if(clothName == 2)
        {   
            //还了任意一件工作服，就不能还另一件了
            TutorialManagerNew.putClothBack = true;
            if(TutorialManagerNew.isWearingClothNum == 2)
            {
                //karara身上的衣服消失
                TutorialManagerNew.inventoryCloth[3].sprite = TutorialManagerNew.transparent;
                TutorialManagerNew.subwayCloth[3].sprite = TutorialManagerNew.transparent;
                TutorialManagerNew.adsCloth[3].sprite = TutorialManagerNew.transparent;
            }
            //放回洗衣机里
            TutorialManagerNew.ClothSlotList[1].SetActive(true);
        }       


        //todo: long press but later chose not to return?


        //print("return cloth is working");

        //print("return cloth is working and double touched");



        // currentSprite = GetComponent<Image>().sprite;
        // Cloth thisCloth = SpriteLoader.ClothDic[currentSprite.name];
        // Debug.Log(thisCloth.owner);
        // bool bagFoundInCar = AllMachines.PutClothBackToMachine(thisCloth.owner, originSlotNum);
        

        // if(timer < 0||bagFoundInCar ==false)
        // {
        //     Debug.Log("put l f");
        //     DropToLostAndFound();
        //     //Debug.Log("put l f");
        // }

        


        // takeOffCloth();
        // AfterTakeOff();

        
    }

    
   
    private void DropToLostAndFound(){
        currentSprite = GetComponent<Image>().sprite;
        //Debug.Log("丢掉" +currentSprite.name);
        Cloth thisCloth = SpriteLoader.ClothDic[currentSprite.name];
        NPC owner = SpriteLoader.NPCDic[thisCloth.owner];
        LostAndFound.AddClothToList(thisCloth, owner);
       
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



    // private void takeOffCloth() 
    // {
    //     if (!isWearing) return;
    //     currentSprite = GetComponent<Image>().sprite;

    //     Cloth thisCloth = SpriteLoader.ClothDic[currentSprite.name];


        
    //     int typeIdx;
    //     switch (thisCloth.type)
    //     {
    //         case Cloth.ClothType.Top:
    //             ShowDefaultTop();
    //             typeIdx = 0;
    //             break;
    //         case Cloth.ClothType.Bottom:
    //             ShowDefaultBottom();
    //             typeIdx = 1;
    //             break;
    //         case Cloth.ClothType.Shoe:
    //             ShowDefaultShoe();
    //             typeIdx = 2;
    //             break;
    //         case Cloth.ClothType.Everything:
    //             ShowDefaultTop();
    //             ShowDefaultBottom();
    //             typeIdx = 3;
    //             break;
    //         default:
    //             return;

    //     }

    //     InventorySlotMgt.wearingNonDefualtClothes[typeIdx] = false;

    // }




    public void ChangeCloth()

    {

        if (!isOccupied) return;
        if (longPress) return;




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
            //Debug.Log(thisCloth.name);
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
        InventorySlotMgt.TopImages[0].sprite = TutorialManagerNew.transparent;
        InventorySlotMgt.TopImages[1].sprite = TutorialManagerNew.transparent;
        AdsController.TakeOffInAds(1);

        InventorySlotMgt.BottomImages[0].sprite = TutorialManagerNew.transparent;
        InventorySlotMgt.BottomImages[1].sprite = TutorialManagerNew.transparent;
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
            
            InventorySlotMgt.EverythingImages[0].sprite = TutorialManagerNew.transparent;
            InventorySlotMgt.EverythingImages[1].sprite = TutorialManagerNew.transparent;
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


    // private void ShowDefaultShoe()
    // {
    //     Cloth defaultCloth = SpriteLoader.defaultClothes[2];
    //     ChangeInThreeScenes(InventorySlotMgt.ShoeImages, defaultCloth, 2);
    //     InventorySlotMgt.wearingNonDefualtClothes[2] = false;
    // }

//     public void ChangeWorkCloth()
//     {
//         if (FinalCameraController.isTutorial)
//         {
// //            FinalCameraController.TutorialManager.tutorialNumber = 13;
//             FinalCameraController.TutorialManager.kararaText.text = "Workwear No";
//             return;
//         }


//         HideTopBottom();

//         Cloth workingCloth = SpriteLoader.ClothDic["work"];
//         ChangeInThreeScenes(InventorySlotMgt.EverythingImages, workingCloth, 3);

//         if (InventorySlotMgt.wearingNonDefualtClothes[3])
//         {
//             InventorySlotMgt.wearingSlots[3].GetComponent<ClothChanging>().isWearing = false;
//         }
//         isWearing = true;
//         InventorySlotMgt.wearingSlots[3] = this.transform.gameObject;

//         InventorySlotMgt.isWorkingCloth = true;
//     }



    



}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventorySlotMgt : MonoBehaviour
{

    
    public GameObject[] slots;
    [SerializeField]
    Button DoneButton;

    public int occupiedNum = 0;

    public bool InventoryIsFull = false;

    [SerializeField]
    GameObject InventoryInstruction;


    
    public SpriteRenderer[] TopImages = new SpriteRenderer[2];

    public SpriteRenderer[] BottomImages = new SpriteRenderer[2];

    public SpriteRenderer[] ShoeImages = new SpriteRenderer[2];

    public SpriteRenderer[] EverythingImages = new SpriteRenderer[2];


    private SpriteLoader SpriteLoader;
    BagsController BagsController;

    public bool showingFullNotice = false;

    public GameObject InventoryFull;


    // 0 top 1 bottom 2 shoe 3 everything
    public GameObject[] wearingSlots = new GameObject[4];
    public bool[] wearingNonDefualtClothes = new bool[4] { false, false, false, false };



    public bool isWorkingCloth = true;
    public Sprite dropToMachineImg, dropToLandFImg;
    
    public List<ClothChanging> slotScripts = new List<ClothChanging>();

    // Start is called before the first frame update
    void Start()
    {
        DoneButton.gameObject.SetActive(false);
        SpriteLoader = GameObject.Find("---SpriteLoader").GetComponent<SpriteLoader>();
        BagsController = GameObject.Find("---BagsController").GetComponent<BagsController>();

        for(int i = 0; i < slots.Length; i++) 
        {
            slotScripts.Add(slots[i].GetComponentInChildren<ClothChanging>());
        }
    }

    // Update is called once per frame
    void Update()
    {
       if(occupiedNum == 6)
       {
            InventoryIsFull = true;
       }
       else{
           InventoryIsFull = false;
       }
    }

    public void castLongPress()
    {
        for (int i = 0; i < slots.Length; i++) {
            slotScripts[i].showReturnConfirm();
        }

        DoneButton.gameObject.SetActive(true);
    }

    public void DoneBottonClicked()
    {
        for(int i = 0; i < slots.Length; i++) {
            slotScripts[i].hideReturnConfirm();
        }
        DoneButton.gameObject.SetActive(false);
    }


    public int GetFirstEmptySlot()
    {
        int first = -1;
        for(int i = 0; i < slots.Length; i++)
        {
            if (!slotScripts[i].isOccupied)
            {
                first = i;
                return first;
            }
        }
        return first;
    }

    //Shuyi：好像没在用？
    public void AddClothToInventory(string clothName, int machineNum, int subwaySlotNum, GameObject bag)
    {
        occupiedNum++;

        int slot = GetFirstEmptySlot();

        if (slot < 0)
        {
            InventoryIsFull = true;

            return;
        }


        GameObject firstEmptySlot = slots[slot];
        ClothChanging firstEmptySlotScipt = slotScripts[slot];
        Cloth cloth = SpriteLoader.ClothDic[clothName];

        float[] timers = BagsController.GetRemainTime(cloth.owner);

        Sprite clothSprite = cloth.spritesInScenes[2];
        firstEmptySlot.GetComponent<Image>().sprite = clothSprite;
        
        firstEmptySlotScipt.NewClothAddToInventory(clothSprite, machineNum, subwaySlotNum, bag, timers[0],timers[1]);


    }


    public void ShowInventoryFullNotice()
    {
        Debug.Log("Inventory is full ");

        showingFullNotice = true;

        InventoryFull.SetActive(true);
    }

    public void CloseInventoryFullNotice()
    {
        showingFullNotice = false;
        InventoryFull.SetActive(false);
    }

    public void CloseAllUI()
    {
        showingFullNotice = false;
        InventoryFull.SetActive(false);
        for (int i = 0; i < slots.Length; i++)
        {
            slotScripts[i].hideReturnConfirm();
        }

    }

    public void TransparentizeClothes(GameObject bag)
    {
        foreach(ClothChanging cg in slotScripts)
        {
            if(cg.originBag == bag) cg.SetFillAmountZero();
        }
    }

}

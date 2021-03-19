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
  


    // Start is called before the first frame update
    void Start()
    {
        DoneButton.gameObject.SetActive(false);
        SpriteLoader = GameObject.Find("---SpriteLoader").GetComponent<SpriteLoader>();
        BagsController = GameObject.Find("---BagsController").GetComponent<BagsController>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void castLongPress()
    {
        for (int i = 0; i < slots.Length; i++) {
            slots[i].GetComponentInChildren<ClothChanging>().showReturnConfirm();
        }

        DoneButton.gameObject.SetActive(true);
    }

    public void DoneBottonClicked()
    {
        for(int i = 0; i < slots.Length; i++) {
            slots[i].GetComponentInChildren<ClothChanging>().hideReturnConfirm();
        }
        DoneButton.gameObject.SetActive(false);
    }


    public int GetFirstEmptySlot()
    {
        int first = -1;
        for(int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].GetComponent<ClothChanging>().isOccupied)
            {
                first = i;
                return first;
            }
        }
        return first;
    }

    public void AddClothToInventory(string clothName, int subwaySlotNum)
    {
        occupiedNum++;
        int slot = GetFirstEmptySlot();

        if (slot < 0)
        {
            ShowInventoryFullNotice();
            return;
        }


        GameObject firstEmptySlot = slots[slot];
        Cloth cloth = SpriteLoader.ClothDic[clothName];

        float[] timers = BagsController.GetRemainTime(cloth.owner);


        firstEmptySlot.GetComponent<Image>().sprite = cloth.spritesInScenes[2];
        firstEmptySlot.GetComponent<ClothChanging>().halfTspImg.sprite = cloth.spritesInScenes[2];
        firstEmptySlot.GetComponent<ClothChanging>().originSlotNum = subwaySlotNum;
        firstEmptySlot.GetComponent<ClothChanging>().setFillAmount(timers);
        firstEmptySlot.GetComponent<ClothChanging>().isOccupied = true;

    }


    public void ShowInventoryFullNotice()
    {
        Debug.Log("full ");
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
            slots[i].GetComponentInChildren<ClothChanging>().hideReturnConfirm();
        }

    }

}

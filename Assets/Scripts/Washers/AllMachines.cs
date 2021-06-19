using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


//功能：管理所有的machine
//machine list
//return notice

public class AllMachines : MonoBehaviour
{

    public GameObject currentBag;
    private BagsController BagsController;

    //a list to record all customer's names


    public bool isReturning;

    public GameObject returnNotice;
    public Button returnNoticeYes;
    public Button returnNoticeNo;

    public Sprite openedDoor;
    public Sprite closedDoor;

    public GameObject noticeParent;

    public GameObject Overdue;
    public bool alreadyNotice = false;

    public List<Sprite> openBags;
    public Dictionary<string, Sprite> openBagsDic = new Dictionary<string, Sprite>();


    public Sprite TutorialCloth1;
    public Sprite TutorialCloth2;


    public List<GameObject> WashingMachines = new List<GameObject>();
    public List<GameObject> FakeMachines = new List<GameObject>();

    public List<WasherController> WasherControllerList = new List<WasherController>();

    public int washTime;

    private bool isLoad;

    public int clothButtonNum;

    public int machineWithNotice;

    public enum MachineState
    {
        empty,
        bagUnder,
        full,
        washing,
        finished,

        noninteractable
    }


    InventorySlotMgt InventorySlotMgt;
    public int[] OccupiedClothUISlots = new int[4];

    private GameObject[] bagsUnderMahines = new GameObject[3];

    void Start()
    {
        InventorySlotMgt = GameObject.Find("---InventoryController").GetComponent<InventorySlotMgt>();
        BagsController = GameObject.Find("---BagsController").GetComponent<BagsController>();

        for (int i = 0; i < WashingMachines.Count; i++)
        {
            WasherControllerList.Add(WashingMachines[i].GetComponent<WasherController>());

        }

        clothButtonNum = WasherControllerList[0].buttons.Length;

    }

    // Update is called once per frame
    void Update()
    {


    }

    public void SetWasherAsNoninteractable(int idx)
    {
        WasherControllerList[idx].myMachineState = MachineState.noninteractable;
    }
    public int FindSuitableMachine(MachineState neededState)
    {
        int machine = -1;
        for (int i = 0; i < WasherControllerList.Count; i++)
        {
            if (WasherControllerList[i].myMachineState == neededState)
            {
                machine = i;
                break;
            }

        }

        return machine;
    }

    public void SetMachineAsBagUnder(int idx, List<string> clothes, GameObject bagGO)
    {
        WasherControllerList[idx].myMachineState = MachineState.bagUnder;
        WasherControllerList[idx].currentBag = bagGO;
        bagsUnderMahines[idx] = bagGO;
        AddClothToMachine(idx, clothes);
    }

    public void SetMachineAsFull(int idx)
    {
        WasherControllerList[idx].SetMachineAsFull();

    }


    // public void AddCustomer(int machineIdx, string customer)
    // {

    //     WasherControllerList[machineIdx].currentCustomer = customer;
    // }

    public bool FinishedOrNot(int idx)
    {
        if (WasherControllerList[idx].myMachineState == MachineState.finished
            || WasherControllerList[idx].myMachineState == MachineState.noninteractable) return true;
        else return false;
    }


    public int ClearMachine(int idx, bool timeup)
    {
        int star = 0;
        //这里及时时间未到还了包，也不是永远加星，少了衣服就要减星！
        // if (!timeup) star = 1;
        if(!timeup && WasherControllerList[idx].clothNum == 4) star = 1;
        else star = WasherControllerList[idx].clothNum - 4;
        Debug.Log("star " + star.ToString());
        GameObject bag = bagsUnderMahines[idx];
        InventorySlotMgt.TransparentizeClothes(bag);
        WasherControllerList[idx].ResetMachine();
        bagsUnderMahines[idx] = null;
        return star;
    }





    private void AddClothToMachine(int idx, List<string> clothes)
    {
        WasherControllerList[idx].AddClothSprite(clothes);
    }

    public void CloseAllMachines()
    {
        foreach (WasherController wc in WasherControllerList)
        {
            // close door image
            // clothui active = false
            if (wc.shut == 1)
            {
                wc.shut = 0;
                // Hide(ClothUI);
                StartCoroutine(wc.MachineFold(3));
                //ClothUiAnimator.SetBool("isUnfold",false);

                //change door to closed sprite
                wc.DoorImage.sprite = closedDoor;
                if (wc.myMachineState == MachineState.finished || wc.myMachineState == MachineState.noninteractable) wc.Occupied.SetActive(true);
            }
        }
    }


    public bool PutClothBackToMachine(GameObject bag, int machineIdx, int slotNum)
    {
       
        if (bagsUnderMahines[machineIdx] == bag)
        {
            
            WasherController wc = WasherControllerList[machineIdx];
            wc.buttons[slotNum - 1].GetComponent<SpriteRenderer>().enabled = true;
            wc.buttons[slotNum - 1].GetComponent<BoxCollider2D>().enabled = true;
            wc.clothNum++;
            Debug.Log("washer " + wc.number + " cloth num: " + wc.clothNum);
            return true;
        }

        return false;
    }

    public bool FoundBagInCar(GameObject bag, int machineIdx)
    {
        Debug.Log("@demo bag:" + bag.name + "machine " + machineIdx);
        if (bagsUnderMahines[machineIdx] == bag) return true;
        else return false;
    }




}

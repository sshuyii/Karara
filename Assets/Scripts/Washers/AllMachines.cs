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

    //a list to record all customer's names
    public List<List<string> >CustomerNameList = new List<List<string>>();


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

    private List<WasherController> WasherControllerList = new List<WasherController>();

    public int washTime;

    private bool isLoad;

    public int clothButtonNum;

    public int machineWithNotice;

    public enum MachineState {
        empty,
        bagUnder,
        full,
        washing,
        finished
    }



    void Start()
    {
        for (int i = 0; i < WashingMachines.Count; i++)
        {
            WasherControllerList.Add(WashingMachines[i].GetComponent<WasherController>());
            CustomerNameList.Add(new List<string>());
        }
        
        clothButtonNum = WasherControllerList[0].buttons.Length;

    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public int FindSuitableMachine(MachineState neededState)
    {
        int machine = -1;
        for(int i = 0; i < WasherControllerList.Count; i++)
        {
            if (WasherControllerList[i].myMachineState == neededState)
            {
                machine = i;
                break;
            }

        }

        return machine;
    }

    public void SetMachineAsBagUnder(int idx, string name, List<string> clothes)
    {
        WasherControllerList[idx].myMachineState = MachineState.bagUnder;
        AddCustomer(idx, name);
        AddClothToMachine(idx, clothes);

    }

    public void SetMachineAsFull(int idx)
    {
        WasherControllerList[idx].myMachineState = MachineState.full;

    }


    public void AddCustomer(int machineIdx, string customer) {
        //Debug.Log(machineIdx);
        CustomerNameList[machineIdx].Add(customer);

        //Debug.Log(machineIdx);

        WasherControllerList[machineIdx].currentCustomer = customer;
    }

    public bool FinishedOrNot(int idx) {
        if (WasherControllerList[idx].myMachineState == MachineState.finished) return true;
        else return false;
    }


    public void ClearMahine(int idx) {
        WasherControllerList[idx].ResetMachine();
    }


    



    private void AddClothToMachine(int idx, List<string> clothes)
    {
        WasherControllerList[idx].AddClothSprite(clothes);
    }

    public void CloseAllMachines(bool isClickMachine) {
        foreach (WasherController wc in WasherControllerList)
        {
            // close door image
            // clothui active = false
            if(wc.shut == 1)
            {
                wc.shut = 0;
                // Hide(ClothUI);
                wc.MachineFold();
                //ClothUiAnimator.SetBool("isUnfold",false);

                //change door to closed sprite
                wc.DoorImage.sprite = closedDoor;
                wc.Occupied.SetActive(true);
            }
        }
    }


    public bool PutClothBackToMachine(string customerName,int slotNum)
    {
        foreach(WasherController wc in WasherControllerList)
        {
            if(wc.currentCustomer == customerName)
            {
                wc.buttons[slotNum-1].GetComponent<SpriteRenderer>().enabled = true;
                wc.buttons[slotNum - 1].GetComponent<BoxCollider2D>().enabled = true;
                wc.clothNum++;
                return true;
            }
        }
        
        return false;
    }

    public bool FoundBagInCar(string customerName)
    {
        foreach (WasherController wc in WasherControllerList)
        {
            if (wc.currentCustomer == customerName)
            { 
                return true;
            }
        }

        return false;
    }

}

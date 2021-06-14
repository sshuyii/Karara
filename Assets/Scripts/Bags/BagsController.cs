using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BagsController : MonoBehaviour
{
    public List<GameObject> bagsInCar = new List<GameObject>();
    public List<List<string>> stationBagOwners = new List<List<string>>();
    public GameObject[] returnNotices;
    public GameObject returningBag;
    private int returnNoticePage;
    FinalCameraController FinalCameraController;
    LevelManager LevelManager;
    public int timeUpBagNum;
    public int unfinishedBagNum;
    public FishBossNotification FishBossNotification;
    // Start is called before the first frame update

    float timer; //for check bag status periodicly 
    AllMachines washers;
    ValueEditor ValueEditor;
    void Start()
    {
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        LevelManager = GameObject.Find("---LevelManager").GetComponent<LevelManager>();
        ValueEditor = GameObject.Find("---ValueEditor").GetComponent<ValueEditor>();
        washers = FinalCameraController.AllMachines;

        stationBagOwners.Add(new List<string>());
        stationBagOwners.Add(new List<string>());
        stationBagOwners.Add(new List<string>());


        timeUpBagNum = 0;
        unfinishedBagNum = 0;

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (LevelManager.stage == 1 && timer> ValueEditor.TimeRelated.fishNotificationPeriod)
        {
            timer = 0f;
            CheckBagsStates();
            
        }

    }


    void CheckBagsStates()
    {
        unfinishedBagNum = 0;
        foreach(GameObject bag in bagsInCar)
        {
            ClothToMachine ctm = bag.GetComponent<ClothToMachine>();
            if(ctm.underMachineNum < 0 || (ctm.underMachineNum>= 0 && !ctm.isFinished))
            {
                //todo:忽略了一种情况就是衣服正在洗但没洗完
                unfinishedBagNum++;
            }
        }

       if(unfinishedBagNum > 1)
        {
            FinalCameraController.BagOverFlow();
        }
    }
    public void AddBagToCar(GameObject bag, int homeStation, string owner)
    {
        bagsInCar.Add(bag);
        if (!stationBagOwners[homeStation].Contains(owner))
        {
            stationBagOwners[homeStation].Add(owner);
        }

    }


    public bool CheckBagsInCar(string owner, int stationNum)
    {
        bool result = false;
        for (int i = 0; i < bagsInCar.Count; i++)
        {
            if (bagsInCar[i].tag == owner)
            {
                
                if(owner == "X" && stationNum != bagsInCar[i].GetComponent<ClothToMachine>().myStation)
                {
                    result = false;
                }
                else
                {
                    result = true;
                    break;
                }
            }
        }

        return result;
    }


    public void RemoveBag(GameObject bag)
    {
        bagsInCar.Remove(bag);
    }


    public bool neverMinusStar = true;
    public void ShowReturnNotice(GameObject bag, int machienNum)
    {
        GameObject notice = returnNotices[machienNum];
        notice.SetActive(true);
        returningBag = bag;
        
        FinalCameraController.alreadyNotice = true;
        
        WasherController wc = washers.WasherControllerList[machienNum];
        
        wc.DoorImage.sprite = washers.openedDoor;
        wc.Occupied.SetActive(false);

        int absentClothNum = wc.clothNum - 4; // 0 -1 -2 -3 -4 五种情况
        int oneStarForBag = 0;
        if(bag.GetComponent<ClothToMachine>().timeUp == false && LevelManager.stage > 1 ) oneStarForBag++;
        Debug.Log("star num :" + absentClothNum + "bag star: "+ oneStarForBag);
        StartCoroutine(LevelManager.StopToMinusStar( notice, absentClothNum, oneStarForBag));
    }

    public void HideNotice()
    {
        if(FinalCameraController.fishShouting) return;
        foreach(GameObject notice in returnNotices)
        {
            notice.SetActive(false);
        }
        FinalCameraController.alreadyNotice = false;
       
    }
    public void ClickReturnNo(bool DropBagOrNot) {
        HideNotice();

        if(returningBag != null && !DropBagOrNot)
        {
            
            int washerNum = returningBag.GetComponent<ClothToMachine>().underMachineNum;
            returnNotices[washerNum].SetActive(false);
            FinalCameraController.alreadyNotice = false;
            FinalCameraController.Hide(FinalCameraController.fishShoutCG);
            WasherController wc = washers.WasherControllerList[washerNum];
            wc.DoorImage.sprite = washers.closedDoor;
            if(wc.clothNum > 0) wc.Occupied.SetActive(true);
        }
        

    }

    public void ClickReturnYes() {
        foreach(GameObject notice in returnNotices)
        {
            notice.SetActive(false);
        }
        FinalCameraController.alreadyNotice = false;
        
        StartCoroutine(returningBag.GetComponent<ClothToMachine>().returnClothYes());
        bagsInCar.Remove(returningBag);
    }



    public float[] GetRemainTime(string clothOwner)
    {
        foreach(GameObject bag in bagsInCar)
        {
            ClothToMachine ctm = bag.GetComponent<ClothToMachine>();
            if (ctm.tag == clothOwner)
            {
                return new float[2] { ctm.timer, ctm.totalTime };
            }
        }

        return new float[2] { -1,-1};
    }



    public int DropAllBagsInScreen3()
    {
        int returnedBagNum = 0;
        for(int i = 0; i < bagsInCar.Count; i++)
        {
            ClothToMachine ctm = bagsInCar[i].GetComponent<ClothToMachine>();
            // timeup 判断可能有问题
            if(ctm.underMachineNum == 1|| ctm.underMachineNum == 2  && ctm.timeUp)
            {
                
                returnedBagNum++;
                //Debug.Log("bags in 3 returning");
                returningBag = bagsInCar[i];
                int washerNum = returningBag.GetComponent<ClothToMachine>().underMachineNum;
                WasherController wc = washers.WasherControllerList[washerNum];
                if(wc.clothNum > 0) wc.Occupied.SetActive(true);
                ClickReturnYes();
            }
        }
        return returnedBagNum;
    }

    public int CountAllBagsInWasher(int washer)
    {
        for (int i = 0; i < bagsInCar.Count; i++)
        {
            ClothToMachine ctm = bagsInCar[i].GetComponent<ClothToMachine>();
            //@ demo possible problem
            if (ctm.underMachineNum == washer && ctm.timeUp)
            {
                return 1;
            }
        }
        return 0;
    }


    public int DropAllBagsInWasher(int washer)
    {
        
        int returnedBagNum = 0;
        for (int i = 0; i < bagsInCar.Count; i++)
        {
            ClothToMachine ctm = bagsInCar[i].GetComponent<ClothToMachine>();
            if (ctm.underMachineNum == washer && ctm.timeUp)
            {
                returnedBagNum++;
                returningBag = bagsInCar[i];
                ClickReturnYes();
            }
        }
        return returnedBagNum;
    }



    public void AddTimeUpBags()
    {
        timeUpBagNum++;
        
    }
    public void BagsOnDueShaderOn()
    {
        foreach(GameObject bag in bagsInCar)
        {
            ClothToMachine ctm = bag.GetComponent<ClothToMachine>();
            if(ctm.underMachineNum>= 0 && ctm.timer < 11f)
            {
                Animator bagAnim = ctm.transform.GetComponent<Animator>();
                bagAnim.SetTrigger("blingbling");
            }
        }
    }
}

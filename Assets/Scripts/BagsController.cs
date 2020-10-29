using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagsController : MonoBehaviour
{
    public List<GameObject> bagsInCar = new List<GameObject>();
    public List<List<string>> stationBagOwners = new List<List<string>>();
    public GameObject returnNotice;
    public GameObject returningBag;
    FinalCameraController FinalCameraController;


    // Start is called before the first frame update
    void Start()
    {
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();

        stationBagOwners.Add(new List<string>());
        stationBagOwners.Add(new List<string>());
        stationBagOwners.Add(new List<string>());
    }

    // Update is called once per frame
    void Update()
    {
        
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


    public void ShowReturnNotice(GameObject bag,bool flip)
    {
        returnNotice.SetActive(true);
        returningBag = bag;
        FinalCameraController.alreadyNotice = true;

        if(flip)
        {
            foreach (Transform child in returnNotice.transform)
            {
                if (child.name == "BG")
                {
                    child.localRotation = new Quaternion(0, 180, 0, 0);
                    return;
                }
            }
        }

    }

    public void ClickReturnNo() {
        returnNotice.SetActive(false);
        FinalCameraController.alreadyNotice = false;

       
        foreach (Transform child in returnNotice.transform)
        {
            if (child.name == "BG")
            {
                child.localRotation = new Quaternion(0, 0, 0, 0);
                return;
            }
        }

    }

    public void ClickReturnYes() {
        returningBag.GetComponent<ClothToMachine>().returnClothYesDirectly();
        bagsInCar.Remove(returningBag);
        FinalCameraController.alreadyNotice = false;

        ClickReturnNo();
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



    public void DropAllBagsInScreen3()
    {
        for(int i = 0; i < bagsInCar.Count; i++)
        {
            ClothToMachine ctm = bagsInCar[i].GetComponent<ClothToMachine>();
            if(ctm.underMachineNum == 1|| ctm.underMachineNum == 2)
            {
                Debug.Log("bags in 3 returning");
                returningBag = bagsInCar[i];
                ClickReturnYes();
            }
        }
    }


    public void DropAllBagsInScreen2()
    {
        for (int i = 0; i < bagsInCar.Count; i++)
        {
            ClothToMachine ctm = bagsInCar[i].GetComponent<ClothToMachine>();
            if (ctm.underMachineNum == 0)
            {
                Debug.Log("bags in 2 returning");
                returningBag = bagsInCar[i];
                ClickReturnYes();
            }
        }
    }
}

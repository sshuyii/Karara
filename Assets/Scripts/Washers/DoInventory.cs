using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DoInventory : MonoBehaviour
{
    private InventorySlotMgt InventorySlotMgt;

    private FinalCameraController FinalCameraController;
    private WasherController WasherController;
    
    private GameObject InventoryController;
    //this dictionary is the player inventory

    //a list that stores UI location
    private Sprite currentSprite;
    public int slotNum;
    private TouchController TouchController;
    // Start is called before the first frame update
    void Start()
    {
        InventoryController = GameObject.Find("---InventoryController");
        InventorySlotMgt = InventoryController.GetComponent<InventorySlotMgt>();
        
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        TouchController =  GameObject.Find("---TouchController").GetComponent<TouchController>();

//        selfButton.onClick.AddListener(AddClothToInventory);

    }

    // Update is called once per frame
    void Update()
    {

// @@@
        if(TouchController.myInputState == TouchController.InputState.Tap){
            if(TouchController.RaycastHitResult[0] == "doIvt" && Int32.Parse(TouchController.RaycastHitResult[1]) == slotNum){
                //Debug.Log("get hit slot #" + slotNum.ToString());
                StartCoroutine(AddClothToInventory());     
            }
        }
    }

    public IEnumerator AddClothToInventory()
    {

        if (FinalCameraController.isSwipping == false)
        {            
            WasherController = GetComponentInParent<WasherController>();

            if (InventorySlotMgt.occupiedNum < 6)
            {
     
                //get the machine this cloth belongs to
                //if all clothes in the machine are taken, close door
                WasherController.clothNum--; //洗衣机里的衣服少一

                //如果洗衣机里没衣服了，那么直接关上
                if (WasherController.clothNum == 0)
                {
       
                    WasherController.MachineFold();
                    WasherController.DoorImage.sprite = WasherController.AllMachines.closedDoor;
                }


                

                currentSprite = GetComponent<SpriteRenderer>().sprite;

                InventorySlotMgt.AddClothToInventory(currentSprite.name,slotNum);



                //for tutorial
                if (FinalCameraController.isTutorial) //打开了洗衣机的门，真的拿了衣服
                {
                    //do somehting

                }

                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<BoxCollider2D>().enabled = false;
            }

            else if (InventorySlotMgt.occupiedNum > 5 && !InventorySlotMgt.showingFullNotice)
            {

                InventorySlotMgt.ShowInventoryFullNotice();
                WasherController.MachineFold();
                WasherController.DoorImage.sprite = WasherController.AllMachines.closedDoor;
            

            }
        }
    yield return null;

    }
}

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
    private WasherController WasherController;
    private FinalCameraController FinalCameraController;
    
    private GameObject InventoryController;
    //this dictionary is the player inventory

    //a list that stores UI location
    private Sprite currentSprite;
    public int slotNum;
    
    private TouchController TouchController;
    // Start is called before the first frame update
    private int washerNum ;
    void Start()
    {
        InventoryController = GameObject.Find("---InventoryController");
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();

        InventorySlotMgt = InventoryController.GetComponent<InventorySlotMgt>();
        TouchController =  GameObject.Find("---TouchController").GetComponent<TouchController>();

        washerNum = GetComponentInParent<WasherController>().number;
        
//        selfButton.onClick.AddListener(AddClothToInventory);

    }

    // Update is called once per frame
    void Update()
    {

// @@@
        if(TouchController.myInputState == TouchController.InputState.Tap){
            int number;
            if(Int32. TryParse( TouchController.RaycastHitResult[0], out number))
            {
                if(number == washerNum && Int32.Parse(TouchController.RaycastHitResult[1]) == slotNum) 
                    AddClothToInventory();
            }
        }
    }

    public void AddClothToInventory()
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
                WasherController.shut = 0;
                StartCoroutine(WasherController.MachineFold(3));

            }

            currentSprite = GetComponent<SpriteRenderer>().sprite;
            InventorySlotMgt.AddClothToInventory(currentSprite.name, washerNum, slotNum, WasherController.currentBag);

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
   
        }

        //inventory里面满了
        else if (InventorySlotMgt.occupiedNum > 5 && !InventorySlotMgt.showingFullNotice)
        {
            
            //Shuyi: Instead of showing inventory is full notice, go to screen1 and let fish say the sentence

            WasherController.shut = 0;
            StartCoroutine(WasherController.MachineFold(0));
        }

    }
    
}

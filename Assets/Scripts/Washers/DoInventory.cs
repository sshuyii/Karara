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
    private int washerNum ;
    void Start()
    {
        InventoryController = GameObject.Find("---InventoryController");
        InventorySlotMgt = InventoryController.GetComponent<InventorySlotMgt>();
        
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
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
                StartCoroutine(WasherController.MachineFold());

            }

            currentSprite = GetComponent<SpriteRenderer>().sprite;
            InventorySlotMgt.AddClothToInventory(currentSprite.name, slotNum);

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
   
        }

        else if (InventorySlotMgt.occupiedNum > 5 && !InventorySlotMgt.showingFullNotice)
        {
            
            InventorySlotMgt.ShowInventoryFullNotice();
            WasherController.shut = 0;
            StartCoroutine(WasherController.MachineFold());
        }

    }
    
           
            

    
}

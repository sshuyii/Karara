﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.UI.Extensions;

public class ClothToMachine : MonoBehaviour
{

    private GameObject ClothInMachineController;

    private AllMachines AllMachines;
    private SubwayMovement SubwayMovement;

    private AudioSource myAudio;


    private FinalCameraController FinalCameraController;


    private bool alreadyWashed;
    private bool isOverdue;
    public int hitTime;//can be made private

    private Image myImage;

    public int myBagPosition;

    public bool isNotice;
    public Image secondImage;
    public bool isNoticePrefab;

    
    //a timer to record how much time has passed since the bag is on the car

    public bool isReady;

    // Start is called before the first frame update

    private CameraMovement cameraMovement;
    private RatingSystem RatingSys;

    SpriteLoader SpriteLoader;

    [SerializeField]
    private GameObject thisBag;

    int underMachineNum = -1;
    private bool timeUp;


    public float timer;
    private float remainTime;
    public float totalTime;
    private float fillAmount;

    private NPC owner;
    public int myStation;

    private List<string> clothesInBag = new List<string>();

    private BagsController BagsController;
    bool isFinished = false;


    private AudioManager AudioManager;


    void Start()
    {
        //find the horizontal scroll snap script
        //myHSS = GameObject.Find("Horizontal Scroll Snap").GetComponent<HorizontalScrollSnap>();

        myAudio = GetComponent<AudioSource>();

        myImage = GetComponent<Image>();
//        secondImage = GetComponentInChildren<Image>();
        hitTime = 0;


        AudioManager = GameObject.Find("---AudioManager").GetComponent<AudioManager>();

        ClothInMachineController = GameObject.Find("---ClothInMachineController");
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        SubwayMovement = GameObject.Find("---StationController").GetComponent<SubwayMovement>();
        cameraMovement = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
        RatingSys = GameObject.Find("FloatingUI").GetComponent<RatingSystem>();
        SpriteLoader = GameObject.Find("---SpriteLoader").GetComponent<SpriteLoader>();
        BagsController = GameObject.Find("---BagsController").GetComponent<BagsController>();

        AllMachines = ClothInMachineController.GetComponent<AllMachines>();

        //todo: generate clothes
        Debug.Log(this.transform.gameObject.tag);

        owner = SpriteLoader.NPCDic[this.transform.gameObject.tag];

        GenerateCloth();

        if(!FinalCameraController.AllStationClothList.ContainsKey(owner.name))
        {
            FinalCameraController.AllStationClothList.Add(owner.name, new List<Sprite>());
        }

        

    }



    void Update()
    {


        if (underMachineNum >= 0) isFinished = AllMachines.FinishedOrNot(underMachineNum);
        if (isFinished) alreadyWashed = true;

        if(timeUp&&alreadyWashed)
        {
            ReturnBag2cases();
        }
       

        if(!FinalCameraController.isTutorial && !isNoticePrefab)
        {
            secondImage.sprite = myImage.sprite;
        }


        
        if(!isNotice)
        {
            timer -= Time.deltaTime;
            //remainTime = (SubwayMovement.moveTime + SubwayMovement.stayTime) * 3 - timer;
            //remainTime = totalTime - timer;
            myImage.fillAmount = timer / totalTime;

            if (!timeUp && timer<0)
            {
                timeUp = true;

                Debug.Log("包包过期！！");

                if (!FinalCameraController.ChapterOneEnd)
                {
                    if (alreadyWashed) //if this bag is already washed
                    {
                        //一次只能还一个包
                        //如果已经有包在还了，就直接还
                        ReturnBag2cases();
                    }
                    else if (!isOverdue)
                    {
                        Debug.Log("这个过期了！");
                        GameObject overdue = Instantiate(AllMachines.Overdue, this.gameObject.transform.position,
                                Quaternion.identity);
                        overdue.transform.SetParent(this.gameObject.transform);
                        isOverdue = true;
                    }
                }
            }


        }
    }


   private void ReturnBag2cases()
   {
        BagsController.returningBag = this.transform.gameObject;
        BagsController.ClickReturnYes();
        FinalCameraController.HorizontalScrollSnap.JumpToPage(1);
        //todo: fish talk late return;
    }



    public IEnumerator returnClothYes ()
    {
        AllMachines.isReturning = true;
        FinalCameraController.ChangeToSubway();

        if (underMachineNum == 0) cameraMovement.currentPage = 2;
        else cameraMovement.currentPage = 3;


        yield return new WaitForSeconds(1f);
      
        FinalCameraController.alreadyNotice = false;

        FinalCameraController.returnMachineNum = underMachineNum;
        AllMachines.ClearMahine(underMachineNum);


        if(!FinalCameraController.isTutorial)
        {
            SpriteLoader.NPCDic[tag].usedIdx.Clear();
        }

        //先播放一个包不见的动画
        //todo: yun disappear animation
        //thisBag.GetComponent<Image>().sprite = CalculateInventory.disappear;

        yield return new WaitForSeconds(0.2f);

        //thisBag.GetComponent<Image>().sprite = CalculateInventory.transparent;

        yield return new WaitForSeconds(0.5f);


        cameraMovement.currentPage = 1;

        yield return new WaitForSeconds(0.2f);


        FinalCameraController.fishTalk.SetActive(false);
        FinalCameraController.fishTalkText.text = "Return your customers' clothes on time! Such bad memory!";
        FinalCameraController.lateReturnComic = false;


        Debug.Log("add star!!!");

        if (!timeUp)
        {
            Debug.Log(" not time up add star!!!");
            RatingSys.AddStars();
        }


        BagsController.RemoveBag(thisBag);
        Destroy(thisBag);

        //todo:subway movement remove bags in car


        AllMachines.isReturning = false;

        Destroy(this);

        
    }

    
    public void returnClothYesDirectly()
    {

        //包消失

        FinalCameraController.alreadyNotice = false;

        FinalCameraController.returnMachineNum = underMachineNum;
        AllMachines.ClearMahine(underMachineNum);

        if (!FinalCameraController.isTutorial)
        {
            SpriteLoader.NPCDic[tag].usedIdx.Clear();
        }


        Debug.Log("add star!!!");

        if (!timeUp)
        {
            Debug.Log(" not time up add star!!!");
            RatingSys.AddStars();
        }
        //if (!timeUp) RatingSys.AddStars();



        Destroy(thisBag);

        //todo:subway movement remove bags in car

        AllMachines.isReturning = false;

        Destroy(this);


    if (FinalCameraController.isTutorial && FinalCameraController.TutorialManager.tutorialNumber == 16)
     {
                        //do something
     }


       
    }


    public void returnClothNo()
    {

        AllMachines.returnNotice.SetActive(false);
        FinalCameraController.alreadyNotice = false;

    }


 
    public void putClothIn()
    {
        if(!FinalCameraController.isTutorial)
        {
            FinalCameraController.LevelManager.isInstruction = false;
        }
        
        FinalCameraController.CancelAllUI(false);

        if (FinalCameraController.isSwipping) return;


        if (hitTime == 0)
        {
            myAudio.pitch = 0.5f;

            myAudio.Play();

            //for tutorial
            if (FinalCameraController.isTutorial)
            {

                AllMachines.SetMachineAsBagUnder(0, owner.name,clothesInBag);


                this.gameObject.transform.SetParent(AllMachines.FakeMachines[0].gameObject.transform);// @@@

                transform.position =
                    AllMachines.FakeMachines[0].transform.position + new Vector3(0, -2.7f, 0); // @@@


                cameraMovement.currentPage = 2;

                FinalCameraController.TutorialManager.KararaStandingImage.enabled = false;
                FinalCameraController.TutorialManager.tutorialNumber = 3;

                FinalCameraController.TutorialManager.stopDisappear = false;

                hitTime++;


            }
            else
            {
                //Debug.Log(transform.position);
                SubwayMovement.bagNum -= 1;

                underMachineNum = AllMachines.FindSuitableMachine(AllMachines.MachineState.empty);
                AllMachines.SetMachineAsBagUnder(underMachineNum, owner.name,clothesInBag);



                this.gameObject.transform.SetParent(AllMachines.FakeMachines[underMachineNum].gameObject.transform);// @@@

                transform.position =
                            AllMachines.FakeMachines[underMachineNum].transform.position + new Vector3(0, -2.7f, 0); // @@@

                if (underMachineNum == 0) cameraMovement.currentPage = 2;
                else cameraMovement.currentPage = 3;

                SubwayMovement.bagPosAvailable[myBagPosition] = false;

                hitTime++;



            }
        }

        else if (hitTime == 1)
        {
            //Debug.Log("second bag hit ");
            myAudio.pitch = 0.6f;
            myAudio.Play();

            //Debug.Log("tag: " + this.tag);
            myImage.sprite = SpriteLoader.NPCDic[this.tag].openBag;


            //in tutorial
            if (FinalCameraController.isTutorial)
            {
                FinalCameraController.TutorialManager.tutorialNumber = 4;
            }


            AllMachines.SetMachineAsFull(underMachineNum);
            hitTime++;


        }
        //return clothes
        else if (hitTime > 1)
        {
            //Debug.Log("third bag hit ");
             isFinished = AllMachines.FinishedOrNot(underMachineNum);
            if (isFinished && !FinalCameraController.alreadyNotice)
            {
                AllMachines.currentBag = this.gameObject;
                if (FinalCameraController.isTutorial)
                {
                    //do something

                }
                else
                {
                    if(underMachineNum ==1) BagsController.ShowReturnNotice(thisBag,true);
                    else BagsController.ShowReturnNotice(thisBag, false);
                }
            }
            hitTime++;
        }
          
    }


    private void GenerateCloth()
    {
        if (FinalCameraController.isTutorial)
        {
            // do something
        }
        else
        {
            int upperBound = owner.myClothes.Count;
            int randomIdx;


            for (int i = 0; i < AllMachines.clothButtonNum; i++)
            {
                while (true)
                {
                    randomIdx = Random.Range(0, upperBound);
                    if (!owner.usedIdx.Contains(randomIdx))
                    {
                        owner.usedIdx.Add(randomIdx);
                        clothesInBag.Add(SpriteLoader.ClothDic[owner.myClothes[randomIdx]].name);

                        break;
                    }
                }

            }

            foreach (int idx in owner.usedIdx)
            {
                clothesInBag.Add(SpriteLoader.ClothDic[owner.myClothes[idx]].name);
            }

            //the clothes belongs to this bag are store as a index list 
        }

        
        
    }

    
}

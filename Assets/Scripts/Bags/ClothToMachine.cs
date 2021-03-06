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

    public int underMachineNum = -1;
    public bool timeUp;


    public float timer;
    private float remainTime;
    public float totalTime;
    private float fillAmount;
    private float delayTime;

    private NPC owner;
    public int myStation;

    private List<string> clothesInBag = new List<string>();

    private BagsController BagsController;
    public bool isFinished = false;


    private AudioManager AudioManager;

    private Animator myAnimator;


    int stage;

    ValueEditor ValueEditor;
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
        ValueEditor = GameObject.Find("---ValueEditor").GetComponent<ValueEditor>();
        AllMachines = ClothInMachineController.GetComponent<AllMachines>();

        myAnimator = this.transform.gameObject.GetComponent<Animator>();
        //todo: generate clothes
        //Debug.Log(this.transform.gameObject.tag);

        owner = SpriteLoader.NPCDic[this.transform.gameObject.tag];

        GenerateCloth();

        if(!FinalCameraController.AllStationClothList.ContainsKey(owner.name))
        {
            FinalCameraController.AllStationClothList.Add(owner.name, new List<Sprite>());
        }


        myImage.enabled = true;
        stage = FinalCameraController.LevelManager.stage;
    }



    void Update()
    {
        stage = FinalCameraController.LevelManager.stage;

        if (SubwayMovement.pauseBeforeMove) return;

        if (stage > 1) 
        {
            if(delayTime > 0) delayTime -= Time.deltaTime;
            else
            {
                timer -= Time.deltaTime;
                myImage.fillAmount = timer / totalTime;
            }

        }        // todo: why timer < 2f


        if (!timeUp && timer < 0f)
        {
            timeUp = true;
            Debug.Log("包包过期！！");

            if (stage > 1 && hitTime <=  1)
            {
                GameObject overdue = Instantiate(AllMachines.Overdue, this.gameObject.transform.position,
                        Quaternion.identity);
                overdue.transform.SetParent(this.gameObject.transform);
            }

        }

        if (underMachineNum >= 0) isFinished = AllMachines.FinishedOrNot(underMachineNum);

        if(timeUp && isFinished)
        {
            StartCoroutine(ReturnBag2cases());
        }
      
    }


   private IEnumerator ReturnBag2cases()
   {
        //todo: overdue bag 洗完，右上角有鱼老板提示
        //if (underMachineNum == 0) FinalCameraController.CameraMovement.JumpToPage(2);
        //else FinalCameraController.CameraMovement.JumpToPage(3);

        //如果在stage 1: 只记录数值

        if(stage == 1)
        {
            BagsController.AddTimeUpBags();
            yield return null;
        }
        else
        {
            AllMachines.SetWasherAsNoninteractable(underMachineNum);
            
        }


        yield return null;
    }

    public void SetTime(float Ttotal, float TDelay)
    {
        totalTime = Ttotal;
        timer = Ttotal;
        delayTime = TDelay;
    }

    public IEnumerator returnClothYes ()
    {
        AudioManager.PlayAudio(AudioType.Cloth_Return);
        AllMachines.isReturning = true;
        // FinalCameraController.ChangeToSubway();
        FinalCameraController.alreadyNotice = false;

        

        
        FinalCameraController.returnMachineNum = underMachineNum;
        AllMachines.ClearMahine(underMachineNum);
        SpriteLoader.NPCDic[tag].usedIdx.Clear();

        float Twait = ValueEditor.TimeRelated.openWasherDelay1 + ValueEditor.TimeRelated.openWasherDelay2;
        myAnimator.SetTrigger("Disappear");

        Twait = myAnimator.GetCurrentAnimatorClipInfo(0).Length;
        yield return new WaitForSeconds(Twait);


        if (!timeUp)
        {
            Debug.Log(" not time up add star!!!");
            RatingSys.AddStars();
        }

        yield return new WaitForSeconds(Twait);
        Destroy(thisBag);
        AllMachines.isReturning = false;
        Destroy(this);
        
    }

    
    public void returnClothYesDirectly()
    {

        //包消失

        AudioManager.PlayAudio(AudioType.Cloth_Return);
        FinalCameraController.alreadyNotice = false;

        FinalCameraController.returnMachineNum = underMachineNum;
        AllMachines.ClearMahine(underMachineNum);

        SpriteLoader.NPCDic[tag].usedIdx.Clear();


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

       
    }


    public void returnClothNo()
    {
        BagsController.ClickReturnNo(false);
    }


 
    public void putClothIn()
    {

        
        FinalCameraController.CancelAllUI(false);

        if (FinalCameraController.isSwipping) return;


        if (hitTime == 0)
        {
            //没有洗衣机就不动啥反应都没有

            underMachineNum = AllMachines.FindSuitableMachine(AllMachines.MachineState.empty);
            if (underMachineNum < 0) return;



            AudioManager.AdjustPitch(AudioType.Bag_Phase1, 0.5f);
            AudioManager.PlayAudio(AudioType.Bag_Phase1);

            SubwayMovement.bagNum -= 1;
            AllMachines.SetMachineAsBagUnder(underMachineNum, owner.name, clothesInBag);



            //this.gameObject.transform.SetParent(AllMachines.FakeMachines[underMachineNum].gameObject.transform);// @@@

            transform.localPosition = SubwayMovement.bagPos[underMachineNum + 3]; // @@@

            if (underMachineNum == 0) cameraMovement.Go2Page(2);
            else cameraMovement.Go2Page(3);

            SubwayMovement.bagPosAvailable[myBagPosition] = false;

            hitTime++;

        }

        else if (hitTime == 1)
        {
            //Debug.Log("second bag hit ");
            myAudio.pitch = 0.6f;
            AudioManager.AdjustPitch(AudioType.Bag_Phase1, 0.6f);
            //myAudio.Play();

            AudioManager.PlayAudio(AudioType.Bag_Phase1);
            myImage.sprite = SpriteLoader.NPCDic[this.tag].openBag;
            secondImage.sprite = SpriteLoader.NPCDic[this.tag].openBag;

            if (timeUp) StartCoroutine(FinalCameraController.FishBossNotification.ShowFish());
            //FinalCameraController.FishBossNotification.ShowFish(); //test

            AllMachines.SetMachineAsFull(underMachineNum);
            hitTime++;


        }
        //return clothes
        else if (hitTime > 1)
        {
            AudioManager.PlayAudio(AudioType.Bag_Phase1);
            isFinished = AllMachines.FinishedOrNot(underMachineNum);
            if (isFinished && !FinalCameraController.alreadyNotice)
            {
                if(underMachineNum ==1) BagsController.ShowReturnNotice(thisBag,true);
                else BagsController.ShowReturnNotice(thisBag, false);
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

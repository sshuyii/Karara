using System.Collections;
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

    public int underMachineNum = -1;
    public bool timeUp;


    public float timer;
    private float remainTime;
    public float totalTime;
    private float fillAmount;

    private NPC owner;
    public int myStation;

    private List<string> clothesInBag = new List<string>();

    private BagsController BagsController;
    public bool isFinished = false;


    private AudioManager AudioManager;

    private Animator myAnimator;


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

        myAnimator = this.transform.gameObject.GetComponent<Animator>();
        //todo: generate clothes
        //Debug.Log(this.transform.gameObject.tag);

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

        if(isOverdue&&alreadyWashed)
        {
            StartCoroutine(ReturnBag2cases());
        }
       

        if(!FinalCameraController.isTutorial && !isNoticePrefab)
        {
            secondImage.sprite = myImage.sprite;
        }


        
        if(!isNotice && !SubwayMovement.pauseBeforeMove)
        {
            timer -= Time.deltaTime;
            //remainTime = (SubwayMovement.moveTime + SubwayMovement.stayTime) * 3 - timer;
            //remainTime = totalTime - timer;
            myImage.fillAmount = timer / totalTime;

            if (!timeUp && timer< 2f)
            {
                timeUp = true;

                Debug.Log("包包过期！！");

                if (!FinalCameraController.ChapterOneEnd)
                {
                    if (alreadyWashed) //if this bag is already washed
                    {
                        //一次只能还一个包
                        //如果已经有包在还了，就直接还
                        //ReturnBag2cases();
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


   private IEnumerator ReturnBag2cases()
   {
        //todo: overdue bag 洗完，右上角有鱼老板提示
        //if (underMachineNum == 0) FinalCameraController.CameraMovement.JumpToPage(2);
        //else FinalCameraController.CameraMovement.JumpToPage(3);

        BagsController.returningBag = this.transform.gameObject;
        BagsController.ClickReturnYes();

        FinalCameraController.fishTalkText.text = "Return your customers' clothes on time! Such bad memory!";
        //FinalCameraController.FishBossNotification.ShowBubble();

        yield return null;
    }



    public IEnumerator returnClothYes ()
    {
        AudioManager.PlayAudio(AudioType.Cloth_Return);
        AllMachines.isReturning = true;
        FinalCameraController.ChangeToSubway();
        FinalCameraController.alreadyNotice = false;

        

        myAnimator.SetTrigger("Disappear");
        FinalCameraController.returnMachineNum = underMachineNum;
        AllMachines.ClearMahine(underMachineNum);
        SpriteLoader.NPCDic[tag].usedIdx.Clear();

        yield return new WaitForSeconds(0.9f);


        //if (SubwayMovement.pauseBeforeMove == false)
        //{
        //    cameraMovement.currentPage = 1;
        //    yield return new WaitForSeconds(0.2f);
        //    FinalCameraController.fishTalk.SetActive(false);
        //    FinalCameraController.fishTalkText.text = "Return your customers' clothes on time! Such bad memory!";
        //    FinalCameraController.lateReturnComic = false;
        //}


        if (!timeUp)
        {
            Debug.Log(" not time up add star!!!");
            RatingSys.AddStars();
        }


        BagsController.RemoveBag(thisBag);
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

            if (timeUp) FinalCameraController.FishBossNotification.ShowFish();
            //FinalCameraController.FishBossNotification.ShowFish(); //test

            AllMachines.SetMachineAsFull(underMachineNum);
            hitTime++;


        }
        //return clothes
        else if (hitTime > 1)
        {
            AudioManager.PlayAudio(AudioType.Bag_Phase1);
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

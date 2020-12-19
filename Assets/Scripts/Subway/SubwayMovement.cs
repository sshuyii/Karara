﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;
public class SubwayMovement : MonoBehaviour
{
    
    private FinalCameraController FinalCameraController;

    public AudioSource doorSound;
    private bool doorSoundPlay = true;
    
    private int NpcCount = 0;
    private LevelManager LevelManager;
    public int currentStation;
    public GameObject highlight;
    private SpriteRenderer hSR;
    
    public GameObject arrow;
    private SpriteRenderer aSR;

    public float doorMovement;
    public float doorWidth;

    public bool outsideStation = true;
    public CanvasGroup clear;

    public List<bool> bagPosAvailable = new List<bool>();
    //public List<GameObject> arrows;

//    public List<Vector3> stationPos;

    public List<GameObject> highlights;

    //all the bags in the car
    public List<GameObject> bagsInCar;
    public bool noSameBag;
    
    public List<Vector3> bagPos;
    public List<Button> clothBags;
    public bool bagFirst = true;
    public GameObject clothBagGroup;

    private List<CanvasGroup> DetailCG = new List<CanvasGroup>();
    public CanvasGroup dSR1;
    public CanvasGroup dSR2;


    //check how many bags are in the car
    public int firstEmptyPos = 0;
    
    //a list recording all the bags on the train that haven't been taken into the washing machine
    private Dictionary<string, bool> AllBagsTaken = new Dictionary<string, bool>();
    public int bagNum = 0;
    
    private Dictionary<string, int> allStation = new Dictionary<string, int>();
    public List<string> stationNames;

    public bool isMoving = false;//if true, the train is moving
    
    //doors
    public Transform left1;
    public Transform right1;
    
//    public RectTransform left2;
//    public GameObject right2;
    
    //door initial positions
    private float left1Pos;
    private float right1Pos;
    private float left2Pos;
    private float right2Pos;

    public float moveTime;
    public float stayTime;
    public float pauseTime;
    public bool pauseBeforeMove = false;
    


    //a list of buttons in detail background
    private List<List<Image>> AllDetailList = new List<List<Image>>();
    public List<Image> detailList0 = new List<Image>();
    public List<Image> detailList1 = new List<Image>();

    
    //time text
    public TextMeshProUGUI ClothCountDownText;
    
    private float CountDownTime;

    private float timer = 0;

    private bool nothingInside = false; //if nothing is in the machine
    public Sprite transparent;
    
  
    public List<Dictionary<String, List<Sprite>>> allStationList = new List<Dictionary<String, List<Sprite>>>();

    //one station has one list recording the potential cloth bags
    public List<Button> bagStation0;
    public List<Button> bagStation1;
    public List<Button> bagStation2;

    private List<List<Button>> NameToStationBags = new List<List<Button>>();


    public TextMeshProUGUI CountDownTimer;
    private float stationTimer;
    private float realTimer;

    public Button NPCBag;

    public bool alreadyStation1;
    public bool alreadyStation2;
    public bool alreadyStation0;

    public GameObject collectionContent;

    [SerializeField]
    private GameObject collection;

    [SerializeField]
    private List<Sprite> bagLogoInCollection;

    public Image tabImg0,tabImg1;
    [SerializeField]
    private GameObject tab0,tab1;


    public int[] bagCounts = new int[3]{0,0,0};

    private string collectionStationNum;
    public bool isDetailed = false;

    //for now the speical npc per station is 1
    private int specialNPCPerStation = 1;


    // how many round has been finished by now
    private int roundNum = 0;

    bool isTerminated = false;


    public Dictionary<string, int> stationNum_SpecialNPC = new Dictionary<string, int>
        {
            { "Alex", 0 },
            { "Bella",1},
            { "Nami", 2},
            { "X", 0 }

        };
    private LostAndFound LostAndFound;
    private int generateBagCount = -1;
    private InstagramController InstagramController;
    private BagsController BagsController;
    private AdsController AdsController;
    private bool atInitailStation = true;

    public bool isDoorOpen = false;
    public bool NoPosition = false;
    public bool OpeningProcess = false;
    public bool ClosingProcess = false;

    public GameObject Banner;
    public UnityEngine.UI.Text remainingTime;
    public GameObject BlackScreen;
    public GameObject MatchState;




    private AudioManager AudioManager;

    [SerializeField]
    private StationForButton StationForButton;

    // Start is called before the first frame update
    void Start() 
    {
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        InstagramController = GameObject.Find("---InstagramController").GetComponent<InstagramController>();
        BagsController = GameObject.Find("---BagsController").GetComponent<BagsController>();
        AdsController = GameObject.Find("---AdsController").GetComponent<AdsController>();
        AudioManager = GameObject.Find("---AudioManager").GetComponent<AudioManager>();
        


        if (!FinalCameraController.isTutorial)
        {
            LevelManager = FinalCameraController.LevelManager;

            bagPosAvailable.Add(false);
            bagPosAvailable.Add(false);
            bagPosAvailable.Add(false);

            noSameBag = true;

            aSR = arrow.GetComponent<SpriteRenderer>();
            hSR = highlight.GetComponent<SpriteRenderer>();

            //tabImg0 = GameObject.Find("bag0Logo").GetComponent<Image>();
            //tabImg1 = GameObject.Find("bag1Logo").GetComponent<Image>();
            LostAndFound = GameObject.Find("Lost&Found_basket").GetComponent<LostAndFound>();

            //tab0 = GameObject.Find("bagTab0");
            //tab1 = GameObject.Find("bagTab1");
            collection.SetActive(false);
        }

        // 不知道 real timer干什么用的
        realTimer = (moveTime + stayTime + pauseTime) * 3 - timer;
        
        NameToStationBags.Add (bagStation0);
        NameToStationBags.Add(bagStation1);
        NameToStationBags.Add(bagStation2);


        AllDetailList.Add(detailList0);
        AllDetailList.Add(detailList1);

        DetailCG.Add(dSR1);
        DetailCG.Add(dSR2);

        currentStation = 0; 

        stationTimer = stayTime;
//        CountDownTimer.text = "";
        
        if(!FinalCameraController.isTutorial)
        {
            //get all the doors position when game starts
            left1Pos = left1.position.x;
            right1Pos = right1.position.x;
        }




        //get all station names into the dictionary
        for (var i = 0; i < stationNames.Count; ++i)
        {
            allStation.Add(stationNames[i], i);
        }


        //trainStop();
    }


    public IEnumerator DelayTrainStop()
    {
        yield return new WaitForSeconds(1f);
    }

    private Button clothBag1;
    private Button clothBag2;
    private Button clothBag3;

    public float timerStay = 0f;
    public float timerS2S = 0f; //station to station move + stay
    public float timerPause = 0f;


    void emptyBagPos()
    {
        //记录第一个画面没有放包的地方
        for (int i = 0; i < 3; i++)
        {
            if (bagPosAvailable[i] == false)//get the first empty bag position
            {
                firstEmptyPos = i;
                break;
            }
           
        }

        //timerStay = 0;
    }
    // Update is called once per frame
    void Update()
    {
        // 转完两圈就terminate
        if (isTerminated||LevelManager.isInstruction) return;

        




        //test if have already arrived at stations
        //for poster generation
        //current station means the station the train is heading to
        if (currentStation == 2 && !alreadyStation1)
        {
            alreadyStation1 = true;
        }
        else if (currentStation == 0 && !alreadyStation2 && alreadyStation1)
        {
            alreadyStation2 = true;
        }
        else if (currentStation == 1 && alreadyStation2 && alreadyStation1)
        {
            alreadyStation0 = true;
        }
        //        //print("FinalCameraController.AllStationClothList.Count  =" + FinalCameraController.AllStationClothList.Count);
        //正式游戏


        if (!pauseBeforeMove)
        {
            timerS2S += Time.deltaTime;
            if (!isMoving) timerStay += Time.deltaTime;
        }
        else
        {
            timerPause += Time.deltaTime;
        }

        // banner shows 10s before moving
        //test
        if (timerStay > stayTime - 10f && Banner.active == false)//test
        {
            Debug.Log("比较时间"+timerStay + " " + (stayTime - 10f));
            Banner.SetActive(true);
        }
        else if (Banner.active)
        {
            int leftT = (int)(stayTime - timerStay);
            Banner.GetComponent<TextMeshProUGUI>().text = leftT.ToString();
        }


        if (timerStay > stayTime)
        {
            timerStay = 0f;
            StartCoroutine(trainPause());
            //if (!atInitailStation) StartCoroutine(trainPause());//test
            //else
            //{
            //    Banner.SetActive(false);
            //    atInitailStation = false;
            //    trainMove();
            //}


        }


        if (timerS2S > stayTime + moveTime)
        {
            trainStop();
            timerS2S = 0;
        }






        if (OpeningProcess)

        {


            if (left1.position.x > left1Pos - doorWidth)
            {
                left1.position -= new Vector3(doorMovement, 0, 0);
            }
            else
            {
                left1.position = new Vector3(left1Pos - doorWidth, left1.position.y, 0);
                OpeningProcess = false;
            }


            if (right1.position.x < right1Pos + doorWidth)
            {
                right1.position += new Vector3(doorMovement, 0, 0);
            }
            else
            {
                right1.position = new Vector3(right1Pos + doorWidth, right1.position.y, 0);
                OpeningProcess = false;
            }

            if (OpeningProcess == false)
            {
                DoorOpenFinish();
            }
        }


        if (ClosingProcess)
        {
            if (left1.position.x < left1Pos)
            {
                left1.position += new Vector3(doorMovement, 0, 0);
            }
            else
            {
                left1.position = new Vector3(left1Pos, left1.position.y, 0);
                ClosingProcess = false;
            }

            if (right1.position.x > right1Pos)
            {
                right1.position -= new Vector3(doorMovement, 0, 0);
            }
            else
            {
                right1.position = new Vector3(right1Pos, right1.position.y, 0);
                ClosingProcess = false;
                isDoorOpen = false;
            }
            //no action is needed after door close

        }

        //todo:门开着就放包暂时ban掉
        //if (isDoorOpen && NoPosition)
        //{
        //    GenerateBag(currentStation);
        //}

    }

    void OpenDoor()
    {
        //没有章节结束
        //if (roundNum == 2)
        //{
        //    //第一章结束
        //    FinalCameraController.ChapterOneEnd = true;
        //    isTerminated = true;
        //}

        OpeningProcess = true;

        StartCoroutine(AudioManager.PlayAudioInStation(stayTime));

    }



    void CloseDoor()
    {
        ClosingProcess = true;
        StartCoroutine(AudioManager.PlayAudioOutStation(moveTime));

    }

    void DoorOpenFinish()
    {


        for (int i = 0; i < specialNPCPerStation; i++)
        {

            // 门完全开之后：
            // 1.产生包
            // 2.丢l&f衣服,发post(包含在3里)
            // 3.更新posture(ad)
            GenerateBag(currentStation);
            if (roundNum>0) LostAndFound.DropLostFoundClothes(currentStation);
            else if(!atInitailStation) InstagramController.RefreshPost("", FinalCameraController.RatingSys.rating);

            bagFirst = false;

            isDoorOpen = true;


        }


    }




    //this is used to create new bags in the car
    void GenerateBag(int stationNum)
    {
        //generateBagCount
        //Debug.Log("generateBag");
        //如果现在车里不够三个包才产生新包

        if (bagNum == 3)
        {
            NoPosition = true;
            return;
        }


        bool allLastRoundBagsInCar = true;
        for (int i = 0; i < NameToStationBags[stationNum].Count; i++)
        {
            if (!BagsController.CheckBagsInCar(NameToStationBags[stationNum][i].tag, stationNum))
            {
                GenerateSpecialBag(stationNum,i);
                allLastRoundBagsInCar = false;
            }
        }



        bool NPCIsInCar = BagsController.CheckBagsInCar("X",stationNum);

        //如果不放进来特殊角色的包，放npc的包
        //前提是上一次进来的npc的包不在车里
        if (allLastRoundBagsInCar)
        {
            if (!NPCIsInCar) GenerateNpcBag(10f);
            return;
        }
        else
        {
            if (!NPCIsInCar) GenerateNpcBag(8f);
        }

        
    }


    private void GenerateSpecialBag(int stationNum, int npcNum)
    {
        if (bagNum == 3)
        {
            NoPosition = true;
            //yield return null;
            return;
        }


        if (bagNum < 3)
        {
            emptyBagPos();

            //yield return new WaitForSeconds(0.1f);
            //if (!bagFirst) yield return new WaitForSeconds(2f);
            //if (!bagFirst && !isDoorOpen) yield return null;


            Button bag = Instantiate(NameToStationBags[stationNum][npcNum], bagPos[firstEmptyPos],
    Quaternion.identity) as Button;

            bag.GetComponent<ClothToMachine>().timer = 4 * stayTime + 3 * moveTime - timerStay;
            bag.GetComponent<ClothToMachine>().totalTime = 4 * stayTime + 3 * moveTime - timerStay;



            //this position is occupied by a bag
            bagPosAvailable[firstEmptyPos] = true;

            //bagsInCar.Add(bag.gameObject);

            bag.GetComponent<ClothToMachine>().myBagPosition = firstEmptyPos;
            bagNum++;
            bag.transform.SetParent(clothBagGroup.transform, false);
            bag.GetComponent<ClothToMachine>().myStation = currentStation;
            bagCounts[currentStation]++;


            BagsController.AddBagToCar(bag.gameObject, currentStation, bag.tag);

        }

    }


    private void GenerateNpcBag(float probability)
    {

        if (bagNum == 3)
        {
            NoPosition = true;
            //yield return null;
            return;
        }
        //如果现在车里不够三个包
        if (bagNum < 3)
        {
            emptyBagPos();

            //yield return new WaitForSeconds(0.1f);
            //if (!bagFirst) yield return new WaitForSeconds(2f);
            //if (!bagFirst && !isDoorOpen) yield return null;

            //only generate a new bag if there is an empty position
            //如果同一个tag的包已经在车厢里了，那么就不要放进来这个人的包:noSameBag
            if (UnityEngine.Random.Range(0f, 10f) < probability) //给npc bag 调高一点概率
            {
                Button bag = Instantiate(NPCBag, bagPos[firstEmptyPos],
                    Quaternion.identity) as Button;
                bag.GetComponent<ClothToMachine>().timer = 4 * stayTime + 3 * moveTime - timerStay;
                bag.GetComponent<ClothToMachine>().totalTime = 4 * stayTime + 3 * moveTime - timerStay;
                bag.GetComponent<ClothToMachine>().myStation = currentStation;
                string temp = "X";
                bag.gameObject.transform.tag = temp;
                
                //this position is occupied by a bag
                        bagPosAvailable[firstEmptyPos] = true;
                
                        //bagsInCar.Add(bag.gameObject);
                
                        bag.GetComponent<ClothToMachine>().myBagPosition = firstEmptyPos;
                        bagNum++;
                        bag.transform.SetParent(clothBagGroup.transform, false);
                        //NameToStationBags[currentStation].Add(bag);

                        bagCounts[currentStation]++;

                    BagsController.AddBagToCar(bag.gameObject, currentStation, "X");
                        
            }
        }
    }


    public IEnumerator trainPause()
    {
        float normalSpeed = 0.3f
;        FinalCameraController.CloseAllUI();
        FinalCameraController.enableScroll = false;
        Banner.SetActive(false);
        pauseBeforeMove = true;

        if(FinalCameraController.alreadyNotice)
        {
            BagsController.ClickReturnNo();
            
        }


        Show(FinalCameraController.disableInputCG);
        BlackScreen.SetActive(true);
        FinalCameraController.ChangeToSubway();
        FinalCameraController.CameraMovement.JumpToPage(4);

        yield return new WaitForSeconds(1f);
        BlackScreen.SetActive(false);

        


        if(roundNum > 0)
        {
            //黑屏结束之后
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(LostAndFound.AnimationDropNUm());
            AdsController.UpdatePosters();


            yield return new WaitForSeconds(1f);
            FinalCameraController.ChangeCameraSpeed(normalSpeed*0.5f);
            FinalCameraController.GotoPage(1);


            int rbn = 0;
            for (int i = 2; i >-1; i--)
            {
                yield return new WaitForSeconds(0.2f);
                rbn = BagsController.DropAllBagsInWasher(i);
            }

            //default speed
            //FinalCameraController.ChangeCameraSpeed(-1f);

            yield return new WaitForSeconds(1.5f);
            FinalCameraController.ChangeCameraSpeed(normalSpeed*3f);
            FinalCameraController.GotoPage(1);

            FinalCameraController.FishTalkAccessFromScript("bag"+bagNum.ToString());
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            if(currentStation > 0) StartCoroutine(LostAndFound.AnimationDropNUm());
            AdsController.UpdatePosters();

            if(!atInitailStation) yield return new WaitForSeconds(2f);
            FinalCameraController.ChangeCameraSpeed(normalSpeed);
            FinalCameraController.GotoPage(1);
        }



        if(roundNum > 0 && currentStation == 0)
        {
            yield return new WaitForSeconds(3f);
            InstagramController.ShowMatchResultFollower();
            StationForButton.DisplayMatchResult(roundNum);

        }
        else {
            yield return new WaitForSeconds(1.5f);
            EndTrainPause();

        }


 

    }

    public void EndTrainPause()
    {

        Hide(FinalCameraController.disableInputCG);
        FinalCameraController.enableScroll = true;
        timerPause = 0;
        timerStay = 0;
        //加起来的时间一定要小于timestay
        pauseBeforeMove = false;
        trainMove();
    }


    public void trainMove()
    {
        atInitailStation = false;
        LocalizedString locString = "Fish/DoYourJob";
        string translation = locString;
        FinalCameraController.fishTalkText.text = translation;

        if(currentStation < 2)
        {
            currentStation++;
        }
        else
        {
            roundNum++;
            currentStation = 0;
        }


        
        isMoving = true;
        bagFirst = true;

        CloseDoor();
    }





    public void trainStop()
    {
        isMoving = false;
        bagFirst = true;
        OpenDoor();
    }
    
    IEnumerator MyCoroutine(float time)
    {
        //This is a coroutine
        //train stays at a station for 30 seconds
        yield return new WaitForSeconds (time);
        currentStation++;

    }

    private void NumberRecalculate(float realTimer, TextMeshProUGUI text)
    {
        
        if (realTimer < 0)
        {
            realTimer = 0;
        }
        
        if (Mathf.RoundToInt(timer / 60) < 10)
        {
            if (Mathf.RoundToInt(realTimer % 60) < 10)
            {
                text.text = "0" + Mathf.RoundToInt(realTimer / 60).ToString() + ":" + "0" +
                                Mathf.RoundToInt(realTimer % 60).ToString();
            }
            else
            {
                text.text = "0" + Mathf.RoundToInt(realTimer / 60).ToString() + ":" +
                                Mathf.RoundToInt(realTimer % 60).ToString();
            }
        }
        else
        {
            if (Mathf.RoundToInt(realTimer % 60) < 10)
            {
                text.text = Mathf.RoundToInt(realTimer / 60).ToString() + ":" + "0" +
                                Mathf.RoundToInt(realTimer % 60).ToString();
            }
            else
            {
                text.text = Mathf.RoundToInt(realTimer / 60).ToString() + ":" +
                                Mathf.RoundToInt(realTimer % 60).ToString();
            }
        }
    }
    
    private string GenerateFishTalkForPause()
    {
        string[] strList = new string[] { "You did a great job!", "Pretty Good!", "Emmm... You need to work harder" ," "};

        string str = "!";
        int idx = bagNum;// bags under frount desk


       if(idx < strList.Length) str = strList[bagNum];

        
        

        return str;

    }


    public void Hide(CanvasGroup UIGroup) {
        UIGroup.alpha = 0f; //this makes everything transparent
        UIGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
        UIGroup.interactable = false;
    }
    
    public void Show(CanvasGroup UIGroup) {
        UIGroup.alpha = 1f;
        UIGroup.blocksRaycasts = true;
        UIGroup.interactable = true;
    }


    

}

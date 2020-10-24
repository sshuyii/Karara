using System;
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

    private AudioManager AudioManager;

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

        realTimer = (moveTime + stayTime) * 3 - timer;
        
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


        
    }


    private Button clothBag1;
    private Button clothBag2;
    private Button clothBag3;

    public float newTimer1;
    public float newTimer2;


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
    }
    // Update is called once per frame
    void Update()
    {
        // 转完两圈就terminate
        if (isTerminated) return;






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
        if (!FinalCameraController.isTutorial && LevelManager.clicktime > 6)
        {
            //instead of InvokeRepeating
            //newTimer1记录在站内应该停留多长时间
            if (!isMoving)
            {
                newTimer1 += Time.deltaTime;
            }
            if (newTimer1 > stayTime)
            {
                trainMove();
                newTimer1 = 0;
            }

            //如果停在一站+走完下一站，现在马上要进入新的一站了
            newTimer2 += Time.deltaTime;
            if (newTimer2 > stayTime + moveTime)
            {
                trainStop();
                newTimer2 = 0;
            }

            //train needs 1 minute on its way
            //this timer is specifically used for station0
            timer += Time.deltaTime;

            if (!FinalCameraController.isTutorial)
            {

                //start the timer once the train's in station
                if (!isMoving)
                {
                    stationTimer -= Time.deltaTime;
                    NumberRecalculate(stationTimer, CountDownTimer);
                }
                else
                {
                    CountDownTimer.text = "";
                    stationTimer = stayTime;
                }
            }

            //decide which station is highlighted on screen
            if (isMoving == false)
            {

                if (roundNum == 2)
                {
                    //第一章结束
                    FinalCameraController.ChapterOneEnd = true;
                    isTerminated = true;
                }


                //open doors

                if (left1.position.x > left1Pos - doorWidth)
                {
                    
                    if(doorSoundPlay)
                    {
                        //doorSound.Play();
                        //AudioManager.PlayAudio(AudioType.Subway_OpenDoor);
                        StartCoroutine(AudioManager.PlayAudioInStation(stayTime));
                        doorSoundPlay = false;
                    }        
                    
                    left1.position -= new Vector3(doorMovement, 0,0);
                    //print("opening left door");
                }
                else
                {
                    left1.position = new Vector3(left1Pos - doorWidth, left1.position.y,0);
                }

                if (right1.position.x < right1Pos + doorWidth)
                {
                    right1.position += new Vector3(doorMovement, 0,0);
                }
                else
                {
                    right1.position = new Vector3(right1Pos + doorWidth, right1.position.y,0);


                    //bagfirst保证只运行一次
                    if (bagFirst && !FinalCameraController.ChapterOneEnd)//结束的时候不能往车上放新的包了
                    {
                       
                        for (int i = 0; i < specialNPCPerStation; i++)
                        {

                            // 门完全开之后：
                            // 1.产生包
                            // 2.丢l&f衣服,发post(包含在3里)
                            // 3.更新posture(ad)
                            GenerateBag(currentStation);
                            if(!atInitailStation) LostAndFound.DropLostFoundClothes(currentStation);
                            AdsController.UpdatePosters();
                            
                            bagFirst = false;

                            isDoorOpen = true;

                        }
                    }


                }
            }//close doors
            //this actually happens when the train has started moving
            else
            {
                if(!doorSoundPlay)
                {
                    //doorSound.Play();
                    //AudioManager.PlayAudio(AudioType.Subway_CloseDoor);
                    StartCoroutine(AudioManager.PlayAudioOutStation(moveTime));
                    doorSoundPlay = true;
                }

                if (left1.position.x < left1Pos)
                {
                    left1.position += new Vector3(doorMovement, 0,0);
                }
                else
                {
                    left1.position = new Vector3(left1Pos, left1.position.y,0);
                }

//
                if (right1.position.x > right1Pos)
                {
                    right1.position -= new Vector3(doorMovement, 0,0);
                }
                else
                {
                    right1.position = new Vector3(right1Pos, right1.position.y,0);
                    isDoorOpen = false;
                }

            }
        }


        if (isDoorOpen && NoPosition)
        {
            GenerateBag(currentStation);
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

        if(bagNum < 3)
        {
            emptyBagPos();
            Button bag = Instantiate(NameToStationBags[stationNum][npcNum], bagPos[firstEmptyPos],
    Quaternion.identity) as Button;

            bag.GetComponent<ClothToMachine>().timer = 4 * stayTime + 3 * moveTime - newTimer1;
            bag.GetComponent<ClothToMachine>().totalTime = 4 * stayTime + 3 * moveTime - newTimer1;



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

        
        //如果现在车里不够三个包
        if (bagNum < 3)
        {
            emptyBagPos();
            //only generate a new bag if there is an empty position
            //如果同一个tag的包已经在车厢里了，那么就不要放进来这个人的包:noSameBag
            if (UnityEngine.Random.Range(0f, 10f) < probability) //给npc bag 调高一点概率
            {
                Button bag = Instantiate(NPCBag, bagPos[firstEmptyPos],
                    Quaternion.identity) as Button;
                bag.GetComponent<ClothToMachine>().timer = 4 * stayTime + 3 * moveTime - newTimer1;
                bag.GetComponent<ClothToMachine>().totalTime = 4 * stayTime + 3 * moveTime - newTimer1;
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
    }

    public void trainStop()
    {
        isMoving = false;
        bagFirst = true;
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

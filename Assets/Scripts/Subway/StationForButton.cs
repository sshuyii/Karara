using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StationForButton : MonoBehaviour
{
    public SubwayMovement SubwayMovement;

    // Start is called before the first frame update

    private bool isSpecialNPCTab = true;


    public bool ShowingProfile = false;

    [SerializeField]
    private Sprite normalTab;

    [SerializeField]
    private Sprite PressedTab;


    [SerializeField]
    private GameObject currentProfile;

    [SerializeField]
    private Animator profileAnimator;

    [SerializeField]
    private RectTransform profileExpand;


    [SerializeField]
    private List<GameObject> ProfileSelectors;

    [SerializeField]
    private GameObject ProfileSelector, tabsParentGO;



    private InstagramController InstagramController;
    private int stationNum;
    private int tabNum;
    private int activeSelectorIdx;
    [SerializeField]
    private string[] selectedProfiles = new string[] { "", "", "", "", "", "" };
    private bool isDetailed = false;



    public List<string> NPCNames = new List<string>();
    public List<Sprite> NPCAvatars = new List<Sprite>();
    private List<List<int>> matchedNPCIdx = new List<List<int>>();

    string NPCs = "XAB";

    FinalCameraController FinalCameraController;
    SpriteLoader SpriteLoader;
    BagsController BagsController;


    int currDetailedStation;


    [SerializeField]
    private GameObject collectionContent;

    [SerializeField]
    private GameObject collection, collection_stage2, collection_stage3;

    [SerializeField]
    private Image tabLogo;

    [SerializeField]
    private List<GameObject> tabsBG;

    [SerializeField]
    private int tabPerStation = 2;

    Dictionary<string, Sprite> bagLogoInCollection = new Dictionary<string, Sprite>();

    private bool EnteringSelection = false;
    public GameObject ButtonDown, ButtonUp, EnterSelectionButton;
    public Sprite beforeEnter, afterEnter;

    //index means each profile.
    private List<string> ProfileUsageState = new List<string>();

    [SerializeField]
    private GameObject greyedOut;

    //private List<List<>>


    [SerializeField]
    GameObject resultPref, resultsContent, results, matchGO;
    [SerializeField]
    Button confirmResult;
    [SerializeField]
    Text roundNum;

    public bool confirmed = false;


    public bool displayingBag, displayingProfile, exiting;

    LevelManager LevelManager;

    void Start()
    {
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        LevelManager = FinalCameraController.LevelManager;
        SpriteLoader = GameObject.Find("---SpriteLoader").GetComponent<SpriteLoader>();
        SubwayMovement = GameObject.Find("---StationController").GetComponent<SubwayMovement>();



        NPCNames.Add("???");
        Sprite defaultProfile = Resources.Load<Sprite>("Images/NPC/X/Profile/DefaultProfile");
        NPCAvatars.Add(defaultProfile);


        foreach (KeyValuePair<string, NPC> nameNPCPair in SpriteLoader.NPCDic)
        {
            //if (nameNPCPair.Key == "X"|| nameNPCPair.Key == "Karara") continue;
            if (nameNPCPair.Key == "Karara") continue;
            NPCNames.Add(nameNPCPair.Key);
            NPCAvatars.Add(nameNPCPair.Value.profile);
            Debug.Log("station for buttons" + nameNPCPair.Key);
            bagLogoInCollection.Add(nameNPCPair.Key, nameNPCPair.Value.bagLogo);
        }


        //三个站 每站4个tab

        matchedNPCIdx.Add(new List<int> { 0, 0, 0, 0 });
        matchedNPCIdx.Add(new List<int> { 0, 0, 0, 0 });
        matchedNPCIdx.Add(new List<int> { 0, 0, 0, 0 });

        // station buttons don't need tab num
        InstagramController = GameObject.Find("---InstagramController").GetComponent<InstagramController>();
        BagsController = GameObject.Find("---BagsController").GetComponent<BagsController>();

        for (int i = 0; i < NPCNames.Count; i++)
        {
            ProfileUsageState.Add("");
        }

        displayingBag = false;
        displayingProfile = false;
        exiting = false;


        UpdateCollectionUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (results.active && results.GetComponent<ScrollRect>().verticalNormalizedPosition < 0.3f)
        {
            confirmResult.interactable = true;
        }
    }

    public void UpdateCollectionUI()
    {
        if (LevelManager.stage == 1) collection = collection_stage2;
        if (LevelManager.stage == 2) collection = collection_stage3;


        foreach (Transform t in collection.transform.GetComponentInChildren<Transform>())
        {
            if (t.gameObject.name == "CollectionContent") collectionContent = t.gameObject;
        }


        tabsParentGO.transform.SetParent(collection.transform);
    }



    public void pressStationForButton0()
    {
        stationNum = 0;
        tabNum = 0;
        PressStationForButton();


    }


    public void pressStationForButton1()
    {
        stationNum = 1;
        tabNum = 0;
        PressStationForButton();
    }

    public void pressStationForButton2()
    {
        stationNum = 2;
        tabNum = 0;
        PressStationForButton();
    }

    public void PressStationForButton()
    {
        //if (LevelManager.isInstruction) return;
        if (LevelManager.stage == 1 && !LevelManager.stageTransiting) return;




        activeSelectorIdx = stationNum * tabPerStation;
        StationDetails();

        if (LevelManager.stage > 2 || (LevelManager.stage == 2 && LevelManager.isInstruction))
        {
            Debug.Log("LETS SEE THE TRANSITION!!!");
            showProfileSelectors();
            HideProfileWhenCloseStationDetail();
        }

    }


    public void CloseStationDetail()
    {
        //if (LevelManager.isInstruction) return;
        if (LevelManager.stage == 1 && !LevelManager.stageTransiting) return;
        PressStationForButton();
    }

    public void ClickTab0()
    {
        tabNum = 0;
        ClickTab();
    }

    public void ClickTab1()
    {
        tabNum = 1;
        ClickTab();
    }

    public void ClickTab2()
    {
        tabNum = 2;
        ClickTab();
    }

    public void ClickTab3()
    {
        tabNum = 3;
        ClickTab();
    }

    public void ClickTab()
    {
        //if (LevelManager.isInstruction) return;
        if (LevelManager.stage == 1 && !LevelManager.stageTransiting) return;
        // * tabPerStation

        Debug.Log("click tab");
        for (int i = 0; i < tabsBG.Count; i++)
        {
            tabsBG[i].GetComponent<Image>().sprite = normalTab;
        }
        tabsBG[tabNum].GetComponent<Image>().sprite = PressedTab;
        string currentBagOwner = BagsController.stationBagOwners[currDetailedStation][tabNum];
        tabLogo.sprite = SpriteLoader.NPCDic[currentBagOwner].closeBag;

        activeSelectorIdx = stationNum * tabPerStation + tabNum;

        // X 的包 不参与用户匹配
        if (LevelManager.stage > 2)
        {
            if (BagsController.stationBagOwners[stationNum][tabNum] != "X")
            {
                ProfileSelector.SetActive(true);
                showProfileSelectors();
                // if (ShowingProfile) refreshProfile();

            }
            else
            {

                // if (ShowingProfile) InstagramController.clearProfileContent();
                ProfileSelector.SetActive(false);
            }
        }


        ClearCollectionContent();
        DisplayCollectionContent(tabNum);
    }



    public void StationDetails()
    {

        //Debug.Log(" station detail");




        // if no bag created in the sation, do not show anything.


        if ((isDetailed) || exiting)
        {
            Debug.Log("EnterStationDetail_Tut_S3 wrong way");

         
            collection.SetActive(false);
            tabsParentGO.SetActive(false);
            isDetailed = false;
            //if (LevelManager.stage == 2 && LevelManager.isInstruction) return;


        }
        else
        {
            if (!isDetailed)
            {
                collection.SetActive(true);
                tabsParentGO.SetActive(true);
                isDetailed = true;
            }




            ClearCollectionContent();
            currDetailedStation = stationNum;
            List<string> bagOwners = BagsController.stationBagOwners[stationNum];


            if (bagOwners.Count > 0)
            {
                for (int i = 0; i < bagOwners.Count; i++)
                {
                    tabsBG[i].GetComponent<Image>().sprite = normalTab;
                }
                tabsBG[tabNum].GetComponent<Image>().sprite = PressedTab;

                string currentBagOwner = BagsController.stationBagOwners[currDetailedStation][tabNum];
                tabLogo.sprite = SpriteLoader.NPCDic[currentBagOwner].closeBag;
                DisplayCollectionContent(tabNum);
            }
            else
            {
                tabsParentGO.SetActive(false);
                
            }

        }

    }


    public void ClearCollectionContent()
    {
        foreach (Transform child in collectionContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void DisplayCollectionContent(int tabIdx)
    {
        displayingBag = true;
        int usedTabNum = SubwayMovement.bagCounts[currDetailedStation];
        for (int i = 0; i < usedTabNum; i++)
        {
            tabsBG[i].SetActive(true);


        }

        for (int i = usedTabNum; i < tabsBG.Count; i++)
        {
            tabsBG[i].SetActive(false);

        }


        if (SubwayMovement.bagCounts[stationNum] == 0) return;

        string currentBagOwner = BagsController.stationBagOwners[currDetailedStation][tabIdx];

        Debug.Log("现在tab 主人" + currentBagOwner);

        if (FinalCameraController.AllStationClothList.ContainsKey(currentBagOwner))
        {
            int washedClothNum = FinalCameraController.AllStationClothList[currentBagOwner].Count;
            for (int q = 0; q < washedClothNum; q++)
            {
                GameObject newObject = new GameObject("c");
                newObject.AddComponent<Image>();
                newObject.transform.parent = collectionContent.transform;
                newObject.GetComponent<Image>().sprite = FinalCameraController.AllStationClothList[currentBagOwner][q];
            }
        }


    }



    public void ProfileDetail()
    {
        if (ProfileSelector.transform.GetComponentInChildren<Text>().text == "???") return;

        if (ShowingProfile) HideProfile();
        else
        {
            string profileName = ProfileSelector.transform.GetComponentInChildren<Text>().text;
            ExpandProfile(profileName);
        }


        ShowingProfile = !ShowingProfile;


    }




    private void ExpandProfile(string name)
    {
        bool matchedCorrect = CheckMatchState(name);
        //if(matchedCorrect) return;

        //todo: tab变色
        displayingProfile = true;
        displayingBag = false;
        // ClearCollectionContent();
        InstagramController.displayProfileContent(name);
        FinalCameraController.MatchToSub();
    }




    private void HideProfile()
    {
        displayingProfile = false;
        InstagramController.clearProfileContent();
        DisplayCollectionContent(tabNum);
    }



    private void showProfileSelectors()
    {
        //   no bags for this station
        if (SubwayMovement.bagCounts[stationNum] == 0)
        {
            ProfileSelector.SetActive(false);
            return;
        }
        else ProfileSelector.SetActive(true);

        int idx = matchedNPCIdx[stationNum][tabNum];
        string thisTabOwner = BagsController.stationBagOwners[stationNum][tabNum];


        if (ProfileUsageState[idx] != "" && ProfileUsageState[idx] != thisTabOwner) greyedOut.SetActive(true);
        else greyedOut.SetActive(false);

        ProfileSelector.transform.GetComponent<Image>().sprite = NPCAvatars[idx];
        ProfileSelector.transform.GetComponentInChildren<Text>().text = NPCNames[idx];
    }



    private void refreshProfile()
    {
        InstagramController.clearProfileContent();
        string profileName = ProfileSelector.transform.GetComponentInChildren<Text>().text;
        InstagramController.displayProfileContent(profileName);
    }


    public void NextProfile()
    {

        matchedNPCIdx[stationNum][tabNum]++;
        if (matchedNPCIdx[stationNum][tabNum] == NPCNames.Count)
        {
            matchedNPCIdx[stationNum][tabNum]--;
        }



        showProfileSelectors();
        if (ShowingProfile) refreshProfile();
    }

    public void LastProfile()
    {

        matchedNPCIdx[stationNum][tabNum]--;
        if (matchedNPCIdx[stationNum][tabNum] < 0)
        {
            matchedNPCIdx[stationNum][tabNum] = 0;
        }

        showProfileSelectors();
        if (ShowingProfile) refreshProfile();
    }


    private void HideProfileWhenCloseStationDetail()
    {

        if (isDetailed == false)
        {
            ShowingProfile = false;
            InstagramController.clearProfileContent();

        }
    }

    private void LoadPics(List<Sprite> sprites, string path)
    {
        object[] loadedPics = Resources.LoadAll(path, typeof(Sprite));
        for (int i = 0; i < loadedPics.Length; i++)
        {
            sprites.Add((Sprite)loadedPics[i]);
        }
    }




    public void EnterSelection()
    {
        EnteringSelection = !EnteringSelection;
        if (EnteringSelection)

        {
            if (LevelManager.stage == 2 && LevelManager.isInstruction)
            {
                StartCoroutine(LevelManager.EnterMatch_Tut_S3());
            }
            int idx = matchedNPCIdx[stationNum][tabNum];
            ProfileUsageState[idx] = "";
            EnterSelectionButton.GetComponent<Image>().sprite = afterEnter;
            ButtonDown.SetActive(true);
            ButtonUp.SetActive(true);
        }
        else
        {
            //确定选择
            if (LevelManager.stage == 2 && LevelManager.isInstruction)
            {
                LevelManager.EndMatch_Tut_S3();
            }
            int idx = matchedNPCIdx[stationNum][tabNum];
            if (ProfileUsageState[idx] != "")
            {
                EnteringSelection = !EnteringSelection;

            }
            else
            {
                ProfileUsageState[idx] = BagsController.stationBagOwners[stationNum][tabNum];
                EnterSelectionButton.GetComponent<Image>().sprite = beforeEnter;
                ButtonDown.SetActive(false);
                ButtonUp.SetActive(false);
            }

        }


    }


    private bool CheckMatchState(string currentProfile)
    {
        int idx = matchedNPCIdx[stationNum][tabNum];
        string TrueOwner = NPCNames[idx];
        Debug.Log("True owner: " + TrueOwner + " current Profile: " + currentProfile);
        return (TrueOwner == currentProfile);
    }
    private void GetMatchResult()
    {
        //clear content first
        results.SetActive(true);
        foreach (Transform child in resultsContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }


        for (int i = 0; i < ProfileUsageState.Count; i++)
        {
            string WhoUseThisProfile = ProfileUsageState[i];
            string TrueOwner = NPCNames[i];

            if (WhoUseThisProfile != "" && WhoUseThisProfile == TrueOwner)
            {

                GameObject match = Instantiate(resultPref, resultsContent.transform);
                Sprite bag = SpriteLoader.NPCDic[TrueOwner].closeBag;
                match.transform.Find("bag").gameObject.GetComponent<Image>().sprite = bag;
                Sprite profile = NPCAvatars[i];
                match.transform.Find("profile").gameObject.GetComponent<Image>().sprite = profile;
            }
        }

        results.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;

        Debug.Log("get matched result");

    }


    public void DisplayMatchResult(int rNum)
    {
        confirmed = false;
        confirmResult.interactable = false;
        GetMatchResult();
        roundNum.text = rNum.ToString();
        matchGO.SetActive(true);


    }

    public void ConfirmMatchResult()
    {
        confirmed = true;
        matchGO.SetActive(false);
        SubwayMovement.EndTrainPause();
    }



    public void ExitMap()
    {
        exiting = true;
        StationDetails();
        HideProfileWhenCloseStationDetail();
        exiting = false;
    }

    //public void ProfileDetail_old()
    //{
    //    if (ShowingProfile) StartCoroutine(HideProfile_old());
    //    else
    //    {
    //        string profileName = ProfileSelectors[activeSelectorIdx].GetComponent<Image>().sprite.name;
    //        StartCoroutine(ExpandProfile_old(profileName));
    //    }


    //    ShowingProfile = !ShowingProfile;


    //}



    //IEnumerator ExpandProfile_old(string name)
    //{
    //    //Color black = new Color(0.2f, 0.2f, 0.2f);
    //    //tab0.color = black;
    //    //tab1.color = black;
    //    //SubwayMovement.tabImg0.color = black;
    //    //SubwayMovement.tabImg1.color = black;

    //    SubwayMovement.clearCollectionContent();



    //    profileAnimator.SetTrigger("expand");
    //    yield return new WaitForSeconds(0.9f);


    //    profileAnimator.SetTrigger("FinishExpand");
    //    profileExpand.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

    //    yield return new WaitForSeconds(0.3f);
    //    InstagramController.displayProfileContent(name);

    //}

    //IEnumerator HideProfile_old()
    //{

    //    //Color white = new Color(1f, 1f, 1f);
    //    //SubwayMovement.tabImg0.color = white;
    //    //SubwayMovement.tabImg1.color = white;


    //    InstagramController.clearProfileContent();

    //    profileAnimator.SetTrigger("shrink");
    //    yield return new WaitForSeconds(0.9f);

    //    profileAnimator.SetTrigger("FinishShrink");
    //    profileExpand.rotation = Quaternion.Euler(new Vector3(0, 0, 0));


    //    if (isSpecialNPCTab) specialNPCTabClicked();
    //    else NPCTabClicked();

    //}


    //public void DropdownValueChanged(Dropdown change)
    //{

    //    selectedProfiles[activeSelectorIdx] = change.itemText.text;
    //    if (ShowingProfile) refreshProfile();

    //}


    //private void showProfileSelectors_old()
    //{
    //    foreach(GameObject PSelector in ProfileSelectors) {
    //        PSelector.SetActive(false);

    //    }

    //    ProfileSelectors[activeSelectorIdx].SetActive(true);

    //}




    //private void refreshProfile_old() {


    //     InstagramController.clearProfileContent();
    //     string profileName = ProfileSelectors[activeSelectorIdx].GetComponent<Image>().sprite.name;
    //     InstagramController.displayProfileContent(profileName);

    //}






}


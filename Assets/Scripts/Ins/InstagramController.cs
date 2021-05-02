using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Button = UnityEngine.UI.Button;


public class InstagramController : MonoBehaviour
{
    public List<GameObject> postList = new List<GameObject>();

    //public CalculateInventory CalculateInventory;
    public FinalCameraController FinalCameraController;
    private AdsController AdsController;

    private SubwayMovement SubwayMovement;
    
    public int currentPostNum;

    public SortedDictionary<GameObject, int> allPostsDict = new SortedDictionary<GameObject, int>();

    public Sprite transparent;
    public Image gotoImage;
    public Image gotoProfile;
    public GameObject gotoText;
    public Text postText;
    public GameObject postReplyParent;

    public GameObject redDot;
   
    public GameObject postPrefab;
    public GameObject postParent;
    
    public GameObject replyPrefab;
    public Transform replyParent;


    public CanvasGroup FishBoss;
    public GameObject commentPrefab;



    public Image NPCProfile;
    public Sprite likeFUll;
    //change post in karara's page
    public GameObject filmParent;
    public GameObject photoPostPrefab;
    
    //reply list
    public List<GameObject> replyList = new List<GameObject>();
    
    //take picture page gameobjects
    public GameObject PosturePostPrefab;
    public GameObject PosturePostPrefabNew;

    public GameObject originalPosture;
    public SpriteRenderer[] PosturePostImageList;
    
    
    
    public bool replyChosen = false;

    public Sprite unfollow;

    public bool followNico;
    public bool followDesigner;

    public List<string> usedAdsList = new List<string>();


    public TextMeshProUGUI followerNum_UI;


    public TextMeshProUGUI fishText;



    [SerializeField]
    private GameObject profileContent;


    private int countTakenPhotos = 0;
    public List<Sprite> AllTakenPhotos = new List<Sprite>();
    //点一下怎么确定是哪个哦？destory




    [SerializeField]
    private Image PicInSavePage;



    SpriteLoader SpriteLoader;

    public int newPostNum;
    public int newStationNum;

    public int takenNum;


    private int endStationPre;

    public int fansNum = 3;
    public int targetFansNum = 30;

    [SerializeField]
    private Text followerNumMatchUI, targetFollowerNumMatchUI;
    [SerializeField]
    private Image progress;
    


    [SerializeField]
    private GameObject albumContent, savedPhotoPrefab,savedPhotoLarge, firstPost;
    


    private GameObject currentLargePhoto;

    public bool waitingForRefresh;
    private string storedName;
    private int storedRating;

    private AudioManager AudioManager;

    [SerializeField]
    private Button sendButton;

    [SerializeField]
    private Image Explanation;
    [SerializeField]
    private Sprite repeatBG, repeatPosture;


    // Start is called before the first frame update
    LevelManager LevelManager;
    void Start()
    {



        //get the tutorial post into postList
        firstPost = postParent.transform.GetChild(0).gameObject;
        
        
        
        redDot.SetActive(false);
        SubwayMovement = GameObject.Find("---StationController").GetComponent<SubwayMovement>();
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        SpriteLoader = GameObject.Find("---SpriteLoader").GetComponent<SpriteLoader>();
        AdsController = GameObject.Find("---AdsController").GetComponent<AdsController>();
        AudioManager = GameObject.Find("---AudioManager").GetComponent<AudioManager>();
        LevelManager = GameObject.Find("---LevelManager").GetComponent<LevelManager>();

        PosturePostImageList = originalPosture.GetComponentsInChildren<SpriteRenderer>();


    }

   
    // Update is called once per frame
    void Update()
    {
       


    }

    
  



    public void clearProfileContent()
    {
        foreach (Transform child in profileContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void ClickProfilePhoto(string name)
    {
        Debug.Log("Click Profile Photo "+name);
        FinalCameraController.MatchToSub();
        clearProfileContent();
        displayProfileContent(name);
    }


    public void displayProfileContent(string name)
    {
        
        //todo: 这里暂时用的只有两个人的post
        
         NPC thisNPC = SpriteLoader.NPCDic[name];
        List<Sprite> selectedList = thisNPC.shownPosts;


        foreach (Sprite sprt in selectedList)
        {

            GameObject newPost = Instantiate(PosturePostPrefabNew, profileContent.transform);
        
            var profile = newPost.transform.Find("ProfilePhoto").gameObject;
            var profileName = newPost.transform.Find("ProfileName").gameObject.GetComponent<TextMeshProUGUI>();
        
            profile.GetComponent<Image>().sprite = thisNPC.profile;
            profileName.text = thisNPC.name;
            newPost.transform.Find("Post").gameObject.GetComponent<Image>().sprite = sprt;

            newPost.transform.SetAsFirstSibling();
            Debug.Log("shown photot: " + sprt);

            // GameObject newObject = new GameObject("c");
            // newObject.AddComponent<Image>();
            // newObject.transform.parent = profileContent.transform;
            // newObject.GetComponent<Image>().sprite = sprt;
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




    public void AddInsPost(string NPCName, Sprite postImg)
    {
        GameObject newPost = Instantiate(PosturePostPrefabNew, postParent.transform);
        
        NPC thisNPC = SpriteLoader.NPCDic[NPCName];
        var profile = newPost.transform.Find("ProfilePhoto").gameObject;
        var profileName = newPost.transform.Find("ProfileName").gameObject.GetComponent<TextMeshProUGUI>();
        profile.GetComponent<Button>().onClick.AddListener( () => ClickProfilePhoto(NPCName));
        profile.GetComponent<Image>().sprite = thisNPC.profile;
        profileName.text = thisNPC.name;
        newPost.transform.Find("Post").gameObject.GetComponent<Image>().sprite = postImg;

        newPost.transform.SetAsFirstSibling();
        
        if(NPCName == "Karara") thisNPC.Add2KararaPostList(postImg);
    }

    private Texture2D texture2Save;
    private int savedPhotoCount = 0;
    [Tooltip("Remember to turn it to TRUE before run on iPhone")]
    public bool RunOnRealPhone = false;
    public void SavePhoto()
    {
        
        if(RunOnRealPhone) NativeToolkit.SaveImage(texture2Save,"KARARA" + savedPhotoCount.ToString());
        else Debug.Log("save image KARARA" + savedPhotoCount.ToString());
        savedPhotoCount++;
    }

   
    
    public void SetSavePage(Sprite photo, Texture2D texture)
    {
        PicInSavePage.sprite = photo;
        texture2Save = texture;
    }



    public void SendPhoto()
    {
        //AddPost
        AllTakenPhotos.Add(PicInSavePage.sprite);
        countTakenPhotos++;
        AddInsPost("Karara", PicInSavePage.sprite);
        AddFollower();
        AdsController.UseAdAndPose();

        
        FinalCameraController.ChangeToApp();

    }

    public void AddFollower()
    {
        // todo: make a judgement about attributes of cloth and ad attributes

        fansNum += AdsController.GetNewFollowerNum();

        Debug.Log("总粉丝 " + fansNum);

        followerNum_UI.SetText(fansNum.ToString());


        Debug.Log("总粉丝 " + followerNum_UI.text);
    }

    public void ShowMatchResultFollower()
    {
        progress.fillAmount = (float)fansNum / (float)targetFansNum;
        followerNumMatchUI.text = fansNum.ToString();
        targetFollowerNumMatchUI.text = targetFansNum.ToString();
    }


    public void RefreshPost(string maxOwner, int rating)
    {
        if(LevelManager.stage <= 2) return;
        storedName = maxOwner;
        storedRating = rating;

        if (FinalCameraController.myCameraState == FinalCameraController.CameraState.App)
        {
            waitingForRefresh = true;
            return;
        }


        RefreshPost();
        //暂时默认：不会一直等到两次经过车站才从app退出

    }


    public void RefreshPost()
    {
        if(LevelManager.stage <= 2) return;
        redDot.SetActive(true);
        waitingForRefresh = false;

        AudioManager.PlayAudio(AudioType.App_Ring);

        foreach (KeyValuePair<string, NPC> pair in SpriteLoader.NPCDic)
        {
            if (pair.Key == "Karara" || pair.Key == "X") continue;
            if (pair.Key != storedName)
            {
                AddInsPost(pair.Key, pair.Value.PostPic());
            }
            else
            {
                if (UnityEngine.Random.Range(0f, 5f) < storedRating)
                {
                    AddInsPost(pair.Key, pair.Value.PostPic());
                }

            }
        }

    }


   



    public void CheckOnePicture()
    {
        currentLargePhoto = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.gameObject as GameObject;
        Sprite thisButtonImg = currentLargePhoto.GetComponentInChildren<Image>().sprite;
        savedPhotoLarge.GetComponent<Image>().sprite = thisButtonImg;
        FinalCameraController.AlbumToLargePicture();

    }


    public void DeleteThisPhoto()
    {
        //记住这是谁
        Destroy(currentLargePhoto);
        FinalCameraController.LargePictureToAlbum();
    }


    public void RepeatPostureOrBG(int condition )
    {
        
        if (condition == 1)
        {
            Explanation.sprite = repeatPosture;
            //sendButton.interactable = false;

        }
        else if (condition ==2)
        {
            Explanation.sprite = repeatBG;
            //sendButton.interactable = false;
        }
        else
        {
            sendButton.interactable = true;
            Explanation.sprite = transparent;
        }

        if(condition == 1 || condition == 2) Unfollow();



    }

    private void Unfollow()
    {
        //random number
        int randomNum = Random.Range(3, 8);
        fansNum -= randomNum;
        followerNum_UI.SetText(fansNum.ToString());
    }

}

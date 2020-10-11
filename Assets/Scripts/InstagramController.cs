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
    private FinalCameraController FinalCameraController;
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

    int fansNum = 3;

    [SerializeField]
    private GameObject albumContent, savedPhotoPrefab,savedPhotoLarge, firstPost;



    private GameObject currentLargePhoto;



    // Start is called before the first frame update
    void Start()
    {



        //get the tutorial post into postList
        firstPost = postParent.transform.GetChild(0).gameObject;
        
        
        
        redDot.SetActive(false);
        SubwayMovement = GameObject.Find("---StationController").GetComponent<SubwayMovement>();
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        SpriteLoader = GameObject.Find("---SpriteLoader").GetComponent<SpriteLoader>();
        AdsController = GameObject.Find("---AdsController").GetComponent<AdsController>();



        PosturePostImageList = originalPosture.GetComponentsInChildren<SpriteRenderer>();


    }

   
    // Update is called once per frame
    void Update()
    {
       
        
        //enable new posters for chapter one
        //when player is not in subway4
        if (!FinalCameraController.isTutorial)
        {

            

        }

    }

    
  



    public void clearProfileContent()
    {
        foreach (Transform child in profileContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }


    public void displayProfileContent(string name)
    {
        
        //todo: 这里暂时用的只有两个人的post
        List<Sprite> selectedList = SpriteLoader.NPCDic[name].shownPosts;

        foreach (Sprite sprt in selectedList)
        {
            GameObject newObject = new GameObject("c");
            newObject.AddComponent<Image>();
            newObject.transform.parent = profileContent.transform;
            newObject.GetComponent<Image>().sprite = sprt;
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
        GameObject newPost = Instantiate(PosturePostPrefabNew);
        //set parent(probably a better way to do
        newPost.transform.parent = postParent.transform;
        newPost.transform.localScale = new Vector3(140,140,1);
 

        NPC thisNPC = SpriteLoader.NPCDic[NPCName];
        var profile = newPost.transform.Find("Profile").gameObject.GetComponent<Image>();
        var profileName = profile.gameObject.GetComponentInChildren<Text>();
        profile.sprite = thisNPC.profile;
        profileName.text = thisNPC.name;
        newPost.transform.Find("Post").gameObject.GetComponent<Image>().sprite = postImg;
        //move to the first of the list
        //postList.Insert(0, newPost);

        newPost.transform.SetAsFirstSibling();
        
    }


    public void SavePhoto()
    {

        

        GameObject newObject = Instantiate(savedPhotoPrefab);
        newObject.GetComponent<Image>().sprite = PicInSavePage.sprite;
        newObject.transform.parent = albumContent.transform;
        newObject.transform.localScale = new Vector3(5, 5, 0);
        newObject.GetComponent<Button>().onClick.AddListener(CheckOnePicture);
        newObject.transform.name = countTakenPhotos.ToString();
        Debug.Log("这个的名字" + newObject.transform.name);

        AllTakenPhotos.Add(PicInSavePage.sprite);
        countTakenPhotos++;

        FinalCameraController.ChangeToApp();
        FinalCameraController.MainPageToAlbum();
    }


    public void SetSavePage(Sprite photo)
    {
        PicInSavePage.sprite = photo;
    }



    public void SendPhoto()
    {
        //AddPost
        AllTakenPhotos.Add(PicInSavePage.sprite);
        countTakenPhotos++;
        AddInsPost("Karara", AllTakenPhotos[AllTakenPhotos.Count - 1]);
        AddFollower();
        FinalCameraController.ChangeToApp();
        
    }

    public void AddFollower()
    {
        // todo: make a judgement about attributes of cloth and ad attributes

        fansNum += AdsController.GetNewFollowerNum();

        Debug.Log("总粉丝 " + fansNum);
        //fansNum += 3;
        followerNum_UI.SetText(fansNum.ToString());
        Debug.Log("总粉丝 " + followerNum_UI.text);
    }



    public void RefreshPost(string maxOwner, int rating)
    {
        //if (maxOwner == "") return;
        foreach(KeyValuePair<string, NPC> pair in SpriteLoader.NPCDic)
        {
            if (pair.Key == "Karara" || pair.Key == "X") continue;
            if(pair.Key != maxOwner)
            {
                AddInsPost(pair.Key, pair.Value.PostPic());
            }
            else
            {
                if(UnityEngine.Random.Range(0f, 5f) < rating)
                {
                    AddInsPost(pair.Key, pair.Value.PostPic());
                }

            }
                

        }
    }


    public void CheckOnePicture()
    {
        currentLargePhoto = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject as GameObject;
        Sprite thisButtonImg = currentLargePhoto.GetComponent<Image>().sprite;
        savedPhotoLarge.GetComponent<Image>().sprite = thisButtonImg;
        FinalCameraController.AlbumToLargePicture();

    }


    public void DeleteThisPhoto()
    {
        //记住这是谁
        Destroy(currentLargePhoto);
        FinalCameraController.LargePictureToAlbum();
    }
}

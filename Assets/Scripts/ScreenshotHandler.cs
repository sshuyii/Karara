using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class ScreenshotHandler : MonoBehaviour
{
    //a list of all the posts
    
    public InstagramController InstagramController;
    private FinalCameraController FinalCameraController;


    private AudioSource shutterSound;

    private int PosterTakenNum = 0;//record how many posters have been used?
    //record which posture has been used
    private Dictionary<string, bool> usedPostures = new Dictionary<string, bool>();

    //record what Karara is wearing
    private string KararaTop;
    private string KararaBottom;
    private string KararaShoe;
    private bool KararaWork;

    private bool startFlash;
    public Image KararaTopImage;
    public Image KararaBottomImage;
    public Image KararaShoeImage;
    public Image KararaWorkImage;

    

    private GameObject toothpastePost;
    public int followerNum;
    public TextMeshProUGUI subwayFollower;
    
    private static ScreenshotHandler instance;

    public Camera myCamera;
    private bool takeScreenshotOnNextFrame;

    private Image postImage;
   
    private string ScreenCapDirectory;

    private Texture2D myScreenshot;

    private Texture2D renderResult;

    private IEnumerator coroutine;

    public Image photoBackground;
    
    private int width = Screen.width;
    private int height = Screen.height;


    public CanvasGroup Notice;
    public CanvasGroup myFlash;
    private bool flash;
    private RatingSystem RatingSys;

    System.Random random = new System.Random();


    AdsController AdsController;

    private AudioManager AudioManager;

    private bool isTaken;
    public bool notAllowSend;
    
    void Start()
    {
        //height = width;
        instance = this;
        //myCamera = gameObject.GetComponent<Camera>();
        shutterSound = GetComponent<AudioSource>();

        AudioManager = GameObject.Find("---AudioManager").GetComponent<AudioManager>();
        ScreenCapDirectory = Application.persistentDataPath;
        FinalCameraController = GetComponent<FinalCameraController>();
        //CalculateInventory = GameObject.Find("---InventoryController").GetComponent<CalculateInventory>();
        RatingSys = GameObject.Find("FloatingUI").GetComponent<RatingSystem>();

        if(FinalCameraController != null) AdsController = GameObject.Find("---AdsController").GetComponent<AdsController>();


        //        postImage = GetComponent<Image>();
        //        //print(postImage.name);

        if (!FinalCameraController.isTutorial)
        {
            //KararaTop = KararaTopImage.sprite.name;
            //KararaBottom = KararaBottomImage.sprite.name;
            //KararaShoe = KararaShoeImage.sprite.name;
        }

        
    }

    
    private void Update()
    {

        //            //print(InstagramController.AdAlreadyTakenList[InstagramController.currentBackground]);



        //set follower number
        //todo: 应该在换衣服按钮那里决定是不是穿着工作服，而不是在这里每一帧都判断一次
        if (!FinalCameraController.isTutorial)
        {
            //InstagramController.followerNum_UI.text = followerNum.ToString();


            //todo: yun

            KararaWork = true;


            if (flash)
            {
                if (FinalCameraController.isTutorial == false)
                {
                    myFlash.alpha = myFlash.alpha - Time.deltaTime;

                    if (myFlash.alpha <= 0)
                    {
                        myFlash.alpha = 0;
                        flash = false;
                        isTaken = false;//闪光灯结束了的话就可以再照相了
                    }
                }
                else
                {
                    FinalCameraController.TutorialManager.pressScreenshot = true;
                    flash = false;
                    isTaken = false;
                }
            }
        }
    }

   



    public void TakeScreenShotSprite()
    {
        
        //instantiate new post object     
        toothpastePost = Instantiate(InstagramController.PosturePostPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        //set parent(probably a better way to do
        toothpastePost.transform.parent = InstagramController.postParent.transform;
            
        //move to the first of the list
        InstagramController.postList.Insert(0,toothpastePost);

        //get the post child
        SpriteRenderer[] postImageList = toothpastePost.transform.Find("PostFolder").gameObject.GetComponentsInChildren<SpriteRenderer>();
        
        //get the background
        for (int i = 0; i < postImageList.Length; i++)
        {
            postImageList[i].sprite = InstagramController.PosturePostImageList[i].sprite;
        }
        
        //re-arrange children object, so the latest is displayed as the first
        for (int i = 0; i < InstagramController.postList.Count; i++)
        {
            InstagramController.postList[i].transform.SetSiblingIndex(i);
        }
        
        StartCoroutine(ExampleCoroutine());

    }




    
    IEnumerator ExampleCoroutine()
    {
        
        yield return new WaitForSeconds(0.5f);

        //in tutorial
        if (FinalCameraController.isTutorial)
        {
            flash = true;
            
            myFlash.alpha = 1;

            yield return new WaitForSeconds(0.1f);
        }
        else if (!usedPostures.ContainsKey(AdsController.currentPoseIdx.ToString()))//if the posture is never used
        {
            flash = true;
            myFlash.alpha = 1;
            yield return new WaitForSeconds(0.1f);

        }



        BeforeJumpToPage();
        FinalCameraController.ChangeToSavingPage();
        
        
    }

    private void BeforeJumpToPage()
    {
        Texture2D sprites = CropImage();
        Rect rec = new Rect(0, 0, sprites.width, sprites.height);
        Sprite thisPhoto = Sprite.Create(sprites, rec, new Vector2(0, 0), 100f);
        InstagramController.SetSavePage(thisPhoto);


    }


    private void OnPostRender()
    {
        if (takeScreenshotOnNextFrame)
        {
            takeScreenshotOnNextFrame = false;        
            
            RenderTexture renderTexture = myCamera.targetTexture;


            renderResult =
                new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/Resources/Screenshots/CameraScreenshot"+InstagramController.takenNum+".png", byteArray);

            renderResult.Apply();
            RenderTexture.ReleaseTemporary(renderTexture);
            myCamera.targetTexture = null;
        }
    }

 
    
    private void TakeScreenshot(int width, int height)
    {
        Debug.Log("width:" + width.ToString());   
        myCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenshotOnNextFrame = true;
    }


    
    public void TakeScreenshot()
    {
        if (isTaken)
        {
            return;
        }

        isTaken = true;
        //shutterSound.Play();
        AudioManager.PlayAudio(AudioType.Photo_Shutter);


        //hide notice bubble
        if(!FinalCameraController.isTutorial)
        {
            FinalCameraController.Hide(Notice);
        }

        //if the background is already used
        InstagramController.sendButton.interactable = true;
        if (!AdsController.IsThisAdOk())
        {
            InstagramController.sendButton.interactable = false;
            //FinalCameraController.Show(Notice);
            //Notice.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "no same background";
            //isTaken = false;
            //return;
        }

        if (!AdsController.IsThisPoseOk() && !FinalCameraController.isTutorial)
        {
            InstagramController.sendButton.interactable = false;
            //FinalCameraController.Show(Notice);
            //Notice.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "no same pose";
            //isTaken = false;
            //return;
        }



        TakeScreenshot(width, height);
        


        InstagramController.takenNum++;

        FinalCameraController.entryTime += 1;

        StartCoroutine(ExampleCoroutine());

      
        //re-arrange children object, so the latest is displayed as the first
        for (int i = 0; i < InstagramController.postList.Count; i++)
        {
            InstagramController.postList[i].transform.SetSiblingIndex(i);
        }


        //AdsController.UseAdAndPose();
        
           

    }


    public void ClickNotice()
    {
        FinalCameraController.Hide(Notice);

    }



    //todo: crop image not by perfect pixels, but relative to the screen size
    Texture2D CropImage()
    {
        Texture2D tex = new Texture2D(width, width, TextureFormat.RGB24, false);
        
        
        //Height of image in pixels
        for (int y = 0; y < tex.height; y++)
        {
            //Width of image in pixels
            for (int x = 0; x < tex.width; x++)
            {
                Color cPixelColour = renderResult.GetPixel(x ,  y + height/2 - Mathf.FloorToInt(0.5f * width));
                tex.SetPixel(x, y, cPixelColour);
            }
        }
        tex.Apply();
        return tex;
    }


 
  
  
}

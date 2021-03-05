using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using I2.Loc;



public class LevelManager : MonoBehaviour
{

    [SerializeField]
    private GameObject MapHint,MapTutorialBubble,GoBackButton, MapTutorialBag;

    [SerializeField]
    private CanvasGroup Map;





    //    public CanvasGroup startScreen;
    public CanvasGroup chapterOne;
    public GameObject hintArrow;
    public CanvasGroup GoBackSubway;
    //private CanvasGroup hintArrowCG;
    //public CanvasGroup clear;
    public CanvasGroup subwayCG;
    public CanvasGroup Stations;
    public GameObject car;
    private CanvasGroup carCG;
    private PathFollower PathFollower;
    
    
    public GameObject station;
    public GameObject bagIn;
    public Sprite bagOut;
    private FinalCameraController FinalCameraController;

    public CanvasGroup arrowButton;

    public GameObject StationDetail;
    public TextMeshProUGUI instructionText;

    public bool isInstruction;
    private Vector2 stationV = new Vector2(-50, 80);

    public GameObject InstructionScroll, closeInstructionButton;
    //[SerializeField]
    //private Button closeInstructionButton;
    public bool skip;
    public GameObject fishBubble;
    public TextMeshPro fishText;

    private SubwayMovement SubwayMovement;
    private RatingSystem RatingSystem;
    // Start is called before the first frame update

    private bool HintShowing = false;
    private bool Practicing = false;
    public bool fakeMatch = false;

    public int stage = 1;
    FishBossNotification FishBossNotification;


    void Start()
    {
        Resources.UnloadUnusedAssets();

        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        SubwayMovement = GameObject.Find("---StationController").GetComponent<SubwayMovement>();
        RatingSystem = GameObject.Find("FloatingUI").GetComponent<RatingSystem>();
        FishBossNotification = GameObject.Find("FishBossUI").GetComponent<FishBossNotification>();



        PathFollower = car.GetComponent<PathFollower>();
        carCG = car.GetComponent<CanvasGroup>();

        int skipInstruction = PlayerPrefs.GetInt("skip",-1);
        if(skipInstruction == 1) skip = true;

        GameObject.Find("Main Camera").transform.position = new Vector3(0, 0, -20);

        if (!skip)
        {
            isInstruction = true;
            ShowRatingSys(true);
            GoBackButton.SetActive(false);
            FinalCameraController.myCameraState = FinalCameraController.CameraState.Map;
           
        }
        else
        {
            //FinalCameraController.myCameraState = FinalCameraController.CameraState.Subway;
            //ShowRatingSys(false);
            //CloseInstruction();
            EndMapTutorial();
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        //只有滚到底的时候才会显示关闭按钮
        if(isInstruction&& InstructionScroll.active &&
            InstructionScroll.GetComponent<ScrollRect>().verticalNormalizedPosition < 0.3f)
        {
            closeInstructionButton.GetComponent<Button>().interactable = true;
        }

        if (Practicing) CheckMatch();

        
    }

    private void CheckMatch()
    {
        //todo: 
        if (fakeMatch)
        {
            Practicing = false;
            
        }
    }

    public void ShowHint()
    {
        HintShowing = true;
        MapHint.SetActive(true);
    }

    public void HideHint()
    {
        HintShowing = false;
        MapHint.SetActive(false);
    }

    public void HideBubble()
    {
        MapTutorialBubble.SetActive(false);
    }

    public void ShowInstructionInMap()
    {
        //quick move 之后

        if (!isInstruction) return;
        StationDetail.SetActive(true);

        MapTutorialBag.SetActive(false);
        HideHint();
        HideBubble();

        Practicing = true;


        Debug.Log("show instruction in map");
    }



    private void ShowRatingSys(bool withParticleEffect)
    {
        RatingSystem.ShowRatingSys(withParticleEffect);

        
    }

    // 如果是chap 1 开屏在地铁内显示图片instruction 关闭时用这个
    public void CloseInstruction()
    {
        //closeInstructionButton.SetActive(false);
        //isInstruction = false;
        //FinalCameraController.InstructionDismiss();
        //StartCoroutine(AnimateFishText(fishText, "See the bags? Time for work!", false, null, Vector2.zero));

        //SubwayMovement.trainStop();
        //SubwayMovement.timerStay = SubwayMovement.stayTime - 9.9f;
        //FinalCameraController.enableScroll = true;


        //SubwayMovement.trainStop();
        //SubwayMovement.timerStay = SubwayMovement.stayTime - 0.01f;

        if (Practicing) return;
        StationDetail.SetActive(false);
        GoBackButton.SetActive(true);
        MapHint.gameObject.transform.parent = GoBackButton.transform;
        MapHint.gameObject.transform.localPosition = new Vector3(25.5f, 0, 0);
        ShowHint();

    }

    public void EndMapTutorial()
    {
        if(isInstruction)
        {
            isInstruction = false;
            Hide(Map);
            HideHint();
            
        }

        FinalCameraController.myCameraState = FinalCameraController.CameraState.Subway;
        SubwayMovement.trainStop();
        //SubwayMovement.timerStay = SubwayMovement.stayTime - 0.01f;
    }



    IEnumerator ShowInstruction()
    {
        FinalCameraController.enableScroll = false;

        yield return new WaitForSeconds(2f);

        FinalCameraController.InstructionShow();
    }

    private bool lastTextFinish;


    
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

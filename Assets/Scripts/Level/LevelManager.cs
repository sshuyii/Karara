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

//    public CanvasGroup startScreen;
    public CanvasGroup chapterOne;
    public GameObject hintArrow;
    public CanvasGroup GoBackSubway;
    private CanvasGroup hintArrowCG;
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

    public CanvasGroup instructionCG;
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
    void Start()
    {
        Resources.UnloadUnusedAssets();

        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        SubwayMovement = GameObject.Find("---StationController").GetComponent<SubwayMovement>();
        RatingSystem = GameObject.Find("FloatingUI").GetComponent<RatingSystem>();

        //        instructionText = InstructionBubble.GetComponentInChildren<TextMeshProUGUI>();
        hintArrowCG = hintArrow.GetComponent<CanvasGroup>();

        PathFollower = car.GetComponent<PathFollower>();
        carCG = car.GetComponent<CanvasGroup>();

        int skipInstruction = PlayerPrefs.GetInt("skip",-1);
        if(skipInstruction == 1) skip = true;

        GameObject.Find("Main Camera").transform.position = new Vector3(0, 0, -20);

        if (!skip)
        {
            isInstruction = true;
            ShowRatingSys(true);

            ShowInstructionInMap();

            //closeInstructionButton.SetActive(true);
            //closeInstructionButton.GetComponent<Button>().interactable = false;
            //StartCoroutine(ShowInstruction());
        }
        else
        {
            ShowRatingSys(false);
            CloseInstruction();
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
    }

    private void ShowInstructionInMap()
    {
        //
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


        SubwayMovement.trainStop();
        SubwayMovement.timerStay = SubwayMovement.stayTime - 0.01f;
    }


    IEnumerator ShowInstruction()
    {
        FinalCameraController.enableScroll = false;

        yield return new WaitForSeconds(2f);

        FinalCameraController.InstructionShow();
    }

    private bool lastTextFinish;

    public IEnumerator AnimateText(TextMeshProUGUI text, string textContent, bool showArrow, GameObject hintObject,
        Vector2 arrowPosition)
    {
        clicktime++;
        lastTextFinish = false;
        Hide(hintArrowCG);

        for (int i = 0; i < (textContent.Length + 1); i++)
        {
            text.text = textContent.Substring(0, i);
            yield return new WaitForSeconds(.01f);

            if (i == textContent.Length)
            {
                yield return new WaitForSeconds(.1f);

                lastTextFinish = true;
                if (showArrow)
                {
                    Show(hintArrowCG);
                    hintArrow.transform.SetParent(hintObject.transform);
                    hintArrow.GetComponent<RectTransform>().anchoredPosition = arrowPosition;
                }
                else
                {
                    Hide(hintArrowCG);
                }
                
                //train starts to move
                if (clicktime == 5)
                {
                    Show(arrowButton);
                    Hide(arrowButton);  
                    Show(Stations);
                }
            }
        }
    }


    public IEnumerator AnimateFishText(TextMeshPro text, string textContent, bool showArrow, GameObject hintObject,
        Vector2 arrowPosition)
    {
        clicktime++;
        lastTextFinish = false;
        Hide(hintArrowCG);
        //debug.Log("lets see when it is called6");
        for (int i = 0; i < (textContent.Length + 1); i++)
        {
            text.text = textContent.Substring(0, i);
            yield return new WaitForSeconds(.01f);

            if (i == textContent.Length)
            {
                //debug.Log("clicktime " + clicktime.ToString());
                isInstruction = false;
                //yield return new WaitForSeconds(1f);
                //fishBubble.SetActive(false);
            }
        }
    }
    IEnumerator StartChapterOne()
    {
        
        Hide(chapterOne);
        

        FinalCameraController.ChangeToMap();
        yield return new WaitForSeconds(0.5f);

       
        Show(instructionCG);
        StartCoroutine(AnimateText(instructionText, "The laundromat is in a subway car", false, null, Vector2.zero));//clicktime = 1;
        Show(arrowButton);
        Show(FinalCameraController.SubwayMap);

//        instructionText.text = "Clothes are delivered in at each station.";
    }

    public int clicktime = 0;
    
    public void ClickInstruction()
    {
        if (clicktime == 0)
        {
            
            //once start, show a screen
//            StartCoroutine(StartChapterOne());
        }
        else if (clicktime == 1 && lastTextFinish)
        {
            StartCoroutine(AnimateText(instructionText, "Customers drop their bags at each station", false, null, Vector2.zero));//clicktime = 2;
            //bagIn.SetActive(true);

            //包箭头指向某一站
//            instructionText.text = "All of them need to be returned before the train reaches the same station for the second time.";

        }
        else if (clicktime == 2 && lastTextFinish)
        {
            StartCoroutine(AnimateText(instructionText, "Train moves and clothes need to be washed ", false, null, Vector2.zero));//clicktime = 3;
            //bagIn.SetActive(false);
            //PathFollower.isInstruction = true;

            //包箭头指向某一站

//            instructionText.text = "All of them need to be returned before the train reaches the same station for the second time.";

        }
        else if (clicktime == 3 && lastTextFinish)
        {
            StartCoroutine(AnimateText(instructionText, "Return clothes when the car reaches the same station for the second time", false, null, Vector2.zero));//clicktime = 4;
//            instructionText.text = "You can check each station's clothes here.";
            //bagIn.SetActive(true);
            bagIn.GetComponent<SpriteRenderer>().sprite = bagOut;
        }
        else if(clicktime == 4 && lastTextFinish)
        {
            StartCoroutine(AnimateText(instructionText, "Check each station's clothes here.", true, station, stationV));//clicktime = 5;
            //bagIn.SetActive(false);
            Show(hintArrowCG);
            Hide(arrowButton);  

        }
        else if(clicktime == 5 && lastTextFinish)//点了station detail
        {
            
        }
        //应该用不上
        else if (clicktime == 7 && lastTextFinish)//鱼开始说话
        {
            StartCoroutine(AnimateFishText(fishText, "See the bags? Time for work!", false, null, Vector2.zero));//clicktime = 7;
            Hide(arrowButton);
            //Hide(clear);
        }
        //点击回地铁按钮就回到地铁
        
        //if chapter one ends
        if (FinalCameraController.ChapterOneEnd)//点击之后进入第二章
        {
            SceneManager.LoadScene("StreetStyleTwo", LoadSceneMode.Single);
            //print("loadSceneTwo");
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

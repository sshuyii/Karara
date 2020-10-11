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

    public GameObject Instructions;
    public bool skip;
    public GameObject fishBubble;
    public TextMeshPro fishText;
    // Start is called before the first frame update
    void Start()
    {
        Resources.UnloadUnusedAssets();

        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();

//        instructionText = InstructionBubble.GetComponentInChildren<TextMeshProUGUI>();
        hintArrowCG = hintArrow.GetComponent<CanvasGroup>();

        PathFollower = car.GetComponent<PathFollower>();
        carCG = car.GetComponent<CanvasGroup>();

        int skipInstruction = PlayerPrefs.GetInt("skip",-1);
        if(skipInstruction == 1) skip = true;

        if (skip)
        {
            GameObject.Find("Main Camera").transform.position = new Vector3(0, 0, -20);
            clicktime = 7;
            //Instructions.SetActive(false);
            //Show(subwayCG);
            Hide(instructionCG);
            Hide(FinalCameraController.SubwayMap);
            //bagIn.SetActive(false);
            Hide(chapterOne);

        }
        else
        {
            isInstruction = true;            
        
            
            GameObject.Find("Main Camera").transform.position = new Vector3(0,14, 20);
//            Show(startScreen);
            Show(instructionCG);
            Instructions.SetActive(true);
            //bagIn.SetActive(false);
            Show(chapterOne);
            Show(arrowButton);
            Show(FinalCameraController.SubwayMap);
            Hide(GoBackSubway);
            //Hide(subwayCG);
            FinalCameraController.myCameraState = FinalCameraController.CameraState.Map;
            StartCoroutine(StartChapterOne());//start map tutorial directly
           

        }
        

    }

    // Update is called once per frame
    void Update()
    {
        if (clicktime == 6)//已经点开了station detail
        {
            Hide(instructionCG);
            Show(GoBackSubway); 
            Hide(hintArrowCG);
            //PathFollower.isInstruction = true;
        }
        else if (clicktime == 7 && FinalCameraController.mySubwayState == FinalCameraController.SubwayState.One)//鱼开始说话
        {
            fishBubble.SetActive(true);
            LocalizationManager.CurrentLanguage = "Chinese(Simplified)";
            LocalizedString locString = "Fish/SeeBags";
            string translation = locString;
            StartCoroutine(AnimateFishText(fishText, translation, false, null, Vector2.zero));//clicktime = 6;
            //Show(arrowButton);
            //debug.Log("lets see when it is called3");
            //PathFollower.isGame = true;
            Hide(arrowButton);
            //Hide(clear);
        }
        
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

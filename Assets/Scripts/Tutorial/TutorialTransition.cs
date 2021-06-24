using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEditor;

using UnityEngine;

public class TutorialTransition : MonoBehaviour
{
    // [Header("City Movement")]

    private Camera MainCamera;
    private RectTransform CameraRT;

    [SerializeField]
    private float cameraStopX;
    [SerializeField]
    private float cameraSpeed;

    public GameObject TrainStart;
    public float TrainSpeed;

    public int TransitionStage = 0;


    public GameObject trainWithWindow;

    public bool forwardOneStep = false;

    private float startTime;

    public CanvasGroup TransparentButton;
    
    [SerializeField]
    private float CameraInsideX, CameraFarX, CameraHiringX;


    [SerializeField]
    private ScrollingBackground ScrollingBackground_1, ScrollingBackground_2;

    private float trainStartWidth;
    private Vector3 screenBounds;

    [SerializeField]
    private float timeAfterTrainStart, timeAfterTrainStop;


    // Start is called before the first frame update
    void Start()
    {
        Hide(TransparentButton);
        trainWithWindow.SetActive(false);

        MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        //教程最开始，镜头拉远
        CameraRT = MainCamera.GetComponent<RectTransform>();
        CameraRT.position = new Vector3(CameraInsideX, 0, -10f);

        //get the size of trainStart and camera
        screenBounds = MainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, MainCamera.transform.position.z))
            - MainCamera.transform.position;
        trainStartWidth = TrainStart.GetComponent<SpriteRenderer>().bounds.size.x;

        StartCoroutine(WaitForLoading(2f));
    }

    
    IEnumerator WaitForLoading(float waitDuration)
    {
        yield return new WaitForSeconds(waitDuration);
        TransitionStage ++;

        ScrollingBackground_1.start = true;
        Show(TransparentButton);
    }

    private void FixedUpdate() 
    {
        if(TransitionStage == 2){

            //如果第一节车厢已经从镜头前出去了，销毁
            if(TrainStart)
            {
                TrainStart.transform.position += ScrollingBackground_2.speed[0] * Time.fixedDeltaTime;

                if(TrainStart.transform.position.x + trainStartWidth < CameraRT.position.x - screenBounds.x)
                {
                    Destroy(TrainStart);
                }
            }
        }
        // //第二阶段：中等大小地铁
        // else if(TransitionStage == 2)
        // {
        //     if(CameraRT.position.x < cameraStopX)
        //     {
        //         CameraRT.position += new Vector3(cameraSpeed, 0, 0) * Time.fixedDeltaTime;
        //         startTime = 0;//计时归零
        //     }
        // }

    }
    // Update is called once per frame
    void Update()
    {
        // print("Start time = " + startTime);
        startTime += Time.deltaTime;

        if(forwardOneStep)
        {
            TransitionStage ++;
            forwardOneStep = false;
            startTime = 0;//计时归零

            if(TransitionStage == 2)
            {
                SetTrainFar();
                StopTrain(ScrollingBackground_1);
            }
            else if(TransitionStage == 3)
            {
                SetHiring();
                StopTrain(ScrollingBackground_2);
                Hide(TransparentButton);//不用点击全屏幕了
            }
            else if(TransitionStage >= 4)
            {
                StopTrain(ScrollingBackground_1);
                StopTrain(ScrollingBackground_2);
                Hide(TransparentButton);//不用点击全屏幕了
            }
        }
    }

    private void SetHiring()
    {
        CameraRT.position = new Vector3(CameraHiringX, 0, -10f);
    }

    private void SetTrainFar()
    {
        CameraRT.position = new Vector3(CameraFarX, 0, -10f);
        ScrollingBackground_2.start = true;
    }

    private void SetTrainMiddle()
    {
        trainWithWindow.SetActive(true);

            MainCamera.GetComponent<Camera>().orthographicSize = 9;
            CameraRT.position = new Vector3(-4f, 2f, -10f);

    }

    public void StopTrain(ScrollingBackground SB)
    {
        SB.enabled = false;

        SpriteRenderer[] SRList = SB.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in SRList)
        {
            sr.enabled = false;
        }
    }

    public void ClickToProceed()
    {
        if(startTime < timeAfterTrainStop)
        {
            return;
        }
        else if(startTime < timeAfterTrainStart)
        {
            if(TransitionStage == 1) return;

        }
        forwardOneStep = true;
         
        print("click to proceed");
    }

    private void Hide(CanvasGroup UIGroup)
    {
        UIGroup.alpha = 0f; //this makes everything transparent
        UIGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
        UIGroup.interactable = false;
    }

    private void Show(CanvasGroup UIGroup)
    {
        UIGroup.alpha = 1f; //this makes everything transparent
        UIGroup.blocksRaycasts = true; //this prevents the UI element to receive input events
        UIGroup.interactable = true;
    }
}


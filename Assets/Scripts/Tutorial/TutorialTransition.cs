using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEditor;
using UnityEditor.EditorTools;
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

    public int TransitionStage = 0;


    public GameObject trainWithWindow;

    public bool forwardOneStep = false;

    private float startTime;

    public CanvasGroup TransparentButton;

    [SerializeField]
    private ScrollingBackground ScrollingBackground;


    // Start is called before the first frame update
    void Start()
    {
        trainWithWindow.SetActive(false);

        MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        //教程最开始，镜头拉远
        CameraRT = MainCamera.GetComponent<RectTransform>();
        CameraRT.position = new Vector3(33f, 0, -10f);

        StartCoroutine(WaitForLoading(2f));
    }

    
    IEnumerator WaitForLoading(float waitDuration)
    {
        yield return new WaitForSeconds(waitDuration);
        TransitionStage ++;

        ScrollingBackground.start = true;
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
                SetStage2();
                ResetStage1();
            }
        }

        //第二阶段：中等大小地铁
        if(TransitionStage == 2)
        {
            if(CameraRT.position.x < cameraStopX)
            {
                CameraRT.position += new Vector3(cameraSpeed, 0, 0);
                startTime = 0;//计时归零
            }
        }
    }


    private void SetStage2()
    {
        trainWithWindow.SetActive(true);

            MainCamera.GetComponent<Camera>().orthographicSize = 9;
            CameraRT.position = new Vector3(-4f, 3f, -10f);

    }

    private void ResetStage1()
    {
        ScrollingBackground.enabled = false;

        
        SpriteRenderer[] SRList = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in SRList)
        {
            sr.enabled = false;
        }
    }

    public void ClickToProceed()
    {
        if(startTime < 1.5f)
        {
            return;
        }
        else if(startTime < 5f)
        {
            if(TransitionStage == 1) return;

        }
        forwardOneStep = true;
         
        print("click to proceed");
    }

}


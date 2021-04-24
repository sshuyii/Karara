using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class TutorialTransition : MonoBehaviour
{
    [Header("Train Movement")]
    [SerializeField]    
    private GameObject subwayTrain;
    [SerializeField]
    private int stopX;
    [SerializeField]
    private float speed;

    [Header("City Movement")]
    [SerializeField]
    private GameObject city;

    [SerializeField]    
    private int cityStopX;

    [SerializeField]    
    private float citySpeed;

    private GameObject MainCamera;
    private RectTransform CameraRT;

    public int TransitionStage = 0;
    [SerializeField]    
    private float cameraSpeed;
    [SerializeField]    
    private float cameraStopX;

    public GameObject trainWithWindow;

    public bool forwardOneStep = false;

    private float startTime;

    public CanvasGroup TransparentButton;


    // Start is called before the first frame update
    void Start()
    {
        trainWithWindow.SetActive(false);

        MainCamera = GameObject.Find("Main Camera");

        //教程最开始，镜头拉远
        CameraRT = MainCamera.GetComponent<RectTransform>();
        CameraRT.position = new Vector3(33f, 0, -10f);

        StartCoroutine(WaitForLoading(2f));
    }

    IEnumerator WaitForLoading(float waitDuration)
    {
        yield return new WaitForSeconds(waitDuration);
        TransitionStage ++;
    }

    // Update is called once per frame
    void Update()
    {
        // print("Start time = " + startTime);
        startTime += Time.deltaTime;

        if(TransitionStage == 1)
        {
            if(subwayTrain.transform.position.x > stopX)
            {
                if(startTime < 2f)
                {                
                    subwayTrain.transform.position += new Vector3 (-3* speed, 0, 0);
                }            
                else{
                    subwayTrain.transform.position += new Vector3 (-speed, 0, 0);
                }
            }

            if(city.transform.position.x > cityStopX)
            {
                city.transform.position += new Vector3 (-citySpeed, 0, 0);

            }
        }
        else if(TransitionStage == 2)
        {
            if(CameraRT.position.x < cameraStopX)
            {
                CameraRT.position += new Vector3(cameraSpeed, 0,0);
            }   
        }

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

        
    }

    private void SetStage2()
    {
        trainWithWindow.SetActive(true);

            MainCamera.GetComponent<Camera>().orthographicSize = 9;
            CameraRT.position = new Vector3(-4f, 3f, -10f);

    }

    private void ResetStage1()
    {
        subwayTrain.SetActive(false);
        city.SetActive(false);
    }

    public void ClickToProceed()
    {
        if(startTime < 3f)
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


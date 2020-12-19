using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class TouchController : MonoBehaviour
{
    private Vector3 fp;   //First touch position
    public Vector3 lp;   //Last touch position
    private float dragDistance;  //minimum distance for a swipe to be registered

    private FinalCameraController FinalCameraController;

    public bool isSwiping = false;
    public bool isSwipable = false;
    private bool TOF;
   
    public Touch touch;

    public bool leftSwipe;
    
    public float offsetX;
    public Transform camTransform;
    public enum InputState {
        LeftSwipe,
        RightSwipe,
        Tap,
        None,
    }
    public InputState myInputState;


    public bool isLongTap;
    public bool isFastSwipe;
    private float startTime;
    private float diffTime;
    private float swipeDistance;

    private float swipeSpeed;

    public bool doubleTouch;

    //for typewriter use
    private string[] typeText;
    int currentlyDisplayingText = 0;

    private CameraMovement cameraMovement;
    
    // public WasherController washerController;
    RaycastHit2D hit;
    public string[] RaycastHitResult = new string[2]{"",""};

    private LostAndFound LostAndFound;
    private AdsController AdsController;
    private AudioManager AudioManager;

    public GameObject DebugOutput;

    void Start()
    {
        myInputState = InputState.None;
        dragDistance = Screen.height * 8 / 100;
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        if (FinalCameraController != null)
        {
            LostAndFound = GameObject.Find("Lost&Found_basket").GetComponent<LostAndFound>();
            AdsController = GameObject.Find("---AdsController").GetComponent<AdsController>();
            AudioManager = GameObject.Find("---AudioManager").GetComponent<AudioManager>();
        }
            

        cameraMovement = GameObject.Find("Main Camera").GetComponent<CameraMovement>();

        
        // washerController = GameObject.Find("").GetComponent<WasherController>();
    }

    
    
    // Update is called once per frame
    void Update()
    {


        // @@@
        //&& EventSystem.current.IsPointerOverGameObject()
        if (FinalCameraController.myCameraState!=FinalCameraController.CameraState.Subway)
        {
            return;
        }
        if (EventSystem.current.currentSelectedGameObject != null) {
            //DebugOutput.GetComponent<Text>().text = EventSystem.current.currentSelectedGameObject.name;
            //AudioManager.PlayAudio(AudioType.UI_Dialogue);
            return;
        }


        if (Input.touchCount == 0) {
            myInputState  = InputState.None;
            hit = new RaycastHit2D();
            RaycastHitResult = new string[2]{"",""};
        } 


//        //print("cancelCloth = " + doubleTouch);
        for (var i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                if (Input.GetTouch(i).tapCount == 2)
                {
                    //Debug.Log("Double Tap");
                    doubleTouch = true;
                }  
            }
            else if (Input.GetTouch(i).phase == TouchPhase.Ended)
            {
                doubleTouch = false;
            }
        }
        
        
        if (Input.touchCount == 1) // user is touching the screen with a single touch
        {
            touch = Input.GetTouch(0); // get the touch
            if (touch.phase == TouchPhase.Began) //check for the first touch
            {
                fp = touch.position;
                lp = touch.position;

                startTime = Time.time;
                
                // offsetX = camTransform.position.x - lp.x;

                myInputState = InputState.None;
                
                //for swipe screen
                isSwipable = true;
            }
            else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
            {
                lp = touch.position;
//                //print("lp = " + lp);

                if(FinalCameraController.enableScroll && FinalCameraController.myCameraState == FinalCameraController.CameraState.Subway)
                {
                    isSwiping = true;
                    cameraMovement.swipping = true;
                    Vector3 oldPos = FinalCameraController.transform.position;
                    Vector3 newPos = new Vector3(oldPos.x - touch.deltaPosition.x * 0.005f, oldPos.y, oldPos.z);
                    FinalCameraController.transform.position = newPos;

                }




            }
            else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
            {
                



                lp = touch.position;  //last touch position. Ommitted if you use list
        
                offsetX = 0;
                isSwiping = false;
                cameraMovement.swipping = false;

                //check if this is a fast swipe
                diffTime = startTime - Time.time;
                swipeDistance = Vector2.Distance(fp,lp);
                swipeSpeed = Mathf.Abs(swipeDistance / diffTime);//<<<<<<<<
//                //print("diffTime =" + diffTime);
//                //print("swipeSpeed =" + swipeSpeed);
                if (swipeSpeed > 500 && Mathf.Abs(diffTime) < 0.2f)
                {
                    isFastSwipe = true;
                    
                }
                else
                {
                    isFastSwipe = false;
                }
                
                //Check if drag distance is greater than 20% of the screen height
                if (Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance)
                {//It's a drag
                 //check if the drag is vertical or horizontal
                    if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
                    {   //If the horizontal movement is greater than the vertical movement...
                        if ((lp.x > fp.x))  //If the movement was to the right)
                        {   //Right swipe
                            //Debug.Log("Right Swipe");
                            leftSwipe = false;
                            myInputState = InputState.RightSwipe;
                            if(FinalCameraController.enableScroll && FinalCameraController.myCameraState == FinalCameraController.CameraState.Subway){
                                cameraMovement.Go2Page(cameraMovement.currentPage - 1);
                            }
                        }
                        else
                        {   //Left swipe
                            //Debug.Log("Left Swipe");
                            leftSwipe = true;
                            
                            myInputState = InputState.LeftSwipe;

                            if(FinalCameraController.enableScroll && FinalCameraController.myCameraState == FinalCameraController.CameraState.Subway){
                                cameraMovement.Go2Page(cameraMovement.currentPage + 1);
                                //cameraMovement.currentPage += 1;
                            }
                        }
                    }
                    else
                    {   //the vertical movement is greater than the horizontal movement
                        if (lp.y > fp.y)  //If the movement was up
                        {   //Up swipe
                            //Debug.Log("Up Swipe");
                        }
                        else
                        {   //Down swipe
                            //Debug.Log("Down Swipe");
                        }
                    }
                }
                else  if(FinalCameraController.enableScroll)
                {   //It's a tap as the drag distance is less than 20% of the screen height
                    Debug.Log("Tap");
                    myInputState = InputState.Tap;
                    

                    checkTap();
                    checkCollider();
                }
            }
            
        }
        
    }

    private void checkTap(){


        lp.z = 0;
        Vector3 screenPoint = Camera.main.ScreenToWorldPoint(lp);
        hit = Physics2D.Raycast(screenPoint,Vector2.zero);
        Debug.Log("checkTap");
        if ( hit.collider!=null ){

            

            string hitObject = hit.transform.gameObject.name;
            switch(hitObject){

                case "FishBoss":
                    AudioManager.PlayAudio(AudioType.UI_Dialogue);
                    FinalCameraController.BossTalk();
                break;

                case "PlayerBodySubway":
                    AudioManager.PlayAudio(AudioType.UI_Dialogue);
                    FinalCameraController.ChangeToCloth();
                    break;

                case "SettingButton":
                    AudioManager.PlayAudio(AudioType.UI_Dialogue);
                    FinalCameraController.clickSetting();
                    break;
                
                //case "MessageLogo":
                //    FinalCameraController.ChangeToApp();
                //    break;
                //case "ClothLogo":
                //    FinalCameraController.ChangeToCloth();
                    //break;
                case "subwayMap":
                    AudioManager.PlayAudio(AudioType.UI_Dialogue);
                    FinalCameraController.ChangeToMap();
                    break;

                case "Lost&Found_basket":
                    AudioManager.PlayAudio(AudioType.UI_Dialogue);
                    LostAndFound.clickLostFound();
                    break;
            }


            Regex mRegular = new Regex(@"Poster\d+", RegexOptions.None);
            if (mRegular.IsMatch(hit.transform.gameObject.name))
            {
                AdsController.ClickBackground(hit.transform.GetComponent<SpriteRenderer>().sprite.name);
            }



            //DebugOutput.GetComponent<Text>().text = hit.transform.gameObject.name;
            Debug.Log(hit.transform.gameObject.name);
            
        }
    }

    public string[] checkCollider(){
        
        HashSet<string> buttons = new HashSet<string>{"Music","Sound","Realtime","Lost&Found_basket"};
        if( hit.collider != null){
            string name = hit.transform.name;
            //DebugOutput.GetComponent<Text>().text = name;
            if (buttons.Contains(name)){
                RaycastHitResult[0] = name;
            }



            DoInventory doIvt = hit.transform.gameObject.GetComponent<DoInventory>();
            if (doIvt != null)
            {
                AudioManager.PlayAudio(AudioType.UI_Dialogue);
                RaycastHitResult[0] = "doIvt";
                RaycastHitResult[1] = doIvt.slotNum.ToString();
                //Debug.Log("do inventory!");
                return RaycastHitResult;
            }

            WasherController wc = hit.transform.gameObject.GetComponentInParent<WasherController>();
            if (wc != null)
            {
                AudioManager.PlayAudio(AudioType.UI_Dialogue);
                RaycastHitResult[0] = "washer";
                RaycastHitResult[1] = wc.number.ToString();
                return RaycastHitResult;

            }

            HandleAnimation ha = hit.transform.gameObject.GetComponent<HandleAnimation>();
            if (ha != null)
            {
                RaycastHitResult[0] = "handle";
                RaycastHitResult[1] = ha.handleNum.ToString();
            }

        }
        return RaycastHitResult;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class TutorialTouchController : MonoBehaviour
{
    private Vector3 fp;   //First touch position
    public Vector3 lp;   //Last touch position
    private float dragDistance;  //minimum distance for a swipe to be registered

    private TutorialCameraController TutorialCameraController;
    private TutorialManagerNew TutorialManagerNew;

    public bool isSwiping = false;
    public bool isSwipable = false;
    private bool TOF;

    public Touch touch;

    public bool leftSwipe;

    public float offsetX;
    public Transform camTransform;
    public enum InputState
    {
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
    public string[] RaycastHitResult = new string[2] { "", "" };

    private LostAndFound LostAndFound;
    //private AdsController AdsController;

    void Start()
    {
        myInputState = InputState.None;
        dragDistance = Screen.height * 8 / 100;
        TutorialCameraController = GameObject.Find("Main Camera").GetComponent<TutorialCameraController>();
        cameraMovement = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
        TutorialManagerNew = GameObject.Find("---TutorialManager").GetComponent<TutorialManagerNew>();

        // washerController = GameObject.Find("").GetComponent<WasherController>();
    }



    // Update is called once per frame
    void Update()
    {


        // @@@

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            return;
        }

        if (Input.touchCount == 0)
        {
            myInputState = InputState.None;
            hit = new RaycastHit2D();
            RaycastHitResult = new string[2] { "", "" };
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

                if (Mathf.Abs(lp.y - fp.y) > dragDistance / 4 || Mathf.Abs(lp.x - fp.x) > dragDistance / 4)
                {
                    isSwiping = true;

                }
                else
                {
                    isSwiping = false;
                    TutorialCameraController.Swipping(false);
                }


                if(isSwiping && TutorialCameraController.allowScroll)
                {
                    TutorialCameraController.Swipping(true);
                    Vector3 oldPos = TutorialCameraController.transform.position;
                    Vector3 newPos = new Vector3(oldPos.x - touch.deltaPosition.x * 0.005f, oldPos.y, oldPos.z);
                    TutorialCameraController.transform.position = newPos;
                    //Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                    //Vector3 oldPos = TutorialCameraController.transform.position;
                    //float changeX = touchPos.x - oldPos.x;
                    //Vector3 newCamPos = new Vector3(oldPos.x - 0.1f * changeX, oldPos.y, oldPos.z);
                    //TutorialCameraController.transform.position = newCamPos;

                }


                if (Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance)
                {

                }
                else
                {
                    //it is a long tap
                    if (Time.time - startTime > 0.8f)
                    {
                        isLongTap = true;
                    }
                    else
                    {
                        isLongTap = false;
                    }
                }


            }
            else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
            {



                lp = touch.position;  //last touch position. Ommitted if you use list

                offsetX = 0;
                isSwiping = false;
                TutorialCameraController.Swipping(false);

                //check if this is a fast swipe
                diffTime = startTime - Time.time;
                swipeDistance = Vector2.Distance(fp, lp);
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
                            if (TutorialCameraController.allowScroll  && TutorialCameraController.myCameraState == TutorialCameraController.CameraState.Subway)
                            {
                                cameraMovement.Go2Page(cameraMovement.currentPage - 1);
                                //cameraMovement.currentPage += -1;
                                //Debug.Log("page " + cameraMovement.currentPage);
                            }
                        }
                        else
                        {   //Left swipe
                            //Debug.Log("Left Swipe");
                            leftSwipe = true;

                            myInputState = InputState.LeftSwipe;

                            if (TutorialCameraController.allowScroll && TutorialCameraController.myCameraState == TutorialCameraController.CameraState.Subway)
                            {
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
                else
                {   //It's a tap as the drag distance is less than 20% of the screen height
                    Debug.Log("Tap");
                    myInputState = InputState.Tap;

                    checkTap();
                    checkCollider();
                }
            }

        }

    }

    private void checkTap()
    {
        lp.z = 0;
        Vector3 screenPoint = Camera.main.ScreenToWorldPoint(lp);
        hit = Physics2D.Raycast(screenPoint, Vector2.zero);
        Debug.Log("checkTap");
        if (hit.collider != null)
        {

            string hitObject = hit.transform.gameObject.name;

            switch(hitObject)
            {
                case "front":
                    TutorialManagerNew.ClickMachine();
                    break;
                case "full":
                    TutorialManagerNew.ClickMachine();
                    break;
                case "phone":
                    TutorialManagerNew.PickChargingPhone();
                    break;
                case "ClothSlot1":
                    TutorialManagerNew.ClickClothUI();
                    break;
                case "PlayerBodySubway":
                    TutorialManagerNew.ClickKarara();
                    break;
                case "!":
                    TutorialManagerNew.ClickExclamation();
                    break;



            }


            Regex mRegular = new Regex(@"Poster\d+", RegexOptions.None);
            if (mRegular.IsMatch(hit.transform.gameObject.name))
            {
                TutorialManagerNew.ClickAd();
            }




            Debug.Log(hit.transform.gameObject.name);

        }
    }

    public string[] checkCollider()
    {

        HashSet<string> buttons = new HashSet<string> { "Music", "Sound", "Realtime", "Lost&Found_basket" };
        if (hit.collider != null)
        {
            string name = hit.transform.name;

            if (buttons.Contains(name))
            {
                RaycastHitResult[0] = name;
            }



            DoInventory doIvt = hit.transform.gameObject.GetComponent<DoInventory>();
            if (doIvt != null)
            {
                RaycastHitResult[0] = "doIvt";
                RaycastHitResult[1] = doIvt.slotNum.ToString();
                //Debug.Log("do inventory!");
                return RaycastHitResult;
            }

            WasherController wc = hit.transform.gameObject.GetComponentInParent<WasherController>();
            if (wc != null)
            {
                RaycastHitResult[0] = "washer";
                RaycastHitResult[1] = wc.number.ToString();

                TutorialManagerNew.ClickMachine();
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

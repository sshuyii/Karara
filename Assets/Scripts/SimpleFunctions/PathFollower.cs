using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PathFollower : MonoBehaviour
{
    // Array of waypoints to walk from one to the next one
    [SerializeField]
    public RectTransform[] waypoints;

    // Walk speed that can be set in Inspector
    [SerializeField]
    private float distance;

    // Index of current waypoint from which Enemy walks
    // to the next one
    public int waypointIndex = 0;
    private RectTransform myRT;
    private SubwayMovement SubwayMovement;
    public LevelManager LevelManager;

    public List<int> stationNumList;
    public float moveSpeed;
    private bool trainMove;
    public Vector3 currentEulerAngles;
    public Vector3 currentPos;

    public bool isInstruction;
    private float instructionMoveSpeed;

    public bool isGame;
    //public Image MapTutorialBag;
    private float fillamount = 1f;
    // Use this for initialization
    private void Start () {

        // Set position of Enemy as position of the first waypoint
        myRT = GetComponent<RectTransform>();
        myRT.anchoredPosition= waypoints[waypointIndex].anchoredPosition;
        waypointIndex = 1;
        
        SubwayMovement = GameObject.Find("---StationController").GetComponent<SubwayMovement>();

        moveSpeed = distance / SubwayMovement.moveTime;
        currentEulerAngles = myRT.eulerAngles;

    }
	
   
    // Update is called once per frame
    private void Update () {

        
        if(isInstruction)//绕一圈
        {
            //QuickMove();
        }    
        else if (!isInstruction && !isGame)
        {
            //myRT.rotation = Quaternion.Euler(Vector3.zero);
        }
        else if(isGame)
        {        
            Move();
        }
        currentPos = myRT.anchoredPosition;
    }


    private void QuickMove()
    {
        instructionMoveSpeed = 500f;
        fillamount -= 0.01f;
        //MapTutorialBag.fillAmount = fillamount;

        if (waypointIndex <= waypoints.Length - 1)
        {
            currentEulerAngles = myRT.eulerAngles;

            ////print(waypointIndex + "<1");
            // Move Enemy from current waypoint to the next one
            // using MoveTowards method
            myRT.anchoredPosition = Vector2.MoveTowards(myRT.anchoredPosition,
                waypoints[waypointIndex].anchoredPosition,
                instructionMoveSpeed * Time.deltaTime);


                
            // If Enemy reaches position of waypoint he walked towards
            // then waypointIndex is increased by 1
            // and Enemy starts to walk to the next waypoint
            if (myRT.anchoredPosition == waypoints[waypointIndex].anchoredPosition)
            {
                waypointIndex += 1;
                
            }
            else
            {
                //car rotates!
                var dir = waypoints[waypointIndex].anchoredPosition - myRT.anchoredPosition;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                //myRT.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
        else
        {
            //到了第一站会先停下来,所以这一行disable了
            //waypointIndex = 0;
            
            //马上要到第一站了
            myRT.anchoredPosition = Vector2.MoveTowards(myRT.anchoredPosition,
                waypoints[0].anchoredPosition,
                instructionMoveSpeed * Time.deltaTime);
//            myRT.rotation = Quaternion.AngleAxis(0, Vector3.forward);

            //myRT.rotation = Quaternion.Euler(Vector3.right);
            
            //如果到了第一站
            if (myRT.anchoredPosition == waypoints[0].anchoredPosition)
            {
                //那么停下来
                LevelManager.ShowHint();
                instructionMoveSpeed = 0;
                isInstruction = false;
//                waypointIndex = 0;
//                myRT.rotation = Quaternion.AngleAxis(90, Vector3.forward);
                //myRT.rotation = Quaternion.Euler(Vector3.zero);

            }
//            else
//            {
//                myRT.rotation = Quaternion.Euler(Vector3.right);
//
//                //car rotates!
//                var dir = waypoints[waypointIndex].anchoredPosition - myRT.anchoredPosition;
//                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
//            }

        }
    
    }

    
    private int nearestStopIndex;

    //waypoint index是要移动到的那个站
    //todo: 应该有一个更通用的方式做这件事！用for loop
    //如果快进的话，找到要立马跳到的那个站
    //现在的waypoint index和站点的index对比，找到最近的那个站点
    public int getNextStopIndex()
    {

        Debug.Log("current idx" + waypointIndex.ToString());
        if ( waypointIndex == 7 || waypointIndex == 0)
        {
            nearestStopIndex = 1;
        }
        else if (waypointIndex == 2 || waypointIndex == 3 || waypointIndex == 1)
        {
            nearestStopIndex = 4;
        }
        else if (waypointIndex == 5 || waypointIndex == 6 || waypointIndex == 4)
        {
            nearestStopIndex = 7;
        }

        return nearestStopIndex;
    }
    
    private float timer;

    private bool timeToGo;
    // Method that actually make Enemy walk
    private void Move()
    {
//        //print("isWorking");
        // If Enemy didn't reach last waypoint it can move
        // If enemy reached last waypoint then it stops
        if (!trainMove)
        {
            if (SubwayMovement.isMoving)
            {
                trainMove = true;
                moveSpeed = distance / SubwayMovement.moveTime;
            }
            else
            {
                moveSpeed = 0;
                myRT.eulerAngles = currentEulerAngles;
            }
        }


        if (waypointIndex <= waypoints.Length - 1 && trainMove)
        {
            currentEulerAngles = myRT.eulerAngles;

                //Debug.Log(waypointIndex + "<1冲");
            // Move Enemy from current waypoint to the next one
            // using MoveTowards method
            

            myRT.anchoredPosition = Vector2.MoveTowards(myRT.anchoredPosition,
                waypoints[waypointIndex].anchoredPosition,
                moveSpeed * Time.deltaTime);
    
            // If Enemy reaches position of waypoint he walked towards
            // then waypointIndex is increased by 1
            // and Enemy starts to walk to the next waypoint
            if (myRT.anchoredPosition == waypoints[waypointIndex].anchoredPosition)
            {
                waypointIndex += 1;

                if (waypointIndex == waypoints.Length) waypointIndex = 0;

                Debug.Log(waypointIndex + "+1到站");
                //如果车到了站点的那几个waypoint
                for (int i = 0; i < stationNumList.Count; i++)
                {
                    if (waypointIndex == stationNumList[i])
                    {
                        trainMove = false;
                        break;
                    }
                }
            }
            else
            {
                //car rotates!
                var dir = waypoints[waypointIndex].anchoredPosition - myRT.anchoredPosition;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                //myRT.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            }
        }
        else if(trainMove)
        {
            waypointIndex = 0;
            trainMove = false;
//            //print("=00000");
        }
    }

    public void jumpTo() {
        getNextStopIndex();
        waypointIndex = nearestStopIndex;
        Debug.Log("Jump" + waypointIndex.ToString());
        myRT.anchoredPosition = waypoints[waypointIndex-1].anchoredPosition;
        Vector3 dir = new Vector3(0,0,0);
        if (waypointIndex == 4) dir = new Vector3(0,0,-90);
        if (waypointIndex == 7) dir = new Vector3(180, 180,0);
        //myRT.rotation = Quaternion.Euler(dir);
    }

}
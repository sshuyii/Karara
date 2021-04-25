using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Lean.Common.Examples;
using UnityEngine;
using UnityEngine.UI;

public class ClockRotate : MonoBehaviour
{
    private SubwayMovement SubwayMovement;
    private RectTransform myRectT;
    public int bagEmptyNum = 0;
    public int machineEmptyNum = 0;


    private PathFollower PathFollower;
    public GameObject car;
    private AllMachines AllMachines;

    public bool fastForward;
    
    public float zRotation;

    public CanvasGroup bubble;

    private Button myButton;
    private Image selfImage;

    public float fillAmount = 1;

    public bool isFast;
    
    // Start is called before the first frame update
    void Start()
    {
        SubwayMovement = GameObject.Find("---StationController").GetComponent<SubwayMovement>();
        AllMachines = GameObject.Find("---ClothInMachineController").GetComponent<AllMachines>();
        car = GameObject.Find("SubwayMapCar");
        PathFollower = car.GetComponent<PathFollower>();

        //used when the timer is always on screen
        //zRotation = 360 / (SubwayMovement.stayTime + SubwayMovement.moveTime);




        myRectT = GetComponent<RectTransform>();

        //SubwayMovement.Hide(bubble);
        myButton = GetComponent<Button>();
        selfImage = GetComponent<Image>();
        selfImage.enabled = false;
        
        if (!isFast&&!SubwayMovement.pauseBeforeMove)
        {
            //now the timer is placed on the bag
            //zRotation = 360 / (3 *(SubwayMovement.stayTime + SubwayMovement.moveTime));
            fillAmount = 1;
            selfImage.fillAmount = fillAmount;
            selfImage.enabled = true;

        }

    }

    // Update is called once per frame
    void Update()
    {
        if(!isFast && !SubwayMovement.pauseBeforeMove)
        {
            fillAmount -= 1 / (2 * (SubwayMovement.stayTime + SubwayMovement.moveTime) + SubwayMovement.moveTime) *
                Time.deltaTime;
            //            myRectT.Rotate(new Vector3(0, 0, -zRotation * Time.deltaTime));
            if(SubwayMovement.LevelManager.stage > 1) selfImage.fillAmount = fillAmount;
        }
        
        bagEmptyNum = 0;
        machineEmptyNum = 0;

        if (!fastForward && isFast)
        {
            //Calculate how many bagPos are available
            for (int i = 0; i < 3; i++)
            {
                if (SubwayMovement.bagPosAvailable[i] == false)
                {
                    bagEmptyNum++;
                    if (bagEmptyNum == 3)
                    {
//                    break;
                    }
                }
            }

            //if all machines are empty
            for (int i = 0; i < 3; i++)
            {
                if (AllMachines.WashingMachines[i].GetComponent<WasherController>().myMachineState ==
                    AllMachines.MachineState.empty)
                {
                    machineEmptyNum++;
                    if (machineEmptyNum == 3)
                    {
//                    break;
                    }
                }
            }


            //上面两个都没有break，说明地铁里没有包，洗衣机也没有被占用，地铁还在行走
            if (bagEmptyNum == 3 && machineEmptyNum == 3 && SubwayMovement.isMoving)
            {
                fastForward = true;
                //SubwayMovement.Show(bubble);
                //myButton.enabled = true;
                selfImage.enabled = true;
            }
            else if (!SubwayMovement.isMoving)
            {
                fastForward = false;
                selfImage.enabled = false;
                //SubwayMovement.Hide(bubble);
                //myButton.enabled = false;
            }
        }

    }

    //click the clock and then skip the rest of the station
    public void clickClock()
    {
        SubwayMovement.trainStop();
        SubwayMovement.timerS2S = 0;
        SubwayMovement.timerStay = 0;
        selfImage.enabled = false;
        fastForward = false;
        //SubwayMovement.Hide(bubble);

        //车立马跑到下一个位置
        //car.GetComponent<RectTransform>().anchoredPosition = PathFollower.waypoints[PathFollower.getNextStopIndex()].anchoredPosition;

        PathFollower.jumpTo();
    }
}

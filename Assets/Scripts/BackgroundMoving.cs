using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMoving : MonoBehaviour
{
    public float speed;

    public float endPoint;

    private float startPoint;
    private SubwayMovement SubwayMovement;
    private SpriteRenderer mySR;

    public Sprite stationSprite;

    public bool isStation;

    private Sprite myStartSprite;
    private bool speedDown;

    private float startSpeed;
    public float stationSpeed;
    private bool leftStation = true;

    public float transitionSpeed;
        
    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.position.x;
        
        SubwayMovement = GameObject.Find("---StationController").GetComponent<SubwayMovement>();
        mySR = GetComponent<SpriteRenderer>();
        myStartSprite = mySR.sprite;

        startSpeed = speed;

        transitionSpeed = stationSpeed / 3f;

    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x < endPoint)
        {
            transform.position += new Vector3(speed, 0, 0);
          
        }
        else
        {
            transform.position -= new Vector3(endPoint - startPoint, 0, 0);
        }

        if (isStation)
        {
            if(SubwayMovement.isMoving == false)
            {
                mySR.sprite = stationSprite;
                speedDown = true;
            }
            else
            {
                speedDown = false;
                //speed = startSpeed;
                //when train leaves station, background needs to accelerate
            }
        }

        if (speedDown)
        {
            if(speed > 0f)
            {
                if (speed == startSpeed)
                {
                    //set the speed differently because pillars move faster than buildings 
                    speed = stationSpeed;
                }
                else
                {
                    speed -= transitionSpeed * Time.deltaTime;
                }
            }
            else //if speed is less than 0, be 0
            {
                speed = 0f;
                leftStation = false;
            }
        }
        else
        { 
            if(speed < stationSpeed && !leftStation)//reach the fastest speed set for station pillars
            {
                speed += transitionSpeed * Time.deltaTime;
                SubwayMovement.outsideStation = false;

            }        
            else
            {
                speed = startSpeed;
                mySR.sprite = myStartSprite;

                SubwayMovement.outsideStation = true;
                leftStation = true;
            }
        }

        if (SubwayMovement.outsideStation)
        {
        }
        
       
    }
}

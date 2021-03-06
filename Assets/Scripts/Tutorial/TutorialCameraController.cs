﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCameraController : MonoBehaviour
{
    CameraMovement CameraMovement;
    // TutorialManagerNew TutorialManagerNew;
    public enum CameraState
    {
        Subway,
        Closet,
        Map,
        App,
        Ad
    }

    public bool allowScroll = false;
    public bool reachTarget = false;

    

    public int targetPage;
    public CameraState myCameraState;

    [SerializeField]
    CanvasGroup disableInputCG;

    public int currentPage;
    // Start is called before the first frame update
    void Start()
    {
        myCameraState = CameraState.Subway;
        CameraMovement = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
        CameraMovement.currentPage = 4;

        currentPage = CameraMovement.currentPage;

        // TutorialManagerNew = GameObject.Find("---TutorialManager").GetComponent<TutorialManagerNew>();

    }

    // Update is called once per frame
    void Update()
    {
        currentPage = CameraMovement.currentPage;

        if (allowScroll)
        {
            if(CameraMovement.currentPage == targetPage)
            {
                //Debug.Log("reach!");
                reachTarget = true;
            }
        }

       
    }
    public void ChangeCameraSpeed(float speed)

    {
        if (speed < 0) CameraMovement.smoothSpeed = CameraMovement.defaultSpeed;
        CameraMovement.smoothSpeed = speed;
    }


    public void GotoPage(int pageNum)
    {
        //CameraMovement.currentPage = pageNum;
        CameraMovement.Go2Page(pageNum);
    }

    public void JumpToPage(int pageNum)
    {
        CameraMovement.JumpToPage(pageNum);
    }


    public void ReturnToApp()
    {
        myCameraState = CameraState.Subway;
        CameraMovement.atInventory = false;
        CameraMovement.JumpToPage(CameraMovement.previousPage);
        
    }

    public void GoToCloset()
    {
        CameraMovement.previousPage = CameraMovement.currentPage;
        transform.position = new Vector3(-25, 0, -10);
        CameraMovement.atInventory = true;

        //disable fish calling
        // TutorialManagerNew.Hide(TutorialManagerNew.scream);
    }

    public void Swipping(bool value)
    {
        CameraMovement.swipping = value;
    }

    public void DisableInput(bool temp)
    {
        disableInputCG.blocksRaycasts = temp;
    }
}

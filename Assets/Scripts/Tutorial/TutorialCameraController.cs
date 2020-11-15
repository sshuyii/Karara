using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCameraController : MonoBehaviour
{
    CameraMovement CameraMovement;
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

    public int currentPage;
    // Start is called before the first frame update
    void Start()
    {
        myCameraState = CameraState.Subway;
        CameraMovement = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
        CameraMovement.currentPage = 4;

        currentPage = CameraMovement.currentPage;
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

     public void GotoPage(int pageNum)
    {
        CameraMovement.currentPage = pageNum;
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
    }

    public void Swipping(bool value)
    {
        CameraMovement.swipping = value;
    }


}

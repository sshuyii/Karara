using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update

    // private TouchController TouchController;
    private bool moveStart  = false;
    public int currentPage = 1;

    private int moveDirection = 0;
    

    private Transform target;

    private float smoothSpeed = 0.125f;
    private Vector3 offset;
    
    public Vector3[] positions;
    private FinalCameraController FinalCameraController;
    private TutorialCameraController TutorialCameraController;


    public bool atInventory = false;


    public int previousPage = -1;

    public bool swipping = false;


    void Start()
    {
        // positions = new Vector3[4];
        // TouchController =  GameObject.Find("---TouchController").GetComponent<TouchController>();
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        TutorialCameraController = GameObject.Find("Main Camera").GetComponent<TutorialCameraController>();

        if (FinalCameraController == null) currentPage = 4;
        else currentPage = 1;

        
    }

    // Update is called once per frame
    void Update()
    {

        if (swipping) return;


        Vector3 currentPos = Vector3.zero;

        if (atInventory) return;

        if (currentPage > -1 && currentPage < positions.Length)
        {
            

            currentPos = positions[currentPage];
            float diff = currentPos.x - transform.position.x;
            

            if (FinalCameraController != null && FinalCameraController.myCameraState == FinalCameraController.CameraState.Map) return;
            if (FinalCameraController != null && FinalCameraController.myCameraState == FinalCameraController.CameraState.Closet) return;
            if (Mathf.Abs(diff) < 0.3f)
            {
                transform.position = currentPos;
            }

            transform.position = Vector3.Lerp(transform.position, currentPos, 0.1f + Time.deltaTime);
        }

        if (currentPage < 1)
        {
            currentPage = 1;
        }

        if (currentPage >= positions.Length - 1)
        {
            currentPage = positions.Length - 2;
        }

    }



    public void JumpToPreviousPage()
    {
        if(previousPage < 0) return;
        currentPage = previousPage;
        transform.position = positions[currentPage];
        previousPage = -1;
    }

    public void JumpToPage(int pageNum)
    {
        currentPage = pageNum;
        Vector3 currentPos = positions[pageNum];
        transform.position = currentPos;
    }


    //public IEnumerator JumpToPage(int pageNum)
    //{
    //    yield return new WaitForSeconds(1f);

    //    Vector3 currentPos = positions[currentPage];
    //    transform.position = currentPos;
    //}

}

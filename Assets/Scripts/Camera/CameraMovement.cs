using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update

    // private TouchController TouchController;
    private bool moveStart  = false;
    public int currentPage = 1;

    private int moveDirection = 0;
    

    private Transform target;

    public float smoothSpeed = 50f;
    private Vector3 offset;
    
    public Vector3[] positions;
    private FinalCameraController FinalCameraController;
    private TutorialCameraController TutorialCameraController;


    public bool atInventoryOrAd = false;
    public bool atAd= false;

    public int previousPage = -1;

    public bool swipping = false;

    public float movingDist = 0.00001f;
    public float movingTimer = 0f;
    public float T_total = 0;

    private float pageDist;

    public float defaultSpeed = 50f;
    private bool going = false;

    public bool stopForComic = false;

    public GameObject Dots;
    private Color gray,white;
    void Start()
    {
        // positions = new Vector3[4];
        // TouchController =  GameObject.Find("---TouchController").GetComponent<TouchController>();
        FinalCameraController = GameObject.Find("Main Camera").GetComponent<FinalCameraController>();
        TutorialCameraController = GameObject.Find("Main Camera").GetComponent<TutorialCameraController>();

        if (FinalCameraController == null) currentPage = 4;
        else currentPage = 1;
        smoothSpeed = 10f;

        pageDist = Mathf.Abs(positions[0].x - positions[1].x);
        gray = new Color32(140,140,140,255);
        white = new Color32(255,255,255,255);
    }

    // Update is called once per frame
    void Update()
    {

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

    public void Go2Page(int page)
    {

        currentPage = page;
        Vector3 target = positions[currentPage];
        movingDist = Mathf.Abs(target.x - transform.position.x);
        movingTimer = 0f;

        T_total = movingDist / smoothSpeed;

        going = true;
        refreshFinish = false;
        // FinalCameraController.CancelAllUI(false);
        //StartCoroutine(Going());

    }

    private void FixedUpdate()
    {
        
        if (!going || swipping ||atInventoryOrAd||stopForComic) return;

        Vector3 target = positions[currentPage];
        movingTimer += Time.deltaTime;
        float frac = movingTimer / T_total;
        transform.position = Vector3.Lerp(transform.position, target, frac);

        if (Vector3.Distance(target, transform.position) < 0.1 * pageDist)
        {
            transform.position = target;
            smoothSpeed = defaultSpeed;
            if(!refreshFinish) RefreshDots();
            
        }
    }

    bool refreshFinish = false;
    private void RefreshDots()
    {
        refreshFinish = true;
        
        int target = currentPage;
        if(currentPage == 5) target = 4;
        if(currentPage == 0) target = 1;
        for(int i = 0; i < positions.Length; i ++)
        {
            Image dot = Dots.transform.GetChild(i).gameObject.GetComponent<Image>();
            if(i == target) {
                Debug.Log("Around target position " + currentPage);
                dot.color = white;
            }
            else dot.color = gray;
        }
        
    }

    IEnumerator Going()
    {
        
        Vector3 target = positions[currentPage];
        while (movingTimer < 0.7 * T_total)
        {
            movingTimer += Time.deltaTime;
            float frac = movingTimer / T_total;
            transform.position = Vector3.Lerp(transform.position, target, frac);

            if (Vector3.Distance(target, transform.position) < 0.1 * pageDist) break;

            yield return null;
        }

        transform.position = target;
        smoothSpeed = defaultSpeed;
    }

    public bool AroundPosition(float posx)
    {
        if(transform.position.x > posx - 0.5 && transform.position.x < posx + 0.5) return true;
        else return false;
    }
  

}

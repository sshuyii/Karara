using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBossNotification : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject FishBubble,FishText;

    private Animator myAnimator;
    bool isOut;

    public GameObject FishBossUI;

    
    void Start()
    {
        myAnimator = transform.gameObject.GetComponent<Animator>();
        FishBossUI = transform.gameObject;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideFish()
    {
        FishBossUI.SetActive(false);
    }

    public void ShowFish()
    {
        FishBossUI.SetActive(true);
    }



    public void ClickFishBossUI()
    {
        isOut = !isOut;
        if(isOut)
        {
            myAnimator.SetTrigger("MovingOut");
        }
        else
        {
            myAnimator.SetTrigger("MoveIn");
            FishBubble.SetActive(false);
        }

        StartCoroutine(SetIdle());
    }
    IEnumerator SetIdle()
    {
        yield return new WaitForSeconds(1.2f);

        myAnimator.SetTrigger("Idle");


    }

    public void ShowBubble()
    {

        FishBubble.SetActive(true);
    }


}

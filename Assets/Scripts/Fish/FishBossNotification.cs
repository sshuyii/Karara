using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBossNotification : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject FishBubble,FishText;

    private Animator myAnimator;
    bool isOut = false;
    public bool isActive = false;

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
        isActive = true;
        
        //auto hide after five seconds;
        StartCoroutine(AutoHide());

    }

    IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(30f);
        isActive = false;
        HideFish();
        
    }


    public void ClickFishBossUI()
    {
        isOut = !isOut;
        Debug.Log("clickFish");
        if(isOut)
        {
            Debug.Log("clickFish out");
            myAnimator.SetTrigger("MovingOut");
        }
        else
        {
            Debug.Log("clickFish in");
            myAnimator.SetTrigger("MovingIn");
            FishBubble.SetActive(false);
        }

        StartCoroutine(SetIdle());
    }


    IEnumerator SetIdle()
    {
        yield return new WaitForSeconds(2f);

        myAnimator.SetTrigger("Idle");
    }

    public void ShowBubble()
    {

        FishBubble.SetActive(true);
    }


}

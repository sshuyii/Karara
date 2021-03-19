using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FishBossNotification : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject FishBubble;

    [SerializeField]
    TextMeshProUGUI FishText;

    private Animator myAnimator;
    bool isOut = false;
    public bool isActive = false;

    public GameObject FishBossUI;

    string defaultString;

  

    void Start()
    {
        myAnimator = transform.gameObject.GetComponent<Animator>();
        FishBossUI = transform.gameObject;

        defaultString = FishText.text;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideFish()
    {
        FishBossUI.SetActive(false);
        FishText.text = defaultString;
    }

    public void ShowFish(string content)
    {
        FishText.text = content;
        ShowFish();
    }


    public void ShowFish()
    {
        FishBossUI.SetActive(true);
        ClickFishBossUI();
        //auto hide after five seconds;
        StartCoroutine(AutoHide());

    }

    IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(4f);
        ClickFishBossUI();
        
    }


    public void ClickFishBossUI()
    {
        isOut = !isOut;
        Debug.Log("clickFish");
        if(isOut)
        {
            Debug.Log("clickFish out");
            myAnimator.SetTrigger("Idle");
            StartCoroutine(SetIdle(3f));
        }
        else
        {
            Debug.Log("clickFish in");
            myAnimator.SetTrigger("Idle");
            StartCoroutine(SetIdle(1f));

        }

        
    }


    IEnumerator SetIdle(float time)
    {
        yield return new WaitForSeconds(time);

        myAnimator.SetTrigger("Idle");
    }

    public void ShowBubble()
    {

        //FishBubble.SetActive(true);
    }

    
}

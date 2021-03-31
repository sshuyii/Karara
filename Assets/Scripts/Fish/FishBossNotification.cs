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
        isActive = false;
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
        isActive = true;
        ClickFishBossUI();
        //auto hide after five seconds;

    }



    public void ClickFishBossUI()
    {
        isOut = !isOut;
        Debug.Log("clickFish");
        myAnimator.SetTrigger("MoveOut");
        
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

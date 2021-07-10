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

  
    ValueEditor ValueEditor;
    void Start()
    {
        myAnimator = transform.gameObject.GetComponent<Animator>();
        FishBossUI = transform.gameObject;
        ValueEditor = GameObject.Find("---ValueEditor").GetComponent<ValueEditor>();
        defaultString = FishText.text;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideFish()
    {

        // FishBossUI.transform.GetComponent<CanvasGroup>().alpha = 0f;
        // isActive = false;
        // FishText.text = defaultString;
    }

    public void ShowFish(string content)
    {
        // FishText.text = content;
        // StartCoroutine(ShowFish());
    }


    public IEnumerator ShowFish()
    {
        // FishBossUI.transform.GetComponent<CanvasGroup>().alpha = 1f;
        // isActive = true;
        // ClickFishBossUI();

        
        // yield return new WaitForSeconds(ValueEditor.TimeRelated.fishNotificationDisplay);
        // ClickFishBossUI();
        // FishBossUI.transform.GetComponent<CanvasGroup>().alpha = 0f;
        // isActive = false;

        yield return null;

        //auto hide after five seconds;

    }



    public void ClickFishBossUI()
    {
        // isOut = !isOut;
        // Debug.Log("clickFish");
        
    }


    IEnumerator SetIdle(float time)
    {
        // yield return new WaitForSeconds(time);

        // myAnimator.SetTrigger("Idle");
        yield return null;
    }

    public void ShowBubble()
    {
        
        //.SetActive(true);
    }

    
}

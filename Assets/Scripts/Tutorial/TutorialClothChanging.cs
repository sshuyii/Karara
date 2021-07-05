using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class TutorialClothChanging : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    //used for long tap
    private bool tapStart = false;
    private Sprite currentSprite;
    private bool pointerDown;
	private float pointerDownTimer;
    private bool longPress = false;

    [SerializeField]
    private Animator myAnimator;

	private float requiredHoldTime = 0.6f;

    private AudioManager AudioManager;

   
    public TutorialManagerNew TutorialManagerNew;

    [SerializeField]
    private int clothName;    //这件衣服是什么
    [SerializeField]
    private GameObject returnClothNotice;//还包的提示
    private Sprite top;
    private Sprite bottom;

    void Start()
    {
        currentSprite = GetComponent<Image>().sprite;
        top =  TutorialManagerNew.inventoryCloth[0].sprite;
        bottom = TutorialManagerNew.inventoryCloth[1].sprite;
        AudioManager = GameObject.Find("---AudioManager").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pointerDown)
		{
            //取消已经生成的所有ui界面
            for(int i = 0; i < TutorialManagerNew.ReturnNoticeList.Count; i++)
            {            
                TutorialManagerNew.ReturnNoticeList[i].SetActive(false);
            }

            longPress = false;

			pointerDownTimer += Time.deltaTime;
			if (pointerDownTimer >= requiredHoldTime)
			{
                longPress = true;

                //Debug.Log("LongPress");
                //if (!isWorkCloth) InventorySlotMgt.castLongPress();
                //新功能
                showReturnConfirm();
                TutorialManagerNew.HintScreen.SetActive(false);

                //if(!isWorkCloth) showReturnConfirm();
                Reset();
			}
		}
    }


    public void ClickClothInventory()
    {
        //如果长按
        if(longPress) return;
        if(TutorialManagerNew.isWearingClothNum > 2) return;
        

        //点击inventory里的衣服UI
        //理论上每件衣服都得点一遍
        // if (deactiveButtons) return;
        Debug.Log("ClickClothInventory");
        //闪烁动画停止
        myAnimator.SetBool("isShining", false);

        AudioManager.PlayAudio(AudioType.UI_Dialogue);

        //change Cloth
        if(clothName == 1 && !TutorialManagerNew.workClothOn)
        {   

            // if(TutorialManagerNew.isWearingClothNum != 1)
            // {
                TutorialManagerNew.isWearingClothNum ++;

                TutorialManagerNew.inventoryCloth[3].sprite = TutorialManagerNew.workInventory;
                TutorialManagerNew.subwayCloth[3].sprite = TutorialManagerNew.workSubway;
                TutorialManagerNew.workClothOn = true;

                //karara讲话
                TutorialManagerNew.ShowKararaBubble(5,"nice");
        }
        else if(clothName == 2 && !TutorialManagerNew.workShoeOn)
        {
            // if(!TutorialManagerNew.isWearingShoe)
            // {
                // TutorialManagerNew.isWearingShoe = true;
                TutorialManagerNew.isWearingClothNum ++;

                TutorialManagerNew.inventoryCloth[2].sprite = TutorialManagerNew.workShoe;
                // TutorialManagerNew.subwayCloth[2].sprite = TutorialManagerNew.workShoeSubway;//最终还会穿回拖鞋
                TutorialManagerNew.workShoeOn = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
	{
        //AudioManager.PlayAudio(AudioType.UI_Dialogue);
		pointerDown = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Reset();
	}

    private void Reset()
	{
		pointerDown = false;
		pointerDownTimer = 0;
		
	}

    public void hideReturnConfirm()
    {
        //cancelReturnButton.SetActive(false);
        returnClothNotice.SetActive(false);
        //returnConfirmButton.gameObject.SetActive(false);
    }

    IEnumerator KararaDeny()
    {
        returnClothNotice.SetActive(true);

        //karara摇头
        TutorialManagerNew.kararaAnimator.SetTrigger("isShaking");
        //等待两秒之后消失，这两秒内也不能点yes
        yield return new WaitForSeconds(1f); 
        returnClothNotice.SetActive(false);
    }

    public void showReturnConfirm()
    {
        //如果没有把所有衣服都点一遍，不能长按还衣服
        if(TutorialManagerNew.isWearingClothNum < 3) return;

        if(clothName != 2)
        {
            //karara摇头
            TutorialManagerNew.kararaAnimator.SetTrigger("isShaking");
            print("Karara shaking");
            return;
        }

        //如果是两件衣服中的一件，可以还
        //鞋不可以还
        //肯定回到洗衣机里
        returnClothNotice.SetActive(true);
    }

    public void ReturnClothPre()
    {
        tapStart = true;
    }


    public void ReturnCloth()
    {
        ///此时把衣服还到洗衣机里的ui已经出现了，玩家点了yes
        //只能还鞋
        if(clothName != 2) return;
        myAnimator.SetBool("isShining", false);

        //fish boss stop talking
        TutorialManagerNew.inventoryFish.SetActive(false);


        //这个script是挂在inventory的衣服UI上的
        returnClothNotice.SetActive(false);
        longPress = false;

        //inventory里的衣服消失
        this.gameObject.SetActive(false);

        //inventory里karara身上的鞋回到拖鞋
        TutorialManagerNew.inventoryCloth[2].sprite = TutorialManagerNew.SlipperInventory;

        //鞋放回洗衣机里
        TutorialManagerNew.ClothSlotList[1].SetActive(true);
    }

}


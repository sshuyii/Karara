using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReturnNotice : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject UI_minusStar, UI_minusCloth;
    public TextMeshProUGUI starNum, clothNum;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStarNum(int star )
    {
        UI_minusStar.SetActive(true);
        if(star < 0)
        {
            starNum.text = star.ToString();
        }
        else if(star > 0)
        {
            starNum.text = "+" + star;
        }
        else{
            UI_minusStar.SetActive(false);
        }
    }


    public void SetClothNum(int cloth )
    {
        UI_minusCloth.SetActive(true);
        if(cloth < 0)
        {
            clothNum.text = cloth.ToString();
        }
        else if(cloth > 0)
        {
            clothNum.text = " ";
            //如果没少衣服的话就不显示数字了，但是星星+1
        }
        else{
            UI_minusCloth.SetActive(false);
        }
    }
}

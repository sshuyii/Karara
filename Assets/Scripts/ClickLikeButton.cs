using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickLikeButton : MonoBehaviour
{
    Image myImage;
    public Sprite fullHeart;
    Sprite emptyHeart;

    bool isEmpty = true;
    // Start is called before the first frame update
    void Start()
    {
        myImage = GetComponent<Image>();
        emptyHeart = myImage.sprite;
    }


    public void ClickLike()
    {
        if(isEmpty)
        {
            myImage.sprite = fullHeart;
        }
        else{
            myImage.sprite = emptyHeart;
        }
        isEmpty = !isEmpty;

    }
}

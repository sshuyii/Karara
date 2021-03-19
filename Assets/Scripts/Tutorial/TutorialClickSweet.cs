using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialClickSweet : MonoBehaviour
{
    // Start is called before the first frame update

    public Button returnConfirmButton;
    public GameObject returnBG;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickSweet()
    {
        this.gameObject.SetActive(false);
    }


    public void hideReturnConfirm()
    {
        //cancelReturnButton.SetActive(false);
        returnBG.SetActive(false);
        returnConfirmButton.gameObject.SetActive(false);
    }
}


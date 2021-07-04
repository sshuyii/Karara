using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatingSystem : MonoBehaviour
{

    int upperBound = 5;
    int lowerBound = 0;
    public int rating = 5;

    public GameObject[] stars;

    [SerializeField]
    private GameObject particleEffect,StoreRating;

    // Start is called before the first frame update
    LevelManager LevelManager;

    private Transform Dots;
    void Start()
    {
        LevelManager = GameObject.Find("---LevelManager").GetComponent<LevelManager>();
        Dots = this.transform.FindChild("bottomDots");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowRatingSys(bool withParticleEffect)
    {
        StoreRating.SetActive(true);
        if(withParticleEffect) StartCoroutine(ParticleEffect());
    }

    public void LeaveSubway(bool onlyHideDots) {
        //this.gameObject.SetActive(false);
        Dots.GetComponent<CanvasGroup>().alpha = 0f;

        if(onlyHideDots) return;
            

        CanvasGroup UIGroup = this.gameObject.GetComponent<CanvasGroup>();
        UIGroup.alpha = 0f; //this makes everything transparent
        UIGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
        UIGroup.interactable = false;

    }

    public void GoBackToSubway() {
        Dots.GetComponent<CanvasGroup>().alpha = 1f;
        CanvasGroup UIGroup = this.gameObject.GetComponent<CanvasGroup>();
        UIGroup.alpha = 1f; //this makes everything transparent
        UIGroup.blocksRaycasts = true; //this prevents the UI element to receive input events
        UIGroup.interactable = true;
    }

    public void SubStars() {
        
        rating--;
        if (rating < lowerBound)
        {
            rating = lowerBound;
        }
        refreshStars();

    }

    public void AddStars()
    {
        
        Debug.Log("add star in rating");
        rating++;
        if (rating > upperBound)
        {
            rating = upperBound;
        }
        refreshStars();
    }



    private void refreshStars()
    {
        Debug.Log("refresh star");


        for (int i = 0; i < rating; i++)
        {
            stars[i].SetActive(true);
        }

        for (int i = rating; i < upperBound; i++)
        {
            Debug.Log("turn off " + i.ToString());
            stars[i].SetActive(false);
        }

        if(LevelManager.stage > 3)StartCoroutine(ParticleEffect()); 


    }

    IEnumerator ParticleEffect()
    {
        particleEffect.SetActive(true);
        yield return new WaitForSeconds(1f);
        particleEffect.SetActive(false);
    }

    public void ChangeRating(int difference)
    {
        if(LevelManager.stage < 2) return;
        // the number could be less than zero.
        if(difference == 0) return;
        rating += difference;
        if (rating > upperBound) rating = upperBound;
        if(rating < lowerBound) rating = lowerBound;
        refreshStars();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatingSystem : MonoBehaviour
{

    int upperBound = 5;
    int lowerBound = 1;
    public int rating = 5;

    public GameObject[] stars;

    [SerializeField]
    private GameObject particleEffect,StoreRating;

    // Start is called before the first frame update
    void Start()
    {
        
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

    public void LeaveSubway() {
        //this.gameObject.SetActive(false);
        CanvasGroup UIGroup = this.gameObject.GetComponent<CanvasGroup>();
        UIGroup.alpha = 0f; //this makes everything transparent
        UIGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
        UIGroup.interactable = false;
    }

    public void GoBackToSubway() {

        CanvasGroup UIGroup = this.gameObject.GetComponent<CanvasGroup>();
        UIGroup.alpha = 1f; //this makes everything transparent
        UIGroup.blocksRaycasts = true; //this prevents the UI element to receive input events
        UIGroup.interactable = true;
    }

    public void SubStars() {
        
        rating--;
        bool changed = true;
        if (rating < lowerBound)
        {
            changed = false;
            rating = lowerBound;
        }
        refreshStars(changed);

    }

    public void AddStars()
    {
        
        Debug.Log("add star in rating");
        rating++;
        bool changed = true;
        if (rating > upperBound)
        {
            changed = false;
            rating = upperBound;
        }
        refreshStars(changed);
    }



    private void refreshStars(bool changed)
    {
        Debug.Log("refresh star");

        if (!changed) return;

        for (int i = 0; i < rating; i++)
        {
            stars[i].SetActive(true);
        }

        for (int i = rating; i < upperBound; i++)
        {
            Debug.Log("turn off " + i.ToString());
            stars[i].SetActive(false);
        }

        StartCoroutine(ParticleEffect()); 


    }

    IEnumerator ParticleEffect()
    {
        particleEffect.SetActive(true);
        yield return new WaitForSeconds(1f);
        particleEffect.SetActive(false);
    }

}

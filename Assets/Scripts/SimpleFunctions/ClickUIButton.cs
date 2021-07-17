using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickUIButton : MonoBehaviour
{
    
    private AudioManager AudioManager;
    public GameObject tap2D;
    public GameObject tapUI;
    private GameObject tap;
    [SerializeField]
    private Canvas overlayCanvas;

    [Range(0,3f)]
    public float T_tap;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager = GameObject.Find("---AudioManager").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ClickButton(bool isOverUI, Vector3 pos)
    {
        tap = isOverUI? tapUI:tap2D;
        tap.SetActive(true);
        // tap.transform.localPosition = new Vector3(0,0,0);
        // Debug.Log("tap pos input"+ pos.ToString());
        tap.transform.localPosition = pos;
        // Debug.Log("tap pos "+ tap.transform.localPosition.ToString());
        tap.GetComponent<Animator>().SetTrigger("tap");
        StartCoroutine(TapPointDismiss());
    }
    public void ClickButton(GameObject GO)
    {   tap = tap2D;
        AudioManager.UIButtonClicked();
        tap.SetActive(true);
        // tap.transform.localPosition = new Vector3(0,0,0);
        tap.transform.localPosition = GO.transform.position;
        // Debug.Log("tap pos "+ tap.transform.position.ToString());
        tap.GetComponent<Animator>().SetTrigger("tap");
        StartCoroutine(TapPointDismiss());

    }



    // public void ClickButton()
    // {
        
    //     AudioManager.UIButtonClicked();
    //     tap.SetActive(true);
    //     // tap.transform.localPosition = new Vector3(0,0,0);
        
    //     Debug.Log("tap pos "+ tap.transform.position.ToString());
    //     tap.GetComponent<Animator>().SetTrigger("tap");
    //     StartCoroutine(TapPointDismiss());

    // }
    private IEnumerator TapPointDismiss()
    {
        yield return new WaitForSeconds(T_tap);
        
        tap.SetActive(false);
    }

    
}

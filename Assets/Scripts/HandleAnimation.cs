using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleAnimation : MonoBehaviour
{

    private Animator myAnimator;

    private float offset;
    private Animation myAnimation;

    private SubwayMovement SubwayMovement;
    // Start is called before the first frame update

    private float timer = 0.00f;

    public int handleNum;
    private TouchController TouchController;
    
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myAnimation = GetComponent<Animation>();

        float randomIdleStart;
        randomIdleStart = Random.Range(0,myAnimator.GetCurrentAnimatorStateInfo(0).length); //Set a random part of the animation to start from
        myAnimator.Play("HandleAnimation", 0, randomIdleStart);
        SubwayMovement = GameObject.Find("---StationController").GetComponent<SubwayMovement>();
        
        TouchController =  GameObject.Find("---TouchController").GetComponent<TouchController>();

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (SubwayMovement.isMoving)
        {
            myAnimator.SetBool("inStation", false);
        }
        else
        {
            myAnimator.SetBool("inStation", true);

        }

        if(TouchController.myInputState == TouchController.InputState.Tap){
            if(TouchController.RaycastHitResult[0] == "handle" 
            && TouchController.RaycastHitResult[1] == handleNum.ToString()){
                clickHandle();     
            }
        }
    }

    public void clickHandle()
    {
        //Debug.Log("handle clicked");
        myAnimator.SetTrigger("clicked");
    }
}

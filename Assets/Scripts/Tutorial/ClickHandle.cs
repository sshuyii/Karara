using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandle : MonoBehaviour
{

    private Animator myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = this.GetComponent<Animator>();
    }

    public void Handle()
    {
        myAnimator.SetTrigger("click");
        print("点了一下扶手");

    }
}

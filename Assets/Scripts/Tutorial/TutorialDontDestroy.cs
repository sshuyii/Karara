using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDontDestroy : MonoBehaviour
{
    private void Awake() {
        DontDestroyOnLoad(this.gameObject);

    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickNoWay()
    {
        Destroy(this.gameObject);
    }
}

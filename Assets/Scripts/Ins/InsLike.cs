using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsLike : MonoBehaviour
{
    private int likeNumber;
    private Text likeText;
    
    // Start is called before the first frame update
    void Start()
    {
        likeText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        likeText.text = likeNumber.ToString();
    }

    public void Likes()
    {
        likeNumber++;
    }
}

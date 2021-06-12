using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoad : MonoBehaviour
{
    // Start is called before the first frame update
    InstagramController InstagramController;
    void Awake(){
        DontDestroyOnLoad(this.gameObject);
    }

}

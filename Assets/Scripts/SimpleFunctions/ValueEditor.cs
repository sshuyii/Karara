using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ValueEditor : MonoBehaviour
{
    // Start is called before the first frame update
    public TimeRelated TimeRelated;
    public PosterRelated PosterRelated;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

  [System.Serializable]
    public class TimeRelated
    {
        // open washer's door -> delay1 -> full image occures -> delay 2 -> close door 
        public float openWasherDelay1,openWasherDelay2;
        public float fishNotificationDisplay,fishNotificationPeriod;
        public float ClothUIDelay;

        public float fishReturnBagDelay;

        public float scrollPage2and3Delay;


    }


    [System.Serializable]
    public class PosterRelated
    {
      public int shownUsedDifference; //1

      public int S1UpperBound; //3
      public int S2UpperBound; //3
    }



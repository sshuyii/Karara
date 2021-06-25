using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ValueEditor : MonoBehaviour
{
    // Start is called before the first frame update

    public bool test;
    public TimeRelated TimeRelated;
    public PosterRelated PosterRelated;

    [HideInInspector]
    public TestingValues TestingValues;

    

    void Start()
    {
        TestingValues = transform.GetComponent<TestingValues>();
        if(test) ReplaceValuesForTest();

    }

    public void ReplaceValuesForTest()
    {
      TimeRelated = TestingValues.TimeRelated;
      PosterRelated = TestingValues.PosterRelated;
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

        public float TrainStayTime, TrainMovingTime;


    }


    [System.Serializable]
    public class PosterRelated
    {
      public int shownUsedDifference; //1

      public int S1UpperBound; //3
      public int S2UpperBound; //3

          //hard coding 4 poses
    // 1 top only 2 top + bottom  3 (top + bottom)/Everything + shoes 
      [Tooltip("1 top only 2 top + bottom  3 (top + bottom)/Everything + shoes ")]
      public List<int> ClothesNumShownInPose;

    }


    
 


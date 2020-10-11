using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectScreenRatio : MonoBehaviour
{
    public CanvasScaler myCS;
    
    // Start is called before the first frame update
    void Start()
    {
        if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX)
        {
            GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 0.6f);

//            myCS.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone7)
        {
//            myCS.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
//            myCS.referenceResolution = new Vector2(790, 1400);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

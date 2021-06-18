using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class StartTutorialNew : MonoBehaviour
{

    [SerializeField]
    private Image door;

    [SerializeField]
    private Sprite openDoor, closeDoor;

    [SerializeField]
    private GameObject Title;



    [SerializeField]
    private Animator doorAnimator;

    // [SerializeField]
    // private GameObject iconBackground;

    // [SerializeField]
    // private float endPoint, startPoint, speedA;



    [Header("Camera Movement")]
    private GameObject MainCamera;
    private RectTransform CameraRT;

    [Header("Audio")]
    [SerializeField]
    private AudioSource audioData;
    [SerializeField]
    private AudioClip doorClose;
    [SerializeField]
    private AudioClip machineWashing;




    
    // Start is called before the first frame update
    void Start()
    {
        MainCamera = GameObject.Find("Main Camera");

       
    }

    // Update is called once per frame
    void Update()
    {
        // //for background moving
        // if(iconBackground.transform.position.x < endPoint)
        // {
        //     iconBackground.transform.position += new Vector3(speedA, 0, 0);
        // }
        // else
        // {
        //     iconBackground.transform.position = new Vector3 (startPoint, 0, 0);
        // }
       
    }
    public void ClickMachine()
    {
        StartCoroutine(StartGame());
        print("click machine");
    }

    private void PlayAudio(AudioClip clip)
    {
        audioData.clip = clip;
        audioData.Play();
    }

    IEnumerator StartGame()
    {
        Title.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        door.sprite = closeDoor;
        //播放一个摔门的声音
        PlayAudio(doorClose);
        yield return new WaitForSeconds(0.3f);

        doorAnimator.SetBool("isRotating", true);
        PlayAudio(machineWashing);

        yield return new WaitForSeconds(3f);

        //教程最开始，镜头拉远
        CameraRT = MainCamera.GetComponent<RectTransform>();
        CameraRT.position = new Vector3(33f, 0, -10f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("TutorialScene");

        //wait until the asynchronous scene fully loads
        while(!asyncLoad.isDone)
        {
            yield return null;
        }

    }

    public void clickSkip()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("StreetStyle");

        print("click skip");

    }
}

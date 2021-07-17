
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FishBoss : MonoBehaviour
{
    public bool isTutorial;
    // Start is called before the first frame update
    [SerializeField]
    GameObject  Fish2D, FishUI, FishTalkButton;


    [SerializeField]
    TextMeshPro fishText2D;
    [SerializeField]
    TextMeshProUGUI fishTextUI;

    FishText currentFT;

    bool isAtFirstScreen = true;
    bool fishForward;
    int currentFS;
    string defaultString;
    private List<AudioType> fishBubbles = new List<AudioType> {AudioType.FishBubble2,
        AudioType.FishBubble3, AudioType.FishBubble4,AudioType.FishBubble5, AudioType.FishBubble6, AudioType.FishBubble7};


    ValueEditor ValueEditor;

    CameraMovement CameraMovement;
    FinalCameraController FinalCameraController;
    TutorialCameraController TutorialCameraController;
    AudioManager AudioManager;
    int screenNum;

    GameObject cam;

    public bool isFishTalking;

    float leftX, rightX;

    Coroutine lastRoutine = null;

    TutorialManagerNew TutorialManagerNew;
    FishTextManager FishTextManager;
    void Start()
    {

        FishUI = transform.gameObject;
        ValueEditor = GameObject.Find("---ValueEditor").GetComponent<ValueEditor>();


        cam = GameObject.Find("Main Camera");
        CameraMovement = cam.GetComponent<CameraMovement>();
        FinalCameraController = cam.GetComponent<FinalCameraController>();
        TutorialCameraController = cam.GetComponent<TutorialCameraController>();
        TutorialManagerNew = GameObject.Find("---TutorialController").GetComponent<TutorialManagerNew>();
        FishTextManager = GameObject.Find("---FishTextManager").GetComponent<FishTextManager>();
        AudioManager = GameObject.Find("---AudioManager").GetComponent<AudioManager>();

        leftX = CameraMovement.positions[0].x - 1f;
        rightX = CameraMovement.positions[0].x + 1f;


    }

    void Update()
    {
        AtScreenOneOrNot();
    }
    private void AtScreenOneOrNot()
    {
        isAtFirstScreen = false;
        if ((isTutorial && TutorialCameraController.myCameraState == CameraState.Subway) ||
            !isTutorial && FinalCameraController.myCameraState == CameraState.Subway)
        {
            float x = cam.transform.position.x;
            if (x < rightX && x > leftX) isAtFirstScreen = true;
        }
    }


    public void ActiveFishes()
    {
        if (isAtFirstScreen)
        {
            Fish2D.SetActive(true);
            FishUI.SetActive(false);
        }
        else
        {
            Fish2D.SetActive(false);
            FishUI.SetActive(true);
        }
    }

    public void DeactiveFishes()
    {
        Fish2D.SetActive(false);
        FishUI.SetActive(false);
    }


    public void ResetFish()
    {
        isFishTalking = false;
        
    }
    public void FishTalk(string keyWord, bool animateOrNot, bool forward)
    {
        if(!isFishTalking)
        {
            FishTalkButton.SetActive(true);
            currentFT = FishTextManager.GetText(keyWord);
            fishForward = forward;
            FishTalk(animateOrNot);
        }
        else DeactiveFishes();
        
        isFishTalking = !isFishTalking;

    }



    public void FishTalk(bool animateOrNot)
    {
        ActiveFishes();

        int idx = currentFT.playingIdx;
        if (animateOrNot)
        {
            lastRoutine = StartCoroutine(AnimateText(currentFT.content[idx]));
        }
        else
        {
            //when last sentence
            if (currentFT.playingIdx > currentFT.content.Count - 2)
            {
                FishTalkButton.SetActive(false);
                currentFT.isPlaying = false;
                if (isTutorial) TutorialManagerNew.forwardOneStep = fishForward;
                // forwardOneStep = fishForward;
            }

            if (isAtFirstScreen) fishText2D.text = currentFT.content[idx];
            else fishTextUI.text = currentFT.content[idx];
        }
    }


    IEnumerator AnimateText(string textContent)
    {
        currentFT.isPlaying = true;
        for (int i = 0; i < (textContent.Length + 1); i++)
        {
            if (!currentFT.isPlaying) break;

            int random = UnityEngine.Random.Range(0, 5);
            AudioManager.PlayAudio(fishBubbles[random]);
            if (isAtFirstScreen) fishText2D.text = textContent.Substring(0, i);
            else fishTextUI.text = textContent.Substring(0, i);
            yield return new WaitForSeconds(.05f);

            //如果进行到最后一句话的最后一个字母，FishTalkButton可以被拿掉
            if (i == textContent.Length && currentFT.playingIdx > currentFT.content.Count - 2)
            {
                FishTalkButton.SetActive(false);
                if (isTutorial) TutorialManagerNew.forwardOneStep = fishForward;
            }
        }
        yield return null;
    }
    public void ClickWhileFishTalking()
    {
        Debug.Log("Click While Fish Talking");
        if (currentFT.isPlaying)
        {
            ShowWholeSentence();
        }
        else if (!currentFT.isPlaying)
        {
            FishTalkNextSentence();
        }
    }

    void ShowWholeSentence()
    {
        currentFT.isPlaying = false;
        StopCoroutine(lastRoutine);
        FishTalk(false);
    }
    private void FishTalkNextSentence()
    {
        Debug.Log("reset " + currentFT.playingIdx);
        currentFT.playingIdx = currentFT.playingIdx + 1;
        Debug.Log("reset " + currentFT.playingIdx);

        if (currentFT.playingIdx < currentFT.content.Count) FishTalk(true);
        else
        {
            Debug.Log("reset 0 ");
            currentFT.playingIdx = currentFT.playingIdx - 1;
            FishTalk(false);
            currentFT.playingIdx = 0;
        }
    }
}

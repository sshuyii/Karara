using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class StartTutorial : MonoBehaviour
{

    public AudioSource clickButton;
    public float speedA;
    public CanvasGroup Menu;
    public SpriteRenderer Title;

    public Vector3 startingPoint;
    public float endPoint;

    public GameObject iconBackground;
    
    public bool isComic;
    public bool isComic0;
    public bool isComic1;
    public bool isComic2;
    public bool isComic3;

    public SpriteRenderer Music;
    public SpriteRenderer Sound;
    public SpriteRenderer Realtime;

    public Sprite noMusic;
    public Sprite noSound;
    
    public Sprite noRealtime;
    
    private Sprite yesMusic;
    private Sprite yesSound;
    private Sprite yesRealtime;
    
    private bool isTutorial;

    public GameObject comicBackground;
    public GameObject comic1;
    public GameObject comic2;
    public GameObject comic3;
    public GameObject comic4;
    public GameObject comic5;


    public Sprite openMachine;
    public Sprite closeMachine;
    public SpriteRenderer Machine;
    public SpriteRenderer credit;
    public SpriteRenderer Shuyi;
    public SpriteRenderer setting;
    public CanvasGroup settingMenu;
    
    private bool flash;
    public CanvasGroup myFlash;
    

    public float speed = 1f;
    
    private bool startTutorial;
    public Vector3 creditVector3;
    public Vector3 settingVector3;

    public GameObject Camera;

    private Color MachineColor;
    private Color TitleColor;
    public CanvasGroup ComicButton;

   

    enum MenuState
    {
        credit,
        setting,
        none
    }

    private MenuState myMenuState;
    
    // Start is called before the first frame update
    void Start()
    {
        
        myMenuState = MenuState.none;

        Hide(ComicButton);
        credit.enabled = false;
        Shuyi.enabled = false;
        Machine.sprite = closeMachine;
        setting.enabled = false;
        Music.enabled = false;
        Sound.enabled = false;
        Realtime.enabled = false;
        Hide(settingMenu);

        yesMusic = Music.sprite;
        yesSound = Sound.sprite;
        yesRealtime = Realtime.sprite;
        
        startingPoint = iconBackground.transform.position;
        comicBackground.SetActive(false);
        comic1.SetActive(false);
        comic2.SetActive(false);
        comic3.SetActive(false);
        comic4.SetActive(false);
        comic5.SetActive(false);
        
        Hide(ComicButton);

    }

    // Update is called once per frame
    void Update()
    {
        //set state
        if (myMenuState == MenuState.credit)
        {
            Camera.transform.position = Vector3.MoveTowards(Camera.transform.position,creditVector3,0.25f);
        }
        else if (myMenuState == MenuState.none)
        {
            Camera.transform.position = Vector3.MoveTowards(Camera.transform.position,new Vector3(0,0,-10), 0.25f);

        }
        else if (myMenuState == MenuState.setting)
        {
            Camera.transform.position = Vector3.MoveTowards(Camera.transform.position,settingVector3, 0.25f);

        }
        
        //for flash
        if (flash)
        {
                myFlash.alpha = myFlash.alpha - Time.deltaTime;

                if (myFlash.alpha <= 0)
                {
                    myFlash.alpha = 0;
                    flash = false;

                    if (startTutorial)
                    {
                        SceneManager.LoadScene("TutorialScene", LoadSceneMode.Single);
                        startTutorial = false;
                    }
                    else
                    {
                        isComic = true;
                        comic1.SetActive(true);
                    }
                }
                
        }

        if (comicStart)
        {

            Menu.alpha -= 0.5f * Time.deltaTime;
            MachineColor.a -= 0.5f * Time.deltaTime;
            TitleColor.a -= 0.5f * Time.deltaTime;
            
            if (MachineColor.a < 0)
            {
                MachineColor.a = 0;
                Menu.alpha = 0;
                TitleColor.a = 0;
                isComic = true;
                comicStart = false;
                
                comicBackground.SetActive(true);
                comic1.SetActive(true);
                Show(ComicButton);
                Hide(Menu);
            }

            Machine.color = MachineColor;
            Title.color = MachineColor;

        }

        //for background moving
        if(iconBackground.transform.position.x < endPoint)
        {
            iconBackground.transform.position += new Vector3(speedA, 0, 0);
        }
        else
        {
            iconBackground.transform.position = startingPoint;
        }

        
        if (isComic0)
        {
            //(comic1, 0, speed, isComic1, true);
            if(comic1.GetComponent<RectTransform>().anchoredPosition.x < 0)
            {
                comic1.GetComponent<RectTransform>().anchoredPosition += new Vector2(speed, 0);
            }
            else
            {
                isComic0 = false;
            }
        }
        else if (isComic1)
        {
            if(comic2.GetComponent<RectTransform>().anchoredPosition.x > 0)
            {
                comic2.GetComponent<RectTransform>().anchoredPosition -= new Vector2(speed, 0);
            }
            else
            {
                isComic1 = false;
                isComic2 = true;
            }
        }
        else if(isComic3)
        {
            comic3.GetComponent<Image>().enabled = true;
            isTutorial = true;
        }
    }

    private bool comicStart;
    public void TutorialStartPre()
    {
        clickButton.Play();
        int ongoingChapter = PlayerPrefs.GetInt("ongoingChapter",0);
        if(ongoingChapter == 0){
            comicStart = true;
        }
        else if(ongoingChapter == 1){
            // no tutoiral, start from chap 1;
            SceneManager.LoadScene("StreetStyle", LoadSceneMode.Single);
        }
        else if(ongoingChapter == 2){
            // start from chapter 2, no tutorial 
        }

    }

    public void clickMusic()
    {
        clickButton.Play();

        if (Music.sprite == noMusic)
        {
            Music.sprite = yesMusic;
        }
        else
        {
            Music.sprite = noMusic;
        }
            
    }
    public void clickSound()
    {
        clickButton.Play();

        if (Sound.sprite == noSound)
        {
            Sound.sprite = yesSound;
        }
        else
        {
            Sound.sprite = noSound;
        }
    }
    public void clickRealtime()
    {
        clickButton.Play();

        if (Realtime.sprite == noRealtime)
        {
            Realtime.sprite = yesRealtime;
        }
        else
        {
            Realtime.sprite = noRealtime;
        }
    }
    public void clickCredit()
    {
        clickButton.Play();

        StartCoroutine(MachineDoorOpenCredit());
        myMenuState = MenuState.credit;
    }
    
    
    
    public void clickSetting()
    {
        clickButton.Play();

        StartCoroutine(MachineDoorOpenSetting());
        myMenuState = MenuState.setting;
    }

    public void clickMachine()
    {
        clickButton.Play();

        credit.enabled = false;
        Shuyi.enabled = false;
        Machine.sprite = closeMachine;
        setting.enabled = false;
        Music.enabled = false;
        Sound.enabled = false;
        Realtime.enabled = false;
        Hide(settingMenu);

        myMenuState = MenuState.none;

    }
    IEnumerator MachineDoorOpenCredit()
    {
        yield return new WaitForSeconds(0.5f);
        Machine.sprite = openMachine;
        yield return new WaitForSeconds(0.5f);
        credit.enabled = true;
        yield return new WaitForSeconds(0.3f);
        Shuyi.enabled = true;


    }
    
    IEnumerator MachineDoorOpenSetting()
    {
        yield return new WaitForSeconds(0.5f);
        Machine.sprite = openMachine;
        yield return new WaitForSeconds(0.5f);
        setting.enabled = true;
        yield return new WaitForSeconds(0.3f);
        Show(settingMenu);
        Music.enabled = true;
        Sound.enabled = true;
        Realtime.enabled = true;


    }
    
    
    public void GoToChapterOne()
    {
        clickButton.Play();

        SceneManager.LoadScene("StreetStyle", LoadSceneMode.Single);
        //comic starts to fly in

    }

    private void ComicFlyIn(GameObject comic, float stopPoint, float speed, bool returnTrue, bool fromLeft)
    {
        if (fromLeft)
        {
            if(comic.GetComponent<RectTransform>().anchoredPosition.x < stopPoint)
            {
                comic.GetComponent<RectTransform>().anchoredPosition += new Vector2(speed, 0);
            }
            else
            {
                returnTrue = true;
                isComic0 = false;
                //print("trrrrrrrr");
            }
        }
        else
        {
            if(comic.GetComponent<RectTransform>().anchoredPosition.x > stopPoint)
            {
                comic.GetComponent<RectTransform>().anchoredPosition += new Vector2(speed, 0);
            }
            else
            {
                returnTrue = true;
            }
        }
    }
    
    void Hide(CanvasGroup UIGroup) {
        UIGroup.alpha = 0f; //this makes everything transparent
        UIGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
        UIGroup.interactable = false;

    }
    
    void Show(CanvasGroup UIGroup) {
        UIGroup.alpha = 1f; //this makes everything transparent
        UIGroup.blocksRaycasts = true; //this prevents the UI element to receive input events
        UIGroup.interactable = true;

    }

    private int clickTime = 0;
    public void ComicClick()
    {
        if(isTutorial)
        {
            //flash = true;
            
        }
      
//        if(isComic0 == false)
//        {
//            flash = true;
//            if (clickTime == 1)
//            {
//                comic1.enabled = true;
//            }
//            else if (clickTime == 2)
//            {
//                comic2.enabled = true;
//            }
//            else if (clickTime == 3)
//            {
//                comic3.enabled = true;
//            }
//            else if (clickTime == 4)
//            {
//                comic4.enabled = true;
//            }
//            else if (clickTime == 5)
//            {
//                SceneManager.LoadScene("TutorialScene", LoadSceneMode.Single);
//            }
//        }
        else if(isComic)
        {
            clickButton.Play();

            clickTime++;
            if (clickTime == 1)
            {
                comic2.SetActive(true);
            }
            else if (clickTime == 2)
            {
                comic3.SetActive(true);
            }
            else if (clickTime == 3)
            {
                comic4.SetActive(true);
            }
            else if (clickTime == 4)
            {
                comic1.SetActive(false);
                comic2.SetActive(false);
                comic3.SetActive(false);
                comic4.SetActive(false);

                comic5.SetActive(true);
            }
            else if(clickTime == 5)
            {
                SceneManager.LoadScene("TutorialScene", LoadSceneMode.Single);
                // myFlash.alpha = 1;
                startTutorial = true;

            }
        }

    }
}

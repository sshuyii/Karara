using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingButton : MonoBehaviour
{
    public Sprite music;

    public Sprite sound;

    public Sprite realtime;

    public Sprite music_no;
    public Sprite sound_no;
    public Sprite realtime_no;

    public SpriteRenderer musicImageRender;
    public SpriteRenderer soundImageRender;
    public SpriteRenderer realtimeImageRender;
    private TouchController TouchController;
    
    // Start is called before the first frame update
    void Start()
    {
        TouchController =  GameObject.Find("---TouchController").GetComponent<TouchController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(TouchController.myInputState == TouchController.InputState.Tap){
            string buttonName = TouchController.RaycastHitResult[0];
            switch(buttonName){
                case "Music":
                    clickMusic();
                break;
                case "Sound":
                    clickSound();
                break;
                case "Realtime":
                    clickRealtime();
                break;
                default:
                break;
            }
        }
    }

    public void clickMusic()
    {
        if(musicImageRender.sprite != music)
        {
            musicImageRender.sprite = music;
        }
        else
        {
            musicImageRender.sprite = music_no;
        }
    }
    
    public void clickSound()
    {
        if(soundImageRender.sprite != sound)
        {
            soundImageRender.sprite = sound;
        }
        else
        {
            soundImageRender.sprite = sound_no;
        }
    }
    
    public void clickRealtime()
    {
        if(realtimeImageRender.sprite != realtime)
        {
            realtimeImageRender.sprite = realtime;
        }
        else
        {
            realtimeImageRender.sprite = realtime_no;
        }
    }
}

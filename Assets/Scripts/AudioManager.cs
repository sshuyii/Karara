using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{

    public AudioTrack[] m_Tracks;
    Dictionary<AudioType, AudioTrack> m_AudioTable = new Dictionary<AudioType, AudioTrack>();
    List<AudioType> ArrivingList = new List<AudioType> { AudioType.Subway_Arriving_1, AudioType.Subway_Arriving_2 }; //刹车声

    List<AudioType> StayingList= new List<AudioType> { AudioType.Subway_Staying_1, AudioType.Subway_Staying_2,
                                        AudioType.Subway_Staying_3,AudioType.Subway_Staying_4}; //站内开门后
    List<AudioType> PreMovingList = new List<AudioType> { AudioType.Subway_PreMoving_1, AudioType.Subway_PreMoving_2,
                                        AudioType.Subway_PreMoving_3, AudioType.Subway_PreMoving_4};
    List<AudioType> MovingList = new List<AudioType> { AudioType.Subway_Moving_1, AudioType.Subway_Moving_2 };

    System.Random random = new System.Random();

    [Range(0.5f, 2f)]
    public float DoorTime;


    [System.Serializable]
    public class AudioObject
    {
        public AudioType m_type;
        public AudioClip clip;

    }


    [System.Serializable]
    public class AudioTrack
    {
        public AudioSource source;
        public AudioObject[] audio;
    }


    // Start is called before the first frame update
    void Start()
    {
        foreach(AudioTrack track in m_Tracks)
        {

            //Debug.Log("this audio track has " + track.audio.Length);

            foreach (AudioObject audio in track.audio)
            {
                if (!m_AudioTable.ContainsKey(audio.m_type))
                {
                    //Debug.Log("the audio type" + audio.m_type);
                    m_AudioTable.Add(audio.m_type, track);
                }
            }
            
        }

        //Debug.Log("the audio type" + m_AudioTable.Count);

    }


    // Update is called once per frame
    void Update()
    {
        
    }


    public void PlayAudio(AudioType _type)
    {
        //Debug.Log("the audio type" + _type);
        AudioTrack m_track = m_AudioTable[_type];
        if (m_track.source.isPlaying) m_track.source.Stop();

        m_track.source.clip = GetAudioClipFromTrack(_type, m_track);


        if(m_track.source.clip != null)
        {
            Debug.Log("play");
            m_track.source.Play();
        }
    }


    public void PauseAudio(AudioType _type)
    {
        AudioTrack m_track = m_AudioTable[_type];
        m_track.source.Stop();
    }



    public void AdjustPitch(AudioType _type,float p)
    {
        AudioTrack m_track = m_AudioTable[_type];
        m_track.source.pitch = p;

    }


    private AudioClip GetAudioClipFromTrack(AudioType _type, AudioTrack _track)
    {

        foreach (AudioObject audio in _track.audio)
        {
            if (audio.m_type == _type)
            {
                return audio.clip;
            }

        }
        return null;
    }




    public IEnumerator PlayAudioInStation(float stayTime)
    {
        //开门声 t1
        
        PlayAudio(AudioType.Subway_OpenDoor);
        yield return new WaitForSeconds(DoorTime);

        //开门后站内声音 t2
        int randomIdx = random.Next(StayingList.Count);
        PlayAudio(StayingList[randomIdx]);
        yield return new WaitForSeconds(stayTime-DoorTime);
        PauseAudio(StayingList[randomIdx]);
    }


    public IEnumerator PlayAudioOutStation(float movingTime)
    {
        //关门声 t1
        PlayAudio(AudioType.Subway_CloseDoor);
        yield return new WaitForSeconds(DoorTime);

        // 启动出站 t2
        int randomIdx = random.Next(PreMovingList.Count);
        PlayAudio(PreMovingList[randomIdx]);
        yield return new WaitForSeconds(2*DoorTime);

        // t3 
        randomIdx = random.Next(MovingList.Count);
        PlayAudio(MovingList[randomIdx]);
        yield return new WaitForSeconds(movingTime - 5* DoorTime);
        PauseAudio(MovingList[randomIdx]);


        //t2
        randomIdx = random.Next(ArrivingList.Count);
        PlayAudio(ArrivingList[randomIdx]);
        yield return new WaitForSeconds(2 * DoorTime);
        PauseAudio(ArrivingList[randomIdx]);

    }


    public void UIButtonClicked()
    {
        PlayAudio(AudioType.UI_Dialogue);
    }

   
}


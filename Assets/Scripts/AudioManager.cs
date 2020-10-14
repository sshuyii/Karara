using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{

    public AudioTrack[] m_Tracks;
    Dictionary<AudioType, AudioTrack> m_AudioTable = new Dictionary<AudioType, AudioTrack>();



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

            Debug.Log("this audio track has " + track.audio.Length);

            foreach (AudioObject audio in track.audio)
            {
                if (!m_AudioTable.ContainsKey(audio.m_type))
                {
                    m_AudioTable.Add(audio.m_type, track);
                }
            }
            
        }

    }


    // Update is called once per frame
    void Update()
    {
        
    }


    public void PlayAudio(AudioType _type)
    {
        AudioTrack m_track = m_AudioTable[_type];
        if (m_track.source.isPlaying) m_track.source.Stop();

        m_track.source.clip = GetAudioClipFromTrack(_type, m_track);

        
        if(m_track.source.clip != null)
        {
            m_track.source.Play();
        }
    }


    public void PauseAudio(AudioType _type)
    {
        AudioTrack m_track = m_AudioTable[_type];
        m_track.source.Stop();
    }

    



    private AudioClip GetAudioClipFromTrack(AudioType _type, AudioTrack _track)
    {

        foreach(AudioObject audio in _track.audio)
        {
            if(audio.m_type == _type)
            {
                return audio.clip;
            }

        }
        return null;
    }


   
}


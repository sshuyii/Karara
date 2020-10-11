using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class Ad
{

    public string name;
    public Sprite sprite;
    public int[] attributes = new int[4] { 0,0,0,0};

    public bool hasTaken;
    public int poseNum = -1;
    // Start is called before the first frame update
    public Ad(string myName, Sprite mySprite, string atbs) {
        name = myName;
        sprite = mySprite;
        hasTaken = false;

        string[] splited = atbs.Split(' ');
        for(int i = 0; i<attributes.Length; i++)
        {
            attributes[i] = Int32.Parse(splited[i + 1]);
        }
    }

    public void TakeAPhoto(int pose)
    {
        hasTaken = true;
        poseNum = pose;
    }
}

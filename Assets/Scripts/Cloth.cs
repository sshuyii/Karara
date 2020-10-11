using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

public class Cloth
{
    // Start is called before the first frame update


    public string name;
    public enum ClothType {
        Bottom,
        Top,
        Shoe,
        Everything
    }

    public enum ClothState {
        None,
        JustGenerated,
        JustAssignedMachine,
        Washing,
        Finished,
        Wearing,
        InventoryNotWear,
        LandF
    }

    public string owner;
    //todo: 暂定为4个
    public int[] attributes = new int[4] { 0,0,0,0};

    // 0 inventory 1 subway 2 ui
    //dictionary占用的空间多 所以改用array
    public Sprite[] spritesInScenes = new Sprite[3];

    public List<Sprite> spritesInPos = new List<Sprite>();

    public bool isWearing = false;
    public bool atInventory = false;



    public int layer = 0;

    public ClothType type;


    public ClothState state;

    public Cloth(string clothName, Sprite firstPic, ClothType thisType, string thisOwner, string atbs)
    {
        name = clothName;
        spritesInScenes[0] = firstPic;
        type = thisType;
        owner = thisOwner;
        state = ClothState.None;

        string[] splited = atbs.Split(' ');
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i] = Int32.Parse(splited[i + 1]);
        }
    }


    public Cloth()
    {
        name = "empty";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC
{

    //    - name: string
    //- profile: sprite
    //- clothes list
    //    - clothes name: string
    //- posts:  <--- todo
    //- homeStation: int

    public string name;
    public string fullName;
    public Sprite profile;
    public Sprite closeBag;
    public Sprite openBag;
    public Sprite bagLogo;
    public List<string> myClothes = new List<string>();
    public HashSet<int> usedIdx = new HashSet<int>(); 

    public int homeStation;


    public List<Sprite> shownPosts = new List<Sprite>();
    public List<Sprite> allPosts = new List<Sprite>();

    public List<string> myClothesInLF = new List<string>();


    public NPC(string NPCName, string full)
    {
        name = NPCName;
        fullName = full;

        openBag = Resources.Load<Sprite>("Images/NPC/"+ name + "/Bag/" + fullName);
        closeBag = Resources.Load<Sprite>("Images/NPC/" + name + "/Bag/Bag" + name );
        bagLogo = Resources.Load<Sprite>("Images/NPC/" + name + "/Bag/Bag" + name+"_logo");
        profile = Resources.Load<Sprite>("Images/NPC/" + name + "/Profile/NPC_" + name + "_Profile");


        object[] pics = Resources.LoadAll("Images/NPC/" + name + "/Post/Chapt1", typeof(Sprite));
        for (int i = 0; i < pics.Length; i++)
        {
            allPosts.Add((Sprite)pics[i]);
        }

        shownPosts = allPosts;
        //Debug.Log("图片地址"+openBag.name);

    }


    public NPC()
    {
        name = "Karara";
        profile = Resources.Load<Sprite>("Images/Karara/Profile/karara_s_glow");
    }

    public Sprite PostPic()
    {
        if (allPosts.Count == 0) return profile;
        Sprite result = allPosts[0];
        shownPosts.Add(result);
        allPosts.RemoveAt(0);
        return result;
    }
}

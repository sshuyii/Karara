using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

public class SpriteLoader : MonoBehaviour
{

    
    public Dictionary<string,NPC> NPCDic;
    public Dictionary<string, Cloth> ClothDic;

    private string mainPath = "Images/Cloth/";
    private string[] scenes = new string[3]{ "Inventory", "Subway", "UI" };
    private string[] clothTypes = new string[4] { "Bottom", "Top", "Shoe", "Everything" };
    private string[] postures = new string[4] { "Pose1_1", "Pose1_2", "Pose1_3", "Pose1_4" };

    public Cloth[] defaultClothes = new Cloth[4];


    // Start is called before the first frame update
    void Start()
    {





        NPCDic = new Dictionary<string, NPC>();
        ClothDic = new Dictionary<string, Cloth>();
        //todo:把各个关卡的NPCName等存储成文件数据，直接load
        List<string> npcFullNamesChap1 = new List<string>() { "Alex", "Bella" ,"Nami","Dolly", "X"};
        List<string> npcNameChap1 = new List<string>() { "A", "B", "C", "D","X" };

        for(int i = 0; i< npcNameChap1.Count; i++)
        {
            string name = npcNameChap1[i];
            NPCDic.Add(name, new NPC(name, npcFullNamesChap1[i]));
        }


        

        LoadClothes();
        LoadKararaClothes();

        ClothDic["work"].state = Cloth.ClothState.Wearing;
        ClothDic["shoe"].state = Cloth.ClothState.Wearing;

        defaultClothes[0] = ClothDic["top"];
        defaultClothes[1] = ClothDic["bottom"];
        defaultClothes[2] = ClothDic["shoe"];
        defaultClothes[3] = ClothDic["work"];

        //Debug.Log("你猜" + defaultClothes[0].name);
        NPCDic.Add("Karara", new NPC());


        foreach(KeyValuePair<string,Cloth> pair in ClothDic)
        {
            if (pair.Value.spritesInPos.Count != 4) Debug.Log("这有问题！！！！！！！" + pair.Key);
        }

    }




    // Update is called once per frame
    void Update()
    {
        
    }

    private void ReadClothAttributes()
    {
        
    }


    private string CheckOwner(string picName)
    {
        string owner = "Karara";
        // A B C Z
        Regex mRegular = new Regex(@"\S+A\d+", RegexOptions.None);
        if(mRegular.IsMatch(picName)){
            owner = "A";
        }

        mRegular = new Regex(@"\S+B\d+", RegexOptions.None);
        if (mRegular.IsMatch(picName))
        {
            owner = "B";
        }

        mRegular = new Regex(@"\S+C\d+", RegexOptions.None);
        if (mRegular.IsMatch(picName))
        {
            owner = "C";
        }

        mRegular = new Regex(@"\S+D\d+", RegexOptions.None);
        if (mRegular.IsMatch(picName))
        {
            owner = "D";
        }


        mRegular = new Regex(@"\S+Z\d+", RegexOptions.None);
        if (mRegular.IsMatch(picName))
        {
            owner = "X";
        }


        return owner;
        
    }


    private Cloth.ClothType CheckType(string type) {
        Cloth.ClothType thisType;
        switch (type)
        {
            case "Bottom":
                thisType = Cloth.ClothType.Bottom;
                break;
            case "Top":
                thisType = Cloth.ClothType.Top;
                break;

            case "Shoe":
                thisType = Cloth.ClothType.Shoe;
                break;

            case "Everything":
                thisType = Cloth.ClothType.Everything;
                break;
            default:
                thisType = Cloth.ClothType.Everything;
                break;


        }
        return thisType;
    }



    private void LoadKararaClothes()
    {


        for (int i = 0; i < scenes.Length; i++)
        {
            string path = "Images/Karara/Cloth/" + scenes[i];
            object[] loadedPics = Resources.LoadAll(path, typeof(Sprite));

            for (int j = 0; j < loadedPics.Length; j++)
            {
                Sprite newPic = (Sprite)loadedPics[j];
                string picName = newPic.name;

                Cloth.ClothType thisType;
                switch (picName)
                {
                    case "bottom":
                        thisType = Cloth.ClothType.Bottom;
                        break;
                    case "top":
                        thisType = Cloth.ClothType.Top;
                        break;
                    case "work":
                        thisType = Cloth.ClothType.Everything;
                        break;
                    case "shoe":
                        thisType = Cloth.ClothType.Shoe;
                        break;
                    default:
                        thisType = Cloth.ClothType.Everything;
                        break;
                }

                if (i > 0)
                {
                    Debug.Log("测试！ " + picName + scenes[i]);
                    ClothDic[picName].spritesInScenes[i] = newPic;
                    //Debug.Log("测试！ " + picName + scenes[i]);

                }
                else
                {
                    ClothDic.Add(picName, new Cloth(picName, newPic, thisType, "Karara", picName+" 1 1 1 1"));
                    //Debug.Log("测试！ "+picName + " 1 1 1 1");
                }
            }
        }


        for (int i = 0; i < postures.Length; i++)
        {

            object[] loadedPics = Resources.LoadAll("Images/Karara/Cloth/Pose/" + postures[i] , typeof(Sprite));

            for (int j = 0; j < loadedPics.Length; j++)
            {
                Sprite newPic = (Sprite)loadedPics[j];
                ClothDic[newPic.name].spritesInPos.Add(newPic);
                //Debug.Log("测试！ " + ClothDic[newPic.name].name);
            }
        }

            
    }

    private void LoadClothes() {
        //StreamWriter writer = new StreamWriter("./file.txt");

        //StreamReader reader = new System.IO.StreamReader("./file.txt");
        StringReader reader = null;
        TextAsset file = (TextAsset)Resources.Load("clothAttributes", typeof(TextAsset));
        reader = new StringReader(file.text);



        Cloth.ClothType thisType;
        for (int i = 0; i < scenes.Length; i++)
        {
            foreach (string type in clothTypes)
            {
                string scene = scenes[i];

                object[] loadedPics = Resources.LoadAll(mainPath + scene + "/" + type, typeof(Sprite));


                thisType = CheckType(type);

                for (int j = 0; j < loadedPics.Length; j++)
                {

                    Sprite newPic = (Sprite)loadedPics[j];
                    string picName = newPic.name;
                    if (i>0)
                    {
                        
                        ClothDic[picName].spritesInScenes[i] = newPic;
                        //Debug.Log("测试！ " + picName + scenes[i]);

                    }
                    else
                    {
                        
                        string owner = CheckOwner(picName);
                        NPCDic[owner].myClothes.Add(picName);
                        string atbs = "a 1 1 1 1";
                        atbs = reader.ReadLine();
                        if (atbs == null) atbs = "a 1 1 1 1";
                        //Debug.Log(atbs);
                        ClothDic.Add(picName, new Cloth(picName, newPic, thisType, owner,atbs));

                        //writer.WriteLine(picName + " 1 1 1 1");


                    }

                }

            }

        }

        //writer.Close();


        for (int i = 0; i < postures.Length; i++)
        {
            foreach (string type in clothTypes)
            {

                object[] loadedPics = Resources.LoadAll(mainPath + "Pose/" + postures[i] + "/" + type, typeof(Sprite));

                for (int j = 0; j < loadedPics.Length; j++)
                {
                    Sprite newPic = (Sprite)loadedPics[j];
                    ClothDic[newPic.name].spritesInPos.Add(newPic);
                }

            }


        }


    }

}

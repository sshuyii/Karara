using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class LostAndFound : MonoBehaviour
{

    
    public GameObject LostFoundClothInScene;

    public List<Cloth> LostAndFoundClothes = new List<Cloth>();
    public List<NPC> ClothesOwners = new List<NPC>();

    int totalCount = 0;

    [SerializeField]
    private TextMeshPro lostFoundNum;
    [SerializeField]
    private GameObject numBubble;

    private bool isShowingNum = false;

    private RatingSystem RatingSys;
    private InstagramController InstagramController;


    private int dropNumThisStation = 0;
    public GameObject dropNumText;
    
    

    // Start is called before the first frame update
    void Start()
    {
        
        RatingSys = GameObject.Find("FloatingUI").GetComponent<RatingSystem>();
        InstagramController = GameObject.Find("---InstagramController").GetComponent<InstagramController>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void clickLostFound()
    {
        if (!isShowingNum)
        {
            
            lostFoundNum.SetText(totalCount.ToString());
            lostFoundNum.enabled = true;
            numBubble.SetActive(true);
        }
        else
        {

            lostFoundNum.enabled = false;
            numBubble.SetActive(false);
        }

        isShowingNum = !isShowingNum;


    }

    public void AddClothToList(Cloth cloth, NPC owner)
    {
        Debug.Log("call drop cloth in l&F script");
        totalCount++;
        if (totalCount > 0) LostFoundClothInScene.SetActive(true);

        cloth.state = Cloth.ClothState.LandF;
        owner.myClothesInLF.Add(cloth.name);

        ClothesOwners.Add(owner);
        LostAndFoundClothes.Add(cloth);
        RatingSys.SubStars();

    }



    public void DropLostFoundClothes(int stationNum)
    {
        int dropNum = 0;
        int maxDrop = -1;
        string maxOwner = "";

        foreach (NPC owner in ClothesOwners)
        {
            if(owner.homeStation == stationNum)
            {
                int num = owner.myClothesInLF.Count;
                if (num > maxDrop) maxDrop = num;
                dropNum += num;
                owner.myClothesInLF.Clear();
            }
        }

        dropNumThisStation = dropNum;
        totalCount -= dropNum;
        InstagramController.RefreshPost(maxOwner, RatingSys.rating);
        
    }


    public IEnumerator AnimationDropNUm()
    {
        totalCount += dropNumThisStation;
        lostFoundNum.text = totalCount.ToString();
        dropNumText.SetActive(true);
        if(dropNumThisStation > 0)dropNumText.GetComponent<TextMeshPro>().text = "- "+dropNumThisStation.ToString();
        clickLostFound();

        yield return new WaitForSeconds(1f);


        totalCount -= dropNumThisStation;
        lostFoundNum.text = totalCount.ToString();
        yield return new WaitForSeconds(1f);
        dropNumText.SetActive(false);
        clickLostFound();

    }
}

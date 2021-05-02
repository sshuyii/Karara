using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField]
    private GameObject[] levels;
    [SerializeField]
    private Vector3[] speed;

    private Camera MainCamera;
    private Vector2 screenBounds;
    public float choke;
    public bool start;
    private bool alreadyInstantiated;


    // Start is called before the first frame update
    void Start()
    {
        MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        screenBounds = MainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, MainCamera.transform.position.z))
            - MainCamera.transform.position;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(start)
        {
            if(!alreadyInstantiated)
            {   
                alreadyInstantiated = true;
                foreach(GameObject obj in levels)
                {
                    loadChildObject(obj);
                }
            }

            for(int i = 0; i < levels.Length; i++)
            {
                levels[i].transform.position += speed[i];
            }
        }
    }

    private void loadChildObject(GameObject obj)
    {
        float objectWidth = obj.GetComponent<SpriteRenderer>().bounds.size.x - choke;
        print("objectWidth = "+ objectWidth);
        int childsNeeded = (int)Mathf.Ceil(screenBounds.x / objectWidth) + 2;
        print("child needed = "+ childsNeeded);

        GameObject clone = Instantiate(obj) as GameObject;
        for(int i = 0; i <= childsNeeded; i++)
        { 
            GameObject g = Instantiate(clone) as GameObject;
            g.transform.SetParent(obj.transform);
            g.transform.localPosition = new Vector3(objectWidth * i, 0, 0);
            g.name = obj.name + i;
            Destroy(g.GetComponent<BackgroundMoving>());
        }
        Destroy(clone);
        Destroy(obj.GetComponent<SpriteRenderer>());
    }

    void repositionChildObject(GameObject obj)
    {
        Transform[] children = obj.GetComponentsInChildren<Transform>();
        if(children.Length > 1)
        {
            GameObject firstChild = children[1].gameObject;
            GameObject lastChild = children[children.Length - 1].gameObject;
            float halfObjectWidth = lastChild.GetComponent<SpriteRenderer>().bounds.extents.x - choke;
            //如果firstChild的末端都还没进入摄像机，先不要调位置
            if(firstChild.transform.position.x + halfObjectWidth > MainCamera.transform.position.x + screenBounds.x) return;
            if(MainCamera.transform.position.x + screenBounds.x > lastChild.transform.position.x)
            {
                firstChild.transform.SetAsLastSibling();
                firstChild.transform.position = new Vector3(lastChild.transform.position.x + halfObjectWidth * 2, lastChild.transform.position.y, lastChild.transform.position.z);

            }else if(MainCamera.transform.position.x - screenBounds.x < firstChild.transform.position.x){
                lastChild.transform.SetAsFirstSibling();
                lastChild.transform.position = new Vector3(firstChild.transform.position.x - halfObjectWidth* 2, firstChild.transform.position.y, firstChild.transform.position.z);
            }
        }
    } 

    private void LateUpdate() {
        foreach(GameObject obj in levels){
            repositionChildObject(obj);
            // print("reposition");
        }
    }
}

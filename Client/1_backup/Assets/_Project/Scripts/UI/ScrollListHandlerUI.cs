using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollListHandlerUI : MonoBehaviour
{
    public GameObject Icon;
    protected List<GameObject> CreateIcon;
    public int LayOutAcceptCount;
    public int ContentSizeUpScale;
   protected int createcount;
   protected Transform LayOut;
    protected int LayOutCount;
    protected virtual GameObject pullingIcon(Transform t)
    {
        if (CreateIcon == null)
            CreateIcon = new List<GameObject>();
        foreach (GameObject s in CreateIcon)
        {
            if (!s.activeSelf)
            {
                s.gameObject.SetActive(true);
                s.transform.SetParent (t);
                return s;
            }
        }
        var b = Instantiate(Icon, t);
        CreateIcon.Add(b);
        return b;
    }

    protected virtual void InitializeList()
    {
        createcount = 0;
        LayOutCount = 0;
        LayOut = null;
        if (CreateIcon != null)
        {
            foreach (GameObject obj in CreateIcon)
            {
                obj.SetActive(false);
            }
        }
        if (LayoutList != null)
        {
            foreach (GameObject obj in LayoutList)
            {
                obj.SetActive(false);
            }
        }
    }
    public GameObject ScrollViewContent;
    public GameObject ListHorizonLayout;
    protected List<GameObject> LayoutList;
    protected virtual GameObject pullingLayout(Transform t)
    {
        if (LayoutList == null)
            LayoutList = new List<GameObject>();
        foreach (GameObject obj in LayoutList)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                obj.transform.SetParent(t);
                return obj;
            }
        }
        var a = Instantiate(ListHorizonLayout, t);
        LayoutList.Add(a);
        return a;

    }
    public virtual void CreateList()
    {
        InitializeList();
    }
    protected virtual GameObject CreateListIcon(Transform LayOutTransform)
    {
        
        if (LayOut == null || createcount == LayOutAcceptCount)
        {
            createcount = 0;
            LayOutCount++;
            ScrollViewContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, ContentSizeUpScale * LayOutCount);
            LayOut = pullingLayout(LayOutTransform).transform;
        }

        createcount++;
        return pullingIcon(LayOut);
    }
   
}

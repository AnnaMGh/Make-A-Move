using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectHolder : MonoBehaviour
{
    private static Dictionary<string, GameObject> objectList;

    [SerializeField]
    public GameObject[] objects;

    public void Awake()
    {
        objectList = new Dictionary<string, GameObject>();
        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                objectList[obj.name] = obj;
                if (CheckForChildren(obj) == null)
                {
                    continue;
                };
            }
        }
    }

    public GameObject CheckForChildren(GameObject potentialParent)
    {
        if (potentialParent.transform.childCount > 0)
        {
            for (int i = 0; i < potentialParent.transform.childCount; i++)
            {
                GameObject child = potentialParent.transform.GetChild(i).gameObject;
                objectList[child.name] = child;
                return CheckForChildren(child);
            }
        }

        return null;
    }

    public GameObject GetObjectByName(string name)
    {
        return objectList.ContainsKey(name) ? objectList[name] : null;
    }

    public GameObject GetSaferObjectByName(string name)
    {
        GameObject obj = null;

        try
        {
            obj = objectList.ContainsKey(name) ? objectList[name] : null;
        }
        catch { };
        if (obj == null)
        {
            obj = GameObject.Find(name);
        }

        return obj;
    }
}

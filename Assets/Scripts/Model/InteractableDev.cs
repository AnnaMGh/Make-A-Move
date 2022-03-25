using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDev<T> : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void SetData(int nr, int min, int max, T obj)
    {
       
    }
    public virtual string CheckData(int index)
    {
        return "Base";
    }
    
    public virtual T GetObject() 
    {
        return (T)Activator.CreateInstance(typeof(T), new object[] { });
    }
}

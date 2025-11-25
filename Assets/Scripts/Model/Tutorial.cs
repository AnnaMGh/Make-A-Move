using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial
{
    public string name;
    public string message;
    public Vector3 messageDirection;
    public bool hasNext;

    public Tutorial(string name, string message, Vector3 messageDirection, bool hasNext)
    {
        this.name = name;
        this.message = message;
        this.messageDirection = messageDirection;
        this.hasNext = hasNext;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enabler
{
    public GameObject GameObj { get { return gameObj; } }

    public Point enablerPoint;
    public Point cubePoint;
    public Color color;
    private GameObject gameObj;


    private Renderer enablerRenderer;

    public void SetGameObject(GameObject gameObj)
    {
        this.gameObj = gameObj;
        this.enablerRenderer = this.gameObj.GetComponent<Renderer>();
    }

    public void ChangeDesign()
    {
        //change color
        enablerRenderer.material.color = color;                                           
    }
}

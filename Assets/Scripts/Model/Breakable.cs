using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Breakable
{
    public GameObject GameObj { get { return gameObj; } }

    public Point point;
    public int maxResistance;
    public int currentResistance;
    public Color color;
    private GameObject gameObj;


    private Renderer enablerRenderer;
    public void SetGameObject(GameObject gameObj)
    {
        this.gameObj = gameObj;
        this.enablerRenderer = this.gameObj.GetComponent<Renderer>();
    }

    public void ChangeGameObjectName(int nr)
    {
        this.gameObj.name = Interactable.InteractableType.BREAKABLE.ToString() + "_" + nr;
    }

    public void ChangeDesign()
    {
        //change color
      //  enablerRenderer.material.color = color;
    }

    public bool IsBroken()
    {
        return currentResistance >= maxResistance;
    }

    public void SteppedOver()
    {
        currentResistance++;
    }

    public int GetBreakableIndex()
    {
        int index = maxResistance - currentResistance;
        return index < 0 ? 0 : index;
    }

    
}

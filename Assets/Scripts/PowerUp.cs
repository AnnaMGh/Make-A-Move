using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Powerup
{
    public enum PowerupType {NONE, DOUBLE_FULL, STRONG, DIZZY}

    public GameObject GameObj { get { return gameObj; } }

    public Point point;
    public int type;
    private GameObject gameObj;


    public void SetGameObject(GameObject gameObj)
    {
        this.gameObj = gameObj;
    }

    public void ChangeGameObjectName(int nr)
    {
        this.gameObj.name = Interactable.InteractableType.POWERUP.ToString() + "_" + nr;
    }


}

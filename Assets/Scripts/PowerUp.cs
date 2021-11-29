using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Powerup
{
    public enum PowerupType {DOUBLE_FULL, STRONG, DIZZY}

    public GameObject GameObj { get { return gameObj; } }

    public Point point;
    public int type;
    private GameObject gameObj;
    public int nrOfFreeMovesAvailable;


    public void SetGameObject(GameObject gameObj)
    {
        this.gameObj = gameObj;
    }

    public void ChangeGameObjectName(int nr)
    {
        this.gameObj.name = Interactable.InteractableType.POWERUP.ToString() + "_" + nr;
    }

    public void ChangeCameraView(bool isCamera3D) {
        if (isCamera3D)
        {
            if (type == (int)PowerupType.DIZZY)
            {
                GameObj.transform.eulerAngles = new Vector3(0f, 0f, 45f);
            }
            else {
                GameObj.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
        }
        else {
           
            if (type == (int)PowerupType.DIZZY)
            {
                GameObj.transform.eulerAngles = new Vector3(90f, 45f, 0f);
            }
            else
            {
                GameObj.transform.eulerAngles = new Vector3(90f, 0f, 0f);
            }
        }
    }


}

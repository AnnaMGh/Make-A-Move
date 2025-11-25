using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy
{
    public CustomPiece.PiecesTypeEnum TypePiece { get { return (CustomPiece.PiecesTypeEnum)type; } }
    public Point Point { get { return point; } }
    public Vector3 Positon { get { return positon; } }
    public GameObject GameObj { get { return gameObj; } }

    public bool enabled;
    public int type;
    public Point point;

    private Vector3 positon;
    private GameObject gameObj;
    public PieceHandler EnemyPieceHandler { get; private set; }

    public Enemy()
    {
        this.enabled = false;
    }

    public Enemy(int type, Point point)
    {
        this.enabled = true;
        this.type = type;
        this.point = point;
    }

    public Enemy(CustomPiece.PiecesTypeEnum typePiece, Point point, Vector3 positon)
    {
        this.enabled = true;
        this.type = (int)typePiece;
        this.point = point;
        this.positon = positon;
    }

    public void SetGameObject(GameObject gameObj)
    {
        this.gameObj = gameObj;
        this.EnemyPieceHandler = this.gameObj.GetComponent<PieceHandler>();
        this.gameObj.transform.GetChild(0).GetComponent<Interactable>().interactableType = Interactable.InteractableType.ENEMY;
        this.EnemyPieceHandler.CurrentPiece.currentPoint = new Point(point.i, point.j);
    }

    public void ChangeGameObjectName(int nr)
    {
        this.gameObj.name = Interactable.InteractableType.ENEMY.ToString() + "_" + nr;
    }

    public void SetObjectScript(Enemy obj)
    {
        gameObj.transform.GetChild(0).GetComponent<Interactable>().SetObject(obj);
    }

    public void ChangeCustomPiece()
    {
        this.EnemyPieceHandler.ChangeEnemyCustomPiece(TypePiece, delegatPointUpdate=> {
            point = (Point)delegatPointUpdate;
        });
    }


    public void ChangeColor()
    {
        this.EnemyPieceHandler.ChangeColor();
    }

    public void Hide() {
        this.gameObj.transform.GetChild(0).gameObject.SetActive(false);
        this.gameObj.transform.GetChild(2).gameObject.SetActive(false);
        this.gameObj.transform.GetChild(3).gameObject.SetActive(false);
        this.gameObj.transform.GetChild(4).gameObject.SetActive(false);
    }

    public Enemy GetPiece(Vector3 pos)
    {
        positon = pos;
        return this;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PieceAvailable
{

    public CustomPiece.PiecesTypeEnum Type { get { return type; } }
    public Point Point { get { return point; } }
    public Vector3 Positon { get { return positon; } }
    public GameObject GameObj { get { return gameObj; } }

    public bool enabled;
    public int typePiece;
    public Point point;

    private CustomPiece.PiecesTypeEnum type;
    private Vector3 positon;
    private GameObject gameObj;

    public PieceAvailable()
    {
        this.enabled = false;
    }

    public PieceAvailable(int type, Point point)
    {
        this.enabled = true;
        this.typePiece = type;
        this.type = (CustomPiece.PiecesTypeEnum)typePiece;
        this.point = point;
    }

    public PieceAvailable(CustomPiece.PiecesTypeEnum type, Point point, Vector3 positon) {
        this.enabled = true;
        this.type = type;
        this.point = point;
        this.positon = positon;
    }

    public void SetGameObject(GameObject gameObj)
    {
        this.gameObj = gameObj;
    }

    public PieceAvailable GetPiece(Vector3 pos)
    {
        type = (CustomPiece.PiecesTypeEnum)typePiece;
        positon = pos;
        return this;
    }
}

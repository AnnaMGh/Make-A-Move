using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public int level;
    public LevelPoints maximumPoints; // moves available left x 10 + changes available left x 50
    public LevelPoints goalPoints; // moves available left x 10 + changes available left x 50
    public int[] piecesAvailable;
    public PieceAvailable newPieceAvailable;
    public Point startPoint;
    public Point finishPoint;
    public Block[] block;
    public Enabler[] enabler;
    public Breakable[] breakable;
    public Powerup[] powerup;
    //public Enemy[] enemy;
    public Point[] disabledPoints;
   

    public CustomPiece.PiecesTypeEnum[] GetPiecesAvailable()
    {
        CustomPiece.PiecesTypeEnum[] pieces = new CustomPiece.PiecesTypeEnum[piecesAvailable.Length];
        for (int i = 0; i < piecesAvailable.Length; i++)
        {
            pieces[i] = (CustomPiece.PiecesTypeEnum)piecesAvailable[i];
        }
        return pieces;
    }
}

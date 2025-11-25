using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakePlayerEnemy
{
    public PieceHandler ReceivedPieceHandler { get; }
    public CustomPiece.MovementTypeEnum Type { get; }

    public TakePlayerEnemy(PieceHandler receivedPieceHandler, CustomPiece.MovementTypeEnum type)
    {
        ReceivedPieceHandler = receivedPieceHandler;
        Type = type;
    }
}

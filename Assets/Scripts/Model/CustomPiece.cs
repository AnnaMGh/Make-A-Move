using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPiece
{
    //statics
    public enum PiecesTypeEnum { TYPE_PAWN, TYPE_ROOK, TYPE_KNIGHT, TYPE_BISHOP, TYPE_QUEEN, TYPE_KING };
    public enum MovementTypeEnum
    {
        MOVEMENT_FORWARD, MOVEMENT_BACKWARD, MOVEMENT_LEFT, MOVEMENT_RIGHT,
        MOVEMENT_DIAGONAL_FORWARD_LEFT, MOVEMENT_DIAGONAL_FORWARD_RIGHT,
        MOVEMENT_DIAGONAL_BACKWARD_LEFT, MOVEMENT_DIAGONAL_BACKWARD_RIGHT,
        MOVEMENT_LEFT_FORWARD_L, MOVEMENT_RIGHT_FORWARD_L, //FOR KNIGHT
        MOVEMENT_LEFT_BACKWARD_L, MOVEMENT_RIGHT_BACKWARD_L, //FOR KNIGHT
        MOVEMENT_FORWARD_LEFT_L, MOVEMENT_FORWARD_RIGHT_L,//FOR KNIGHT
        MOVEMENT_BACKWARD_LEFT_L, MOVEMENT_BACKWARD_RIGHT_L, //FOR KNIGHT
    };
    private Dictionary<PiecesTypeEnum, CustomPiece> customPawnsDictionary;
    public static string prefabName = "CustomPiece";

    public PiecesTypeEnum pieceType;
    public MovementTypeEnum[] movementType;
    public int limitedMoves; //limited
    public string meshName;
    public Point currentPoint;


    public void ChangePieceByType(PiecesTypeEnum pawnType)
    {
        if (customPawnsDictionary == null)
        {
            customPawnsDictionary = new Dictionary<PiecesTypeEnum, CustomPiece>();
            customPawnsDictionary.Add(PiecesTypeEnum.TYPE_PAWN, new Pawn());
            customPawnsDictionary.Add(PiecesTypeEnum.TYPE_ROOK, new Rook());
            customPawnsDictionary.Add(PiecesTypeEnum.TYPE_KNIGHT, new Knight());
            customPawnsDictionary.Add(PiecesTypeEnum.TYPE_BISHOP, new Bishop());
            customPawnsDictionary.Add(PiecesTypeEnum.TYPE_QUEEN, new Queen());
            customPawnsDictionary.Add(PiecesTypeEnum.TYPE_KING, new King());
        }
        ChangePieceValues(customPawnsDictionary[pawnType]);
    }

    private void ChangePieceValues(CustomPiece newPieceValues)
    {
        this.pieceType = newPieceValues.pieceType;
        this.movementType = newPieceValues.movementType;
        this.limitedMoves = newPieceValues.limitedMoves;
        this.meshName = newPieceValues.meshName;
    }

    public class Pawn : CustomPiece
    {
        public Pawn()
        {
            pieceType = PiecesTypeEnum.TYPE_PAWN;
            movementType = new MovementTypeEnum[] { MovementTypeEnum.MOVEMENT_FORWARD };
            limitedMoves = 1; //limited
            meshName = PiecesTypeEnum.TYPE_PAWN.ToString();
        }
    }

    public class Rook : CustomPiece
    {
        public Rook()
        {
            pieceType = PiecesTypeEnum.TYPE_ROOK;
            movementType = new MovementTypeEnum[] { MovementTypeEnum.MOVEMENT_FORWARD, MovementTypeEnum.MOVEMENT_BACKWARD, MovementTypeEnum.MOVEMENT_LEFT, MovementTypeEnum.MOVEMENT_RIGHT };
            limitedMoves = 0; // no limits
            meshName = PiecesTypeEnum.TYPE_ROOK.ToString();
        }
    }

    public class Knight : CustomPiece
    {
        public Knight()
        {
            pieceType = PiecesTypeEnum.TYPE_KNIGHT;
            movementType = new MovementTypeEnum[] {
                MovementTypeEnum.MOVEMENT_LEFT_FORWARD_L,
                MovementTypeEnum.MOVEMENT_RIGHT_FORWARD_L,
                MovementTypeEnum.MOVEMENT_LEFT_BACKWARD_L,
                MovementTypeEnum.MOVEMENT_RIGHT_BACKWARD_L,
                MovementTypeEnum.MOVEMENT_FORWARD_LEFT_L,
                MovementTypeEnum.MOVEMENT_FORWARD_RIGHT_L,
                MovementTypeEnum.MOVEMENT_BACKWARD_LEFT_L,
                MovementTypeEnum.MOVEMENT_BACKWARD_RIGHT_L,
            };
            limitedMoves = 3; //limited
            meshName = PiecesTypeEnum.TYPE_KNIGHT.ToString();
        }
    }

    public class Bishop : CustomPiece
    {
        public Bishop()
        {
            pieceType = PiecesTypeEnum.TYPE_BISHOP;
            movementType = new MovementTypeEnum[] { MovementTypeEnum.MOVEMENT_DIAGONAL_FORWARD_LEFT, MovementTypeEnum.MOVEMENT_DIAGONAL_FORWARD_RIGHT, MovementTypeEnum.MOVEMENT_DIAGONAL_BACKWARD_LEFT, MovementTypeEnum.MOVEMENT_DIAGONAL_BACKWARD_RIGHT };
            limitedMoves = 0; // no limits
            meshName = PiecesTypeEnum.TYPE_BISHOP.ToString();
        }
    }

    public class Queen : CustomPiece
    {
        public Queen()
        {
            pieceType = PiecesTypeEnum.TYPE_QUEEN;
            movementType = new MovementTypeEnum[] { MovementTypeEnum.MOVEMENT_FORWARD, MovementTypeEnum.MOVEMENT_BACKWARD, MovementTypeEnum.MOVEMENT_LEFT, MovementTypeEnum.MOVEMENT_RIGHT,
                            MovementTypeEnum.MOVEMENT_DIAGONAL_FORWARD_LEFT, MovementTypeEnum.MOVEMENT_DIAGONAL_FORWARD_RIGHT, MovementTypeEnum.MOVEMENT_DIAGONAL_BACKWARD_LEFT, MovementTypeEnum.MOVEMENT_DIAGONAL_BACKWARD_RIGHT };
            limitedMoves = 0; // no limits
            meshName = PiecesTypeEnum.TYPE_QUEEN.ToString();
        }
    }

    public class King : CustomPiece
    {
        public King()
        {
            pieceType = PiecesTypeEnum.TYPE_KING;
            movementType = new MovementTypeEnum[] { MovementTypeEnum.MOVEMENT_FORWARD, MovementTypeEnum.MOVEMENT_BACKWARD, MovementTypeEnum.MOVEMENT_LEFT, MovementTypeEnum.MOVEMENT_RIGHT,
            MovementTypeEnum.MOVEMENT_DIAGONAL_FORWARD_LEFT, MovementTypeEnum.MOVEMENT_DIAGONAL_FORWARD_RIGHT, MovementTypeEnum.MOVEMENT_DIAGONAL_BACKWARD_LEFT, MovementTypeEnum.MOVEMENT_DIAGONAL_BACKWARD_RIGHT };
            limitedMoves = 1; // limited
            meshName = PiecesTypeEnum.TYPE_KING.ToString();
        }
    }
}

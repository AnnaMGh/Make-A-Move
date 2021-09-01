using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step
{
    CustomPiece.MovementTypeEnum movement;
    CustomPiece.MovementTypeEnum opositeMovement;
    Point point;
    bool split;
    bool splitPos;
    Vector3 position;
    Vector3 position1;
    Vector3 position2;

    public CustomPiece.MovementTypeEnum Movement { get => movement; }
    public CustomPiece.MovementTypeEnum OpositeMovement { get => opositeMovement; }
    public Point Point { get => point; }
    public bool Split { get => split; }
    public bool SplitPos { get { return splitPos; } set { splitPos = value; } }
    public Vector3 Position { get => position;}
    public Vector3 Position1 { get => position1;}
    public Vector3 Position2 { get => position2;}

    public Step(CustomPiece.MovementTypeEnum movement, CustomPiece.MovementTypeEnum opositeMovement, Point point, Vector3 position, bool split)
    {
        this.opositeMovement = opositeMovement;
        this.movement = movement;
        this.point = point;
        this.position = position;
        this.split = split;
        this.splitPos = false;

        Vector3 pos1 = Vector3.zero;
        Vector3 pos2 = Vector3.zero;
        if (split)
        {
            float x = position.x;
            float z = position.z;
            if (Math.Abs(x) > Math.Abs(z))
            {
                pos1 = new Vector3(position.x, 1.5f, 0f);
                pos2 = new Vector3(0f, -1f, position.z);
            }
            else
            {
                pos1 = new Vector3(0f, 1.5f, position.z);
                pos2 = new Vector3(position.x, -1f, 0f);
            }
        }

        position1 = pos1;
        position2 = pos2;
    }

    public Step(CustomPiece.MovementTypeEnum movement, CustomPiece.MovementTypeEnum opositeMovement, Point point, Vector3 position)
    {
        this.opositeMovement = opositeMovement;
        this.movement = movement;
        this.point = point;
        this.position = position;
        this.split  = false;
        this.splitPos = false;
        this.position1 = Vector3.zero;
        this.position2 = Vector3.zero;
    }

    public Step(CustomPiece.MovementTypeEnum movement, Point point, Vector3 position)
    {
        this.movement = movement;
        this.point = point;
        this.position = position;
        this.split = false;
        this.splitPos = false;
        this.position1 = Vector3.zero;
        this.position2 = Vector3.zero;
    }

    public Step( Point point, Vector3 position)
    {
        this.point = point;
        this.position = position;
        this.split = false;
        this.splitPos = false;
        this.position1 = Vector3.zero;
        this.position2 = Vector3.zero;
    }

    public Step()
    {
        this.point = new Point(0,0);
        this.position = new Vector3(0,0,0);
        this.split = false;
        this.splitPos = false;
        this.position1 = Vector3.zero;
        this.position2 = Vector3.zero;
    }
}

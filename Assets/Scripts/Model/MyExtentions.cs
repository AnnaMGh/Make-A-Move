using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyExtentions
{
    public static string ListToString(this Dictionary<Cube, CustomPiece.MovementTypeEnum> dicCube)
    {
        string cubeNames = "";

        foreach (Cube c in dicCube.Keys)
        {
            cubeNames += c + "\n";
        }
        return cubeNames;
    }

}

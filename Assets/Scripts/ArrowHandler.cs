using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class ArrowHandler : MonoBehaviour
{
    private Dictionary<CustomPiece.MovementTypeEnum, GameObject> imageMovementDictionary;

    // Start is called before the first frame update
    void Awake()
    {
        InitializeArrows();
    }

    private void InitializeArrows()
    {
        imageMovementDictionary = new Dictionary<CustomPiece.MovementTypeEnum, GameObject>();
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            imageMovementDictionary.Add(gameObject.transform.GetChild(i).GetComponent<Arrow>().movement, gameObject.transform.GetChild(i).gameObject);
        }
    }

    public Dictionary<CustomPiece.MovementTypeEnum, Vector3> GetPositions()
    {
        CustomPiece.MovementTypeEnum[] movementTypes = (CustomPiece.MovementTypeEnum[])Enum.GetValues(typeof(CustomPiece.MovementTypeEnum));

        if (imageMovementDictionary == null) { InitializeArrows(); }

        Dictionary<CustomPiece.MovementTypeEnum, Vector3> positionsDictionary = new Dictionary<CustomPiece.MovementTypeEnum, Vector3>();
        foreach (CustomPiece.MovementTypeEnum movement in movementTypes)
        {
            positionsDictionary.Add(movement, imageMovementDictionary[movement].transform.position);
        }

        return positionsDictionary;
    }

    public void ChangeArrowsAvailability(bool enable, CustomPiece.MovementTypeEnum[] movementTypes)
    {
        if (movementTypes == null) { movementTypes = (CustomPiece.MovementTypeEnum[])Enum.GetValues(typeof(CustomPiece.MovementTypeEnum)); }

        if (imageMovementDictionary == null) { InitializeArrows(); }

        foreach (CustomPiece.MovementTypeEnum movement in movementTypes)
        {
            imageMovementDictionary[movement].SetActive(enable);
        }
    }

    public void ChangeArrowsColor(Color color, CustomPiece.MovementTypeEnum[] movementTypes)
    {
        if (movementTypes == null) { movementTypes = (CustomPiece.MovementTypeEnum[])Enum.GetValues(typeof(CustomPiece.MovementTypeEnum)); }

        if (imageMovementDictionary == null) { InitializeArrows(); }

        foreach (CustomPiece.MovementTypeEnum movement in movementTypes)
        {
            imageMovementDictionary[movement].transform.GetChild(0).GetComponent<MeshRenderer>().GetComponent<Renderer>().material.color = color;
        }
    }

}

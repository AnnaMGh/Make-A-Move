using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Block
{
    public GameObject GameObj { get { return gameObj; } }

    public int cost;
    public Point oldPoint;
    private GameObject gameObj;

    private Color colorBlock;
    private Renderer blockRenderer;
    private Renderer numberFrontRenderer;
    private Renderer numberTopRenderer;
    private MeshFilter numberFrontMeshFilter;
    private MeshFilter numberTopMeshFilter;

    public void SetGameObject(GameObject gameObj)
    {
        this.gameObj = gameObj;
        this.blockRenderer = this.gameObj.GetComponent<Renderer>();
        this.numberFrontRenderer = this.gameObj.transform.GetChild(0).GetComponent<Renderer>();
        this.numberTopRenderer = this.gameObj.transform.GetChild(1).GetComponent<Renderer>();
        this.numberFrontMeshFilter = this.gameObj.transform.GetChild(0).GetComponent<MeshFilter>();
        this.numberTopMeshFilter = this.gameObj.transform.GetChild(1).GetComponent<MeshFilter>();
    }

    public void ChangeDesign()
    {
        //change color
        colorBlock = ((oldPoint.i + oldPoint.j) % 2 == 0 ? Constants.MATRIX_BLOCK_COLOR_01 : Constants.MATRIX_BLOCK_COLOR_02);
        Color colorNumber = ((oldPoint.i + oldPoint.j) % 2 == 0 ? Constants.MATRIX_BLOCK_COLOR_02 : Constants.MATRIX_BLOCK_COLOR_01);
        //change block color
        blockRenderer.material.color = colorBlock; //change block color                                                                   

        //change cost color
        numberFrontRenderer.material.color = colorNumber;
        numberTopRenderer.material.color = colorNumber;

        //change cost mesh
        Mesh mesh = Resources.Load<Mesh>("Meshes/NUMBER_" + (cost < 10 ? "0" : "") + cost);
        numberFrontMeshFilter.sharedMesh = mesh;
        numberTopMeshFilter.sharedMesh = mesh;
    }

    public void ChangeToNextMove(bool isNextMove)
    {
        //check if it is not destoyed
        if (blockRenderer == null)
        {
            return;
        }

        if (isNextMove)
        {
            //change block color to next
            blockRenderer.material.color = Constants.MATRIX_BOX_NEXT_COLOR;
        }
        else
        {
            //change block color back
            blockRenderer.material.color = colorBlock;
        }
    }

    public void ChangeGameObjectName(int nr)
    {
        this.gameObj.name = Interactable.InteractableType.BLOCK.ToString() +"_" + nr;
    }

    public override string ToString() {
        string str = "";
        if (this.gameObj != null)
        {
            str += gameObj.name + " - ";
        }
        str+="(" +oldPoint.i + "," + oldPoint.j + ")";
        return str;
    }
}

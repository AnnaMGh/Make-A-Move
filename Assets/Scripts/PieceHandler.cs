using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PieceHandler : MonoBehaviour
{
    public GameObject CurrentPieceGameObj { get { return currentPieceGameObj; } }
    public CustomPiece CurrentPiece { get { return currentPiece; } }
    public Step LastStep { get { return lastStep; } }
    public GameManager gameManager;
    [SerializeField]
    public Point currentPoint;

    private GameObject currentPieceGameObj;
    private CustomPiece currentPiece;
    private Dictionary<CustomPiece.MovementTypeEnum, Step> movementVectorDictionary;
    private Dictionary<CustomPiece.PiecesTypeEnum, CustomPiece> customPiecesDictionary;

    private int stepsToMove;
    private Step lastStep = new Step();
    private int accuracy = 10;

    private Delegates.ObjectDelegate movementFinishedDelegate;

    private AudioClip movementClip;
    private AudioClip changeClip;


    // Start is called before the first frame update
    void Awake()
    {
        if (gameManager == null)
        {
            try {
                gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            }
            catch (Exception e) { Debug.Log("No GameManager found in PieceHandler: " + e.Message); }
        }
        currentPieceGameObj = gameObject;
        currentPiece = new CustomPiece();

        movementVectorDictionary = new Dictionary<CustomPiece.MovementTypeEnum, Step>
        {
            { CustomPiece.MovementTypeEnum.MOVEMENT_FORWARD,
                new Step(CustomPiece.MovementTypeEnum.MOVEMENT_FORWARD, CustomPiece.MovementTypeEnum.MOVEMENT_BACKWARD, new Point(0,1),new Vector3(0, 0, 1))},
            { CustomPiece.MovementTypeEnum.MOVEMENT_BACKWARD,
                new Step(CustomPiece.MovementTypeEnum.MOVEMENT_BACKWARD,
                CustomPiece.MovementTypeEnum.MOVEMENT_FORWARD,new Point(0,-1),new Vector3(0, 0, -1)) },
            { CustomPiece.MovementTypeEnum.MOVEMENT_LEFT,
                new Step(CustomPiece.MovementTypeEnum.MOVEMENT_LEFT, CustomPiece.MovementTypeEnum.MOVEMENT_RIGHT,new Point(-1,0),new Vector3(-1, 0, 0)) },
            { CustomPiece.MovementTypeEnum.MOVEMENT_RIGHT,
                new Step(CustomPiece.MovementTypeEnum.MOVEMENT_RIGHT, CustomPiece.MovementTypeEnum.MOVEMENT_LEFT,new Point(1,0),new Vector3(1, 0, 0)) },
            { CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_FORWARD_LEFT,
                new Step(CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_FORWARD_LEFT, CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_BACKWARD_RIGHT,new Point(-1,1),new Vector3(-1, 0, 1)) },
            { CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_FORWARD_RIGHT,
                new Step(CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_FORWARD_RIGHT, CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_BACKWARD_LEFT,new Point(1,1),new Vector3(1, 0, 1)) },
            { CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_BACKWARD_LEFT,
                new Step(CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_BACKWARD_LEFT, CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_FORWARD_RIGHT,new Point(-1,-1),new Vector3(-1, 0, -1)) },
            { CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_BACKWARD_RIGHT,
                new Step(CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_BACKWARD_RIGHT, CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_FORWARD_LEFT,new Point(1,-1),new Vector3(1, 0, -1)) },
            { CustomPiece.MovementTypeEnum.MOVEMENT_LEFT_FORWARD_L,
                new Step(CustomPiece.MovementTypeEnum.MOVEMENT_LEFT_FORWARD_L, CustomPiece.MovementTypeEnum.MOVEMENT_RIGHT_FORWARD_L,new Point(-2,1),new Vector3(-2, 0, 1), true) },
            { CustomPiece.MovementTypeEnum.MOVEMENT_RIGHT_FORWARD_L,
                new Step(CustomPiece.MovementTypeEnum.MOVEMENT_RIGHT_FORWARD_L, CustomPiece.MovementTypeEnum.MOVEMENT_LEFT_FORWARD_L,new Point(2,1),new Vector3(2, 0, 1), true) },
            { CustomPiece.MovementTypeEnum.MOVEMENT_LEFT_BACKWARD_L,
                new Step(CustomPiece.MovementTypeEnum.MOVEMENT_LEFT_BACKWARD_L, CustomPiece.MovementTypeEnum.MOVEMENT_RIGHT_BACKWARD_L,new Point(-2,-1),new Vector3(-2, 0, -1), true) },
            { CustomPiece.MovementTypeEnum.MOVEMENT_RIGHT_BACKWARD_L,
                new Step(CustomPiece.MovementTypeEnum.MOVEMENT_RIGHT_BACKWARD_L, CustomPiece.MovementTypeEnum.MOVEMENT_LEFT_BACKWARD_L,new Point(2,-1),new Vector3(2, 0, -1), true) },
            { CustomPiece.MovementTypeEnum.MOVEMENT_FORWARD_LEFT_L,
                new Step(CustomPiece.MovementTypeEnum.MOVEMENT_FORWARD_LEFT_L, CustomPiece.MovementTypeEnum.MOVEMENT_BACKWARD_RIGHT_L,new Point(-1,2),new Vector3(-1, 0, 2), true) },
            { CustomPiece.MovementTypeEnum.MOVEMENT_FORWARD_RIGHT_L,
                new Step(CustomPiece.MovementTypeEnum.MOVEMENT_FORWARD_RIGHT_L, CustomPiece.MovementTypeEnum.MOVEMENT_BACKWARD_LEFT_L,new Point(1,2),new Vector3(1, 0, 2), true) },
            { CustomPiece.MovementTypeEnum.MOVEMENT_BACKWARD_LEFT_L,
                new Step(CustomPiece.MovementTypeEnum.MOVEMENT_BACKWARD_LEFT_L, CustomPiece.MovementTypeEnum.MOVEMENT_FORWARD_RIGHT_L,new Point(-1,-2),new Vector3(-1, 0, -2), true) },
            { CustomPiece.MovementTypeEnum.MOVEMENT_BACKWARD_RIGHT_L,
                new Step(CustomPiece.MovementTypeEnum.MOVEMENT_BACKWARD_RIGHT_L, CustomPiece.MovementTypeEnum.MOVEMENT_FORWARD_LEFT_L,new Point(1,-2),new Vector3(1, 0, -2), true) }
        };


        movementClip = Resources.Load<AudioClip>("Sounds/piece_movement_01");
        changeClip = Resources.Load<AudioClip>("Sounds/piece_change_01");
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPiece == null) {
            return;
        }
        currentPoint = currentPiece.currentPoint;
        if (stepsToMove > 0)
        {
            if (lastStep.Split)
            {
                gameObject.transform.Translate(((lastStep.SplitPos ? lastStep.Position2 : lastStep.Position1) / accuracy), Space.World);
            }
            else
            {
                gameObject.transform.Translate(lastStep.Position / accuracy, Space.World);
            }

            stepsToMove--;
            if (stepsToMove == 0)
            {
                if (lastStep.Split && !lastStep.SplitPos)
                {
                    stepsToMove = accuracy;
                    lastStep.SplitPos = true;
                }
                else
                {
                    lastStep.SplitPos = false;
                    gameObject.transform.position = new Vector3((float)Math.Round(gameObject.transform.position.x, 0), gameObject.transform.position.y, (float)Math.Round(gameObject.transform.position.z));
                    currentPieceGameObj.transform.GetChild(0).GetComponents<BoxCollider>()[(int)currentPiece.pieceType].isTrigger = false;
                    currentPieceGameObj.transform.GetChild(0).GetComponent<Rigidbody>().useGravity = true;
                    movementFinishedDelegate?.Invoke(true);
                }

            }
        }
    }

    public void ChangeCustomPiece(CustomPiece.PiecesTypeEnum type)
    {
        //make sound
        if (PlayerPrefs.GetInt(Constants.KEY_SOUND) == 1)
        {
            currentPieceGameObj.transform.GetChild(1).GetComponent<AudioSource>().clip = changeClip;
            currentPieceGameObj.transform.GetChild(1).GetComponent<AudioSource>().Play();
        }

        //activate new collider
        currentPieceGameObj.transform.GetChild(0).GetComponents<BoxCollider>()[(int)type].enabled = true;
        currentPieceGameObj.transform.GetChild(0).GetComponents<BoxCollider>()[(int)currentPiece.pieceType].isTrigger = false;
        currentPieceGameObj.transform.GetChild(0).GetComponent<Rigidbody>().useGravity = true;

        //deactivate old collider
        if ((int)currentPiece.pieceType != (int)type)
        {
            currentPieceGameObj.transform.GetChild(0).GetComponents<BoxCollider>()[(int)currentPiece.pieceType].enabled = false;
        }


        //change piece
        currentPiece.ChangePieceByType(type);
        //change mesh
        currentPieceGameObj.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh = Resources.Load<Mesh>("Meshes/" + currentPiece.meshName);
        //change material
        currentPieceGameObj.transform.GetChild(0).GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/" + currentPiece.meshName + "_material_" + (PlayerPrefs.GetInt(Constants.KEY_COLOR) == 0 ? "white" : "black"));

        //positon a bit higer to have the fall effect
        gameObject.transform.Translate(0f, 0.25f, 0f, Space.World);
    }

    public void ChangeNewAvailableCustomPiece(CustomPiece.PiecesTypeEnum type)
    {
        currentPieceGameObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        currentPieceGameObj.transform.GetChild(0).GetComponent<Interactable>().enabled = true;
        ChangeCustomPiece(type);
    }

    public void ChangeColor()
    {
        //change color material
        currentPieceGameObj.transform.GetChild(0).GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/" + currentPiece.meshName + "_material_" + (PlayerPrefs.GetInt(Constants.KEY_COLOR) == 0 ? "white" : "black"));
    }

    public void MakeMovement(CustomPiece.MovementTypeEnum movement, int n, Delegates.ObjectDelegate objDelegate)
    {
        //handle sound
        if (PlayerPrefs.GetInt(Constants.KEY_SOUND) == 1)
        {
            currentPieceGameObj.transform.GetChild(1).GetComponent<AudioSource>().clip = movementClip;
            currentPieceGameObj.transform.GetChild(1).GetComponent<AudioSource>().Play();
        }

        GlobalSingleton.GetInstance().SetTimeAsync(50, (o) =>
        {
            lastStep = movementVectorDictionary[movement];
            stepsToMove = (n * accuracy);
            movementFinishedDelegate = objDelegate;
            currentPiece.currentPoint.i += n * lastStep.Point.i;
            currentPiece.currentPoint.j += n * lastStep.Point.j;

            //if knight => flow
            if (currentPiece.pieceType == CustomPiece.PiecesTypeEnum.TYPE_KNIGHT)
            {
                currentPieceGameObj.transform.GetChild(0).GetComponent<Rigidbody>().useGravity = false;
                currentPieceGameObj.transform.GetChild(0).GetComponents<BoxCollider>()[(int)currentPiece.pieceType].isTrigger = true;
            }


            if (n == 0)
            {
                lastStep.SplitPos = false;
                currentPiece.currentPoint.i += lastStep.Point.i;
                currentPiece.currentPoint.j += lastStep.Point.j;
                gameObject.transform.position = new Vector3((float)Math.Round(gameObject.transform.position.x, 0), gameObject.transform.position.y, (float)Math.Round(gameObject.transform.position.z));

                currentPieceGameObj.transform.GetChild(0).GetComponents<BoxCollider>()[(int)currentPiece.pieceType].isTrigger = false;
                currentPieceGameObj.transform.GetChild(0).GetComponent<Rigidbody>().useGravity = true;
                movementFinishedDelegate?.Invoke(true);
            }

        });

    }

    public Point GetPoint(CustomPiece.MovementTypeEnum movement, int n)
    {
        return new Point(currentPiece.currentPoint.i + n * movementVectorDictionary[movement].Point.i,
            currentPiece.currentPoint.j + n * movementVectorDictionary[movement].Point.j);
    }
    public Point GetPointByPointDirection(Point direction, int n)
    {
        return new Point(currentPiece.currentPoint.i + n * direction.i,
            currentPiece.currentPoint.j + n * direction.j);
    }


    public void RestoreToPos(Point p, Vector3 position)
    {
        currentPiece.currentPoint = p;
        currentPieceGameObj.transform.position = new Vector3(position.x, 1f, position.z);
        currentPieceGameObj.transform.localRotation = Quaternion.identity;
        currentPieceGameObj.transform.GetChild(0).localRotation = Quaternion.identity;
        currentPieceGameObj.transform.GetChild(0).localPosition = new Vector3(0f, currentPieceGameObj.transform.GetChild(0).transform.localPosition.y, 0f);
    }

    public void DestroyPiece()
    {
        Destroy(this.gameObject);
    }
}

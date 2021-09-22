using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PieceHandler : MonoBehaviour
{
    public GameObject CurrentPieceGameObj { get { return currentPieceGameObj; } }
    public GameObject CurrentPieceGameObjChild { get { return currentPieceGameObjChild; } }
    public CustomPiece CurrentPiece { get { return currentPiece; } }
    public Step LastStep { get { return lastStep; } }
    public GameManager gameManager;
    //[SerializeField]
    //public Point currentPoint;

    private GameObject currentPieceGameObj;
    private GameObject currentPieceGameObjChild;
    private MeshFilter currentPieceGameObjChildMF;
    private MeshRenderer currentPieceGameObjChildMR;
    private Rigidbody currentPieceGameObjChildRB;
    private BoxCollider[] currentPieceGameObjChildBCs;
    private AudioSource currentPieceGameObjAudio;
    private CustomPiece currentPiece;
    private Dictionary<CustomPiece.MovementTypeEnum, Step> movementVectorDictionary;
    private Dictionary<CustomPiece.PiecesTypeEnum, CustomPiece> customPiecesDictionary;

    private int accuracy = Constants.ACCURECY_FRONT_CAMERA;
    private int stepsToMove;
    private Step lastStep = new Step();

    private Delegates.ObjectDelegate movementFinishedDelegate;

    private AudioClip movementClip;


    // Start is called before the first frame update
    void Awake()
    {
        if (gameManager == null)
        {
            try
            {
                gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            }
            catch (Exception e) { Debug.Log("No GameManager found in PieceHandler: " + e.Message); }
        }

        //get game object components
        currentPieceGameObj = gameObject;
        currentPieceGameObjChild = currentPieceGameObj.transform.GetChild(0).gameObject;
        currentPieceGameObjChildMF = currentPieceGameObjChild.GetComponent<MeshFilter>();
        currentPieceGameObjChildMR = currentPieceGameObjChild.GetComponent<MeshRenderer>();
        currentPieceGameObjChildRB = currentPieceGameObjChild.GetComponent<Rigidbody>();
        currentPieceGameObjChildBCs = currentPieceGameObjChild.GetComponents<BoxCollider>();
        currentPieceGameObjAudio = currentPieceGameObj.transform.GetChild(1).GetComponent<AudioSource>();
        currentPiece = new CustomPiece();

        //create movement dictionary
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
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPiece == null)
        {
            return;
        }
        //currentPoint = currentPiece.currentPoint;
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
                    currentPieceGameObjChildBCs[(int)currentPiece.pieceType].isTrigger = false;
                    currentPieceGameObjChildRB.useGravity = true;
                    movementFinishedDelegate?.Invoke(true);
                }

            }
        }
    }

    public void ChangeCustomPiece(CustomPiece.PiecesTypeEnum type, bool withSound)
    {
        //activate new collider
        currentPieceGameObjChildBCs[(int)type].enabled = true;
        currentPieceGameObjChildBCs[(int)currentPiece.pieceType].isTrigger = false;
        currentPieceGameObjChildRB.useGravity = true;

        //deactivate old collider
        if ((int)currentPiece.pieceType != (int)type)
        {
            currentPieceGameObjChildBCs[(int)currentPiece.pieceType].enabled = false;
        }

        //change piece
        currentPiece.ChangePieceByType(type);

        //change mesh   
        currentPieceGameObjChildMF.sharedMesh = currentPiece.mesh;

        //change material
        currentPieceGameObjChildMR.material = (PlayerPrefs.GetInt(Constants.KEY_COLOR) == 0 ? currentPiece.materialWhite : currentPiece.materialBlack);

        //positon a bit higer to have the fall effect
        gameObject.transform.Translate(0f, 0.25f, 0f, Space.World);

        //make sound
        if (withSound && PlayerPrefs.GetInt(Constants.KEY_SOUND) == 1)
        {
            currentPieceGameObjAudio.clip = currentPiece.audio;
            currentPieceGameObjAudio.Play();
        }
    }

    public void ChangeNewAvailableCustomPiece(CustomPiece.PiecesTypeEnum type)
    {
        currentPieceGameObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        currentPieceGameObjChild.GetComponent<Interactable>().enabled = true;
        ChangeCustomPiece(type, false);
    }

    public void ChangeColor()
    {
        //change color material
        currentPieceGameObjChildMR.material = (PlayerPrefs.GetInt(Constants.KEY_COLOR) == 0 ? currentPiece.materialWhite : currentPiece.materialBlack);
    }

    public bool IsDiagonalMovement(CustomPiece.MovementTypeEnum movement)
    {
        if (movement.Equals(CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_FORWARD_LEFT)
            || movement.Equals(CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_FORWARD_RIGHT)
            || movement.Equals(CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_BACKWARD_LEFT)
            || movement.Equals(CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_BACKWARD_RIGHT)
            )
        {
            return true;
        }
        return false;
    }

    public void MakePieceFallThrough()
    {
        currentPieceGameObjChildRB.useGravity = true;
        currentPieceGameObjChildBCs[(int)currentPiece.pieceType].isTrigger = true;
    }

    public void MakeMovement(CustomPiece.MovementTypeEnum movement, int n, Delegates.ObjectDelegate objDelegate)
    {
        //handle sound
        if (PlayerPrefs.GetInt(Constants.KEY_SOUND) == 1)
        {
            currentPieceGameObjAudio.clip = movementClip;
            currentPieceGameObjAudio.Play();
        }

        GlobalSingleton.GetInstance().SetTimeAsync(50, (o) =>
        {
     
            //check accuracy
            accuracy = (gameManager.IsFrontCamera() ? Constants.ACCURECY_FRONT_CAMERA
            : Constants.ACCURECY_TOP_CAMERA);
           

            lastStep = movementVectorDictionary[movement];
            stepsToMove = (n * accuracy);
            movementFinishedDelegate = objDelegate;
            currentPiece.currentPoint.i += n * lastStep.Point.i;
            currentPiece.currentPoint.j += n * lastStep.Point.j;

            //if knight => flow
            if (currentPiece.pieceType == CustomPiece.PiecesTypeEnum.TYPE_KNIGHT)
            {
                currentPieceGameObjChildRB.useGravity = false;
                currentPieceGameObjChildBCs[(int)currentPiece.pieceType].isTrigger = true;
            }
            /*  //if bishow => deactivate collider
              else if (currentPiece.pieceType == CustomPiece.PiecesTypeEnum.TYPE_BISHOP)
              {
                  currentPieceGameObjChildRB.useGravity = false;
                  currentPieceGameObjChildBCs[(int)currentPiece.pieceType].isTrigger = true;
              }
              //if queen => deactivate collider
              else if (currentPiece.pieceType == CustomPiece.PiecesTypeEnum.TYPE_QUEEN
              && IsDiagonalMovement(movement))
              {
                  currentPieceGameObjChildRB.useGravity = false;
                  currentPieceGameObjChildBCs[(int)currentPiece.pieceType].isTrigger = true;
              }*/

            else if (IsDiagonalMovement(movement))
            {
                currentPieceGameObjChildRB.useGravity = false;
                currentPieceGameObjChildBCs[(int)currentPiece.pieceType].isTrigger = true;
            }

            if (n == 0)
            {
                lastStep.SplitPos = false;
                currentPiece.currentPoint.i += lastStep.Point.i;
                currentPiece.currentPoint.j += lastStep.Point.j;
                gameObject.transform.position = new Vector3((float)Math.Round(gameObject.transform.position.x, 0), gameObject.transform.position.y, (float)Math.Round(gameObject.transform.position.z));

                currentPieceGameObjChildBCs[(int)currentPiece.pieceType].isTrigger = false;
                currentPieceGameObjChildRB.useGravity = true;
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

    public Point[] GetPointsBishopBlock(CustomPiece.MovementTypeEnum movement, Point blockPoint)
    {
        Point[] bishopBlockPoints = new Point[2];
        if (movement.Equals(CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_FORWARD_LEFT))
        {
            Point forwardPoint = movementVectorDictionary[CustomPiece.MovementTypeEnum.MOVEMENT_FORWARD].Point;
            Point leftPoint = movementVectorDictionary[CustomPiece.MovementTypeEnum.MOVEMENT_LEFT].Point;
            bishopBlockPoints[0] = new Point(blockPoint.i + forwardPoint.i, blockPoint.j + forwardPoint.j);
            bishopBlockPoints[1] = new Point(blockPoint.i + leftPoint.i, blockPoint.j + leftPoint.j);
        }
        else if (movement.Equals(CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_BACKWARD_LEFT))
        {
            Point backwardPoint = movementVectorDictionary[CustomPiece.MovementTypeEnum.MOVEMENT_BACKWARD].Point;
            Point leftPoint = movementVectorDictionary[CustomPiece.MovementTypeEnum.MOVEMENT_LEFT].Point;
            bishopBlockPoints[0] = new Point(blockPoint.i + backwardPoint.i, blockPoint.j + backwardPoint.j);
            bishopBlockPoints[1] = new Point(blockPoint.i + leftPoint.i, blockPoint.j + leftPoint.j);
        }
        else if (movement.Equals(CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_FORWARD_RIGHT))
        {
            Point forwardPoint = movementVectorDictionary[CustomPiece.MovementTypeEnum.MOVEMENT_FORWARD].Point;
            Point rightPoint = movementVectorDictionary[CustomPiece.MovementTypeEnum.MOVEMENT_RIGHT].Point;
            bishopBlockPoints[0] = new Point(blockPoint.i + forwardPoint.i, blockPoint.j + forwardPoint.j);
            bishopBlockPoints[1] = new Point(blockPoint.i + rightPoint.i, blockPoint.j + rightPoint.j);
        }
        else if (movement.Equals(CustomPiece.MovementTypeEnum.MOVEMENT_DIAGONAL_BACKWARD_RIGHT))
        {
            Point backwardPoint = movementVectorDictionary[CustomPiece.MovementTypeEnum.MOVEMENT_BACKWARD].Point;
            Point rightPoint = movementVectorDictionary[CustomPiece.MovementTypeEnum.MOVEMENT_RIGHT].Point;
            bishopBlockPoints[0] = new Point(blockPoint.i + backwardPoint.i, blockPoint.j + backwardPoint.j);
            bishopBlockPoints[1] = new Point(blockPoint.i + rightPoint.i, CurrentPiece.currentPoint.j + rightPoint.j);
        }

        return bishopBlockPoints;
    }

    public void RestoreToCurrentPos()
    {
        Vector3 pos = new Vector3(
            gameManager.GetMatrixHandler().MatrixOfCubes[CurrentPiece.currentPoint.i, CurrentPiece.currentPoint.j].transform.position.x,
            1f,
             gameManager.GetMatrixHandler().MatrixOfCubes[CurrentPiece.currentPoint.i, CurrentPiece.currentPoint.j].transform.position.z);

        RestoreToPos(CurrentPiece.currentPoint, pos, true);
    }

    public void RestoreToPos(Point p, Vector3 position, bool keepChildYPos)
    {
        currentPiece.currentPoint = p;
        currentPieceGameObj.transform.position = new Vector3(position.x, 1f, position.z);
        currentPieceGameObj.transform.localRotation = Quaternion.identity;
        currentPieceGameObjChild.transform.localRotation = Quaternion.identity;

        Vector3 childPosInParent = Vector3.zero;
        float currentChildYPos = currentPieceGameObj.transform.GetChild(0).transform.localPosition.y;
        if (keepChildYPos && currentChildYPos<=1f && currentChildYPos>= 0)
        {
            childPosInParent = new Vector3(0f, currentChildYPos, 0f);
        }
        currentPieceGameObjChild.transform.localPosition = childPosInParent; 
    }

    public void DestroyPiece()
    {
        Destroy(this.gameObject);
    }
}

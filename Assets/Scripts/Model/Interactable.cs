using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactable : MonoBehaviour
{
    public enum InteractableType { NEW_PIECE_AVAILABLE, BLOCK, ENABLER, BREAKABLE, ENEMY }

    public InteractableType interactableType;

    private ParticleSystem particles;
    private GameManager gameManager;
    private bool collideOrTrigger;
    private System.Object receivedObject;

    private int stepsToMove;
    public Vector3 lastDirection = new Vector3(0, 0, 0);
    private int accuracy = 10;
    private bool goDown;

    // Start is called before the first frame update
    void Start()
    {
        if (!SceneManager.GetActiveScene().name.Contains("Dev"))
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }


        if (interactableType == InteractableType.BLOCK)
        {
            particles = this.gameObject.transform.GetChild(this.gameObject.transform.childCount - 1).GetComponent<ParticleSystem>();
            particles.gameObject.SetActive(false);
            particles.Stop();

            Color blockColor = this.gameObject.GetComponent<Renderer>().material.color;
            ParticleSystem.MainModule main = particles.main;
            main.startColor = blockColor;

            //ParticleSystem.ColorOverLifetimeModule colorOverLifetime = new ParticleSystem.ColorOverLifetimeModule();
            //colorOverLifetime.
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (interactableType == InteractableType.BLOCK && collideOrTrigger)
        {
            if (stepsToMove > 0)
            {
                gameObject.transform.Translate((lastDirection.y >= 0 ? lastDirection / accuracy : new Vector3(0f, lastDirection.y / accuracy * 2, 0f)), Space.World);
                stepsToMove--;
                if (stepsToMove == 0)
                {
                    if (lastDirection.y >= 0 && goDown)
                    {
                        stepsToMove = accuracy / 2;
                        lastDirection -= new Vector3(0f, 1f, 0f);
                    }
                    else
                    {
                        Block block = (Block)receivedObject;
                        gameObject.SetActive(!goDown);
                        gameManager.OnBlock(goDown, (Block)block, -1);
                        goDown = false;
                        collideOrTrigger = false;
                    }

                }
            }
        }

    }

    public void SetObject(System.Object obj)
    {
        receivedObject = obj;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.tag != null && other.gameObject.tag.Equals("Player"))
        {
            switch (interactableType)
            {
                case InteractableType.NEW_PIECE_AVAILABLE:
                    {
                        HandleNewPiece();
                        break;
                    }

                case InteractableType.BLOCK:
                    {
                        HandleBlock(other, false);
                        break;
                    }
                case InteractableType.ENABLER:
                    {
                        HandleEnabler();
                        break;
                    }
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other != null && other.gameObject.tag != null && other.gameObject.tag.Equals("Player") && !collideOrTrigger)
        {
            switch (interactableType)
            {
                case InteractableType.NEW_PIECE_AVAILABLE:
                    {
                        HandleNewPiece();
                        break;
                    }
                case InteractableType.BLOCK:
                    {
                        HandleBlock(other.collider, true);
                        break;
                    }
                case InteractableType.ENABLER:
                    {
                        HandleEnabler();
                        break;
                    }

            }
        }
    }

    private void HandleNewPiece()
    {
        collideOrTrigger = true;
        gameObject.SetActive(false);
        gameManager.OnNewPieceAvailable(this.gameObject.transform.parent.GetComponent<PieceHandler>().CurrentPiece.pieceType);
    }

    private void HandleBlock(Collider collider, bool fromCollision)
    {
        collideOrTrigger = true;
        PieceHandler piece = collider.transform.parent.GetComponent<PieceHandler>();

        //if is from collision and its bishop or is from trigger and is something else than bishop => exit
        if (!piece.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_KNIGHT) &&
            (
            (fromCollision && !piece.IsDiagonalMovement(piece.LastStep.Movement))
            // (!fromCollision && piece.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_BISHOP))
            || (!fromCollision && piece.IsDiagonalMovement(piece.LastStep.Movement))
          //  || (fromCollision && !piece.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_BISHOP))
           
            ))
        {
            Block block = (Block)receivedObject;


            //check if the block with piece interact is the target one
            if (block.oldPoint.i!=gameManager.GetPieceHandler().currentPoint.i
                || block.oldPoint.j != gameManager.GetPieceHandler().currentPoint.j)
            {
                collideOrTrigger = false;
                return;
            }

            Point newPoint = new Point(block.oldPoint.i + gameManager.GetPieceHandler().LastStep.Point.i, block.oldPoint.j + gameManager.GetPieceHandler().LastStep.Point.j);
            bool canMove = gameManager.GetMatrixHandler().CheckIfCanStep(true, gameManager.GetPieceHandler().GetPoint(gameManager.GetPieceHandler().LastStep.Movement, 1));
            canMove &= !gameManager.GetMatrixHandler().IsBlockOnPoint(newPoint);
            if (canMove) //block is moved
            {
                goDown = !gameManager.GetMatrixHandler().CheckIfCanStep(false, gameManager.GetPieceHandler().GetPoint(gameManager.GetPieceHandler().LastStep.Movement, 1));
                lastDirection = gameManager.GetPieceHandler().LastStep.Position;
                block.oldPoint = newPoint;
                stepsToMove += accuracy;
            }
            else //block stays same position
            {
                goDown = false;
                gameManager.OnBlock(false, null, (int)gameManager.GetPieceHandler().LastStep.OpositeMovement); //back to previous position
                collideOrTrigger = false;
            }

        }
        //block is destroyed by Knight
        else if (fromCollision && piece.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_KNIGHT))
        {
            //make sound
            if (PlayerPrefs.GetInt(Constants.KEY_SOUND) == 1)
            {
                this.gameObject.GetComponent<AudioSource>().Play();
            }

            particles.Play();
            particles.gameObject.SetActive(true);
            this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            this.gameObject.GetComponent<Collider>().enabled = false;
            this.gameObject.GetComponent<Renderer>().enabled = false;

            goDown = false;
            gameManager.OnBlock(false, null, -2); //refresh current position
            collideOrTrigger = false;

            Destroy(gameObject, 1f);
            GlobalSingleton.GetInstance().SetTimeAsync(100, (async) =>
            {
                gameManager.OnBlock(false, (Block)receivedObject, -2); //refresh current position
            });
        }
        else
        {
            collideOrTrigger = false;
        }
    }

    private void HandleEnabler()
    {
        if (!collideOrTrigger)
        {
            collideOrTrigger = true;

            //make sound
            if (PlayerPrefs.GetInt(Constants.KEY_SOUND) == 1)
            {
                AudioSource audio = this.gameObject.GetComponent<AudioSource>();
                audio.Play();
            }

            Enabler enabler = (Enabler)receivedObject;

            //change enabler color
            Color enablerColor = ((enabler.enablerPoint.i + enabler.enablerPoint.j) % 2 == 0 ? Constants.MATRIX_BOX_BLACK_COLOR : Constants.MATRIX_BOX_WHITE_COLOR);
            enabler.GameObj.GetComponent<Renderer>().material.color = enablerColor;
            //change cube color
            Color cubeColor = ((enabler.cubePoint.i + enabler.cubePoint.j) % 2 == 0 ? Constants.MATRIX_BOX_BLACK_COLOR : Constants.MATRIX_BOX_WHITE_COLOR);
            gameManager.GetMatrixHandler().ChangeCubeStatus(enabler.cubePoint, true, cubeColor);

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{

    [Header(" - CAMERAS - ")]
    public Camera cameraFront;
    public Camera cameraTop;
    private Camera cameraMain;

    [Header(" - CANVAS - ")]
    public Canvas canvas;

    [Header(" - MENU - ")]
    public GameObject panelMenu;
    public TMP_Text txtMenuLevel;
    public Button btnArrowL;
    public Button btnArrowR;
    public Button btnStart;
    public GameObject panelPawnAnimation;
    public Image[] imgsStars;

    [Header(" - GAME - ")]
    public Image imgCamera;
    public GameObject panelGame;
    public GameObject panelPieces;
    public Image imgCurrentLevel;
    public TMP_Text txtCurrentLevel;
    public Image imgTooks;
    public TMP_Text txtTooks;
    public Image imgMoves;
    public TMP_Text txtMoves;
    public Image imgChanges;
    public TMP_Text txtChanges;
    public GameObject[] powerupsIcons;


    [Header(" - OTHERS - ")]
    public Image imgBg;
    public ParticleSystem particles;
    public GameObjectHolder gameObjectHolder;
    public GameAdHandler gameAdHandler;

    private PieceHandler pieceHandler;
    private PiecesHandler piecesHandler;
    private ArrowHandler arrowHandler;
    private MatrixHandler matrixHandler;
    private TutorialHandler tutorialHandler;
    private GlobalAudioHandler globalAudioHandler;
    private AlertHandler alertHandler;
    private PanelHandler settingsHandler;
    private PanelHandler gameFinishedHandler;
    private PanelHandler gameOverHandler;
    private PanelHandler levelFinishedHandler;
    private SwipeDetector swipeDetector;
    private Sprite spriteCameraFront;
    private Sprite spriteCameraTop;

    private Delegates.ObjectDelegate objDelegate;

    private bool moveEnabled = true;
    private bool gameFinished;
    private bool levelFinished;
    private bool clickedNext;

    private Vector3 orbitDistance = Vector3.zero;
    private Vector3 orbitPosition = Vector3.zero;

    private Powerup currentPowerup;
    private bool justTookedPowerup;

    //test GUI variables
    //string vvvalue = "not set";
    //string crash = "Crash";

    Dictionary<Cube, CustomPiece.MovementTypeEnum> possibleNextMoves = new Dictionary<Cube, CustomPiece.MovementTypeEnum>();
    Dictionary<Block, CustomPiece.MovementTypeEnum> possibleNextBlocks = new Dictionary<Block, CustomPiece.MovementTypeEnum>();

    //public float w;
    //public float h;
    public float c_s_y;



    private void OnGUI()
    {
        //GUI.Label(new Rect(100f, 100f, 100f, 100f), vvvalue);

        /*if (GUI.Button(new Rect(250, 250, 100, 100), crash))
        {
            if (FirebaseSingleton.GetInstance().areDependencesChecked)
            {
                crash = "CRASH";
                FirebaseSingleton.GetInstance().CrashApp();
            }
            else
            {
                crash = "NO CRASH";
                StartCoroutine(FirebaseSingleton.GetInstance().ICheckFirebaseDependences((obj1) => { }));
            }
        }*/
    }


    // Start is called before the first frame update
    void Awake()
    {
        //PlayerPrefs.SetInt(Constants.KEY_TUTORIAL_STATE, 5);

        if (gameObjectHolder == null)
        {
            gameObjectHolder = GameObject.Find("GameObjectHolder").GetComponent<GameObjectHolder>();
        }

        if (arrowHandler == null)
        {
            //arrowHandler = gameObjectHolder.GetSaferObjectByName("PanelArrows").GetComponent<ArrowHandler>();
            arrowHandler = gameObjectHolder.GetSaferObjectByName("ParentArrows").GetComponent<ArrowHandler>();
        }

        if (piecesHandler == null)
        {
            piecesHandler = gameObjectHolder.GetSaferObjectByName("PanelPieces").GetComponent<PiecesHandler>();
        }

        if (matrixHandler == null)
        {
            matrixHandler = gameObjectHolder.GetSaferObjectByName("ParentMatrix").GetComponent<MatrixHandler>();
        }

        if (tutorialHandler == null)
        {
            tutorialHandler = gameObjectHolder.GetSaferObjectByName("PanelTutorial").GetComponent<TutorialHandler>();
        }

        if (globalAudioHandler == null)
        {
            globalAudioHandler = gameObjectHolder.GetSaferObjectByName("GlobalAudioHandler").GetComponent<GlobalAudioHandler>();
        }

        if (alertHandler == null)
        {
            alertHandler = gameObjectHolder.GetSaferObjectByName("PanelAlert").GetComponent<AlertHandler>();
        }

        if (settingsHandler == null)
        {
            settingsHandler = gameObjectHolder.GetSaferObjectByName("PanelSettings").GetComponent<PanelHandler>();
        }

        if (levelFinishedHandler == null)
        {
            levelFinishedHandler = gameObjectHolder.GetSaferObjectByName("PanelLevelFinished").GetComponent<PanelHandler>();
        }

        if (gameFinishedHandler == null)
        {
            gameFinishedHandler = gameObjectHolder.GetSaferObjectByName("PanelGameFinished").GetComponent<PanelHandler>();
        }

        if (gameOverHandler == null)
        {
            gameOverHandler = gameObjectHolder.GetSaferObjectByName("PanelGameOver").GetComponent<PanelHandler>();
        }

        if (swipeDetector == null)
        {
            swipeDetector = gameObjectHolder.GetSaferObjectByName("SwipeDetector").GetComponent<SwipeDetector>();
        }

        //set sprites
        spriteCameraFront = Resources.Load<Sprite>("Images/Icons/Camera3D");
        spriteCameraTop = Resources.Load<Sprite>("Images/Icons/Camera2D");

        //set firebase
        StartCoroutine(FirebaseSingleton.GetInstance().ICheckFirebaseDependences((obj1) => { }));

        CheckFirstLaunch();
        HandlePatricle();
        InitiateUI();

        //todo test tutorial
        //PlayerPrefs.SetInt(Constants.KEY_TUTORIAL_STATE, 21);

    }

    // Update is called once per frame
    void Update()
    {
        //w = Screen.width;
        //h = Screen.height;
        c_s_y = canvas.scaleFactor;

        //check for touched next moves
        if (!GlobalSingleton.GetInstance().gamePaused)
        {
            if (!swipeDetector.isSwiping)
            {
                if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
                {
                    //android
                    MoveToTouchedCubePosition(Camera.main.ScreenPointToRay(
                        Input.GetTouch(0).position));
                }
                else if (Input.GetMouseButton(0))
                {
                    //windows for debug
                    MoveToTouchedCubePosition(Camera.main.ScreenPointToRay(Input.mousePosition));
                }
            }

        }
    }

    private void MoveToTouchedCubePosition(Ray raycast)
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(raycast, out raycastHit))
        {
            if (raycastHit.collider.name.Contains("Cube"))
            {
                //Debug.Log(raycastHit.collider.name);
                //vvvalue = raycastHit.collider.name;

                Cube cube = GetCubeFromName(raycastHit.collider.name);
                if (possibleNextMoves.ContainsKey(cube))
                {
                    OnClickArrow(cube.StepsToPoint, (int)possibleNextMoves[cube]);
                }
            }
            else if (raycastHit.collider.name.Contains(Interactable.InteractableType.BLOCK.ToString()))
            {
                //Debug.Log(raycastHit.collider.name);
                //vvvalue = raycastHit.collider.name;

                Block block = GetBlockFromName(raycastHit.collider.name);
                Cube cube = matrixHandler.MatrixOfCubes[block.oldPoint.i, block.oldPoint.j];
                if (possibleNextMoves.ContainsKey(cube))
                {
                    OnClickArrow(cube.StepsToPoint, (int)possibleNextMoves[cube]);
                }
            }
        }
    }

    private Cube GetCubeFromName(String cubeName)
    {
        if (matrixHandler.MatrixOfCubes == null)
        {
            return null;
        }
        cubeName = cubeName.Substring(cubeName.IndexOf("_") + 1);
        int i = Int32.Parse(cubeName.Substring(0, cubeName.IndexOf("_")));
        cubeName = cubeName.Substring(cubeName.IndexOf("_") + 1);
        int j = Int32.Parse(cubeName.Substring(0));

        return matrixHandler.MatrixOfCubes[i, j];
    }

    private Block GetBlockFromName(String blockName)
    {
        blockName = blockName.Substring(blockName.IndexOf("_") + 1);
        int i = Int32.Parse(blockName.Substring(0));

        return matrixHandler.BlockArray[i];
    }

    private void CheckFirstLaunch()
    {
        if (PlayerPrefs.GetInt(Constants.KEY_FIRST_LAUNCH) == 0)
        {
            PlayerPrefs.SetInt(Constants.KEY_FIRST_LAUNCH, 1);
            PlayerPrefs.SetInt(Constants.KEY_SOUND, 1);
            PlayerPrefs.SetInt(Constants.KEY_LAST_LEVEL, 1);
        }
    }

    private void HandlePatricle()
    {
        //particle
        particles.Stop();
        var main = particles.main;
        if (SystemInfo.systemMemorySize > Constants.RAM_HIGH)
        {
            main.duration = 150;
            main.maxParticles = 150;
            particles.gameObject.SetActive(true);
            particles.Play();
        }
        else if (SystemInfo.systemMemorySize > Constants.RAM_MEDIUM)
        {
            main.duration = 100;
            main.maxParticles = 100;
            particles.gameObject.SetActive(true);
            particles.Play();
        }
        else if (SystemInfo.systemMemorySize > Constants.RAM_LOW_ACCEPTED)
        {
            main.duration = 100;
            main.maxParticles = 50;
            particles.gameObject.SetActive(true);
            particles.Play();
        }
        else
        {
            Constants.MATRIX_PILLAR_COLOR_02 = Constants.MATRIX_PILLAR_COLOR_02_OPAQUE;
            main.duration = 0;
            main.maxParticles = 0;
            particles.gameObject.SetActive(false);
        }

        particles.gameObject.SetActive(false);
    }

    private void InitiateUI()
    {

        //set camera
        imgCamera.sprite = spriteCameraFront;
        cameraFront.gameObject.SetActive(true);
        cameraTop.gameObject.SetActive(false);
        cameraMain = cameraFront;

        imgBg.gameObject.SetActive(false);

        //menu
        panelMenu.SetActive(true);
        panelGame.SetActive(false);
        alertHandler.HideAlert();
        tutorialHandler.HideTutorial();
        settingsHandler.gameObject.SetActive(false);
        settingsHandler.HidePanel();
        levelFinishedHandler.gameObject.SetActive(false);
        levelFinishedHandler.HidePanel();
        gameFinishedHandler.gameObject.SetActive(false);
        gameFinishedHandler.HidePanel();
        gameOverHandler.gameObject.SetActive(false);
        gameOverHandler.HidePanel();

        //check level
        int level = PlayerPrefs.GetInt(Constants.KEY_LAST_LEVEL);
        bool currentLevelFinished = (GlobalSingleton.GetInstance().GetLevelStarDictionaryValue(level) > 0);
        bool existNextLevel = (level < GlobalSingleton.GetInstance().GetLevelAssetsTexts(false).Length);
        if (currentLevelFinished && existNextLevel)
        {
            level++;
            PlayerPrefs.SetInt(Constants.KEY_LAST_LEVEL, level);
        }
        txtMenuLevel.SetText(level.ToString());
        btnArrowL.gameObject.SetActive((level > 1));
        btnArrowR.gameObject.SetActive(false);

        //change stars
        ChangeStars(level);

        //hide all arrows
        arrowHandler.ChangeArrowsAvailability(false, null);
        piecesHandler.ChangePiecessAvailability(false, null);

        //enable/disable sounds
        settingsHandler.EnableSound(PlayerPrefs.GetInt(Constants.KEY_SOUND));

        //change color
        settingsHandler.ChangeColor(PlayerPrefs.GetInt(Constants.KEY_COLOR));
    }

    private void InitializeLevel(string from)
    {
        gameAdHandler.ShowBannerAd();

        txtCurrentLevel.SetText(matrixHandler.CurrentLevel.ToString());
        txtTooks.SetText("0");
        txtMoves.SetText(matrixHandler.MovesAvailable.ToString());
        txtChanges.SetText(matrixHandler.ChangesAvailable.ToString());


        piecesHandler.ChangePiecessAvailability(false, null);
        arrowHandler.ChangeArrowsAvailability(false, null);

        pieceHandler.ChangeCustomPiece(CustomPiece.PiecesTypeEnum.TYPE_PAWN, false);
        pieceHandler.CurrentPiece.currentPoint = new Point(matrixHandler.StartPoint.i, matrixHandler.StartPoint.j);
        pieceHandler.RestoreToCurrentPos();
        piecesHandler.ChangePiecessAvailability(true, matrixHandler.PiecesAvailable);
        arrowHandler.ChangeArrowsAvailability(true, pieceHandler.CurrentPiece.movementType);

        //
        levelFinished = false;

        //unblock next movement
        moveEnabled = true;
        arrowHandler.ChangeArrowsColor(Constants.ARROW_ENABLED_COLOR, pieceHandler.CurrentPiece.movementType);
        CheckNextMoves();


        //tutorial -> NO. 1
        if (matrixHandler.CurrentLevel == 1)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(1, imgCurrentLevel.transform.position + new Vector3(0f, 10f, 0f), Vector3.up, c_s_y, "Current level", true, (objTutorial1) =>
            {
                tutorialHandler.ShowTutorial(2, cameraMain.WorldToScreenPoint(pieceHandler.CurrentPieceGameObj.transform.position), Vector3.right, c_s_y, "Current chess piece", true, (objTutorial2) =>
                {
                    tutorialHandler.ShowTutorial(3, cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[matrixHandler.FinishPoint.i, matrixHandler.FinishPoint.j].transform.position + Vector3.forward), Vector3.right, c_s_y, "Target", true, (objTutorial3) =>
                    {
                        Cube nextPossibleMoveCube = new Cube();
                        foreach (KeyValuePair<Cube, CustomPiece.MovementTypeEnum> c in possibleNextMoves)
                        {
                            nextPossibleMoveCube = c.Key;
                            break;
                        }

                        //tutorialHandler.ShowTutorial(4, cameraMain.WorldToScreenPoint(arrowHandler.GetPositions()[CustomPiece.MovementTypeEnum.MOVEMENT_FORWARD]), Vector3.left, c_s_y, "Movement arrows according to piece type", true, (objTutorial4) =>
                        tutorialHandler.ShowTutorial(4, cameraMain.WorldToScreenPoint(nextPossibleMoveCube.transform.position + Vector3.forward), Vector3.left, c_s_y, "Tap on the marked cubes to move the player", true, (objTutorial4) =>
                       {
                           tutorialHandler.ShowTutorial(5, imgMoves.transform.position + new Vector3(0f, 10f, 0f), Vector3.up, c_s_y, "Number of moves left", true, (objTutorial5) =>
                           {
                               float cameraSize = imgCamera.GetComponent<RectTransform>().rect.width / 2f * c_s_y;

                               tutorialHandler.ShowTutorial(6, imgCamera.transform.position + new Vector3(-cameraSize, -cameraSize, 0f), Vector3.left, c_s_y, "Change camera perspective - Toggle between 2D and 3D", true, (objTutorial6) =>
                               {
                                   tutorialHandler.ShowTutorial(7, imgCamera.transform.position + new Vector3(-cameraSize, -cameraSize, 0f), Vector3.left, c_s_y, "While camera 3D is on you can rotate the view by SWIPING", false, (objTutorial7) =>
                                   {
                                       moveEnabled = true;
                                       //exit
                                   });
                               });
                           });
                       });
                    });
                });
            });

        }
        else if (matrixHandler.CurrentLevel == 2)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(8, cameraMain.WorldToScreenPoint(matrixHandler.NewPieceAvailable.GameObj.transform.position - new Vector3(0f, 0f, 0f)),
                Vector3.right, c_s_y, "Take the Rook, to enable a new chess piece", false, (objTutorial1) =>
            {
                moveEnabled = true;
                //exit
            });
        }
        else if (matrixHandler.CurrentLevel == 4)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(11, cameraMain.WorldToScreenPoint(matrixHandler.BlockArray[0].GameObj.transform.position - new Vector3(0f, 0f, 0f)),
                Vector3.right, c_s_y, "Push the block to take the shortcut. Careful it will cost you extra moves", false, (objTutorial1) =>
            {
                moveEnabled = true;
                //exit
            });
        }
        else if (matrixHandler.CurrentLevel == 6)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(12, cameraMain.WorldToScreenPoint(matrixHandler.BlockArray[5].GameObj.transform.position - new Vector3(0f, 0f, -0.5f)),
                Vector3.right, c_s_y, "You CAN'T push more than 1 block simultaneously", false, (objTutorial1) =>
            {
                moveEnabled = true;
                //exit
            });
        }
        else if (matrixHandler.CurrentLevel == 19)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(16, cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[matrixHandler.EnablerArray[0].cubePoint.i, matrixHandler.EnablerArray[0].cubePoint.j].transform.position), Vector3.left, c_s_y, "You can't step on red cubes", true, (objTutorial15) =>
            {
                tutorialHandler.ShowTutorial(17, cameraMain.WorldToScreenPoint(matrixHandler.EnablerArray[0].GameObj.transform.position), Vector3.left, c_s_y, "To enable them you need to push the matched colored button", false, (objTutorial16) =>
                {
                    moveEnabled = true;
                    //exit
                });
            });
        }
        else if (matrixHandler.CurrentLevel == 34)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(22, cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[matrixHandler.BreakableArray[0].point.i, matrixHandler.BreakableArray[0].point.j].transform.position),
                Vector3.left, c_s_y, "CRACKED boxes BREAK when pieces are MOVED or CHANGED over it", false, (objTutorial15) =>
            {
                moveEnabled = true;
                //exit
            });
        }


        //send event
        Firebase.Analytics.Parameter[] eventParams = {
            new Firebase.Analytics.Parameter( Constants.PARAM_START_TYPE, from),
            new Firebase.Analytics.Parameter(Constants.PARAM_LEVEL, matrixHandler.CurrentLevel.ToString()),
            new Firebase.Analytics.Parameter(Constants.PARAM_PIECE_COLOR, (PlayerPrefs.GetInt(Constants.KEY_COLOR) == 0 ? Constants.TYPE_WHITE : Constants.TYPE_BLACK)),
            new Firebase.Analytics.Parameter(Constants.PARAM_SOUND_STATUS, (PlayerPrefs.GetInt(Constants.KEY_SOUND) == 0 ?Constants.TYPE_OFF :Constants.TYPE_ON))
        };
        FirebaseSingleton.GetInstance().SendEvents(Constants.EVENT_LEVEL_UP, eventParams);
    }



    private void ChangeStars(int level)
    {
        if (level != 0)
        {
            int stars = GlobalSingleton.GetInstance().GetLevelStarDictionaryValue(level);
            for (int i = 0; i < imgsStars.Length; i++)
            {
                imgsStars[i].fillAmount = (i < stars ? 1 : 0);
            }
        }
    }

    public MatrixHandler GetMatrixHandler()
    {
        return matrixHandler;
    }

    public PieceHandler GetPieceHandler()
    {
        return pieceHandler;
    }

    private void CheckNextMoves()
    {
        //clean cubes and blocks
        CleanNextMovesAndBlocks();

        //check new ones
        foreach (CustomPiece.MovementTypeEnum type in pieceHandler.CurrentPiece.movementType)
        {
            int stepsToPoint = 1;
            Point point = pieceHandler.GetPoint(type, stepsToPoint);
            if (matrixHandler.CheckIfCanStep(false, point))
            {
                CheckNextMovesOnPoint(point, type, stepsToPoint);
            }
            if (currentPowerup != null)
            {
                if (currentPowerup.type == (int)(Powerup.PowerupType.DOUBLE_FULL))
                {
                    if (pieceHandler.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_PAWN))
                    {
                        stepsToPoint = 2;
                        point = pieceHandler.GetPoint(type, stepsToPoint);
                        if (matrixHandler.CheckIfCanStep(false, point))
                        {
                            CheckNextMovesOnPoint(point, type, stepsToPoint);
                        }
                    }
                }
            }
        }
    }

    private void CheckNextMovesOnPoint(Point point, CustomPiece.MovementTypeEnum type, int steptToPoint)
    {

        //add cube
        matrixHandler.MatrixOfCubes[point.i, point.j].StepsToPoint = steptToPoint;
        possibleNextMoves.Add(matrixHandler.MatrixOfCubes[point.i, point.j], type);

        //add block and check if cube remains in list
        CheckNextBlocks(point, type);

        //add change cube type to next move if remain in list after the block check
        if (possibleNextMoves.ContainsKey(matrixHandler.MatrixOfCubes[point.i, point.j]))
        {
            matrixHandler.MatrixOfCubes[point.i, point.j].SetCubeType(Cube.CubeType.TYPE_NEXT);
        }
    }

    private void CheckNextBlocks(Point point, CustomPiece.MovementTypeEnum type)
    {
        foreach (Block block in matrixHandler.BlockArray)
        {
            //check if block is on same position as current point
            if (block.oldPoint.i == point.i && block.oldPoint.j == point.j)
            {
                Point direction = new Point(block.oldPoint.i - pieceHandler.CurrentPiece.currentPoint.i,
                    block.oldPoint.j - pieceHandler.CurrentPiece.currentPoint.j);
                Point newPoint = new Point(block.oldPoint.i + direction.i,
                    block.oldPoint.j + direction.j);

                bool canMove;
                if (pieceHandler.CurrentPiece.pieceType == CustomPiece.PiecesTypeEnum.TYPE_KNIGHT)
                {
                    canMove = GetMatrixHandler().CheckIfCanStep(true, GetPieceHandler()
                    .GetPointByPointDirection(direction, 1));
                }
                else if (pieceHandler.CurrentPiece.pieceType == CustomPiece.PiecesTypeEnum.TYPE_BISHOP
                    || (pieceHandler.CurrentPiece.pieceType == CustomPiece.PiecesTypeEnum.TYPE_QUEEN)
                    && pieceHandler.IsDiagonalMovement(type))
                {
                    //check if has where to step and push the block
                    canMove = GetMatrixHandler().CheckIfCanStep(true, GetPieceHandler()
                    .GetPointByPointDirection(direction, 2));

                    //check if has blocks in its way
                    Point[] pp = pieceHandler.GetPointsBishopBlock(type, block.oldPoint);
                    canMove &= !GetMatrixHandler().IsBlockOnPoint(newPoint);
                    canMove &= !GetMatrixHandler().IsBlockOnPoint(pp[0]);
                    canMove &= !GetMatrixHandler().IsBlockOnPoint(pp[1]);
                }
                else
                {
                    canMove = GetMatrixHandler().CheckIfCanStep(true, GetPieceHandler()
                    .GetPointByPointDirection(direction, 2));

                    canMove &= !GetMatrixHandler().IsBlockOnPoint(newPoint);
                }


                if (canMove)
                {
                    //add to move list
                    block.ChangeToNextMove(true);
                    possibleNextBlocks.Add(block, type);
                }
                else if (block.GameObj != null && block.GameObj.activeInHierarchy)
                {
                    //remove the cube from the list of next moves if block is above
                    matrixHandler.MatrixOfCubes[block.oldPoint.i, block.oldPoint.j].StepsToPoint = 1;
                    possibleNextMoves.Remove(matrixHandler.MatrixOfCubes[block.oldPoint.i, block.oldPoint.j]);
                }

                return;
            }
        }
    }

    private void CleanNextMovesAndBlocks()
    {
        //restore cubes from old points
        foreach (Cube cube in possibleNextMoves.Keys)
        {
            cube.StepsToPoint = 1;
            cube.SetCubeType(cube.GetPreviousType());
        }
        possibleNextMoves.Clear();

        //restore blocks from old points
        foreach (Block block in possibleNextBlocks.Keys)
        {
            block.ChangeToNextMove(false);
        }
        possibleNextBlocks.Clear();
    }

    private void GameOver(string subtitle)
    {
        //clean cubes and blocks
        CleanNextMovesAndBlocks();

        gameOverHandler.ChangeTitleSubtitle("Game Over!", subtitle);
        gameOverHandler.ShowPanel();

        canvas.enabled = false;
        gameAdHandler.ShowInterstitialAd((objAd) =>
        {
            canvas.enabled = true;

            //make sound
            globalAudioHandler.PlaySound(GlobalAudioHandler.AudioType.GAME_OVER);
        });
    }

    public bool IsFrontCamera()
    {
        return cameraMain == cameraFront;
    }

    private void CleanPowerup()
    {
        if (currentPowerup != null)
        {
            powerupsIcons[currentPowerup.type].SetActive(false);
            currentPowerup = null;
        }
    }


    #region From other classes

    public void OnNewPieceAvailable(CustomPiece.PiecesTypeEnum type)
    {
        matrixHandler.AddNewTypeOfPiece(type);
        piecesHandler.ChangePiecessAvailability(true, matrixHandler.PiecesAvailable);


        if (type == CustomPiece.PiecesTypeEnum.TYPE_ROOK)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(9, piecesHandler.GetObjectWorldPotionCentered(CustomPiece.PiecesTypeEnum.TYPE_ROOK, canvas),
                Vector3.right, c_s_y, "ROOK piece type has been enabled", true, (objTutorial8) =>
                {
                    tutorialHandler.ShowTutorial(10, imgChanges.transform.position + new Vector3(0f, 10f, 0f), Vector3.up, c_s_y, "Number of piece changes left", false, (objTutorial9) =>
                    {
                        moveEnabled = true;
                        //exit
                    });
                });
        }
        else if (type == CustomPiece.PiecesTypeEnum.TYPE_KNIGHT)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(13, piecesHandler.GetObjectWorldPotionCentered(CustomPiece.PiecesTypeEnum.TYPE_KNIGHT, canvas),
                Vector3.right, c_s_y, "KNIGHT piece has been enabled", true, (objTutorial12) =>
                {
                    tutorialHandler.ShowTutorial(14, cameraMain.WorldToScreenPoint(matrixHandler.BlockArray[0].GameObj.transform.position), Vector3.right, c_s_y, "KNIGHT is the only piece that can destroy blocks which lands on", true, (objTutorial13) =>
                    {
                        tutorialHandler.ShowTutorial(15, cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[3, 4].transform.position), Vector3.right, c_s_y, "KNIGHT is the only piece that can jump over obstacles (Cubes, Pieces, Holes)", false, (objTutorial14) =>
                        {
                            moveEnabled = true;
                            //exit
                        });
                    });
                });
        }
        else if (type == CustomPiece.PiecesTypeEnum.TYPE_BISHOP)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(18, piecesHandler.GetObjectWorldPotionCentered(CustomPiece.PiecesTypeEnum.TYPE_BISHOP, canvas),
                Vector3.right, c_s_y, "BISHOP piece has been enabled", true, (objTutorial18) =>
                {
                    tutorialHandler.ShowTutorial(19, cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[matrixHandler.BlockArray[3].oldPoint.i, matrixHandler.BlockArray[3].oldPoint.j].transform.position), Vector3.right, c_s_y, "BISHOP can push blocks diagonally", true, (objTutorial19) =>
                    {
                        Vector3 direction = matrixHandler.MatrixOfCubes[matrixHandler.BlockArray[5].oldPoint.i, matrixHandler.BlockArray[5].oldPoint.j].transform.position;
                        direction += Vector3.forward / 2f;
                        direction += Vector3.right / 2f;

                        tutorialHandler.ShowTutorial(20, cameraMain.WorldToScreenPoint(direction), Vector3.right, c_s_y, "If the blocks are surrounded by other blocks, BISHOP can't push them", false, (objTutorial20) =>
                        {
                            moveEnabled = true;
                            //exit

                        });

                    });
                });
        }
        else if (type == CustomPiece.PiecesTypeEnum.TYPE_QUEEN)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(21, piecesHandler.GetObjectWorldPotionCentered(CustomPiece.PiecesTypeEnum.TYPE_QUEEN, canvas),
                Vector3.right, c_s_y, "QUEEN piece has been enabled", false, (objTutorial21) =>
                {

                    moveEnabled = true;
                    //exit

                });
        }
        else if (type == CustomPiece.PiecesTypeEnum.TYPE_KING)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(23, piecesHandler.GetObjectWorldPotionCentered(CustomPiece.PiecesTypeEnum.TYPE_QUEEN, canvas),
                Vector3.right, c_s_y, "KING piece has been enabled", false, (objTutorial23) =>
                {
                    moveEnabled = true;
                    //exit
                });
        }
    }

    public void OnBlock(bool activeCube, Block block, int sendPieceBackMovement)
    {
        if (sendPieceBackMovement > -1) // return (hit the block)
        {
            pieceHandler.MakeMovement((CustomPiece.MovementTypeEnum)sendPieceBackMovement, 0, (obj) =>
            {
                pieceHandler.RestoreToPos(pieceHandler.CurrentPiece.currentPoint,
                    matrixHandler.MatrixOfCubes[pieceHandler.CurrentPiece.currentPoint.i, pieceHandler.CurrentPiece.currentPoint.j].transform.position,
                    true);
                txtMoves.SetText(matrixHandler.MovesAvailable.ToString());
                moveEnabled = true;
            });
        }
        else if (sendPieceBackMovement > -2) //moved
        {
            if (activeCube)
            {
                matrixHandler.ChangeCubeStatus(block.oldPoint, true, block.GameObj.GetComponent<Renderer>().material.color);
            }

            piecesHandler.ChangePiecessAvailability(true, matrixHandler.PiecesAvailable);
            matrixHandler.RecalculateMoves(block.cost);
            txtMoves.SetText(matrixHandler.MovesAvailable.ToString());
            if (matrixHandler.MovesAvailable <= 0 && !levelFinished)
            {
                GameOver("");
            }
        }
        else
        {
            //just restore new position - when knight jumped
            if (block != null)
            {
                matrixHandler.RecalculateMoves(block.cost);
                txtMoves.SetText(matrixHandler.MovesAvailable.ToString());
                if (matrixHandler.MovesAvailable <= 0 && !levelFinished)
                {
                    GameOver("");
                }

            }
            pieceHandler.RestoreToPos(pieceHandler.CurrentPiece.currentPoint,
                matrixHandler.MatrixOfCubes[pieceHandler.CurrentPiece.currentPoint.i, pieceHandler.CurrentPiece.currentPoint.j].transform.position,
                true);
        }

        GlobalSingleton.GetInstance().SetTimeAsync(50, (obj) =>
        {
            //restore movement if damaged (outside and inside)
            pieceHandler.RestoreToCurrentPos();
            if (matrixHandler.MovesAvailable > 0)
            {
                //check next moves
                CheckNextMoves();
            }
        });
    }

    public void OnBreakable(Breakable breakable)
    {
        breakable.SteppedOver();

        GetMatrixHandler().ChangeBreakableStatus(
               breakable.GetBreakableIndex(),
                new Point(breakable.point.i, breakable.point.j));

        if (breakable.IsBroken())
        {
            breakable.GameObj.transform.GetChild(0).gameObject.SetActive(true);
            Destroy(breakable.GameObj, 1f);
        }
    }

    public void OnPowerup(Powerup powerup)
    {
        //disable last powerup 
        CleanPowerup();

        justTookedPowerup = true;
        currentPowerup = powerup;
        powerupsIcons[powerup.type].SetActive(true);
        matrixHandler.RemovePowerup(powerup);
    }

    public void OnOrbit(Vector3 direction)
    {
        if (!GlobalSingleton.GetInstance().gamePaused
             && cameraMain == cameraFront
             && matrixHandler.MatrixOfCubes != null
           )
        {
            //move camera in direction received, by Space Self
            //NOT Space World cuz the camera will rotate and tehere will be a mess
            cameraFront.gameObject.transform.Translate(direction, Space.Self);

            //calculate original distance and position
            if (orbitDistance.Equals(Vector3.zero) || orbitPosition.Equals(Vector3.zero))
            {
                Vector3 orbitDir = matrixHandler.MatrixOfCubes[7, 7].gameObject.transform.position
               - matrixHandler.MatrixOfCubes[0, 0].gameObject.transform.position;

                orbitPosition = matrixHandler.MatrixOfCubes[0, 0].gameObject.transform.position
                    + (orbitDir / 2f);
                orbitDistance = orbitPosition - cameraFront.transform.position;
            }

            //roate in Space World, cuz it roatate around a point out of the camera game object
            cameraFront.transform.LookAt(orbitPosition);

            //check distance
            Vector3 newOrbitDistance = orbitPosition - cameraFront.transform.position;
            if (newOrbitDistance.magnitude != orbitDistance.magnitude)
            {
                float difference = newOrbitDistance.magnitude - orbitDistance.magnitude;
                //move camera in space world
                cameraFront.transform.transform.Translate(newOrbitDistance.normalized * difference, Space.World);
            }
        }
    }
    #endregion


    #region Click Events

    public void OnClickChangeStartLevel(int levelModification)
    {
        int level = System.Convert.ToInt32(txtMenuLevel.GetParsedText());
        int lastLevel = PlayerPrefs.GetInt(Constants.KEY_LAST_LEVEL);

        if ((levelModification > 0 && (level + levelModification) <= lastLevel)
            || (levelModification < 0 && (level + levelModification) > 0))
        {
            level += levelModification;
            txtMenuLevel.SetText(level.ToString());

            btnArrowR.gameObject.SetActive(true);
            btnArrowL.gameObject.SetActive(true);
            if (levelModification > 0 && level == lastLevel)
            {
                btnArrowR.gameObject.SetActive(false);
            }
            else if (levelModification < 0 && level == 1)
            {
                btnArrowL.gameObject.SetActive(false);
            }
        }

        ChangeStars(level);
    }

    public void OnClickStart()
    {
        if (panelPawnAnimation.GetComponent<Animator>().GetFloat("State") == 0)
        {
            btnStart.enabled = false;
            panelPawnAnimation.GetComponent<Animator>().SetFloat("State", 1);
            if (PlayerPrefs.GetInt(Constants.KEY_SOUND) == 1)
            {
                panelPawnAnimation.GetComponent<AudioSource>().Play();
            }


            return;
        }

        GlobalSingleton.GetInstance().gamePaused = false;

        //make sound
        globalAudioHandler.PlaySound(GlobalAudioHandler.AudioType.GAME);

        moveEnabled = true;

        panelPawnAnimation.GetComponent<Animator>().SetFloat("State", 0);

        piecesHandler.InitializePanelPieces(canvas);

        panelGame.SetActive(true);
        btnStart.enabled = true;
        panelMenu.SetActive(false);
        imgBg.gameObject.SetActive(true);


        //create piece
        GameObject prefabPiece = (GameObject)Resources.Load("Prefabs/" + CustomPiece.prefabName, typeof(GameObject));
        pieceHandler = Instantiate(prefabPiece, new Vector3(0, 1, 0), Quaternion.identity).GetComponent<PieceHandler>();
        pieceHandler.gameObject.tag = "Player";
        pieceHandler.gameObject.transform.GetChild(0).tag = "Player";

        //hide all arrows and pieces
        arrowHandler.ChangeArrowsAvailability(false, null);
        piecesHandler.ChangePiecessAvailability(false, null);

        //change values
        pieceHandler.ChangeCustomPiece(CustomPiece.PiecesTypeEnum.TYPE_PAWN, false);
        arrowHandler.ChangeArrowsAvailability(true, pieceHandler.CurrentPiece.movementType);

        //make matrix
        int level = System.Convert.ToInt32(txtMenuLevel.GetParsedText());
        matrixHandler.InitializeMatrix(level, true, (obj) =>
        {
            InitializeLevel(Constants.TYPE_START);

            //orbit camera
            OnOrbit(Vector3.zero);

            GlobalSingleton.GetInstance().SetTimeAsync(10, (objAsync) =>
            {
                imgBg.gameObject.SetActive(false);
            });
        });
    }

    public void OnClickRestart()
    {
        //make sound
        globalAudioHandler.PlaySound(GlobalAudioHandler.AudioType.GAME);

        moveEnabled = true;
        settingsHandler.HidePanel();
        levelFinishedHandler.HidePanel();
        gameFinishedHandler.HidePanel();
        gameOverHandler.HidePanel();

        matrixHandler.GoLevel(matrixHandler.CurrentLevel, (obj) =>
        {
            InitializeLevel(Constants.TYPE_RESTART);
        });
    }

    public void OnClickHome()
    {
        //make sound
        globalAudioHandler.PlaySound(GlobalAudioHandler.AudioType.SILENCE);

        gameAdHandler.HideBannerAd();
        matrixHandler.DestroyMatrix();
        pieceHandler.DestroyPiece();
        InitiateUI();
    }

    public void OnClickBack(PanelHandler panel)
    {
        gameAdHandler.ShowBannerAd();
        panel.HidePanel();
    }

    public void OnClickSounds()
    {
        int sound = 1 - PlayerPrefs.GetInt(Constants.KEY_SOUND); // to change values
        settingsHandler.EnableSound(sound);
        globalAudioHandler.EnableAudioSource(sound == 1);
    }

    public void OnClickColor()
    {
        int color = 1 - PlayerPrefs.GetInt(Constants.KEY_COLOR); // to change values
        settingsHandler.ChangeColor(color);
        piecesHandler.ChangePiecessAvailability(false, null);
        piecesHandler.ChangePiecessAvailability(true, matrixHandler.PiecesAvailable);
        pieceHandler.ChangeColor();
    }

    public void OnClickReview()
    {
        Application.OpenURL("market://details?id=" + Application.productName);
    }

    public void OnClickNext()
    {
        if (clickedNext) { return; }
        clickedNext = true;

        levelFinishedHandler.HidePanel();

        if (gameFinished)
        {
            gameFinishedHandler.ShowPanel();
            gameAdHandler.HideBannerAd();

            //make sound
            globalAudioHandler.PlaySound(GlobalAudioHandler.AudioType.FINISH);
        }
        else
        {
            if (UnityEngine.Random.Range(0, 10) < 5)
            {
                canvas.enabled = false;
                gameAdHandler.ShowInterstitialAd((objAd) =>
                {
                    canvas.enabled = true;
                });
            }
            matrixHandler.GoLevel(matrixHandler.CurrentLevel + 1, (obj) =>
            {
                InitializeLevel(Constants.TYPE_NEXT);
            });
        }
    }

    public void OnClickSettings()
    {
        if (GlobalSingleton.GetInstance().gamePaused) { return; }

        gameAdHandler.HideBannerAd();
        settingsHandler.ShowPanel(30);
    }

    public void OnClickCamera()
    {
        if (!moveEnabled || GlobalSingleton.GetInstance().gamePaused)
        {
            return;
        }

        if (cameraMain == cameraFront)
        {
            imgCamera.sprite = spriteCameraTop;
            cameraFront.gameObject.SetActive(false);
            cameraTop.gameObject.SetActive(true);
            cameraMain = cameraTop;
            matrixHandler.ChangePowerupCamera(false);
        }
        else
        {
            imgCamera.sprite = spriteCameraFront;
            cameraFront.gameObject.SetActive(true);
            cameraTop.gameObject.SetActive(false);
            cameraMain = cameraFront;
            matrixHandler.ChangePowerupCamera(true);
        }
    }

    public void OnClickArrow(int steps, int index)
    {
        //  Debug.Log("OnClickArrow | Move:" + (moveEnabled ? "enabled" : "disabled") + " | Paused:" + (GlobalSingleton.GetInstance().gamePaused ? "enabled" : "disabled"));
        if (!moveEnabled || GlobalSingleton.GetInstance().gamePaused) { return; }

        CustomPiece.MovementTypeEnum mov = (CustomPiece.MovementTypeEnum)index;


        //regular movement
        Point p = pieceHandler.GetPoint(mov, steps);

        if (matrixHandler.CheckIfCanStep(false, pieceHandler.GetPoint(mov, steps)))
        {
            //block next movement
            moveEnabled = false;
            arrowHandler.ChangeArrowsColor(Constants.ARROW_DISABLED_COLOR, pieceHandler.CurrentPiece.movementType);
            //make movement
            pieceHandler.MakeMovement(mov, steps, (obj) =>
            {
                //need async  to be sure that all the movements are finished (when moves a cube)
                GlobalSingleton.GetInstance().SetTimeAsync(100, (async) =>
                {
                    int movesSpent = 1;
                    bool b = true;
                    //todo check if the onPowerup is called before this
                    if (currentPowerup != null && !justTookedPowerup)
                    {
                        if (currentPowerup.type == (int)Powerup.PowerupType.DOUBLE_FULL)
                        {
                            if (pieceHandler.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_KNIGHT))
                            {
                                b = false;
                                movesSpent = 0; 
                            }
                            if (!pieceHandler.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_KING))
                            {
                                CleanPowerup();
                            }
                        }
                        justTookedPowerup = !b;
                    }
                    else {
                        justTookedPowerup = false;
                    }
                   

                    matrixHandler.RecalculateMoves(movesSpent);
                    txtMoves.SetText(matrixHandler.MovesAvailable.ToString());
                    txtCurrentLevel.SetText(matrixHandler.CurrentLevel.ToString());
                    //Debug.Log("Recalculate onClickArrow: " + txtMoves.text);

                    //restore position if damaged (outside and inside)
                    pieceHandler.RestoreToCurrentPos();

                    if (matrixHandler.CheckGoNextLevel(pieceHandler.CurrentPieceGameObj.transform.position))
                    {
                        levelFinished = true;
                        //clean cubes and blocks
                        CleanNextMovesAndBlocks();

                        gameAdHandler.HideBannerAd();
                        int bestScore = GlobalSingleton.GetInstance().GetLevelStarDictionaryValue(matrixHandler.CurrentLevel);
                        int currentStars = matrixHandler.CalculateStars();
                        if (bestScore < currentStars)
                        {
                            bestScore = currentStars;
                            GlobalSingleton.GetInstance().ChangeLevelStar(matrixHandler.CurrentLevel, matrixHandler.CalculateStars());
                        }

                        levelFinishedHandler.ShowPanel(matrixHandler.CurrentLevel, (objLevelFinishedShowed) =>
                        {
                            clickedNext = false;
                            levelFinishedHandler.ShowStars(currentStars);
                        });
                        gameFinished = (matrixHandler.CurrentLevel + 1 > GlobalSingleton.GetInstance().GetLevelAssetsTexts(false).Length);
                        if (!gameFinished && matrixHandler.CurrentLevel + 1 > PlayerPrefs.GetInt(Constants.KEY_LAST_LEVEL))
                        {
                            PlayerPrefs.SetInt(Constants.KEY_LAST_LEVEL, matrixHandler.CurrentLevel + 1);
                        }
                    }
                    else if (matrixHandler.MovesAvailable <= 0)
                    {
                        GameOver("");
                    }
                    else
                    {
                        //unblock next movement
                        moveEnabled = true;
                        arrowHandler.ChangeArrowsColor(Constants.ARROW_ENABLED_COLOR, pieceHandler.CurrentPiece.movementType);

                        //check next possible moves
                        CheckNextMoves();
                    }
                });
            });
        }
    }

    public void OnClickPiece(int index)
    {
        if (!moveEnabled || GlobalSingleton.GetInstance().gamePaused)
        {
            return;
        }

        if (matrixHandler.ChangesAvailable > 0 && matrixHandler.IsPieceChangeAvailable(index) && (int)pieceHandler.CurrentPiece.pieceType != index)
        {
            //disable movement while changing piece
            moveEnabled = false;

            //check if had powerup
            if (currentPowerup != null
                && currentPowerup.type == (int)Powerup.PowerupType.DOUBLE_FULL
                && !pieceHandler.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_KING))
            {
                powerupsIcons[currentPowerup.type].SetActive(false);
                currentPowerup = null;
            }

            //update changes left
            arrowHandler.ChangeArrowsAvailability(false, pieceHandler.CurrentPiece.movementType);
            pieceHandler.ChangeCustomPiece((CustomPiece.PiecesTypeEnum)index, true);
            arrowHandler.ChangeArrowsAvailability(true, pieceHandler.CurrentPiece.movementType);
            matrixHandler.RecalculateChanges(1);
            txtChanges.SetText(matrixHandler.ChangesAvailable.ToString());

            //check if on breakable box
            if (matrixHandler.GetBreakableBox(pieceHandler.CurrentPiece.currentPoint) != null)
            {
                OnBreakable(matrixHandler.GetBreakableBox(pieceHandler.CurrentPiece.currentPoint));
                pieceHandler.MakePieceFallThrough();
                GlobalSingleton.GetInstance().SetTimeAsync(150, (obj) =>
                {
                    GameOver("- Woops this should not happened! -");
                });
                return;
            }

            //restore movement if damaged (outside and inside)
            pieceHandler.RestoreToCurrentPos();
            //check next moves
            CheckNextMoves();
            //enable movement
            moveEnabled = true;
        }
        else if (matrixHandler.ChangesAvailable == 0)
        {
            alertHandler.ShowAlert("No more piece changes left");
        }
        else if (!matrixHandler.IsPieceChangeAvailable(index))
        {
            alertHandler.ShowAlert("Piece not available");
        }
    }

    #endregion
}

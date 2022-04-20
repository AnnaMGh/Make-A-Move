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
    private EnemyIconHandler enemyIconHandler;
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

    public Powerup CurrentPowerup { get; private set; }
    public Powerup LastPowerupInUse { get; set; }
    private bool justTookedPowerup;

    //test GUI variables
    string vvvalue = "not set";
    //string crash = "Crash";

    //Dictionary<Cube, CustomPiece.MovementTypeEnum> possibleNextMoves = new Dictionary<Cube, CustomPiece.MovementTypeEnum>();
    //Dictionary<Block, CustomPiece.MovementTypeEnum> possibleNextBlocks = new Dictionary<Block, CustomPiece.MovementTypeEnum>();

    //public float w;
    //public float h;
    public float c_s_y;

    private int level49Powerup = 2; //2 - pawn pawerup, 1 - horse powerup, 0 - oher piece powerup
    private bool lastMoveWasPowerupStrongOnABox = false;
    private bool lastPowerupWasStrongOnABoxByKing = false;
    private bool checkingNextMovesInProgress = false;

    private PieceHandler currentPieceHandlerToMove;


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

        if (enemyIconHandler == null)
        {
            enemyIconHandler = gameObjectHolder.GetSaferObjectByName("PanelEnemies").GetComponent<EnemyIconHandler>();
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


        //todo test tutorial
        PlayerPrefs.SetInt(Constants.KEY_TUTORIAL_STATE, 0);
        PlayerPrefs.SetString(Constants.KEY_TUTORIAL_STATE_CSV, "");
        HardcodeToLevel(50);
        //end todo


        CheckFirstLaunch();
        HandlePatricle();
        InitiateUI();
    }

    // Update is called once per frame
    void Update()
    {
        //w = Screen.width;
        //h = Screen.height;
        GlobalSingleton.GetInstance().SetCanvasScale(canvas.scaleFactor);

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

    private void HardcodeToLevel(int level)
    {
        if (PlayerPrefs.GetInt(Constants.KEY_LAST_LEVEL) < level)
        {
            PlayerPrefs.SetInt(Constants.KEY_LAST_LEVEL, level);
            for (int i = 1; i < level; i++)
            {
                GlobalSingleton.GetInstance().ChangeLevelStar(i, 3);
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
                //Debug.Log("MoveToTouchedCubePosition Cube: " + raycastHit.collider.name);
                //vvvalue = raycastHit.collider.name;

                Cube cube = GetCubeFromName(raycastHit.collider.name);
                //Debug.Log("MoveToTouchedCubePosition Cube Possible: " + pieceHandler.PossibleNextMoves.ListToString());

                if (pieceHandler.PossibleNextMoves.ContainsKey(cube))
                {
                    //Debug.Log("MoveToTouchedCubePosition Found: " + cube.ToString());
                    OnClickArrow(cube.StepsToPoint, (int)pieceHandler.PossibleNextMoves[cube]);
                }
            }
            else if (raycastHit.collider.name.Contains(Interactable.InteractableType.BLOCK.ToString()))
            {
                //Debug.Log("MoveToTouchedCubePosition Block: " + raycastHit.collider.name);
                //vvvalue = raycastHit.collider.name;

                Block block = GetBlockFromName(raycastHit.collider.name);
                Cube cube = matrixHandler.MatrixOfCubes[block.oldPoint.i, block.oldPoint.j];
                //Debug.Log("MoveToTouchedCubePosition Block Cube: " + raycastHit.collider.name);
                if (pieceHandler.PossibleNextMoves.ContainsKey(cube))
                {
                    // Debug.Log("MoveToTouchedCubePosition Found: " + cube.ToString());
                    OnClickArrow(cube.StepsToPoint, (int)pieceHandler.PossibleNextMoves[cube]);
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

        //set camera if not leave it as it is
        if (cameraMain == null)
        {
            imgCamera.sprite = spriteCameraFront;
            cameraFront.gameObject.SetActive(true);
            cameraTop.gameObject.SetActive(false);
            cameraMain = cameraFront;
        }


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

        int enemiesOnLevel = 0;
        if (matrixHandler.EnemyArray != null && matrixHandler.EnemyArray.Length > 0)
        {
            enemiesOnLevel = matrixHandler.EnemyArray.Length;
        }
        enemyIconHandler.SetEnemiesOnLevel(enemiesOnLevel);

        piecesHandler.ChangePiecessAvailability(false, null);
        arrowHandler.ChangeArrowsAvailability(false, null);

        pieceHandler.ChangeCustomPiece(CustomPiece.PiecesTypeEnum.TYPE_PAWN, false);
        pieceHandler.CurrentPiece.currentPoint = new Point(matrixHandler.StartPoint.i, matrixHandler.StartPoint.j);
        pieceHandler.RestoreToCurrentPos();
        piecesHandler.ChangePiecessAvailability(true, matrixHandler.PiecesAvailable);
        arrowHandler.ChangeArrowsAvailability(true, pieceHandler.CurrentPiece.movementType);

        //
        levelFinished = false;

        //remove powerup if any activated
        CleanPowerup(true);

        //unblock next movement
        moveEnabled = true;
        arrowHandler.ChangeArrowsColor(Constants.ARROW_ENABLED_COLOR, pieceHandler.CurrentPiece.movementType);

        //mark player as current piece to move
        SetCurrentPieceTurn(pieceHandler);
        //check next moves (for player and enemy)
        CheckNextAllMoves();

        //refresh camera
        matrixHandler.ChangePowerupCamera(IsFrontCamera());


        //tutorial -> NO. 1
        if (matrixHandler.CurrentLevel == 1)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_LEVEL,
                imgCurrentLevel.transform.position + new Vector3(0f, 10f, 0f),
               (objTutorial1) =>
            {
                tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_PIECE,
                    cameraMain.WorldToScreenPoint(pieceHandler.CurrentPieceGameObj.transform.position),
                   (objTutorial2) =>
                {
                    tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_TARGET,
                        cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[matrixHandler.FinishPoint.i, matrixHandler.FinishPoint.j].transform.position + Vector3.forward),
                       (objTutorial3) =>
                    {
                        Cube nextPossibleMoveCube = new Cube();
                        foreach (KeyValuePair<Cube, CustomPiece.MovementTypeEnum> c in pieceHandler.PossibleNextMoves)
                        {
                            nextPossibleMoveCube = c.Key;
                            break;
                        }

                        tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_MOVEMENT,
                            cameraMain.WorldToScreenPoint(nextPossibleMoveCube.transform.position + Vector3.forward),
                            (objTutorial4) =>
                       {
                           tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_MOVEMENT_NUMBER,
                                imgMoves.transform.position + new Vector3(0f, 10f, 0f),
                                (objTutorial5) =>
                            {
                                float cameraSize = imgCamera.GetComponent<RectTransform>().rect.width / 2f * c_s_y;

                                tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_CAMERA,
                                    imgCamera.transform.position + new Vector3(-cameraSize, -cameraSize, 0f),
                                     (objTutorial6) =>
                                {
                                    tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.UPDATE_CAMERA,
                                        imgCamera.transform.position + new Vector3(-cameraSize, -cameraSize, 0f),
                                        (objTutorial7) =>
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
            tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_ROOK,
                cameraMain.WorldToScreenPoint(matrixHandler.NewPieceAvailable.GameObj.transform.position),
                 (objTutorial1) =>
            {
                moveEnabled = true;
                //exit
            });
        }
        else if (matrixHandler.CurrentLevel == 4)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_BLOCK,
                cameraMain.WorldToScreenPoint(matrixHandler.BlockArray[0].GameObj.transform.position),
                  (objTutorial1) =>
            {
                moveEnabled = true;
                //exit
            });
        }
        else if (matrixHandler.CurrentLevel == 6)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_BLOCK_02,
                cameraMain.WorldToScreenPoint(matrixHandler.BlockArray[5].GameObj.transform.position - new Vector3(0f, 0f, -0.5f)),
                (objTutorial1) =>
            {
                moveEnabled = true;
                //exit
            });
        }
        else if (matrixHandler.CurrentLevel == 19)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_ENABLER,
                cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[matrixHandler.EnablerArray[0].cubePoint.i, matrixHandler.EnablerArray[0].cubePoint.j].transform.position),
                   (objTutorial15) =>
            {
                tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_ENABLER_BUTTON,
                    cameraMain.WorldToScreenPoint(matrixHandler.EnablerArray[0].GameObj.transform.position),
                     (objTutorial16) =>
                {
                    moveEnabled = true;
                    //exit
                });
            });
        }
        else if (matrixHandler.CurrentLevel == 34)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_BREAKABLE_BOXES,
                cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[matrixHandler.BreakableArray[0].point.i, matrixHandler.BreakableArray[0].point.j].transform.position),
               (objTutorial) =>
            {
                moveEnabled = true;
                //exit
            });
        }
        else if (matrixHandler.CurrentLevel == 49)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.POWERUP_DOUBLE_FULL_INTRODUCE,
                cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[matrixHandler.PowerupArray[0].point.i, matrixHandler.PowerupArray[0].point.j].transform.position),
                 (objTutorial) =>
                {

                    tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.POWERUP_DOUBLE_FULL_LOST,
                        cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[matrixHandler.PowerupArray[0].point.i, matrixHandler.PowerupArray[0].point.j].transform.position),
                         (objTutoria2) =>
                        {
                            moveEnabled = true;
                            //exit
                        });
                });
        }
        else if (matrixHandler.CurrentLevel == 52)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.POWERUP_STRONG_INTRODUCE,
                cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[matrixHandler.PowerupArray[0].point.i,
                matrixHandler.PowerupArray[0].point.j].transform.position),
               (objTutorial) =>
            {
                tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.POWERUP_STRONG_KNIGHT,
                    cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[matrixHandler.PowerupArray[0].point.i,
                    matrixHandler.PowerupArray[0].point.j].transform.position),
                 (objTutoria2) =>
                 {
                     tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.POWERUP_STRONG_KING,
                    cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[matrixHandler.PowerupArray[0].point.i,
                    matrixHandler.PowerupArray[0].point.j].transform.position),
                 (objTutoria3) =>
                 {
                     moveEnabled = true;
                     //exit
                 });
                 });
            });
        }
        else if (matrixHandler.CurrentLevel == 57)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.POWERUP_DIZZY_INTRODUCE,
                cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[matrixHandler.PowerupArray[0].point.i, matrixHandler.PowerupArray[0].point.j].transform.position),
                  (objTutorial) =>
            {
                tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.POWERUP_DIZZY_INTRODUCE_02,
                    cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[matrixHandler.PowerupArray[0].point.i, matrixHandler.PowerupArray[0].point.j].transform.position),
                 (objTutoria2) =>
                 {
                     moveEnabled = true;
                     //exit
                 });
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

    private void CalculateCurrentPieceTurn()
    {
        if (pieceHandler.IsPieceTurn)
        {
            //if was the player and has enemy => next is the enemy
            pieceHandler.IsPieceTurn = false;
            if (matrixHandler.EnemyArray != null && matrixHandler.EnemyArray.Length > 0)
            {

                int randomIndex = UnityEngine.Random.Range(0, matrixHandler.EnemyArray.Length);
                SetCurrentPieceTurn(matrixHandler.EnemyArray[randomIndex].EnemyPieceHandler);
                /*for (int i = 0; i < matrixHandler.EnemyArray.Length; i++)
                {
                    if (matrixHandler.EnemyArray[i].EnemyPieceHandler.IsLastPieceTurn)
                    {
                        //disable last
                        matrixHandler.EnemyArray[i].EnemyPieceHandler.IsLastPieceTurn = false;
                        int nextOne = i + 1;
                        if (nextOne >= matrixHandler.EnemyArray.Length)
                        {
                            nextOne = 0;
                        }
                        //enable current
                        SetCurrentPieceTurn(matrixHandler.EnemyArray[nextOne].EnemyPieceHandler);
                        break;
                    }
                }*/
            }
            else
            {
                //if was the player and does't have enemy => next is the player
                SetCurrentPieceTurn(pieceHandler);
            }
        }
        else
        {
            SetCurrentPieceTurn(pieceHandler);
        }

        Debug.Log("Piece turn: " + (GetCurrentPieceTurn().isEnemy ? "enemy" : "player"));

    }

    private void SetCurrentPieceTurn(PieceHandler pieceHandlerTurn)
    {
        //reset player piece handler
        pieceHandler.IsPieceTurn = false;

        //reset enemies piece handler
        if (matrixHandler.EnemyArray != null && matrixHandler.EnemyArray.Length > 0)
        {
            for (int i = 0; i < matrixHandler.EnemyArray.Length; i++)
            {
                matrixHandler.EnemyArray[i].EnemyPieceHandler.IsLastPieceTurn = false;
                matrixHandler.EnemyArray[i].EnemyPieceHandler.IsPieceTurn = false;
            }
        }

        currentPieceHandlerToMove = pieceHandlerTurn;
        currentPieceHandlerToMove.IsPieceTurn = true;
    }

    private PieceHandler GetCurrentPieceTurn()
    {
        Debug.Log("Piece turn get: " + (currentPieceHandlerToMove.isEnemy ? "enemy" : "player"));
        return currentPieceHandlerToMove;
    }

    private void CheckNextAllMoves()
    {
        if (GetCurrentPieceTurn().Equals(pieceHandler))
        {
            CheckNextEnemiesMoves();
            CheckNextPlayerMoves();
        }
        else
        {
            CheckNextPlayerMoves();
            CheckNextEnemiesMoves();
        }

    }
    private void CheckNextEnemiesMoves()
    {
        if (matrixHandler.EnemyArray != null && matrixHandler.EnemyArray.Length > 0)
        {
            foreach (Enemy enemy in matrixHandler.EnemyArray)
            {
                CheckNextMoves(enemy.EnemyPieceHandler);
            }
        }
    }
    private void CheckNextPlayerMoves()
    {
        CheckNextMoves(pieceHandler);
    }

    private void CheckNextMoves(PieceHandler receivedPieceHandler)
    {
        if (checkingNextMovesInProgress)
        {
            return;
        }
        checkingNextMovesInProgress = true;

        //clean cubes and blocks
        CleanNextMovesAndBlocks(receivedPieceHandler.PossibleNextMoves, receivedPieceHandler.PossibleNextBlocks);

        //check new ones
        foreach (CustomPiece.MovementTypeEnum type in receivedPieceHandler.CurrentPiece.movementType)
        {
            if (receivedPieceHandler.isEnemy)
            {
                //just normal check
                SetAndCheckNextMovesOnPoint(receivedPieceHandler, type, 1);
            }
            else
            {
                // normal step
                if (CurrentPowerup == null || CurrentPowerup.type != (int)Powerup.PowerupType.DOUBLE_FULL)
                {
                    SetAndCheckNextMovesOnPoint(receivedPieceHandler, type, 1);
                }
                //powerup step
                else if (CurrentPowerup != null)
                {
                    if (CurrentPowerup.type == (int)Powerup.PowerupType.DOUBLE_FULL)
                    {
                        CheckPowerupDoubleFullNextMoves(receivedPieceHandler, type);
                    }
                }
            }
        }

        checkingNextMovesInProgress = false;
    }

    private void CheckPowerupDoubleFullNextMoves(PieceHandler receivedPieceHandler, CustomPiece.MovementTypeEnum type)
    {

        //set stop steps
        int stopSteps = 2;
        if (receivedPieceHandler.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_PAWN))
        {
            stopSteps = 3;
        }
        else if (receivedPieceHandler.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_ROOK)
        || receivedPieceHandler.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_BISHOP)
        || receivedPieceHandler.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_QUEEN))
        {
            stopSteps = matrixHandler.MatrixIndexLength;
        }

        for (int i = 1; i < stopSteps; i++)
        {
            if (!SetAndCheckNextMovesOnPoint(receivedPieceHandler, type, i))
            {
                break;
            }
        }
    }

    /* private bool SetAndCheckNextMovesOnPoint(CustomPiece.MovementTypeEnum type, int stepsToPoint)
     {
         Point point = pieceHandler.GetPoint(type, stepsToPoint);
         if (matrixHandler.CheckIfCanStep(false, true, point))
         {
             int possibleNextBlocksNr = pieceHandler.PossibleNextBlocks.Count;

             CheckNextMovesOnPoint(point, type, stepsToPoint);

             return (possibleNextBlocksNr == pieceHandler.PossibleNextBlocks.Count
                 && pieceHandler.PossibleNextMoves.ContainsKey(matrixHandler.MatrixOfCubes[point.i, point.j]));
         }

         return false;
     }*/

    private bool SetAndCheckNextMovesOnPoint(PieceHandler receivedPieceHandler,
        CustomPiece.MovementTypeEnum type, int stepsToPoint)
    {
        Point point = receivedPieceHandler.GetPoint(type, stepsToPoint);
        if (matrixHandler.CheckIfCanStep(false, true, point))
        {
            int possibleNextBlocksNr = receivedPieceHandler.PossibleNextBlocks.Count;

            CheckNextMovesOnPoint(receivedPieceHandler, point, type, stepsToPoint);

            return (possibleNextBlocksNr == receivedPieceHandler.PossibleNextBlocks.Count
                && receivedPieceHandler.PossibleNextMoves.ContainsKey(matrixHandler.MatrixOfCubes[point.i, point.j]));
        }

        return false;
    }

    private void CheckNextMovesOnPoint(PieceHandler receivedPieceHandler, Point point, CustomPiece.MovementTypeEnum type, int stepsToPoint)
    {
        //add cube
        matrixHandler.MatrixOfCubes[point.i, point.j].StepsToPoint = stepsToPoint;
        receivedPieceHandler.PossibleNextMoves.Add(matrixHandler.MatrixOfCubes[point.i, point.j], type);

        //add block and check if cube remains in list
        CheckNextBlocks(receivedPieceHandler, point, type, stepsToPoint);

        //add change cube type to next move if remain in list after the block check
        if (receivedPieceHandler.PossibleNextMoves.ContainsKey(matrixHandler.MatrixOfCubes[point.i, point.j]))
        {
            Cube.CubeType cubeType = Cube.CubeType.TYPE_NEXT;
            if (receivedPieceHandler.isEnemy)
            {
                cubeType = Cube.CubeType.TYPE_NEXT_ENEMY;
            }
            matrixHandler.MatrixOfCubes[point.i, point.j].SetCubeType(cubeType);
        }
    }

    private void CheckNextBlocks(PieceHandler receivedPieceHandler, Point point, CustomPiece.MovementTypeEnum type, int stepsToPoint)
    {
        foreach (Block block in matrixHandler.BlockArray)
        {
            //check if block is on same position as current point
            if (block.oldPoint.i == point.i && block.oldPoint.j == point.j)
            {
                //Point direction = new Point(block.oldPoint.i - pieceHandler.CurrentPiece.currentPoint.i,
                //    block.oldPoint.j - pieceHandler.CurrentPiece.currentPoint.j);
                Point piecePoint = receivedPieceHandler.GetPoint(type, stepsToPoint - 1);
                Point direction = new Point(block.oldPoint.i - piecePoint.i,
                    block.oldPoint.j - piecePoint.j);
                Point newPoint = new Point(block.oldPoint.i + direction.i,
                    block.oldPoint.j + direction.j);

                bool canMove;
                if (receivedPieceHandler.CurrentPiece.pieceType == CustomPiece.PiecesTypeEnum.TYPE_KNIGHT)
                {
                    canMove = GetMatrixHandler().CheckIfCanStep(true, true, receivedPieceHandler
                    .GetSetPointByPointDirection(piecePoint, direction, 1));
                }
                else if (receivedPieceHandler.IsDiagonalMovement(type))
                {
                    //check if has where to step and push the block
                    bool checkEnabler = (CurrentPowerup == null || CurrentPowerup.type != (int)Powerup.PowerupType.STRONG);
                    canMove = GetMatrixHandler().CheckIfCanStep(true, checkEnabler, receivedPieceHandler
                   .GetSetPointByPointDirection(piecePoint, direction, 2));

                    //check if has blocks in its way
                    if (checkEnabler)
                    {
                        Point[] pp = receivedPieceHandler.GetPointsBishopBlock(type, block.oldPoint);
                        canMove &= !GetMatrixHandler().IsBlockOnPoint(newPoint);
                        canMove &= !GetMatrixHandler().IsBlockOnPoint(pp[0]);
                        canMove &= !GetMatrixHandler().IsBlockOnPoint(pp[1]);
                    }
                }
                else
                {
                    bool checkEnabler = (CurrentPowerup == null || CurrentPowerup.type != (int)Powerup.PowerupType.STRONG);
                    canMove = GetMatrixHandler().CheckIfCanStep(true, checkEnabler, receivedPieceHandler
                    .GetSetPointByPointDirection(piecePoint, direction, 2));

                    //check if has blocks in its way
                    if (checkEnabler)
                    {
                        canMove &= !GetMatrixHandler().IsBlockOnPoint(newPoint);
                    }
                }

                //check if powerup behind only when powerup strong is not active
                if ((CurrentPowerup == null || CurrentPowerup.type != (int)Powerup.PowerupType.STRONG))
                {
                    canMove &= !GetMatrixHandler().IsPowerupOnPoint(newPoint);
                }



                if (canMove)
                {
                    //add to move list
                    block.ChangeToNextMove(true);
                    receivedPieceHandler.PossibleNextBlocks.Add(block, type);
                }
                else if (block.GameObj != null && block.GameObj.activeInHierarchy)
                {
                    //remove the cube from the list of next moves if block is above
                    matrixHandler.MatrixOfCubes[block.oldPoint.i, block.oldPoint.j].StepsToPoint = 1;
                    receivedPieceHandler.PossibleNextMoves.Remove(matrixHandler.MatrixOfCubes[block.oldPoint.i, block.oldPoint.j]);
                }

                break;
            }
        }
    }

    private void CleanNextMovesAndBlocks(Dictionary<Cube, CustomPiece.MovementTypeEnum> receivedPossibleNextMoves,
        Dictionary<Block, CustomPiece.MovementTypeEnum> receivedPossibleNextBlocks)
    {
        //restore cubes from old points
        foreach (Cube cube in receivedPossibleNextMoves.Keys)
        {
            cube.StepsToPoint = 1;
            cube.SetCubeType(cube.GetPreviousType());
        }
        receivedPossibleNextMoves.Clear();

        //restore blocks from old points
        foreach (Block block in receivedPossibleNextBlocks.Keys)
        {
            block.ChangeToNextMove(false);
        }
        receivedPossibleNextBlocks.Clear();
    }

    /*private void CleanNextMovesAndBlocks()
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
    }*/

    private void GameOver(string subtitle)
    {
        //clean cubes and blocks
        CleanNextMovesAndBlocks(pieceHandler.PossibleNextMoves, pieceHandler.PossibleNextBlocks);

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

    private void CleanPowerup(bool cleanAll)
    {
        lastMoveWasPowerupStrongOnABox = false;
        lastPowerupWasStrongOnABoxByKing = false;
        if (CurrentPowerup != null)
        {
            powerupsIcons[CurrentPowerup.type].SetActive(false);
            CurrentPowerup = null;
        }
        if (cleanAll)
        {
            LastPowerupInUse = null;
        }
        pieceHandler.HidePowerupTrotus();
    }

    private CustomPiece.MovementTypeEnum GetDizzyMovement(PieceHandler receivedPieceHandler, CustomPiece.MovementTypeEnum mov)
    {

        int i = UnityEngine.Random.Range(0, 2);

        if (i % 2 == 0 && receivedPieceHandler.PossibleNextMoves.Count > 1)
        {
            int randomIndex = UnityEngine.Random.Range(0,
                receivedPieceHandler.PossibleNextMoves.Count);
            int j = 0;
            int putRandomBetweenValues = -1;
            foreach (CustomPiece.MovementTypeEnum movement in
                receivedPieceHandler.PossibleNextMoves.Values)
            {
                if (j == randomIndex)
                {
                    if (mov == movement)
                    {
                        if (j == receivedPieceHandler.PossibleNextMoves.Count - 1)
                        {
                            putRandomBetweenValues = UnityEngine.Random.Range(0,
                                receivedPieceHandler.PossibleNextMoves.Count - 1);
                        }
                        else if (j == 0)
                        {
                            putRandomBetweenValues = UnityEngine.Random.Range(1,
                                receivedPieceHandler.PossibleNextMoves.Count);
                        }
                    }
                    else
                    {
                        mov = movement;
                    }
                    break;
                }
                j++;
            }

            //put first
            if (putRandomBetweenValues >= 0)
            {
                j = 0;
                foreach (CustomPiece.MovementTypeEnum movement in receivedPieceHandler.PossibleNextMoves.Values)
                {
                    if (j == putRandomBetweenValues)
                    {
                        mov = movement;
                        break;
                    }
                    j++;
                }
            }
        }

        return mov;
    }

    private void RecalculateAndCheckMoves(int movesToCalculate)
    {
        matrixHandler.RecalculateMoves(movesToCalculate);
        txtMoves.SetText(matrixHandler.MovesAvailable.ToString());
        if (matrixHandler.MovesAvailable <= 0 && !levelFinished)
        {
            GameOver("");
        }
    }


    #region From other classes

    public void OnTestGUI(string text)
    {
        vvvalue = text;
    }

    public void OnNewPieceAvailable(CustomPiece.PiecesTypeEnum type)
    {
        matrixHandler.AddNewTypeOfPiece(type);
        piecesHandler.ChangePiecessAvailability(true, matrixHandler.PiecesAvailable);


        if (type == CustomPiece.PiecesTypeEnum.TYPE_ROOK)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_ROOK_02,
                piecesHandler.GetObjectWorldPotionCentered(CustomPiece.PiecesTypeEnum.TYPE_ROOK, canvas),
                (obj) =>
                {
                    tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_CHANGES,
                        imgChanges.transform.position + new Vector3(0f, 10f, 0f),
                        (obj2) =>
                    {
                        moveEnabled = true;
                        //exit
                    });
                });
        }
        else if (type == CustomPiece.PiecesTypeEnum.TYPE_KNIGHT)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_KNIGHT,
                piecesHandler.GetObjectWorldPotionCentered(CustomPiece.PiecesTypeEnum.TYPE_KNIGHT, canvas),
                (obj) =>
                {
                    tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_KNIGHT_02,
                        cameraMain.WorldToScreenPoint(matrixHandler.BlockArray[0].GameObj.transform.position),
                        (obj2) =>
                    {
                        tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_KNIGHT_03,
                            cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[3, 4].transform.position),
                            (obj3) =>
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
            tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_BISHOP,
                piecesHandler.GetObjectWorldPotionCentered(CustomPiece.PiecesTypeEnum.TYPE_BISHOP, canvas),
                (obj) =>
                {
                    tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_BISHOP_02,
                        cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[matrixHandler.BlockArray[3].oldPoint.i, matrixHandler.BlockArray[3].oldPoint.j].transform.position),
                        (obj2) =>
                    {
                        Vector3 direction = matrixHandler.MatrixOfCubes[matrixHandler.BlockArray[5].oldPoint.i, matrixHandler.BlockArray[5].oldPoint.j].transform.position;
                        direction += Vector3.forward / 2f;
                        direction += Vector3.right / 2f;

                        tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_BISHOP_03,
                            cameraMain.WorldToScreenPoint(direction),
                           (obj3) =>
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
            tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_QUEEN,
                piecesHandler.GetObjectWorldPotionCentered(CustomPiece.PiecesTypeEnum.TYPE_QUEEN, canvas),
              (obj) =>
                {

                    moveEnabled = true;
                    //exit

                });
        }
        else if (type == CustomPiece.PiecesTypeEnum.TYPE_KING)
        {
            moveEnabled = false;
            tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.INTRODUCE_KING,
                piecesHandler.GetObjectWorldPotionCentered(CustomPiece.PiecesTypeEnum.TYPE_QUEEN, canvas),
               (objTutorial23) =>
                {
                    moveEnabled = true;
                    //exit
                });
        }
    }

    public void OnBlock(bool activeCube, Block block, int sendPieceBackMovement)
    {
        Debug.Log("OnBlock");
        if (sendPieceBackMovement > -1) // return (hit the block)
        {
            pieceHandler.MakeMovement((CustomPiece.MovementTypeEnum)sendPieceBackMovement, 0, (obj) =>
            {
                pieceHandler.RestoreToPos(pieceHandler.CurrentPiece.currentPoint,
                    matrixHandler.MatrixOfCubes[pieceHandler.CurrentPiece.currentPoint.i, pieceHandler.CurrentPiece.currentPoint.j].transform.position,
                    true);
                RecalculateAndCheckMoves(0);
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
            RecalculateAndCheckMoves(block.cost);
        }
        else if (sendPieceBackMovement == -3) //destroyed by powerup
        {
            // check if last powerup was the strong powerup
            // and it was tooked by king (to check if half of the king's strong powerup was used)
            if (pieceHandler.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_KING))
            {
                lastPowerupWasStrongOnABoxByKing = true;
            }

            if (LastPowerupInUse != null && LastPowerupInUse.type == (int)Powerup.PowerupType.STRONG
            && CurrentPowerup != null && CurrentPowerup == LastPowerupInUse)
            {
                if (!lastMoveWasPowerupStrongOnABox
                    && pieceHandler.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_KING))
                {
                    lastMoveWasPowerupStrongOnABox = true;
                }
                else if ((lastMoveWasPowerupStrongOnABox
                    && pieceHandler.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_KING))
                    || !pieceHandler.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_KING))
                {
                    CleanPowerup(true);
                }
            }
            else
            {
                lastMoveWasPowerupStrongOnABox = true;
            }

            matrixHandler.RemoveBlock(block, false);
            RecalculateAndCheckMoves(0);
        }
        else
        {
            //just restore new position - when knight jumped
            if (block != null)
            {
                matrixHandler.RemoveBlock(block, false);
                RecalculateAndCheckMoves(block.cost);
            }
            /*pieceHandler.RestoreToPos(pieceHandler.CurrentPiece.currentPoint,
                matrixHandler.MatrixOfCubes[pieceHandler.CurrentPiece.currentPoint.i, pieceHandler.CurrentPiece.currentPoint.j].transform.position,
                true);*/
        }

        OnPolish(0);

        /*GlobalSingleton.GetInstance().SetTimeAsync(50, (obj) =>
        {
            //restore movement if damaged (outside and inside)
            pieceHandler.RestoreToCurrentPos();
            if (matrixHandler.MovesAvailable > 0)
            {
                //check next moves
               // Debug.Log("OnBlock- CheckNextMoves");
                CheckNextMoves();
            }
        });*/
    }

    public void OnPolish(int timeInMilliseconds)
    {
        GlobalSingleton.GetInstance().SetTimeAsync(timeInMilliseconds, (async) =>
        {
            //Debug.Log("OnPolish - Enter");
            //restore movement if damaged (outside and inside)
            pieceHandler.RestoreToCurrentPos();
            if (matrixHandler.MovesAvailable > 0)
            {
                //check next moves
                //Debug.Log("OnPolish - CheckNextMoves");
                CheckNextAllMoves();
            }
            else
            {
                CleanNextMovesAndBlocks(pieceHandler.PossibleNextMoves, pieceHandler.PossibleNextBlocks);
            }
            //Debug.Log("OnPolish - Done");
        });


    }

    public void OnEnabler(Enabler enabler)
    {
        //change cube color
        Color cubeColor = ((enabler.cubePoint.i + enabler.cubePoint.j) % 2 == 0 ? Constants.MATRIX_BOX_BLACK_COLOR : Constants.MATRIX_BOX_WHITE_COLOR);

        GetMatrixHandler().ChangeCubeStatus(enabler.cubePoint, true, cubeColor);
        CheckNextAllMoves();
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
        CleanPowerup(false);

        justTookedPowerup = true;
        CurrentPowerup = powerup;
        powerupsIcons[powerup.type].SetActive(true);
        pieceHandler.ShowPowerupTrotus(powerup.type);

        //remove after sound done
        GlobalSingleton.GetInstance().SetTimeAsync(1000, obj =>
        {
            matrixHandler.RemovePowerup(powerup);
        });


        if (matrixHandler.CurrentLevel == 49)
        {
            if (level49Powerup == 2)
            {
                moveEnabled = false;

                Point p = pieceHandler.GetSetPointByPointDirection(powerup.point, new Point(0, 1), 2);

                tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.POWERUP_DOUBLE_FULL_PAWN,
                    cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[p.i, p.j].transform.position),
                     (objTutorial1) =>
                    {
                        level49Powerup--;
                        moveEnabled = true;
                        //exit
                    });
            }
            else if (level49Powerup == 1)
            {
                moveEnabled = false;
                tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.POWERUP_DOUBLE_FULL_KNIGHT,
                    cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[powerup.point.i, powerup.point.j].transform.position),
                    (obj) =>
                    {
                        level49Powerup--;
                        moveEnabled = true;
                        //exit
                    });
            }
            else if (level49Powerup == 0)
            {
                tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.POWERUP_DOUBLE_FULL_ROOK_BISHOP_QUEEN,
                    cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[powerup.point.i, powerup.point.j].transform.position),
                       (obj) =>
                       {
                           tutorialHandler.ShowTutorial(TutorialHandler.TutorialTitle.POWERUP_DOUBLE_FULL_KING,
                               cameraMain.WorldToScreenPoint(matrixHandler.MatrixOfCubes[powerup.point.i, powerup.point.j].transform.position),
                              (obj2) =>
                   {
                       level49Powerup--;
                       moveEnabled = true;
                       //exit
                   });
                       });
            }
        }

    }

    public void OnEnemy(Enemy enemy)
    {
        enemyIconHandler.NewTook();

     

        //remove after sound done
        GlobalSingleton.GetInstance().SetTimeAsync(1000, obj =>
        {
            matrixHandler.RemoveEnemy(enemy);
            CheckNextAllMoves();
        });
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

        //change enemies colors
        if (matrixHandler.EnemyArray != null && matrixHandler.EnemyArray.Length > 0)
        {
            foreach (Enemy enemy in matrixHandler.EnemyArray)
            {
                enemy.ChangeColor();
            }
        }
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
        //Debug.Log("OnClickArrow");
        //  Debug.Log("OnClickArrow | Move:" + (moveEnabled ? "enabled" : "disabled") + " | Paused:" + (GlobalSingleton.GetInstance().gamePaused ? "enabled" : "disabled"));
        if (!moveEnabled || GlobalSingleton.GetInstance().gamePaused) { return; }

        CustomPiece.MovementTypeEnum mov = (CustomPiece.MovementTypeEnum)index;

        PieceHandler receivedPieceHandler = GetCurrentPieceTurn();
        //restore position if damaged (outside and inside)
        receivedPieceHandler.RestoreToCurrentPos();


        if (CurrentPowerup != null)
        {
            if (CurrentPowerup.type == (int)Powerup.PowerupType.DIZZY)
            {
                mov = GetDizzyMovement(pieceHandler, mov);
            }
        }


        //regular movement
        Point p = receivedPieceHandler.GetPoint(mov, steps);
        bool checkEnabler = (CurrentPowerup == null || CurrentPowerup.type != (int)Powerup.PowerupType.STRONG);
        if (matrixHandler.CheckIfCanStep(false, checkEnabler, receivedPieceHandler.GetPoint(mov, steps)))
        {
            //block next movement
            moveEnabled = false;
            arrowHandler.ChangeArrowsColor(Constants.ARROW_DISABLED_COLOR, receivedPieceHandler.CurrentPiece.movementType);

            //keep last powerup in use
            if (CurrentPowerup != null)
            {
                if (CurrentPowerup.type == (int)Powerup.PowerupType.DOUBLE_FULL)
                {
                    if (!receivedPieceHandler.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_KING))
                    {
                        LastPowerupInUse = CurrentPowerup;
                    }
                }
            }

            //make movement
            receivedPieceHandler.MakeMovement(mov, steps, (obj) =>
            {
                //need async  to be sure that all the movements are finished (when moves a cube)
                GlobalSingleton.GetInstance().SetTimeAsync(100, (async) =>
                    {
                        int movesSpent = (receivedPieceHandler.isEnemy?0:1);
                        bool b = true;
                        //todo check if the onPowerup is called before this
                        if (CurrentPowerup != null && !justTookedPowerup)
                        {
                            if (CurrentPowerup.type == (int)Powerup.PowerupType.DOUBLE_FULL)
                            {

                                if (receivedPieceHandler.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_KNIGHT))
                                {
                                    b = false;
                                    movesSpent = 0;
                                }
                                if (!receivedPieceHandler.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_KING))
                                {
                                    CleanPowerup(true);
                                }
                            }
                            else if (CurrentPowerup.type == (int)Powerup.PowerupType.DIZZY)
                            {

                                b = true;
                                movesSpent = (CurrentPowerup.nrOfFreeMovesAvailable > 0 ? 0 : 1);
                                CurrentPowerup.nrOfFreeMovesAvailable--;
                                if (CurrentPowerup.nrOfFreeMovesAvailable <= 0)
                                {
                                    CleanPowerup(true);
                                }
                            }
                            justTookedPowerup = !b;
                        }
                        else
                        {
                            justTookedPowerup = false;
                        }


                        matrixHandler.RecalculateMoves(movesSpent);
                        txtMoves.SetText(matrixHandler.MovesAvailable.ToString());
                        txtCurrentLevel.SetText(matrixHandler.CurrentLevel.ToString());
                        //Debug.Log("Recalculate onClickArrow: " + txtMoves.text);

                        //restore position if damaged (outside and inside)
                        receivedPieceHandler.RestoreToCurrentPos();

                        if (matrixHandler.CheckGoNextLevel(receivedPieceHandler.CurrentPieceGameObj.transform.position))
                        {
                            levelFinished = true;
                            //clean cubes and blocks
                            CleanNextMovesAndBlocks(receivedPieceHandler.PossibleNextMoves, receivedPieceHandler.PossibleNextBlocks);

                            gameAdHandler.HideBannerAd();
                            int bestScore = GlobalSingleton.GetInstance().GetLevelStarDictionaryValue(matrixHandler.CurrentLevel);
                            int currentStars = matrixHandler.CalculateStars();
                            if (bestScore < currentStars)
                            {
                                bestScore = currentStars;
                                GlobalSingleton.GetInstance().ChangeLevelStar(matrixHandler.CurrentLevel, bestScore);
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
                            arrowHandler.ChangeArrowsColor(Constants.ARROW_ENABLED_COLOR, receivedPieceHandler.CurrentPiece.movementType);

                            //move the enemy
                            CalculateCurrentPieceTurn();

                            //check next possible moves
                            //Debug.Log("OnClickArrow - CheckNextMoves");

                            CheckNextAllMoves();

                            //make enemy move
                            if (GetCurrentPieceTurn().isEnemy)
                            {
                                if (GetCurrentPieceTurn().PossibleNextMoves != null && GetCurrentPieceTurn().PossibleNextMoves.Count > 0)
                                {
                                    int randomIndex = UnityEngine.Random.Range(0, GetCurrentPieceTurn().PossibleNextMoves.Count);
                                    int randomMovementIndex = 0;
                                    int index = 0;
                                    foreach (Cube c in GetCurrentPieceTurn().PossibleNextMoves.Keys)
                                    {
                                        if (index == randomIndex)
                                        {
                                            randomMovementIndex = (int)GetCurrentPieceTurn().PossibleNextMoves[c];
                                            break;
                                        }
                                        index++;
                                    }
                                    moveEnabled = true;
                                    OnClickArrow(1, randomMovementIndex);
                                }
                            }
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

        if (level49Powerup == 2 && matrixHandler.CurrentLevel == 49)
        {
            alertHandler.ShowAlert("Take the DOUBLE_FULL POWERUP with the PAWN");
        }
        else if (matrixHandler.ChangesAvailable > 0 && matrixHandler.IsPieceChangeAvailable(index) && (int)pieceHandler.CurrentPiece.pieceType != index)
        {
            //disable movement while changing piece
            moveEnabled = false;

            //check if had powerup
            if (CurrentPowerup != null)
            {
                if ((CurrentPowerup.type == (int)Powerup.PowerupType.DOUBLE_FULL
                     && !pieceHandler.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_KING))
                   ||
                   CurrentPowerup.type == (int)Powerup.PowerupType.DIZZY)
                {
                    //powerupsIcons[CurrentPowerup.type].SetActive(false);
                    //CurrentPowerup = null;
                    CleanPowerup(true);
                }
                else if (CurrentPowerup.type == (int)Powerup.PowerupType.STRONG
                   && lastPowerupWasStrongOnABoxByKing
                   && pieceHandler.CurrentPiece.pieceType.Equals(CustomPiece.PiecesTypeEnum.TYPE_KING))
                {
                    // if as strong powerup and it was used before by the king
                    // then piece was changed but powerup not used
                    // then piece was changed again with the king
                    // make the king incapable of using x2 of powerup again
                    lastPowerupWasStrongOnABoxByKing = false;
                    lastMoveWasPowerupStrongOnABox = true;
                }
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
            CheckNextAllMoves();
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

using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DevelopmentManager : MonoBehaviour
{

    public enum DevelopmentType { TYPE_CREATE, TYPE_EDIT }
    public enum DevelopmentMenuType { TYPE_BASIC, TYPE_EXTRAS, TYPE_NEW }

    [Header(" - CAMERAS - ")]
    public Camera cameraFront;
    public Camera cameraTop;
    private Camera cameraMain;

    [Header(" - MENU - ")]
    public GameObject panelMenu;
    public TMP_Text txtMenuLevel;
    public Button btnArrowL;
    public Button btnArrowR;

    [Header(" - DEVELOP - ")]
    public Image imgCamera;
    public GameObject panelDev;
    public MatrixHandler matrixHandler;
    public Button save;

    [Header("1. Basic")]
    public GameObject btnBasic;
    public GameObject panelBasic;
    public TMP_InputField impLevel;
    public TMP_InputField impMaximumMoves;
    public TMP_InputField impMaximumChanges;
    public TMP_InputField impGoalMoves;
    public TMP_InputField impGoalChanges;
    public TMP_InputField impStartPointI;
    public TMP_InputField impStartPointJ;
    public TMP_InputField impFinishPointI;
    public TMP_InputField impFinishPointJ;
    public TMP_InputField impPieces;

    [Header("2. Extras")]
    public GameObject btnExtras;
    public GameObject panelExtras;
    public TMP_InputField impNewAvailablePieceType;
    public TMP_InputField impNewAvailablePiecePointI;
    public TMP_InputField impNewAvailablePiecePointJ;
    public TMP_InputField impBlockNr;
    public GameObject contentBlocks;
    public List<BlockDev> blockDevList;
    public TMP_InputField impEnablerNr;
    public GameObject contentEnablers;
    public List<EnablerDev> enablerDevList;
    public TMP_InputField impBreakableNr;
    public GameObject contentBreakable;
    public List<BreakableDev> breakableDevList;

    [Header("3. New")]
    public GameObject btnNew;
    public GameObject panelNew;
    public TMP_InputField impPowerupNr;
    public GameObject contentPowerup;
    public List<PowerupDev> powerupDevList;

    [Header(" - OTHERS - ")]
    public AlertHandler alertHandler;
    public DevelopmentType CurrentDevelopmentType;
    public TextAsset[] CurrentLevels;
    public int CurrentLevel;
    public Point startPoint;
    public Point endPoint;
    public PieceAvailable pieceAvailable;
    public CustomPiece.PiecesTypeEnum[] piecesType;


    private Sprite spriteCameraFront;
    private Sprite spriteCameraTop;


    private DevelopmentType developmentType;
    private GameObject prefabBlock;
    private GameObject prefabEnabler;
    private GameObject prefabBreakable;
    private GameObject prefabPowerup;
    private TextAsset[] textAssets;
    private int currentLevel;
    private Level level;
    private bool initialize;


    // Start is called before the first frame update
    void Start()
    {
        InitializeUI();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentDevelopmentType = developmentType;
        CurrentLevels = textAssets;
        CurrentLevel = currentLevel;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                Cube c = hit.transform.gameObject.GetComponent<Cube>();
                if (c != null)
                {
                    c.enabledStatus = !c.enabledStatus;
                    Debug.Log(c.name);
                }
            }
        }
    }

    private void InitializeUI()
    {
        //prefabs
        prefabBlock = Resources.Load<GameObject>("Prefabs/BlockDev");
        prefabEnabler = Resources.Load<GameObject>("Prefabs/EnablerDev");
        prefabBreakable = Resources.Load<GameObject>("Prefabs/BreakableDev");
        prefabPowerup = Resources.Load<GameObject>("Prefabs/PowerupDev");

        //set sprites
        spriteCameraFront = Resources.Load<Sprite>("Images/Icons/Camera_01");
        spriteCameraTop = Resources.Load<Sprite>("Images/Icons/Camera_02");

        panelMenu.SetActive(true);
        panelDev.SetActive(false);

        //set camera
        imgCamera.sprite = spriteCameraTop;
        cameraFront.gameObject.SetActive(true);
        cameraTop.gameObject.SetActive(false);
        cameraMain = cameraFront;

        PrepareMenuLevels();
    }

    private void PrepareMenuLevels() {
        textAssets = GlobalSingleton.GetInstance().GetLevelAssetsTexts(true);
        int maxLevel = textAssets.Length;
        txtMenuLevel.text = maxLevel.ToString();
        btnArrowR.interactable = false;
        btnArrowL.interactable = (maxLevel > 1 ? true : false);
    }

    private void DesignLevel()
    {
        panelMenu.SetActive(false);
        panelDev.SetActive(true);
        save.interactable = false;
        textAssets = GlobalSingleton.GetInstance().GetLevelAssetsTexts(false);
        OnClickDevMenu((int)DevelopmentMenuType.TYPE_BASIC);

        matrixHandler.InitializeMatrix(currentLevel, false, (obj) =>
        {
            InitializeLevel();
        });
    }

    private void ChangeMenuButtonColor(GameObject btn, GameObject panel, bool active)
    {
        btn.GetComponent<Image>().color = new Color(0.35f, 0.3f, 0.3f, (active ? 0.5f : 0.3f));
        panel.SetActive(active);
    }

    private void InitializeLevel()
    {
        initialize = true;
        //matrixHandler.gameObject.transform.position = new Vector3(2.5f, 0f, 0f);

        //1. Basic
        impLevel.text = matrixHandler.CurrentLevel.ToString();
        impGoalMoves.text = matrixHandler.GoalPoints.moves.ToString();
        impGoalChanges.text = matrixHandler.GoalPoints.changes.ToString();
        impMaximumMoves.text = matrixHandler.MovesAvailable.ToString();
        impMaximumChanges.text = matrixHandler.ChangesAvailable.ToString();
        impStartPointI.text = matrixHandler.StartPoint.i.ToString();
        impStartPointJ.text = matrixHandler.StartPoint.j.ToString();
        impFinishPointI.text = matrixHandler.FinishPoint.i.ToString();
        impFinishPointJ.text = matrixHandler.FinishPoint.j.ToString();
        //pieces
        string piecesId = "";
        for (int i = 0; i < matrixHandler.PiecesAvailable.Length; i++)
        {
            piecesId += "," + (int)matrixHandler.PiecesAvailable[i];
        }
        piecesId = piecesId.Substring(1);
        impPieces.text = piecesId;

        //2. Extras
        //new piece
        if (matrixHandler.NewPieceAvailable != null)
        {
            impNewAvailablePieceType.text = ((int)matrixHandler.NewPieceAvailable.Type).ToString();
            impNewAvailablePiecePointI.text = matrixHandler.NewPieceAvailable.point.i.ToString();
            impNewAvailablePiecePointJ.text = matrixHandler.NewPieceAvailable.point.j.ToString();
        }
        else
        {
            impNewAvailablePieceType.text = "";
            impNewAvailablePiecePointI.text = "";
            impNewAvailablePiecePointJ.text = "";
        }

        //block
        if (blockDevList != null && blockDevList.Count > 0)
        {
            int count = blockDevList.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                Destroy(blockDevList[i].gameObject);
                blockDevList.Remove(blockDevList[i]);
            }
        }
        impBlockNr.text = (matrixHandler.BlockArray==null?"0" :matrixHandler.BlockArray.Length.ToString());
        blockDevList = new List<BlockDev>();
        if (matrixHandler.BlockArray != null && matrixHandler.BlockArray.Length > 0)
        {
            for (int i = 0; i < matrixHandler.BlockArray.Length; i++)
            {
               // BlockDev blockDev = Instantiate(prefabBlock, new Vector3(0,0,0),  Quaternion.identity, contentBlocks.transform).GetComponent<BlockDev>();
                BlockDev blockDev = Instantiate(prefabBlock, contentBlocks.transform).GetComponent<BlockDev>();
                blockDev.SetData(i+1, -1, matrixHandler.MatrixOfCubes.GetLength(0), matrixHandler.BlockArray[i]);
                blockDevList.Add(blockDev);
            }
        }

        //enabler
        if (enablerDevList != null && enablerDevList.Count > 0)
        {
            int count = enablerDevList.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                Destroy(enablerDevList[i].gameObject);
                enablerDevList.Remove(enablerDevList[i]);
            }
        }
        impEnablerNr.text = (matrixHandler.EnablerArray == null ? "0" : matrixHandler.EnablerArray.Length.ToString());
        enablerDevList = new List<EnablerDev>();
        if (matrixHandler.EnablerArray != null && matrixHandler.EnablerArray.Length > 0)
        {
            for (int i = 0; i < matrixHandler.EnablerArray.Length; i++)
            {
                EnablerDev enablerkDev = Instantiate(prefabEnabler, contentEnablers.transform).GetComponent<EnablerDev>();
                enablerkDev.SetData(i + 1, -1, matrixHandler.MatrixOfCubes.GetLength(0), matrixHandler.EnablerArray[i]);
                enablerDevList.Add(enablerkDev);
            }
        }
        
        //breakable
        if (breakableDevList != null && breakableDevList.Count > 0)
        {
            int count = breakableDevList.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                Destroy(breakableDevList[i].gameObject);
                breakableDevList.Remove(breakableDevList[i]);
            }
        }
        impBreakableNr.text = (matrixHandler.BreakableArray == null ? "0" : matrixHandler.BreakableArray.Length.ToString());
        breakableDevList = new List<BreakableDev>();
        if (matrixHandler.BreakableArray != null && matrixHandler.BreakableArray.Length > 0)
        {
            for (int i = 0; i < matrixHandler.BreakableArray.Length; i++)
            {
                BreakableDev breakableDev = Instantiate(prefabBreakable, contentBreakable.transform).GetComponent<BreakableDev>();
                breakableDev.SetData(i + 1, -1, matrixHandler.MatrixOfCubes.GetLength(0), matrixHandler.BreakableArray[i]);
                breakableDevList.Add(breakableDev);
            }
        } 
        
        //powerups
        if (powerupDevList != null && powerupDevList.Count > 0)
        {
            int count = powerupDevList.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                Destroy(powerupDevList[i].gameObject);
                powerupDevList.Remove(powerupDevList[i]);
            }
        }
        impPowerupNr.text = (matrixHandler.PowerupArray == null ? "0" : matrixHandler.PowerupArray.Length.ToString());
        powerupDevList = new List<PowerupDev>();
        if (matrixHandler.PowerupArray != null && matrixHandler.PowerupArray.Length > 0)
        {
            for (int i = 0; i < matrixHandler.PowerupArray.Length; i++)
            {
                PowerupDev powerupDev = Instantiate(prefabPowerup, contentPowerup.transform).GetComponent<PowerupDev>();
                powerupDev.SetData(i + 1, -1, matrixHandler.MatrixOfCubes.GetLength(0), matrixHandler.PowerupArray[i]);
                powerupDevList.Add(powerupDev);
            }
        }
        initialize = false;
    }

    private string CheckData()
    {
        //1. BASIC
        //target moves
        if (impGoalMoves.text.Equals(""))
        {
            return "Number of target moves not added";
        }

        //target changes
        if (impGoalChanges.text.Equals(""))
        {
            return "Number of target moves not added";
        }

        //moves
        if (impMaximumMoves.text.Equals(""))
        {
            return "Number of moves not added";
        }
        if (Int32.Parse(impMaximumMoves.text) < Int32.Parse(impGoalMoves.text))
        {
            return "Number of total moves less than target moves";
        }


        //changes
        if (impMaximumChanges.text.Equals(""))
        {
            return "Number of changes not added";
        }
        if (Int32.Parse(impMaximumChanges.text) < Int32.Parse(impGoalChanges.text))
        {
            return "Number of total changes less than target changes";
        }

        //start point
        if (impStartPointI.text.Equals(""))
        {
            return "Start point I not added";
        }
        if (!CheckNrInRange(Int32.Parse(impStartPointI.text)))
        {
            return "Start point I not in matrix range";
        }
        if (impStartPointJ.text.Equals(""))
        {
            return "Start point J not added";
        }
        if (!CheckNrInRange(Int32.Parse(impStartPointJ.text)))
        {
            return "Start point J not in matrix range";
        }

        //finish point
        if (impFinishPointI.text.Equals(""))
        {
            return "Finish point I not added";
        }
        if (!CheckNrInRange(Int32.Parse(impFinishPointI.text)))
        {
            return "Finish point I not in matrix range";
        }
        if (impFinishPointJ.text.Equals(""))
        {
            return "Finish point J not added";
        }
        if (!CheckNrInRange(Int32.Parse(impFinishPointJ.text)))
        {
            return "Finish point J not in matrix range";
        }

        //ids
        if (impPieces.text.Equals(""))
        {
            return "Pieces id not added";
        }
        string[] piecesS = impPieces.text.Split(',');
        for (int i = 0; i < piecesS.Length; i++)
        {
            if (!Int32.TryParse(piecesS[i], out Int32 resultPieces))
            {
                return "Pieces id incorrectly formatted. Should be: id1,id2,id3";
            }
        }


        //2. EXTRAS
        //new piece


        //blocks
        if (blockDevList != null && blockDevList.Count > 0)
        {
            for (int i = 0; i < blockDevList.Count; i++)
            {
                string check = blockDevList[i].CheckData(i);
                if (check != null)
                {
                    return check;
                }
            }
        }  
        
        //enabler
        if (enablerDevList != null && enablerDevList.Count > 0)
        {
            for (int i = 0; i < enablerDevList.Count; i++)
            {
                string check = enablerDevList[i].CheckData(i);
                if (check != null)
                {
                    return check;
                }
            }
        } 
        
        //breakable
        if (breakableDevList != null && breakableDevList.Count > 0)
        {
            for (int i = 0; i < breakableDevList.Count; i++)
            {
                string check = breakableDevList[i].CheckData(i);
                if (check != null)
                {
                    return check;
                }
            }
        }
        
        //powerup
        if (powerupDevList != null && powerupDevList.Count > 0)
        {
            for (int i = 0; i < powerupDevList.Count; i++)
            {
                string check = powerupDevList[i].CheckData(i);
                if (check != null)
                {
                    return check;
                }
            }
        }

        return null;
    }

    private bool CheckNrInRange(int nr)
    {
        return (nr > -1 && nr < matrixHandler.MatrixOfCubes.Length);
    }

    #region From other classes


    #endregion

    #region Events

    public void OnClickChangeStartLevel(int levelModification)
    {
        int level = System.Convert.ToInt32(txtMenuLevel.GetParsedText());
        if ((levelModification > 0 && (level + levelModification) <= textAssets.Length)
            || (levelModification < 0 && (level + levelModification) > 0))
        {
            level += levelModification;
            txtMenuLevel.SetText(level.ToString());

            btnArrowR.interactable = true;
            btnArrowL.interactable = true;
            if (levelModification > 0 && level == textAssets.Length)
            {
                btnArrowR.interactable = false;
            }
            else if (levelModification < 0 && level == 1)
            {
                btnArrowL.interactable = false;
            }
        }
    }

    public void OnClickSync()
    {
        PrepareMenuLevels();
    }

    public void OnClickCreate()
    {
        developmentType = DevelopmentType.TYPE_CREATE;
        currentLevel = textAssets.Length + 1;
        DesignLevel();
    }

    public void OnClickEdit()
    {
        developmentType = DevelopmentType.TYPE_EDIT;
        currentLevel = System.Convert.ToInt32(txtMenuLevel.GetParsedText());
        DesignLevel();
    }

    public void OnClickDevMenu(int devMenuType)
    {
        if ((DevelopmentMenuType)devMenuType == DevelopmentMenuType.TYPE_BASIC)
        {
            ChangeMenuButtonColor(btnBasic, panelBasic, true);
            ChangeMenuButtonColor(btnExtras, panelExtras, false);
            ChangeMenuButtonColor(btnNew, panelNew, false);
        }
        else if ((DevelopmentMenuType)devMenuType == DevelopmentMenuType.TYPE_EXTRAS)
        {
            ChangeMenuButtonColor(btnBasic, panelBasic, false);
            ChangeMenuButtonColor(btnExtras, panelExtras, true);
            ChangeMenuButtonColor(btnNew, panelNew, false);
        }
        else
        {
            ChangeMenuButtonColor(btnBasic, panelBasic, false);
            ChangeMenuButtonColor(btnExtras, panelExtras, false);
            ChangeMenuButtonColor(btnNew, panelNew, true);
        }

    }

    public void OnClickCamera()
    {
        if (cameraMain == cameraFront)
        {
            imgCamera.sprite = spriteCameraFront;
            cameraFront.gameObject.SetActive(false);
            cameraTop.gameObject.SetActive(true);
            cameraMain = cameraTop;
        }
        else
        {
            imgCamera.sprite = spriteCameraTop;
            cameraFront.gameObject.SetActive(true);
            cameraTop.gameObject.SetActive(false);
            cameraMain = cameraFront;
        }
    }

    public void OnChangeStateOfBlocksNr()
    {
        if (!initialize) {

            int count = blockDevList.Count;
            int difference = count - Int32.Parse(impBlockNr.text);
          
            if (difference>0)
            {
                //need to remove
                for (int i = count - 1; i > count - 1 - difference; i--)
                {
                    Destroy(blockDevList[i].gameObject);
                    blockDevList.Remove(blockDevList[i]);
                }
            }
            else
            {
                //need to add
                for (int i = 0; i < Math.Abs(difference); i++)
                {
                    BlockDev blockDev = Instantiate(prefabBlock, contentBlocks.transform).GetComponent<BlockDev>();
                    blockDev.SetData(blockDevList.Count + 1,-1, matrixHandler.MatrixOfCubes.GetLength(0), new Block());
                    blockDevList.Add(blockDev);
                }
                
            }
        }
        
    }

    public void OnChangeStateOfEnablersNr()
    {
        if (!initialize)
        {
            int count = enablerDevList.Count;
            int difference = count - Int32.Parse(impEnablerNr.text);

            if (difference > 0)
            {
                //need to remove
                for (int i = count - 1; i > count - 1 - difference; i--)
                {
                    Destroy(enablerDevList[i].gameObject);
                    enablerDevList.Remove(enablerDevList[i]);
                }
            }
            else
            {
                //need to add
                for (int i = 0; i < Math.Abs(difference); i++)
                {
                    EnablerDev enablerDev = Instantiate(prefabEnabler, contentEnablers.transform).GetComponent<EnablerDev>();
                    enablerDev.SetData(enablerDevList.Count + 1, -1, matrixHandler.MatrixOfCubes.GetLength(0), new Enabler());
                    enablerDevList.Add(enablerDev);
                }

            }
        }

    }
    
    public void OnChangeStateOfBreakableNr()
    {
        if (!initialize)
        {
            int count = breakableDevList.Count;
            int difference = count - Int32.Parse(impBreakableNr.text);

            if (difference > 0)
            {
                //need to remove
                for (int i = count - 1; i > count - 1 - difference; i--)
                {
                    Destroy(breakableDevList[i].gameObject);
                    breakableDevList.Remove(breakableDevList[i]);
                }
            }
            else
            {
                //need to add
                for (int i = 0; i < Math.Abs(difference); i++)
                {
                    BreakableDev breakableDev = Instantiate(prefabBreakable, contentBreakable.transform).GetComponent<BreakableDev>();
                    breakableDev.SetData(breakableDevList.Count + 1, -1, matrixHandler.MatrixOfCubes.GetLength(0), new Breakable());
                    breakableDevList.Add(breakableDev);
                }

            }
        }

    }

    public void OnChangeStateOfPowerupNr()
    {
        if (!initialize)
        {
            int count = powerupDevList.Count;
            int difference = count - Int32.Parse(impPowerupNr.text);

            if (difference > 0)
            {
                //need to remove
                for (int i = count - 1; i > count - 1 - difference; i--)
                {
                    Destroy(powerupDevList[i].gameObject);
                    powerupDevList.Remove(powerupDevList[i]);
                }
            }
            else
            {
                //need to add
                for (int i = 0; i < Math.Abs(difference); i++)
                {
                    PowerupDev powerupDev = Instantiate(prefabPowerup, contentPowerup.transform).GetComponent<PowerupDev>();
                    powerupDev.SetData(powerupDevList.Count + 1, -1, matrixHandler.MatrixOfCubes.GetLength(0), new Powerup());
                    powerupDevList.Add(powerupDev);
                }
            }
        }

    }


    public void OnClickRefresh()
    {
        string check = CheckData();
        if (check != null)
        {
            alertHandler.ShowAlert(check);
            return;
        }

        save.interactable = true;

        //1. Basic
        level = new Level();
        level.level = Int32.Parse(impLevel.text);
        level.maximumPoints = new LevelPoints();
        level.maximumPoints.moves = Int32.Parse(impMaximumMoves.text);
        level.maximumPoints.changes = Int32.Parse(impMaximumChanges.text);
        level.maximumPoints.totalPoints = 0;
        level.goalPoints = new LevelPoints();
        level.goalPoints.moves = Int32.Parse(impGoalMoves.text);
        level.goalPoints.changes = Int32.Parse(impGoalChanges.text);
        level.goalPoints.totalPoints = (level.maximumPoints.moves - level.goalPoints.moves) * Constants.GOAL_POINTS_MOVES + (level.maximumPoints.changes - level.goalPoints.changes) * Constants.GOAL_POINTS_CHANGES;
        level.startPoint = new Point(Int32.Parse(impStartPointI.text), Int32.Parse(impStartPointJ.text));
        level.finishPoint = new Point(Int32.Parse(impFinishPointI.text), Int32.Parse(impFinishPointJ.text));

        string[] piecesS = impPieces.text.Split(',');
        int[] piecesI = new int[piecesS.Length];
        for (int i = 0; i < piecesS.Length; i++)
        {
            piecesI[i] = Int32.Parse(piecesS[i]);
        }
        level.piecesAvailable = piecesI;

        List<Point> disabledPoints = new List<Point>();
        for (int i = 0; i < matrixHandler.MatrixOfCubes.GetLength(0); i++)
        {
            for (int j = 0; j < matrixHandler.MatrixOfCubes.GetLength(1); j++)
            {
                //if (!matrixHandler.MatrixOfCubes[i, j].gameObject.activeInHierarchy)
                if (!matrixHandler.MatrixOfCubes[i, j].enabledStatus)
                {
                    disabledPoints.Add(new Point(i, j));
                }
            }
        }
        level.disabledPoints = disabledPoints.ToArray();


        //2. Extras
        //piece available
        PieceAvailable pieceAvailable = null;
        if (!impNewAvailablePieceType.text.Equals("") && !impNewAvailablePiecePointI.text.Equals("") && !impNewAvailablePiecePointJ.text.Equals(""))
        {
            pieceAvailable = new PieceAvailable(
               Int32.Parse(impNewAvailablePieceType.text),
               new Point(Int32.Parse(impNewAvailablePiecePointI.text),
               Int32.Parse(impNewAvailablePiecePointJ.text)));
        }
        else
        {
            pieceAvailable = new PieceAvailable();
        }
        level.newPieceAvailable = pieceAvailable;

        //block 
        List<Block> blockCubes = new List<Block>();
        if (blockDevList != null && blockDevList.Count > 0)
        {
            for (int i = 0; i < blockDevList.Count; i++)
            {
                blockCubes.Add(blockDevList[i].GetObject());
            }
        }
        level.block = blockCubes.ToArray();

        //enabler 
        List<Enabler> enablerCubes = new List<Enabler>();
        if (enablerDevList != null && enablerDevList.Count > 0)
        {
            for (int i = 0; i < enablerDevList.Count; i++)
            {
                enablerCubes.Add(enablerDevList[i].GetObject());
            }
        }
        level.enabler = enablerCubes.ToArray(); 
        
        //breakable 
        List<Breakable> breakableCubes = new List<Breakable>();
        if (breakableDevList != null && breakableDevList.Count > 0)
        {
            for (int i = 0; i < breakableDevList.Count; i++)
            {
                breakableCubes.Add(breakableDevList[i].GetObject());
            }
        }
        level.breakable = breakableCubes.ToArray();
        
        //powerup 
        List<Powerup> powerups = new List<Powerup>();
        if (powerupDevList != null && powerupDevList.Count > 0)
        {
            for (int i = 0; i < powerupDevList.Count; i++)
            {
                powerups.Add(powerupDevList[i].GetObject());
            }
        }
        level.powerup = powerups.ToArray();


        matrixHandler.MatrixDesignLevel(level);
    }

    public void OnClickSave()
    {
        if (level != null)
        {
            string levelJson = JsonUtility.ToJson(level);
            File.WriteAllText(Application.dataPath + "/Resources/LevelsJson/level_" + (level.level < 10 ? "0" : "") + level.level + ".txt", levelJson);
            GlobalSingleton.GetInstance().RefreshLevelAssetsTexts();
            textAssets = GlobalSingleton.GetInstance().GetLevelAssetsTexts(false);
            OnClickBack();
        }
    }

    public void OnClickBack()
    {
        InitializeUI();
        matrixHandler.DestroyMatrix();
        matrixHandler.gameObject.transform.position = new Vector3(0f, 0f, 0f);
    }

    #endregion
}

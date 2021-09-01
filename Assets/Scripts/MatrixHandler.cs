using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixHandler : MonoBehaviour
{

    public Cube[,] MatrixOfCubes { get { return matrixOfCubes; } }
    public int CurrentLevel { get { return currentLevel; } }
    public LevelPoints MaximumPoints { get { return maximumPoints; } }
    public LevelPoints GoalPoints { get { return goalPoints; } }
    public int MovesAvailable { get { return movesAvailable; } }
    public int ChangesAvailable { get { return changesAvailable; } }
    public Point StartPoint { get { return startPoint; } }
    public Point FinishPoint { get { return finishPoint; } }
    public CustomPiece.PiecesTypeEnum[] PiecesAvailable { get { return piecesAvailable; } }
    public Point[] DisabledPoints { get { return disabledPoints; } }
    public PieceAvailable NewPieceAvailable { get { return newPieceAvailable; } }
    public Block[] BlockArray { get { return blockArray; } }
    public Enabler[] EnablerArray { get { return enablerArray; } }

    private Cube[,] matrixOfCubes; // i = - ; j = |
    private int matrixIndexLength;
    private int currentLevel;
    private LevelPoints maximumPoints;
    private LevelPoints goalPoints;
    private int movesAvailable;
    private int changesAvailable;
    private Point startPoint;
    private Point finishPoint;
    private CustomPiece.PiecesTypeEnum[] piecesAvailable;
    private Point[] disabledPoints;
    private PieceAvailable newPieceAvailable;
    private Block[] blockArray;
    private Enabler[] enablerArray;
    private bool fromGame;
    private GameObject prefabBlock;
    private GameObject prefabEnabler;



    private Delegates.ObjectDelegate objDelegate;

    // Start is called before the first frame update
    void Start()
    {
        prefabBlock = (GameObject)Resources.Load("Prefabs/Block", typeof(GameObject));
        prefabEnabler = (GameObject)Resources.Load("Prefabs/Enabler", typeof(GameObject));
    }

    public void InitializeMatrix(int level, bool fromGame, Delegates.ObjectDelegate objDelegateReceived)
    {
        this.fromGame = fromGame;

        CreateMatrix(8);
        objDelegate = (obj) =>
        {
            objDelegateReceived?.Invoke(obj);
        };
        currentLevel = level;
        GoLevel(currentLevel, objDelegate);
        //DesignMatrix(matrixOfCubes, currentLevel);
    }

    private void CreateMatrix(int n)
    {
        matrixIndexLength = n;
        matrixOfCubes = new Cube[n, n];
        GameObject cube = (GameObject)Resources.Load("Prefabs/MatrixCube", typeof(GameObject));

        int k = 0;
        disabledPoints = new Point[n * n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                matrixOfCubes[i, j] = Instantiate(cube, new Vector3(i, 0, j), Quaternion.identity, gameObject.transform).GetComponent<Cube>();
                matrixOfCubes[i, j].InitializeCube(new Point(i, j));
                disabledPoints[k] = new Point(i, j);
                k++;
            }
        }
        startPoint = new Point(0, 0);
        finishPoint = new Point(0, 1);
    }

    public void RestoreDisabledCubes()
    {
        if (disabledPoints == null || matrixOfCubes == null || matrixOfCubes.Length == 0)
        {
            return;
        }
        foreach (Point p in disabledPoints)
        {
            //matrixOfCubes[p.i, p.j].gameObject.SetActive(true);

            RestoreEvenColorCubes(p);
        }
        RestoreEvenColorCubes(startPoint);
        RestoreEvenColorCubes(finishPoint);
    }

    private void RestoreEvenColorCubes(Point p)
    {
        matrixOfCubes[p.i, p.j].RestoreEvenColorCubes();
    }

    public void DestroyMatrix()
    {
        //destroy matrix cubes
        for (int i = matrixIndexLength - 1; i >= 0; i--)
        {
            for (int j = matrixIndexLength - 1; j >= 0; j--)
            {
                Destroy(matrixOfCubes[i, j].gameObject);
            }
        }
        matrixOfCubes = null;
        CleanMatrix();
    }

    private void DesignMatrix(Cube[,] matrixOfCubes, int level)
    {
        // this will run MatrixDesignLevel in zero seconds
        //Invoke("MatrixDesignLevel" + level, 0);
        MatrixDesignLevel(level);
    }

    public bool CheckGoNextLevel(Vector3 piecePosition)
    {
        if (currentLevel > 0 && CheckPosition(piecePosition, new Vector3(matrixOfCubes[finishPoint.i, finishPoint.j].transform.position.x, 1f, matrixOfCubes[finishPoint.i, finishPoint.j].transform.position.z)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckPosition(Vector3 pos1, Vector3 pos2)
    {
        return (GlobalSingleton.GetInstance().IsApproximately(pos1.x, pos2.x) && GlobalSingleton.GetInstance().IsApproximately(pos1.z, pos2.z));
    }

    public void GoLevel(int level, Delegates.ObjectDelegate objDelegateReceived)
    {
        RestoreDisabledCubes();
        RemoveNewPieceAvailable();
        RemoveBlocks();
        RemoveEnabler();
        objDelegate = (obj) => { objDelegateReceived?.Invoke(obj); };
        DesignMatrix(matrixOfCubes, level);
    }

    public bool CheckIfCanStep(bool justInMatrix, Point p)
    {
        bool inMatrix = p.i < matrixIndexLength && p.j < matrixIndexLength
            && p.i > -1 && p.j > -1;
        //bool andActive = inMatrix && (justInMatrix? true : matrixOfCubes[p.i, p.j].activeInHierarchy);
        //bool andActive = inMatrix && (justInMatrix? true : matrixOfCubes[p.i, p.j].GetComponent<Renderer>().material.color.a >0);
        //bool andActive = inMatrix && (justInMatrix ? true : matrixOfCubes[p.i, p.j].GetComponent<Renderer>().material.color.a > 0);
        bool andActive = inMatrix && (justInMatrix ? true : matrixOfCubes[p.i, p.j].GetComponent<Renderer>().enabled && !matrixOfCubes[p.i, p.j].needEnabler);
        return inMatrix && andActive;
    }

    public void RecalculateMoves(int i)
    {
        movesAvailable -= i;
    }

    public void RecalculateChanges(int i)
    {
        changesAvailable -= i;
    }

    public int CalculateStars()
    {
        int pointsMade = movesAvailable * Constants.GOAL_POINTS_MOVES + changesAvailable * Constants.GOAL_POINTS_CHANGES;
        if (pointsMade >= goalPoints.totalPoints || goalPoints.totalPoints == 0) { return 3; }
        if (pointsMade >= goalPoints.totalPoints / 2 || pointsMade > 0) { return 2; }
        return 1;
    }

    public void AddNewTypeOfPiece(CustomPiece.PiecesTypeEnum newType)
    {
        CustomPiece.PiecesTypeEnum[] piecesType = new CustomPiece.PiecesTypeEnum[piecesAvailable.Length + 1];
        piecesType[0] = newType;
        for (int i = 0; i < piecesAvailable.Length; i++)
        {
            piecesType[i + 1] = piecesAvailable[i];
        }
        piecesAvailable = piecesType;
    }

    public bool IsPieceChangeAvailable(int indexOfNewType)
    {
        return piecesAvailable.Length > indexOfNewType;
    }

    public bool IsBlockOnPoint(Point point)
    {
        if (blockArray != null && blockArray.Length > 0)
        {
            foreach (Block b in blockArray)
            {
                if (b.oldPoint.i == point.i && b.oldPoint.j == point.j && b.GameObj.activeInHierarchy)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsCubeEnabled(Point point)
    {
        if (enablerArray != null && enablerArray.Length > 0)
        {
            foreach (Enabler e in enablerArray)
            {
                if (e.cubePoint.i == point.i && e.cubePoint.j == point.j && !matrixOfCubes[e.cubePoint.i, e.cubePoint.j].needEnabler)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void ChangeCubeStatus(Point point, bool status, Color color)
    {
        //matrixOfCubes[point.i, point.j].SetActive(true);
        matrixOfCubes[point.i, point.j].ChangeCubeStatus(status, color);
    }


    private void AddNewPieceAvailable()
    {
        GameObject prefabPiece = (GameObject)Resources.Load("Prefabs/" + CustomPiece.prefabName, typeof(GameObject));
        newPieceAvailable.SetGameObject(Instantiate(prefabPiece, newPieceAvailable.Positon, Quaternion.identity));
        newPieceAvailable.GameObj.GetComponent<PieceHandler>().ChangeNewAvailableCustomPiece(newPieceAvailable.Type);
    }

    private void AddBlock(Block block, int nr)
    {
        block.SetGameObject(
            Instantiate(prefabBlock, 
            matrixOfCubes[block.oldPoint.i, block.oldPoint.j].transform.position 
            + new Vector3(0f, 1f, 0f) + new Vector3((fromGame ? 0f : 0f), 0f, 0f), 
            Quaternion.identity));
        block.ChangeGameObjectName(nr);
        block.ChangeDesign();
        block.GameObj.GetComponent<Interactable>().SetObject(block);
    }

    private void AddEnabler(Enabler enabler)
    {
        enabler.SetGameObject(Instantiate(prefabEnabler, matrixOfCubes[enabler.enablerPoint.i, enabler.enablerPoint.j].transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity));
        enabler.ChangeDesign();
        enabler.GameObj.GetComponent<Interactable>().SetObject(enabler);
        matrixOfCubes[enabler.cubePoint.i, enabler.cubePoint.j].needEnabler = true;
        matrixOfCubes[enabler.cubePoint.i, enabler.cubePoint.j].GetComponent<Renderer>().material.color = enabler.color;
    }

    private void RemoveNewPieceAvailable()
    {
        if (newPieceAvailable != null)
        {
            Destroy(newPieceAvailable.GameObj);
            newPieceAvailable = null;
        }

    }

    private void RemoveBlocks()
    {
        if (blockArray != null)
        {
            foreach (Block block in blockArray)
            {
                Destroy(block.GameObj);
            }
        }
        blockArray = null;
    }

    private void RemoveEnabler()
    {
        if (enablerArray != null)
        {
            foreach (Enabler enabler in enablerArray)
            {
                Destroy(enabler.GameObj);
            }
        }
        enablerArray = null;
    }

    private void CleanMatrix()
    {
        RestoreDisabledCubes();
        RemoveNewPieceAvailable();
        RemoveBlocks();
        RemoveEnabler();
    }

    #region design

    void MatrixDesignLevel(int lvl)
    {

        Level level;
        if (GlobalSingleton.GetInstance().GetLevelAssetsTexts(false).Length > lvl - 1)
        {
            var textFile = GlobalSingleton.GetInstance().GetLevelAssetsTexts(false)[lvl - 1];
            string json = textFile.text;
            level = JsonUtility.FromJson<Level>(json);
        }
        else
        {
            level = new Level();
            level.level = lvl;
            level.goalPoints = new LevelPoints();
            level.maximumPoints = new LevelPoints();
            level.startPoint = new Point(0, 0);
            level.finishPoint = new Point(0, 1);
            level.disabledPoints = new Point[] { };
            level.piecesAvailable = new int[] { 0 };
            level.newPieceAvailable = null;
            level.block = new Block[0];
            level.enabler = new Enabler[0];
        }

        MatrixDesignLevel(level);
    }

    public void MatrixDesignLevel(Level level)
    {
        if (!fromGame)
        {
            CleanMatrix();
        }

        currentLevel = level.level;
        movesAvailable = level.maximumPoints.moves;
        changesAvailable = level.maximumPoints.changes;
        goalPoints = level.goalPoints;
        startPoint = level.startPoint;
        finishPoint = level.finishPoint;
        disabledPoints = level.disabledPoints;
        piecesAvailable = level.GetPiecesAvailable();
        if (level.newPieceAvailable != null && level.newPieceAvailable.enabled) { newPieceAvailable = level.newPieceAvailable.GetPiece(matrixOfCubes[level.newPieceAvailable.point.i, level.newPieceAvailable.point.j].transform.position + new Vector3(0f, 1f, 0f)); }
        else { newPieceAvailable = null; }
        blockArray = level.block;
        enablerArray = level.enabler;

        //make base
        matrixOfCubes[startPoint.i, startPoint.j].SetCubeType(Cube.CubeType.TYPE_START);

        //make finish
        matrixOfCubes[finishPoint.i, finishPoint.j].SetCubeType(Cube.CubeType.TYPE_FINISH);

        //disable cubes
        for (int i = 0; i < disabledPoints.Length; i++)
        {
            //matrixOfCubes[disabledPoints[i].i, disabledPoints[i].j].SetActive(false);
            matrixOfCubes[disabledPoints[i].i, disabledPoints[i].j].enabledStatus = false;
            matrixOfCubes[disabledPoints[i].i, disabledPoints[i].j].SetCubeType(Cube.CubeType.TYPE_UNAVAILABLE);
        }

        //add new piece available
        if (newPieceAvailable != null)
        {
            AddNewPieceAvailable();
        }

        //block
        if (blockArray != null && blockArray.Length > 0)
        {
            for (int i=0; i< blockArray.Length; i++)
            {
                AddBlock(blockArray[i], i);
            }
        }

        //enabler
        if (enablerArray != null && enablerArray.Length > 0)
        {
            foreach (Enabler enabler in enablerArray)
            {
                AddEnabler(enabler);
            }
        }

        objDelegate?.Invoke(true);
    }

    #endregion
}

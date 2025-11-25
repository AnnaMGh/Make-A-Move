using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;

public class GlobalSingleton
{
    private static GlobalSingleton instance;
    private TextAsset[] textAssets;
    private Dictionary<int, int> levelStarDictionary;
    private Dictionary<string, int> tutorialDictionary;
    private StandardShaderUtils.BlendMode mainCubesBlendMode;

    public float CanvasScale { get;  private set; }
    public bool gamePaused;
    public bool gameInTutorialOrAlert;

    public static GlobalSingleton GetInstance()
    {
        if (instance == null)
        {
            instance = new GlobalSingleton();
        }
        return instance;
    }

    public void SetCanvasScale(float scale) {
        CanvasScale = scale;
    }

    public async void SetTimeAsync(int time, Delegates.ObjectDelegate objectDelegate)
    {
        await Task.Delay(time);
        objectDelegate(null);
    }

    public bool IsApproximately( float a, float b)
    {
        return IsApproximately(a, b, 0.02f);
    }

    public bool IsApproximately(float a, float b, float tolerance)
    {
        return (Mathf.Abs(a - b) < tolerance);
    }

    public TextAsset[] GetLevelAssetsTexts(bool forceRefresh) {
        if (textAssets == null || forceRefresh)
        {
            RefreshLevelAssetsTexts();
        }
        return textAssets;
    }

    public void RefreshLevelAssetsTexts()
    {
        textAssets = Resources.LoadAll<TextAsset>("LevelsJson");
    }

    #region level stars
    public Dictionary<int, int> GetLevelStarDictionary() {
        if (levelStarDictionary == null)
        {
            RefreshLevelStarDictionary();
        }
       
        return levelStarDictionary;
    }

    public int GetLevelStarDictionaryValue(int level)
    {
        if (levelStarDictionary == null)
        {
            RefreshLevelStarDictionary();
        }
        if (levelStarDictionary.ContainsKey(level)) {
            return levelStarDictionary[level];
        }
        return 0;
    }

    public void ChangeLevelStar(int level, int star)
    {
        if (levelStarDictionary == null)
        {
            RefreshLevelStarDictionary();
        }
        if (levelStarDictionary.ContainsKey(level))
        {
            levelStarDictionary[level] = star;
        }
        else {
            levelStarDictionary.Add(level, star);
        }

        string cvs = "";
        for (int i = 0; i < levelStarDictionary.Count; i++)
        {
            cvs += "," + levelStarDictionary[i+1];
        }
        if (cvs.Length > 0)
        {
            PlayerPrefs.SetString(Constants.KEY_LEVEL_STARS_CSV, cvs.Substring(1));
        }
    }

    public void RefreshLevelStarDictionary() {
        levelStarDictionary = new Dictionary<int, int>();
        string csvStr = PlayerPrefs.GetString(Constants.KEY_LEVEL_STARS_CSV);
        if (csvStr != null && csvStr.Length > 0)
        {
            string[] csv = csvStr.Split(',');
            for (int i = 0; i < csv.Length; i++)
            {
                levelStarDictionary.Add(i+1, Int32.Parse(csv[i]));
            }
        }
    }
    #endregion


    #region tutorial
    public int GetTutorialDictionaryState(string tutorial)
    {
        if (tutorialDictionary == null)
        {
            RefreshTutorialDictionary();
        }
        if (tutorialDictionary.ContainsKey(tutorial))
        {
            return tutorialDictionary[tutorial];
        }
        return 0;
    }

    public void ChangeTutorialState(string tutorial, int state)
    {
        if (tutorialDictionary == null)
        {
            RefreshTutorialDictionary();
        }
        if (tutorialDictionary.ContainsKey(tutorial))
        {
            tutorialDictionary[tutorial] = state;
        }
        else
        {
            tutorialDictionary.Add(tutorial, state);
        }

        string cvs = "";
        foreach (KeyValuePair<string, int> t in tutorialDictionary)
        {
            cvs += "," + t.Key + "-" + t.Value;
        }

        if (cvs.Length > 0)
        {
            PlayerPrefs.SetString(Constants.KEY_TUTORIAL_STATE_CSV, cvs.Substring(1));
        }
    }

    public void RefreshTutorialDictionary()
    {
        tutorialDictionary = new Dictionary<string, int>();
        string csvStr = PlayerPrefs.GetString(Constants.KEY_TUTORIAL_STATE_CSV);
        if (csvStr != null && csvStr.Length > 0)
        {
            string[] csv = csvStr.Split(',');
            for (int i = 0; i < csv.Length; i++)
            {
                string[] tutorialCSV = csv[i].Split('-');
                tutorialDictionary.Add(tutorialCSV[0], Int32.Parse(tutorialCSV[1]));
            }
        }
    }
    #endregion

    public StandardShaderUtils.BlendMode GetMainCubesBlendMode() {
        if (mainCubesBlendMode == 0)
        {
            mainCubesBlendMode = (SystemInfo.systemMemorySize > Constants.RAM_LOW_ACCEPTED ? StandardShaderUtils.BlendMode.Transparent : StandardShaderUtils.BlendMode.Opaque);
        }
        return mainCubesBlendMode;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class EnemyDev : InteractableDev<Enemy>
{
    private TMP_Text impEnemyLbl;
    private TMP_InputField impEnemyType;
    private TMP_InputField impEnemyPointI;
    private TMP_InputField impEnemyPointJ;


    private int minLimit = -1;
    private int maxLimit = 8;

    private readonly int minType = -1;
    private readonly int maxType = 6;

    private void Initialize()
    {
        impEnemyLbl = transform.GetChild(0).GetComponent<TMP_Text>();
        impEnemyType = transform.GetChild(1).GetComponent<TMP_InputField>();
        impEnemyPointI = transform.GetChild(2).GetComponent<TMP_InputField>();
        impEnemyPointJ = transform.GetChild(3).GetComponent<TMP_InputField>();
    }

    public override void SetData(int nr, int min, int max, Enemy enemy)
    {
        if (impEnemyLbl == null) { Initialize(); }

        minLimit = min;
        maxLimit = max;

        if (enemy != null)
        {
            impEnemyLbl.text = "Enemy " + (nr < 10 ? "0" : "") + nr + ":";
            impEnemyType.text = enemy.type.ToString();
            impEnemyPointI.text = (enemy.point != null ? enemy.point.i.ToString() : "0");
            impEnemyPointJ.text = (enemy.point != null ? enemy.point.j.ToString() : "0");
        }
    }

    public override string CheckData(int nr)
    {
        if (impEnemyLbl == null) { Initialize(); }

        if (impEnemyType.text.Equals(""))
        {
            return "Enemy type " + nr + " not added";
        }
        if (!CheckTypeInRange(Int32.Parse(impEnemyType.text)))
        {
            return "Enemy type " + nr + " not in matrix range [" + (minType + 1) + "," + (maxType - 1) + "]";
        }
        if (impEnemyPointI.text.Equals(""))
        {
            return "Enemy " + nr + " point i not added";
        }
        if (!CheckNrInRange(Int32.Parse(impEnemyPointI.text)))
        {
            return "Enemy " + nr + "point i not in matrix range [" + (minLimit + 1) + "," + (maxLimit - 1) + "]";
        }
        if (impEnemyPointJ.text.Equals(""))
        {
            return "Enemy " + nr + "point j not added";
        }
        if (!CheckNrInRange(Int32.Parse(impEnemyPointJ.text)))
        {
            return "Enemy " + nr + "point j not in matrix range [" + minLimit + "," + maxLimit + "]";
        }
        return null;
    }

    private bool CheckNrInRange(int nr)
    {
        return (nr > minLimit && nr < maxLimit);
    }

    private bool CheckTypeInRange(int nr)
    {
        return (nr > minType && nr < maxType);
    }

    public override Enemy GetObject()
    {
        string check = CheckData(0);
        if (check != null)
        {
            return null;
        }

        Enemy enemy = new Enemy(Int32.Parse(impEnemyType.text),
            new Point(Int32.Parse(impEnemyPointI.text), Int32.Parse(impEnemyPointJ.text)));
        return enemy;
    }
}

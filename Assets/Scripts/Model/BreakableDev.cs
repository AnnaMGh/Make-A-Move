using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BreakableDev : InteractableDev
{
    private TMP_Text impBreakableLbl;
    private TMP_InputField impBreakableResistance;
    private TMP_InputField impBreakablePointI;
    private TMP_InputField impBreakablePointJ;


    private int minLimit = -1;
    private int maxLimit = 8;

    private readonly int minResistance = 1;
    private readonly int maxResistance = 21;

    private void Initialize()
    {
        impBreakableLbl = transform.GetChild(0).GetComponent<TMP_Text>();
        impBreakableResistance = transform.GetChild(1).GetComponent<TMP_InputField>();
        impBreakablePointI = transform.GetChild(2).GetComponent<TMP_InputField>();
        impBreakablePointJ = transform.GetChild(3).GetComponent<TMP_InputField>();
    }

    public void SetData(int nr, int min, int max, Breakable breakable)
    {
        if (impBreakableLbl == null) { Initialize(); }

        minLimit = min;
        maxLimit = max;

        if (breakable != null)
        {
            impBreakableLbl.text = "Breakable " + (nr < 10 ? "0" : "") + nr + ":";
            impBreakableResistance.text = breakable.maxResistance.ToString();
            impBreakablePointI.text = (breakable.point != null ? breakable.point.i.ToString() : "0");
            impBreakablePointJ.text = (breakable.point != null ? breakable.point.j.ToString() : "0");
        }
    }

    public string CheckData(int nr)
    {
        if (impBreakableLbl == null) { Initialize(); }

        if (impBreakableResistance.text.Equals(""))
        {
            return "Breakable resistance " + nr + " not added";
        }
        if (!CheckNrInRange(Int32.Parse(impBreakableResistance.text)))
        {
            return "Breakable resistance " + nr + "point i not in matrix range";
        }
        if (impBreakablePointI.text.Equals(""))
        {
            return "Breakable " + nr + " point i not added";
        }
        if (!CheckNrInRange(Int32.Parse(impBreakablePointI.text)))
        {
            return "Breakable " + nr + "point i not in matrix range";
        }
        if (impBreakablePointJ.text.Equals(""))
        {
            return "Breakable " + nr + "point j not added";
        }
        if (!CheckNrInRange(Int32.Parse(impBreakablePointJ.text)))
        {
            return "Breakable " + nr + "point j not in matrix range";
        }
        return null;
    }

    private bool CheckNrInRange(int nr)
    {
        return (nr > minLimit && nr < maxLimit);
    }

    private bool CheckResistanceInRange(int nr)
    {
        return (nr > minResistance && nr < maxResistance);
    }

    public Breakable GetObject()
    {
        string check = CheckData(0);
        if (check != null)
        {
            return null;
        }

        Breakable breakable = new Breakable();
        breakable.maxResistance = Int32.Parse(impBreakableResistance.text);
        breakable.point = new Point(Int32.Parse(impBreakablePointI.text), Int32.Parse(impBreakablePointJ.text));
        return breakable;
    }
}

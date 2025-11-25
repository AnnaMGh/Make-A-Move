using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class EnablerDev : InteractableDev<Enabler>
{
    private TMP_Text impEnablerLbl;
    private TMP_InputField impEnablerPointI;
    private TMP_InputField impEnablerPointJ;
    private TMP_InputField impCubePointI;
    private TMP_InputField impCubePointJ;
    private TMP_InputField impColorR;
    private TMP_InputField impColorG;
    private TMP_InputField impColorB;
    private TMP_InputField impColorA;

    private int minLimit = -1;
    private int maxLimit = 8;

    private void Initialize()
    {
        impEnablerLbl = transform.GetChild(0).GetComponent<TMP_Text>();
        impEnablerPointI = transform.GetChild(1).GetComponent<TMP_InputField>();
        impEnablerPointJ = transform.GetChild(2).GetComponent<TMP_InputField>();
        impCubePointI = transform.GetChild(3).GetComponent<TMP_InputField>();
        impCubePointJ = transform.GetChild(4).GetComponent<TMP_InputField>();
        impColorR = transform.GetChild(5).GetComponent<TMP_InputField>();
        impColorG = transform.GetChild(6).GetComponent<TMP_InputField>();
        impColorB = transform.GetChild(7).GetComponent<TMP_InputField>();
        impColorA = transform.GetChild(8).GetComponent<TMP_InputField>();
    }

    public override void SetData(int nr, int min, int max, Enabler enabler)
    {
        if (impEnablerLbl == null) { Initialize(); }

        minLimit = min;
        maxLimit = max;

        if (enabler != null)
        {
            impEnablerLbl.text = "Enabler " + (nr < 10 ? "0" : "") + nr + ":";
            impEnablerPointI.text = (enabler.enablerPoint != null ? enabler.enablerPoint.i.ToString() : "0");
            impEnablerPointJ.text = (enabler.enablerPoint != null ? enabler.enablerPoint.j.ToString() : "0");
            impCubePointI.text = (enabler.cubePoint != null ? enabler.cubePoint.i.ToString() : "0");
            impCubePointJ.text = (enabler.cubePoint != null ? enabler.cubePoint.j.ToString() : "0");
            impColorR.text = (enabler.color != null ? enabler.color.r.ToString() : "0");
            impColorG.text = (enabler.color != null ? enabler.color.g.ToString() : "0");
            impColorB.text = (enabler.color != null ? enabler.color.b.ToString() : "0");
            impColorA.text = (enabler.color != null ? enabler.color.a.ToString() : "0");
        }
    }

    public override string CheckData(int nr)
    {
        if (impEnablerLbl == null) { Initialize(); }

        if (impEnablerPointI.text.Equals(""))
        {
            return "Enabler " + nr + " point i not added";
        }
        if (!CheckNrInRange(Int32.Parse(impEnablerPointI.text)))
        {
            return "Enabler " + nr + "point i not in matrix range [" + (minLimit + 1) + "," + (maxLimit - 1) + "]";
        }
        if (impEnablerPointJ.text.Equals(""))
        {
            return "Enabler " + nr + "point j not added";
        }
        if (!CheckNrInRange(Int32.Parse(impEnablerPointJ.text)))
        {
            return "Enabler " + nr + "point j not in matrix range [" + (minLimit + 1) + "," + (maxLimit - 1) + "]";
        }
        if (impCubePointI.text.Equals(""))
        {
            return "Enabler Cube " + nr + " point i not added";
        }
        if (!CheckNrInRange(Int32.Parse(impCubePointI.text)))
        {
            return "Enabler Cube " + nr + "point i not in matrix range [" + (minLimit + 1) + "," + (maxLimit - 1) + "]";
        }
        if (impCubePointJ.text.Equals(""))
        {
            return "Enabler Cube " + nr + "point j not added";
        }
        if (!CheckNrInRange(Int32.Parse(impCubePointJ.text)))
        {
            return "Enabler Cube " + nr + "point j not in matrix range [" + (minLimit + 1) + "," + (maxLimit - 1) + "]";
        }
        if (impColorR.text.Equals(""))
        {
            return "Enabler Color R " + nr + "not added";
        }
        if (impColorG.text.Equals(""))
        {
            return "Enabler Color G " + nr + "not added";
        }
        if (impColorB.text.Equals(""))
        {
            return "Enabler Color B " + nr + "not added";
        }
        if (impColorA.text.Equals(""))
        {
            return "Enabler Color A " + nr + "not added";
        }
        return null;
    }

    private bool CheckNrInRange(int nr)
    {
        return (nr > minLimit && nr < maxLimit);
    }

    public override Enabler GetObject()
    {
        string check = CheckData(0);
        if (check != null)
        {
            return null;
        }

        Enabler enabler = new Enabler
        {
            enablerPoint = new Point(Int32.Parse(impEnablerPointI.text), Int32.Parse(impEnablerPointJ.text)),
            cubePoint = new Point(Int32.Parse(impCubePointI.text), Int32.Parse(impCubePointJ.text)),
            color = new Color((float)Decimal.Parse(impColorR.text), (float)Decimal.Parse(impColorG.text), (float)Decimal.Parse(impColorB.text), (float)Decimal.Parse(impColorA.text))
        };
        return enabler;
    }
}

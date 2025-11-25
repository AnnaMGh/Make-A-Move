using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PowerupDev : InteractableDev<Powerup>
{
    private TMP_Text impPowerupLbl;
    private TMP_InputField impPowerupType;
    private TMP_InputField impPowerupPointI;
    private TMP_InputField impPowerupPointJ;


    private int minLimit = -1;
    private int maxLimit = 8;

    private readonly int minType = -1;
    private readonly int maxType = 3;

    private void Initialize()
    {
        impPowerupLbl = transform.GetChild(0).GetComponent<TMP_Text>();
        impPowerupType = transform.GetChild(1).GetComponent<TMP_InputField>();
        impPowerupPointI = transform.GetChild(2).GetComponent<TMP_InputField>();
        impPowerupPointJ = transform.GetChild(3).GetComponent<TMP_InputField>();
    }

    public override void SetData(int nr, int min, int max, Powerup powerup)
    {
        if (impPowerupLbl == null) { Initialize(); }

        minLimit = min;
        maxLimit = max;

        if (powerup != null)
        {
            impPowerupLbl.text = "Powerup " + (nr < 10 ? "0" : "") + nr + ":";
            impPowerupType.text = powerup.type.ToString();
            impPowerupPointI.text = (powerup.point != null ? powerup.point.i.ToString() : "0");
            impPowerupPointJ.text = (powerup.point != null ? powerup.point.j.ToString() : "0");
        }
    }

    public override string CheckData(int nr)
    {
        if (impPowerupLbl == null) { Initialize(); }

        if (impPowerupType.text.Equals(""))
        {
            return "Powerup type " + nr + " not added";
        }
        if (!CheckTypeInRange(Int32.Parse(impPowerupType.text)))
        {
            return "Powerup type " + nr + "point i not in matrix range [" + (minType + 1) + "," + (maxType - 1) + "]";
        }
        if (impPowerupPointI.text.Equals(""))
        {
            return "Powerup " + nr + " point i not added";
        }
        if (!CheckNrInRange(Int32.Parse(impPowerupPointI.text)))
        {
            return "Powerup " + nr + "point i not in matrix range [" + (minLimit + 1) + "," + (maxLimit - 1) + "]";
        }
        if (impPowerupPointJ.text.Equals(""))
        {
            return "Powerup " + nr + "point j not added";
        }
        if (!CheckNrInRange(Int32.Parse(impPowerupPointJ.text)))
        {
            return "Powerup " + nr + "point j not in matrix range [" + (minLimit + 1) + "," + (maxLimit - 1) + "]";
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

    public override Powerup GetObject()
    {
        string check = CheckData(0);
        if (check != null)
        {
            return null;
        }

        Powerup powerup = new Powerup
        {
            type = Int32.Parse(impPowerupType.text),
            nrOfFreeMovesAvailable = 0
        };
        if (powerup.type == (int)Powerup.PowerupType.DOUBLE_FULL)
        {
            powerup.nrOfFreeMovesAvailable = 1;
        }
        else if (powerup.type == (int)Powerup.PowerupType.DIZZY)
        {
            powerup.nrOfFreeMovesAvailable = 3;
        }
        powerup.point = new Point(Int32.Parse(impPowerupPointI.text), Int32.Parse(impPowerupPointJ.text));
        return powerup;
    }
}

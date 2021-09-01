using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class BlockDev : MonoBehaviour
{
    private TMP_Text impBlockLbl;
    private TMP_InputField impBlockCost;
    private TMP_InputField impBlockOldPointI;
    private TMP_InputField impBlockOldPointJ;

    private int minLimit = -1;
    private int maxLimit = 8;

    private void Initialize()
    {
        impBlockLbl = transform.GetChild(0).GetComponent<TMP_Text>();
        impBlockCost = transform.GetChild(1).GetComponent<TMP_InputField>();
        impBlockOldPointI = transform.GetChild(2).GetComponent<TMP_InputField>();
        impBlockOldPointJ = transform.GetChild(3).GetComponent<TMP_InputField>();
    }

    public void SetData(int nr, int min, int max, Block block)
    {
        if (impBlockLbl == null) { Initialize(); }

        minLimit = min;
        maxLimit = max;

        if (block != null)
        {
            impBlockLbl.text = "Block " + (nr<10?"0":"") + nr+":";
            impBlockCost.text = block.cost.ToString();
            impBlockOldPointI.text = (block.oldPoint != null? block.oldPoint.i.ToString():"0") ;
            impBlockOldPointJ.text = (block.oldPoint != null ? block.oldPoint.j.ToString() : "0");
        }
    }

    public string CheckData(int blockNr)
    {
        if (impBlockCost == null) { Initialize(); }


        if (impBlockCost.text.Equals(""))
        {
            return "Block " + blockNr + " cost not added";
        }
        if (!CheckNrInRange(Int32.Parse(impBlockCost.text)))
        {
            return "Block " + blockNr + " cost not in matrix range";
        }
        if (impBlockOldPointI.text.Equals(""))
        {
            return "Block " + blockNr + " old point i not added";
        }
        if (!CheckNrInRange(Int32.Parse(impBlockOldPointI.text)))
        {
            return "Block " + blockNr + "old i not in matrix range";
        }
        if (impBlockOldPointJ.text.Equals(""))
        {
            return "Block " + blockNr + " old point j not added";
        }
        if (!CheckNrInRange(Int32.Parse(impBlockOldPointJ.text)))
        {
            return "Block " + blockNr + "old j not in matrix range";
        }
        return null;
    }

    private bool CheckNrInRange(int nr) {
        return (nr > minLimit && nr < maxLimit);
    }

    public Block GetBlock() {

        string check = CheckData(0);
        if (check != null)
        {
            return null;
        }

        Block block = new Block();
        block.cost = Int32.Parse(impBlockCost.text);
        block.oldPoint = new Point(Int32.Parse(impBlockOldPointI.text), Int32.Parse(impBlockOldPointJ.text));
        return block;
    }
}

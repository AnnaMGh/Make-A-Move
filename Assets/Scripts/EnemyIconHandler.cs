using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIconHandler : MonoBehaviour
{
 
    private GameObject gameObjGlowing;
    private Image gameObjFilling;

    private float fillingPercentToGo; 
    private float fillingPercentCurrent;
    private float enemiesOnLevel;
    private float enemiesTook;

    // Start is called before the first frame update
    void Awake()
    {
        gameObjGlowing = this.transform.GetChild(0).gameObject;
        gameObjFilling = this.transform.GetChild(2).GetComponent<Image>();
        ResetPercent();
    }

    // Update is called once per frame
    void Update()
    {
        if (fillingPercentCurrent > fillingPercentToGo)
        {
            fillingPercentCurrent -= 0.1f;
            gameObjFilling.fillAmount = fillingPercentCurrent;
            if (enemiesTook > 0 && enemiesTook == enemiesOnLevel)
            {
                gameObjGlowing.SetActive(true);
            }
        }
    }

    private void ResetPercent() {
        enemiesOnLevel = 0;
        enemiesTook = 0;
        fillingPercentToGo = 1;
        fillingPercentCurrent = 1;
        gameObjGlowing.SetActive(false);
        gameObjFilling.fillAmount = fillingPercentCurrent;
    }

    public void SetEnemiesOnLevel(int receivedEnemiesOnLevel) {
        ResetPercent();
        enemiesOnLevel = receivedEnemiesOnLevel;
    }

    public void NewTook() {
        enemiesTook++;
        
        //calculate new percent
        float percentDiff = enemiesTook / enemiesOnLevel;
        fillingPercentToGo = 1 - percentDiff;
    }

}

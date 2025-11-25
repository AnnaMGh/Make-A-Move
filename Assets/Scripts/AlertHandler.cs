using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlertHandler : MonoBehaviour
{

    private Image imgBackgorund;
    private TMP_Text txtAlert;

    public static Color DEFAULT_ALERT_BG_COLOR = Constants.DEFAULT_ALERT_BG_COLOR;
    public static Color DEFAULT_ALERT_TXT_COLOR = Constants.DEFAULT_ALERT_TXT_COLOR;
    public static int DEFAULT_SHOW_DURATION = 120;  //seconds
    public static int DEFAULT_FADE_DURATION = 10; //seconds

    private Color bgColor = DEFAULT_ALERT_BG_COLOR;
    private Color txtColor = DEFAULT_ALERT_TXT_COLOR;
    private int showDuration = DEFAULT_SHOW_DURATION;
    private int fadeDuration = DEFAULT_FADE_DURATION; 

    private int remainingShowDuration;
    private int remainingFadeDuration;

    // Start is called before the first frame update
    void Awake()
    {
        InitializeUI();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (remainingFadeDuration > 0)
        {
            remainingFadeDuration--;

            float alpha = 1f-0.3f-((float)remainingFadeDuration * 0.6f / (float)fadeDuration);

            if (remainingShowDuration == 0)
            {
                alpha = ((float)remainingFadeDuration*0.6f / (float)fadeDuration);
                if (remainingFadeDuration == 0)
                {
                    HideAlert();
                }
            }

            bgColor.a = alpha;
            txtColor.a = alpha;
            imgBackgorund.color = bgColor;
            txtAlert.color = txtColor;
        }
        else if (remainingShowDuration > 0)
        {
            remainingShowDuration--;
            if (remainingShowDuration == 0)
            {
                remainingFadeDuration = fadeDuration;
            }
        }
    }

    private void InitializeUI()
    {
        imgBackgorund = this.transform.GetChild(0).GetComponent<Image>();
        txtAlert = imgBackgorund.gameObject.transform.GetChild(0).GetComponent<TMP_Text>();

        bgColor.a = 0;
        txtColor.a = 0;
        imgBackgorund.color = bgColor;
        txtAlert.color = txtColor;
    }

    public void ShowAlert(string message)
    {
        if (txtAlert == null)  { InitializeUI(); }
        remainingShowDuration = showDuration;
        remainingFadeDuration = fadeDuration;
        txtAlert.SetText(message);
        this.gameObject.SetActive(true);
    }


    public void ShowAlert(string message, Color colorBackground, Color colorText, int showDurationSeconds, int fadeDurationSeconds)
    {
        bgColor = colorBackground;
        txtColor = colorText;
        showDuration = showDurationSeconds;
        fadeDuration = fadeDurationSeconds;
        ShowAlert(message);
    }

    public void HideAlert()
    {
        if (txtAlert == null){  InitializeUI();  }
        bgColor = DEFAULT_ALERT_BG_COLOR;
        txtColor = DEFAULT_ALERT_TXT_COLOR;
        showDuration = DEFAULT_SHOW_DURATION;
        fadeDuration = DEFAULT_FADE_DURATION;
        txtAlert.SetText("");
        this.gameObject.SetActive(false);
    }


}

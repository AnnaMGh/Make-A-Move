using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialHandler : MonoBehaviour
{
    // enum PositionEnum {ABOVE, BELOW, LEFT, RIGHT };
    //   private Vector3[] position = { Vector3.Up};

    private GameObject invertMaskUI;
    private Image invertMaskUIChildImage;
    private GameObject panelMessage;
    private Image imgNext;
    private TMP_Text txtTutorial;

    private Sprite spriteNext;
    private Sprite spriteExit;

    public static int DEFAULT_SHOW_DURATION = 120;  //seconds
    public static int DEFAULT_FADE_DURATION = 10; //seconds

    private Color maskColor;
    private int remainingShowDuration;
    private int remainingFadeDuration;

    private Delegates.ObjectDelegate objectDelegate;

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

            float alpha = 1f - 0.3f - ((float)remainingFadeDuration * 0.6f / (float)DEFAULT_FADE_DURATION);

            if (remainingShowDuration == 0)
            {
                alpha = ((float)remainingFadeDuration * 0.6f / (float)DEFAULT_FADE_DURATION);
                if (remainingFadeDuration == 0)
                {
                    HideTutorial();
                }
            }

            maskColor.a = alpha;
            invertMaskUIChildImage.color = maskColor;
        }
        else if (remainingShowDuration > 0)
        {
            remainingShowDuration--;
        }
    }

    private void InitializeUI()
    {
        invertMaskUI = this.transform.GetChild(0).gameObject;
        invertMaskUIChildImage = invertMaskUI.transform.GetChild(0).gameObject.GetComponent<Image>();
        maskColor = invertMaskUIChildImage.color;
        panelMessage = this.transform.GetChild(1).gameObject;
        txtTutorial = panelMessage.transform.GetChild(0).GetComponent<TMP_Text>();
        imgNext = panelMessage.transform.GetChild(1).GetChild(0).GetComponent<Image>();
      
        spriteNext = Resources.Load<Sprite>("Images/Icons/Next");
        spriteExit = Resources.Load<Sprite>("Images/Icons/Exit");

        maskColor.a = 0;
        invertMaskUIChildImage.color = maskColor;
    }

    public void OnClickNext()
    {
        remainingShowDuration = 0;
        remainingFadeDuration = DEFAULT_FADE_DURATION;
    }

    public void ShowTutorial(int id, Vector3 circlePositon, Vector3 messageDirection, float canvasScale, string message, 
        bool hasNext, Delegates.ObjectDelegate receivedDelegate)
    {
        if (id <= PlayerPrefs.GetInt(Constants.KEY_TUTORIAL_STATE))
        {
            receivedDelegate?.Invoke(null);
            return;
        }

        if (invertMaskUI == null) { InitializeUI(); }

        PlayerPrefs.SetInt(Constants.KEY_TUTORIAL_STATE, id);
        GlobalSingleton.GetInstance().gamePaused = true;
        gameObject.SetActive(true);

        invertMaskUI.transform.position = circlePositon;

        if (message != null)
        {
            txtTutorial.text = message;

            if (messageDirection != null)
            {
                float accuracy = canvasScale;
                float distance = 300;
                if (messageDirection == Vector3.left || messageDirection == Vector3.right) { distance = 400; }
               
                panelMessage.transform.position = circlePositon + accuracy * distance * messageDirection;
            }
        }

        imgNext.sprite = (hasNext ? spriteNext : spriteExit);

        remainingShowDuration = DEFAULT_SHOW_DURATION;
        remainingFadeDuration = DEFAULT_FADE_DURATION;

        objectDelegate = receivedDelegate;


    }

    public void HideTutorial()
    {
        GlobalSingleton.GetInstance().gamePaused = false;
        gameObject.SetActive(false);

        objectDelegate?.Invoke(null);


    }
}

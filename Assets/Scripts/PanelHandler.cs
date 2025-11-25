using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelHandler : MonoBehaviour
{
    private Image imgTop;
    private TMP_Text txtTitle;
    private TMP_Text txtSubTitle;
    private Image imgLineTop;
    private Image imgBack;
    private Image imgHome;
    private Image imgRestart;
    private Image imgSounds;
    private Image imgColor;
    private Image imgReview;
    private Image imgNext;
    private Image imgLineBottom;
    private GameObject panelImgs;
    private Image[] imgsPanel;
    private AudioSource audioSource;
    private AudioClip audioClipStar;

    private static int DURATION = 60;
    private int duration = DURATION;
    private int durationRemain = 0;
    private bool fadeIn = true;
    private bool filling;
    private int starsToFill;
    private int starsFilled;

    public float height;

    private Delegates.ObjectDelegate showPanelDelegate;

    // Start is called before the first frame update
    void Awake()
    {
        Initialize();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (filling && durationRemain>0)
        {
           
            durationRemain--;
            imgsPanel[starsFilled].transform.GetChild(0).GetComponent<Image>().fillAmount = 1f - durationRemain / 10f;
            if (durationRemain == 0)
            {
                starsFilled++;
                if (starsFilled < starsToFill)
                {
                    durationRemain = duration;
                }
            }
            else if (durationRemain == duration/2 && PlayerPrefs.GetInt(Constants.KEY_SOUND) == 1) { audioSource.Play(); }
        }
        else if (durationRemain > 0)
        {
            durationRemain--;
            float alpha = 1f - 0.1f - ((float)durationRemain / (float)duration);

            if (!fadeIn)
            {
                alpha = ((float)durationRemain / (float)duration);
                if (durationRemain == 0)
                {
                    HidePanelPrivate();
                }
            }
            else if (durationRemain == 0)
            {
                showPanelDelegate?.Invoke(true); // invoke delegate when panel alpha  = 1f
            }

            ChangeAlpha(alpha);
        }
    }

    private void Initialize()
    {
        imgTop = this.gameObject.transform.GetChild(0).GetComponent<Image>();
        txtTitle = this.gameObject.transform.GetChild(1).GetComponent<TMP_Text>();
        txtSubTitle = this.gameObject.transform.GetChild(2).GetComponent<TMP_Text>();
        imgLineTop = this.gameObject.transform.GetChild(3).GetComponent<Image>();
        imgBack = this.gameObject.transform.GetChild(4).GetComponent<Image>();
        imgHome = this.gameObject.transform.GetChild(5).GetComponent<Image>();
        imgRestart = this.gameObject.transform.GetChild(6).GetComponent<Image>();
        imgSounds = this.gameObject.transform.GetChild(7).GetComponent<Image>();
        imgColor = this.gameObject.transform.GetChild(8).GetComponent<Image>();
        imgReview = this.gameObject.transform.GetChild(9).GetComponent<Image>();
        imgNext = this.gameObject.transform.GetChild(10).GetComponent<Image>();
        imgLineBottom = this.gameObject.transform.GetChild(11).GetComponent<Image>();
        panelImgs = this.gameObject.transform.GetChild(12).gameObject;
        imgsPanel = new Image[panelImgs.transform.childCount];
        for (int i = 0; i < panelImgs.transform.childCount; i++)
        {
            imgsPanel[i] = panelImgs.transform.GetChild(i).GetComponent<Image>();
        }
        audioSource = this.gameObject.transform.GetChild(13).GetComponent<AudioSource>();
        audioClipStar = Resources.Load<AudioClip>("Sounds/chess_clock");

        if (this.gameObject.name.Equals("PanelGameFinished")) {
            if ((float)Screen.width / (float)Screen.height < 1.76)
            {
                GlobalSingleton.GetInstance().SetTimeAsync(100,(obj)=> {
                    height = txtSubTitle.GetComponent<RectTransform>().rect.height;
                    Vector3 pos = txtSubTitle.GetComponent<RectTransform>().position;
                    pos.y -= (txtSubTitle.GetComponent<RectTransform>().rect.height/2 * GameObject.Find("GameManager").GetComponent<GameManager>().canvas.scaleFactor);
                    txtSubTitle.GetComponent<RectTransform>().position = pos;
                });
             }
        }
    }

    private void ChangeAlpha(float alpha)
    {
        ChangeImageColor(imgTop, alpha);
        ChangeTextColor(txtTitle, alpha);
        ChangeTextColor(txtSubTitle, alpha);
        ChangeImageColor(imgLineTop, alpha);
        ChangeImageColor(imgBack, alpha);
        ChangeImageColor(imgHome, alpha);
        ChangeImageColor(imgRestart, alpha);
        ChangeImageColor(imgSounds, alpha);
        ChangeImageColor(imgColor, alpha);
        ChangeImageColor(imgReview, alpha);
        ChangeImageColor(imgNext, alpha);
        ChangeImageColor(imgLineBottom, alpha);
        ChangeImageColor(this.gameObject.GetComponent<Image>(), alpha);
        for (int i = 0; i < imgsPanel.Length; i++)
        {
            ChangeImageColor(imgsPanel[i], alpha);
            Image filling = imgsPanel[i].transform.GetChild(0).GetComponent<Image>();
            ChangeImageColor(filling, alpha);
            if (fadeIn) { filling.fillAmount = 0; }
        }
    }

    private void ChangeImageColor(Image obj, float alpha)
    {
        Color newColor = obj.color;
        newColor.a = alpha;
        obj.color = newColor;
    }

    private void ChangeTextColor(TMP_Text obj, float alpha)
    {
        Color newColor = obj.color;
        newColor.a = alpha;
        obj.color = newColor;
    }

    private void HidePanelPrivate()
    {
        if (txtTitle == null) { Initialize(); }

        this.gameObject.SetActive(false);
        ChangeAlpha(0);
        durationRemain = 0;
        duration = DURATION;
        filling = false;
        starsToFill = 0;
        starsFilled = 0;

        GlobalSingleton.GetInstance().gamePaused = false;
    }

    public void EnableSound(int sound)
    {
        if (txtTitle == null) { Initialize(); }
        PlayerPrefs.SetInt(Constants.KEY_SOUND, sound);
        imgSounds.sprite = Resources.Load<Sprite>("Images/Icons/Sound_" + (sound));
    }

    public void ChangeColor(int color)
    {
        if (txtTitle == null) { Initialize(); }
        PlayerPrefs.SetInt(Constants.KEY_COLOR, color);
        imgColor.sprite = Resources.Load<Sprite>("Images/Icons/Color_" + (color));
    }

    public void ShowStars(int nrOfStartsToFill)
    {
        if (txtTitle == null) { Initialize(); }
        audioSource.clip = audioClipStar;
        filling = (nrOfStartsToFill>0);
        if (filling)
        {
            starsToFill = nrOfStartsToFill;
            starsFilled = 0;
            duration = 30;
            durationRemain = duration;
        } 
    }

    public void ShowPanel()
    {
        if (txtTitle == null) { Initialize(); }

        this.gameObject.SetActive(true);
        ChangeAlpha(0);

        durationRemain = duration;
        fadeIn = true;

        GlobalSingleton.GetInstance().gamePaused = true;
    }

    public void ChangeTitleSubtitle(string title, string subtitle) {
        if (txtTitle == null) { Initialize(); }
        txtTitle.text = title;
        txtSubTitle.text = subtitle;   
    }

    public void ShowPanel(int level, Delegates.ObjectDelegate receivedShowPanelDelegate)
    {
        if (txtTitle == null) { Initialize(); }

        showPanelDelegate = receivedShowPanelDelegate;
        txtSubTitle.text = "Level " + level;
        this.gameObject.SetActive(true);
        ChangeAlpha(0);

        durationRemain = duration;
        fadeIn = true;

        GlobalSingleton.GetInstance().gamePaused = true;
    }

    public void ShowPanel(int durationReceived)
    {
        duration = durationReceived;
        ShowPanel();
    }

    public void HidePanel()
    {
        //to fade out
        fadeIn = false;
        filling = false;
        durationRemain = duration = 30;
    }
}

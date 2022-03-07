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

    public enum TutorialTitle
    {
        INTRODUCE_LEVEL, INTRODUCE_PIECE, INTRODUCE_TARGET,
        INTRODUCE_MOVEMENT, INTRODUCE_MOVEMENT_NUMBER, INTRODUCE_CAMERA, UPDATE_CAMERA,
        INTRODUCE_BREAKABLE_BOXES, INTRODUCE_ROOK, INTRODUCE_ROOK_02, INTRODUCE_CHANGES,
        INTRODUCE_BLOCK, INTRODUCE_BLOCK_02,
        INTRODUCE_KNIGHT, INTRODUCE_KNIGHT_02, INTRODUCE_KNIGHT_03,
        INTRODUCE_BISHOP, INTRODUCE_BISHOP_02, INTRODUCE_BISHOP_03,
        INTRODUCE_QUEEN, INTRODUCE_KING,
        INTRODUCE_ENABLER, INTRODUCE_ENABLER_BUTTON,
        POWERUP_DOUBLE_FULL_INTRODUCE, POWERUP_DOUBLE_FULL_LOST, POWERUP_DOUBLE_FULL_PAWN,
        POWERUP_DOUBLE_FULL_KNIGHT, POWERUP_DOUBLE_FULL_ROOK_BISHOP_QUEEN, POWERUP_DOUBLE_FULL_KING,
        POWERUP_STRONG_INTRODUCE, POWERUP_STRONG_KNIGHT, POWERUP_DIZZY_INTRODUCE, POWERUP_DIZZY_INTRODUCE_02

    };

    private Dictionary<TutorialTitle, Tutorial> tutorialList;

    private Delegates.ObjectDelegate objectDelegate;

    // Start is called before the first frame update
    void Awake()
    {
        InitializeUI();
        InitializeTutorials();
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

    void InitializeTutorials()
    {
        tutorialList = new Dictionary<TutorialTitle, Tutorial>
        {
            //LEVEL 01
            {
                TutorialTitle.INTRODUCE_LEVEL,
                new Tutorial(TutorialTitle.INTRODUCE_LEVEL.ToString(),
            "Current level",
                Vector3.up,
                true)
            },
            {
                TutorialTitle.INTRODUCE_PIECE,
                new Tutorial(TutorialTitle.INTRODUCE_PIECE.ToString(),
             "Current chess piece",
                     Vector3.right,
                true)
            },
            {
                TutorialTitle.INTRODUCE_TARGET,
                new Tutorial(TutorialTitle.INTRODUCE_TARGET.ToString(),
             "Target",
                     Vector3.right,
                true)
            },
            {
                TutorialTitle.INTRODUCE_MOVEMENT,
                new Tutorial(TutorialTitle.INTRODUCE_MOVEMENT.ToString(),
             "Tap on the marked cubes to move the player",
                     Vector3.left,
                true)
            },
            {
                TutorialTitle.INTRODUCE_MOVEMENT_NUMBER,
                new Tutorial(TutorialTitle.INTRODUCE_MOVEMENT_NUMBER.ToString(),
             "Number of moves left",
                     Vector3.up,
                true)
            },
            {
                TutorialTitle.INTRODUCE_CAMERA,
                new Tutorial(TutorialTitle.INTRODUCE_CAMERA.ToString(),
             "Change camera perspective - Toggle between 2D and 3D",
                     Vector3.left,
                true)
            },
            {
                TutorialTitle.UPDATE_CAMERA,
                new Tutorial(TutorialTitle.UPDATE_CAMERA.ToString(),
             "While camera 3D is on you can rotate the view by SWIPING",
                     Vector3.left,
                false)
            },
            
            //LEVEL 02
            {
                TutorialTitle.INTRODUCE_ROOK,
                new Tutorial(TutorialTitle.INTRODUCE_ROOK.ToString(),
             "Take the Rook, to enable a new chess piece",
                     Vector3.right,
                false)
            },
            {
                TutorialTitle.INTRODUCE_ROOK_02,
                new Tutorial(TutorialTitle.INTRODUCE_ROOK_02.ToString(),
              "ROOK piece type has been enabled",
                     Vector3.right,
                true)
            },
            {
                TutorialTitle.INTRODUCE_CHANGES,
                new Tutorial(TutorialTitle.INTRODUCE_CHANGES.ToString(),
               "Number of piece changes left",
                     Vector3.up,
                false)
            }, 
            
            //LEVEL 04
            {
                TutorialTitle.INTRODUCE_BLOCK,
                new Tutorial(TutorialTitle.INTRODUCE_BLOCK.ToString(),
             "Push the block to take the shortcut. Careful it will cost you extra moves",
                     Vector3.right,
                false)
            },
            
            //LEVEL 06
            {
                TutorialTitle.INTRODUCE_BLOCK_02,
                new Tutorial(TutorialTitle.INTRODUCE_BLOCK_02.ToString(),
             "You CAN'T push more than 1 block simultaneously",
                     Vector3.right,
                false)
            },
            
            //LEVEL 11
            {
                TutorialTitle.INTRODUCE_KNIGHT,
                new Tutorial(TutorialTitle.INTRODUCE_KNIGHT.ToString(),
             "KNIGHT piece has been enabled",
                     Vector3.right,
                true)
            },
            {
                TutorialTitle.INTRODUCE_KNIGHT_02,
                new Tutorial(TutorialTitle.INTRODUCE_KNIGHT_02.ToString(),
             "KNIGHT is the only piece that can destroy blocks which lands on",
                     Vector3.right,
                true)
            },
            {
                TutorialTitle.INTRODUCE_KNIGHT_03,
                new Tutorial(TutorialTitle.INTRODUCE_KNIGHT_03.ToString(),
              "KNIGHT is the only piece that can jump over obstacles (Cubes, Pieces, Holes)",
                     Vector3.right,
                false)
            },
            
            //LEVEL 21
            {
                TutorialTitle.INTRODUCE_BISHOP,
                new Tutorial(TutorialTitle.INTRODUCE_BISHOP.ToString(),
             "BISHOP piece has been enabled",
                     Vector3.right,
                true)
            },
            {
                TutorialTitle.INTRODUCE_BISHOP_02,
                new Tutorial(TutorialTitle.INTRODUCE_BISHOP_02.ToString(),
             "BISHOP can push blocks diagonally",
                     Vector3.right,
                true)
            },
            {
                TutorialTitle.INTRODUCE_BISHOP_03,
                new Tutorial(TutorialTitle.INTRODUCE_BISHOP_03.ToString(),
               "If the blocks are surrounded by other blocks, BISHOP can't push them",
                     Vector3.right,
                false)
            },
            
            //LEVEL 31
            {
                TutorialTitle.INTRODUCE_QUEEN,
                new Tutorial(TutorialTitle.INTRODUCE_QUEEN.ToString(),
               "QUEEN piece has been enabled",
                     Vector3.right,
                false)
            },
            
            //LEVEL 41
            {
                TutorialTitle.INTRODUCE_KING,
                new Tutorial(TutorialTitle.INTRODUCE_KING.ToString(),
                "KING piece has been enabled",
                     Vector3.right,
                false)
            },
            
            //LEVEL 19
            {
                TutorialTitle.INTRODUCE_ENABLER,
                new Tutorial(TutorialTitle.INTRODUCE_ENABLER.ToString(),
              "You can't step on red cubes",
                     Vector3.left,
                true)
            },
            {
                TutorialTitle.INTRODUCE_ENABLER_BUTTON,
                new Tutorial(TutorialTitle.INTRODUCE_ENABLER_BUTTON.ToString(),
              "To enable them you need to push the matched colored button",
                     Vector3.left,
                false)
            },

            
            //LEVEL 34
            {
                TutorialTitle.INTRODUCE_BREAKABLE_BOXES,
                new Tutorial(TutorialTitle.INTRODUCE_BREAKABLE_BOXES.ToString(),
             "CRACKED boxes BREAK when pieces are MOVED or CHANGED over it",
                     Vector3.left,
                false)
            },

            //LEVEL 49
            {
                TutorialTitle.POWERUP_DOUBLE_FULL_INTRODUCE,
                new Tutorial(TutorialTitle.POWERUP_DOUBLE_FULL_INTRODUCE.ToString(),
            "DOUBLE_FULL POWERUP gives different powers for every piece",
                Vector3.right,
                true)
            },
            {
                TutorialTitle.POWERUP_DOUBLE_FULL_LOST,
                new Tutorial(TutorialTitle.POWERUP_DOUBLE_FULL_LOST.ToString(),
            "DOUBLE_FULL POWERUP will be lost after the first move or the first piece change",
                Vector3.right,
                false)
            },
            {
                TutorialTitle.POWERUP_DOUBLE_FULL_PAWN,
                new Tutorial(TutorialTitle.POWERUP_DOUBLE_FULL_PAWN.ToString(),
            "PAWN with DOUBLE_FULL POWERUP can move up to 2 steps",
                Vector3.right,
                false)
            },
            {
                TutorialTitle.POWERUP_DOUBLE_FULL_KNIGHT,
                new Tutorial(TutorialTitle.POWERUP_DOUBLE_FULL_KNIGHT.ToString(),
            "KNIGHT with DOUBLE_FULL POWERUP have one free move",
                Vector3.right,
                false)
            },
            {
                TutorialTitle.POWERUP_DOUBLE_FULL_ROOK_BISHOP_QUEEN,
                new Tutorial(TutorialTitle.POWERUP_DOUBLE_FULL_ROOK_BISHOP_QUEEN.ToString(),
            "ROOK/BISHOP/QUEEN with DOUBLE_FULL POWERUP can move as many steps as possible",
                Vector3.left,
                true)
            },
            {
                TutorialTitle.POWERUP_DOUBLE_FULL_KING,
                new Tutorial(TutorialTitle.POWERUP_DOUBLE_FULL_KING.ToString(),
            "KING with DOUBLE_FULL POWERUP can carry the powerup to the next piece",
                Vector3.left,
                false)
            },

            //LEVEL 52
            {
                TutorialTitle.POWERUP_STRONG_INTRODUCE,
                new Tutorial(TutorialTitle.POWERUP_STRONG_INTRODUCE.ToString(),
            "STRONG POWERUP makes the pieces destroy blocks instead of pushing them",
                Vector3.right,
                true)
            },
            {
                TutorialTitle.POWERUP_STRONG_KNIGHT,
                new Tutorial(TutorialTitle.POWERUP_STRONG_KNIGHT.ToString(),
            "STRONG POWERUP doesn't apply to KNIGHT",
                Vector3.right,
                false)
            },
            
            //LEVEL 57
            {
                TutorialTitle.POWERUP_DIZZY_INTRODUCE,
                new Tutorial(TutorialTitle.POWERUP_DIZZY_INTRODUCE.ToString(),
            "DIZZY POWERUP gives 3 free moves",
                Vector3.right,
                true)
            },
            {
                TutorialTitle.POWERUP_DIZZY_INTRODUCE_02,
                new Tutorial(TutorialTitle.POWERUP_DIZZY_INTRODUCE_02.ToString(),
            "DIZZY POWERUP have 50% chances of moving the piece in another direction",
                Vector3.right,
                false)
            },
        };
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

    public void ShowTutorial(TutorialTitle title, Vector3 circlePositon,
        Delegates.ObjectDelegate receivedDelegate)
    {
        if (invertMaskUI == null) { InitializeUI(); }
        if (tutorialList == null) { InitializeTutorials(); }

        Tutorial tutorial = tutorialList[title];

        if (GlobalSingleton.GetInstance().GetTutorialDictionaryState(tutorial.name) == 1)
        {
            receivedDelegate?.Invoke(null);
            return;
        }

        GlobalSingleton.GetInstance().ChangeTutorialState(tutorial.name, 1);
        GlobalSingleton.GetInstance().gamePaused = true;
        gameObject.SetActive(true);

        invertMaskUI.transform.position = circlePositon;

        if (tutorial.message != null)
        {
            txtTutorial.text = tutorial.message;

            if (tutorial.messageDirection != null)
            {
                float accuracy = GlobalSingleton.GetInstance().CanvasScale;
                float distance = 300;
                if (tutorial.messageDirection == Vector3.left || tutorial.messageDirection == Vector3.right) { distance = 400; }

                panelMessage.transform.position = circlePositon + accuracy * distance * tutorial.messageDirection;
            }
        }

        imgNext.sprite = (tutorial.hasNext ? spriteNext : spriteExit);

        remainingShowDuration = DEFAULT_SHOW_DURATION;
        remainingFadeDuration = DEFAULT_FADE_DURATION;

        objectDelegate = receivedDelegate;

    }

    public void ShowTutorial(int id, Vector3 circlePositon, Vector3 messageDirection, string message,
        bool hasNext, Delegates.ObjectDelegate receivedDelegate)
    {
        if (id <= PlayerPrefs.GetInt(Constants.KEY_TUTORIAL_STATE))
        {
            receivedDelegate?.Invoke(null);
            return;
        }

        if (invertMaskUI == null) { InitializeUI(); }
        if (tutorialList == null) { InitializeTutorials(); }

        PlayerPrefs.SetInt(Constants.KEY_TUTORIAL_STATE, id);
        GlobalSingleton.GetInstance().gamePaused = true;
        gameObject.SetActive(true);

        invertMaskUI.transform.position = circlePositon;

        if (message != null)
        {
            txtTutorial.text = message;

            if (messageDirection != null)
            {
                float accuracy = GlobalSingleton.GetInstance().CanvasScale;
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

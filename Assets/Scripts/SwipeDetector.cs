using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SwipeDetector : MonoBehaviour
{
    public GameManager gameManager;

    private Vector2 fingerDown;
    private Vector2 fingerUp;
    public bool detectSwipeOnlyAfterRelease = false;

    public bool isSwiping;
    private bool startSwiping;

    public float SWIPE_THRESHOLD = 20f;

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GlobalSingleton.GetInstance().gamePaused)
        {
#if UNITY_EDITOR
            MouseSwipe();
#else
                 TapSwipe();
#endif
        }
    }


    private void MouseSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            fingerUp = Input.mousePosition;
            fingerDown = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            if (!detectSwipeOnlyAfterRelease)
            {
                fingerDown = Input.mousePosition;
                CheckSwipe();
                isSwiping = true;
            }
        }

        //Detects swipe after finger is released
        if (Input.GetMouseButtonUp(0))
        {
            fingerDown = Input.mousePosition;
            CheckSwipe();
            isSwiping = false;
        }
    }

    private void TapSwipe()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUp = touch.position;
                fingerDown = touch.position;
            }

            //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
                if (!detectSwipeOnlyAfterRelease)
                {
                    fingerDown = touch.position;
                    CheckSwipe();
                    isSwiping = true;
                }
            }

            //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDown = touch.position;
                CheckSwipe();
                isSwiping = false;
            }
        }
    }

    void CheckSwipe()
    {
        //Check if Vertical swipe
        if (VerticalMove() > SWIPE_THRESHOLD && VerticalMove() > HorizontalValMove())
        {
            //Debug.Log("Vertical");
            if (fingerDown.y - fingerUp.y > 0)//up swipe
            {
                OnSwipeUp();
            }
            else if (fingerDown.y - fingerUp.y < 0)//Down swipe
            {
                OnSwipeDown();
            }
            fingerUp = fingerDown;
        }

        //Check if Horizontal swipe
        else if (HorizontalValMove() > SWIPE_THRESHOLD && HorizontalValMove() > VerticalMove())
        {
            //Debug.Log("Horizontal");
            if (fingerDown.x - fingerUp.x > 0)//Right swipe
            {
                OnSwipeRight();
            }
            else if (fingerDown.x - fingerUp.x < 0)//Left swipe
            {
                OnSwipeLeft();
            }
            fingerUp = fingerDown;
        }

        //No Movement at-all
        else
        {
            //Debug.Log("No Swipe!");
        }
    }

    float VerticalMove()
    {
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    float HorizontalValMove()
    {
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }

    //////////////////////////////////CALLBACK FUNCTIONS/////////////////////////////
    void OnSwipeUp()
    {
        if (gameManager.cameraFront.transform.position.y < 27.5f)
        {
            gameManager.OrbitCamera(Vector3.up);
        }

        Debug.Log("Swipe UP");
    }

    void OnSwipeDown()
    {
        if (gameManager.cameraFront.transform.position.y > 15f)
        {
            gameManager.OrbitCamera(Vector3.down);
        }
        Debug.Log("Swipe Down");

    }

    void OnSwipeLeft()
    {
        gameManager.OrbitCamera(Vector3.left);
        Debug.Log("Swipe Left");
    }

    void OnSwipeRight()
    {
        gameManager.OrbitCamera(Vector3.right);
        Debug.Log("Swipe Right");
    }

}

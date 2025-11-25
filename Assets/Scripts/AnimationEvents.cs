using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{

    public GameManager gameManager;


    #region animEvents

    public void OnPawnAnimationEvent() {
        if (gameManager != null)
        {
            gameManager.OnClickStart();
        }

    }

    #endregion
}

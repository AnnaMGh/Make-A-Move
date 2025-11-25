using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    public GameManager gameManager;
    public CustomPiece.MovementTypeEnum movement; 

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        gameManager.OnClickArrow(1, (int)movement);
    }
}

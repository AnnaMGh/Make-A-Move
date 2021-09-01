using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PiecesHandler : MonoBehaviour
{
    private Dictionary<CustomPiece.PiecesTypeEnum, Image> imagePiecesDictionary;

    private Color colorWhiteEnabled = new Color(0.8f, 0.8f, 0.8f, 1f);
    private Color colorWhiteDisabled = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    private Color colorBlackEnabled = new Color(0.4f, 0.4f, 0.4f, 1f);
    private Color colorBlackDisabled = new Color(0.1f, 0.1f, 0.1f, 0.5f);

    // Start is called before the first frame update
    void Awake()
    {
        InitializePieces();
    }

    private void InitializePieces()
    {
        imagePiecesDictionary = new Dictionary<CustomPiece.PiecesTypeEnum, Image>();
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            imagePiecesDictionary.Add(gameObject.transform.GetChild(i).GetComponent<Piece>().piece, gameObject.transform.GetChild(i).GetComponent<Image>());
        }
    }

    public void InitializePanelPieces(Canvas canvas)
    {
        if (imagePiecesDictionary == null) { InitializePieces(); }

        //calculate pieces sides and y possiton
        float scale = canvas.scaleFactor; // it is not calculated in Start
        float panelHeight = transform.GetComponent<RectTransform>().rect.height;
        float oldPositionX = transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition.x;
        float oldSizeX = transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
        float newSizeY = panelHeight * scale + (panelHeight - panelHeight * scale);
        newSizeY /= transform.childCount;

        for (int i = 0; i < transform.childCount; i++)
        {
            float newPositionY = 0 - i * newSizeY;
            transform.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(oldSizeX, newSizeY);
            transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(oldPositionX, newPositionY);
        }
    }

    public Vector3 GetObjectWorldPotionCentered(CustomPiece.PiecesTypeEnum pieceType, Canvas canvas)
    {
        int childIndex = (int)pieceType;
        float hight = transform.GetChild(childIndex).GetComponent<RectTransform>().rect.height*canvas.scaleFactor;
        Vector3 oldPosition = transform.GetChild(childIndex).transform.position;
        return new Vector3(oldPosition.x, oldPosition.y - hight / 2, oldPosition.y);
    }

    public void ChangePiecessAvailability(bool enable, CustomPiece.PiecesTypeEnum[] pieceTypes)
    {
        if (pieceTypes == null) { pieceTypes = (CustomPiece.PiecesTypeEnum[])Enum.GetValues(typeof(CustomPiece.PiecesTypeEnum)); }

        if (imagePiecesDictionary == null) { InitializePieces(); }

        foreach (CustomPiece.PiecesTypeEnum piece in pieceTypes)
        {

            if (PlayerPrefs.GetInt(Constants.KEY_COLOR) == 0) //white
            {
                imagePiecesDictionary[piece].color = (enable ? colorWhiteEnabled : colorWhiteDisabled);
            }
            else {
                imagePiecesDictionary[piece].color = (enable ? colorBlackEnabled : colorBlackDisabled);
            }
          
        }
    }
}

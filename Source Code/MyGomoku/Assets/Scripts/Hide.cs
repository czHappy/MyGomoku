using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : MonoBehaviour {

    public void Hiding()
    {
        gameObject.SetActive(false);
        ChessBoard.Instacne.gameStart = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public ChessType chessColor = ChessType.Black;
    public  bool isDoibleMode = false;
    Button btn;

    protected virtual void Start()
    {
        btn = GameObject.Find("RetractBtn").GetComponent<Button>();
        //print(PlayerPrefs.GetInt("Double"));
        if (PlayerPrefs.GetInt("Double") == 10)
            isDoibleMode = true;
    }
    //固定更新
    protected virtual  void FixedUpdate()
    {
        //如果当前是我走 并且时间间隔超过0.3S
        if(chessColor ==ChessBoard.Instacne.turn && ChessBoard.Instacne.timer > 0.3f)
            PlayeChess();
        if(!isDoibleMode)
            ChangeBtnColor();
    }

    public  virtual void PlayeChess()
    {
        //如果点下左键并且游戏还未结束那么就继续下棋
        if (Input.GetMouseButtonDown(0)&& !EventSystem.current.IsPointerOverGameObject())
        {

            Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);//取出点击位置
            //print((int)(pos.x + 7.5f)+ " " + (int)(pos.y + 7.5f));
            //四舍五入方式确定点击点
            if(ChessBoard.Instacne.PlayChess(new int[2] { (int)(pos.x + 7.5f) , (int)(pos.y + 7.5f) }))
                ChessBoard.Instacne.timer = 0;//
        }
    }

    //更改先后手
    protected virtual void ChangeBtnColor()
    {
        if (chessColor == ChessType.Watch) //观战的就算了
            return;
        if (ChessBoard.Instacne.turn == chessColor) //
            btn.interactable = true;
        else
            btn.interactable = false;
    }
}

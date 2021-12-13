using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILevelOne : Player {
    //存放所有棋形的对应分数
    protected Dictionary<string, float> toScore = new Dictionary<string, float>();
    protected float[,] score = new float[15, 15];//记录每个棋子的得分

    protected override  void Start()
    {        
        toScore.Add("aa_", 50);
        toScore.Add("_aa", 50);
        toScore.Add("_aa_", 100);

        toScore.Add("_aaa_", 1000);
        toScore.Add("aaa_", 500);
        toScore.Add("_aaa", 500);

        toScore.Add("_aaaa_", 10000);
        toScore.Add("aaaa_", 5000);
        toScore.Add("_aaaa", 5000);

        toScore.Add("aaaaa", float.MaxValue);
        toScore.Add("aaaaa_", float.MaxValue);
        toScore.Add("_aaaaa", float.MaxValue);
        toScore.Add("_aaaaa_", float.MaxValue);


        if (chessColor != ChessType.Watch)
            Debug.Log(chessColor + "AILevelOne");
    }
    //不设边界地将该点转化成一个局势字符串以评估分数  
    //一级AI 只能死板地进行一维进攻和一维防守 不考虑任何技巧
    public virtual void CheckOneLine(int[] pos, int[] offset,int chess)
    {
        string str = "a";//先假设下在这一个位置
        //正方向 
        for (int i = offset[0], j = offset[1]; (pos[0] + i >= 0 && pos[0] + i < 15) &&
            pos[1] + j >= 0 && pos[1] + j < 15; i += offset[0], j += offset[1])
        {
            //如果是自己的子 
            if (ChessBoard.Instacne.grid[pos[0] + i, pos[1] + j] == chess)
            {
                str += "a";

            }
            //如果是空子
            else if (ChessBoard.Instacne.grid[pos[0] + i, pos[1] + j] == 0)
            {
                str += "_";
                break;
            }
            //如果被堵住
            else
            {
                break;
            }
        }
        //负方向同理
        for (int i = -offset[0], j = -offset[1]; (pos[0] + i >= 0 && pos[0] + i < 15) &&
            pos[1] + j >= 0 && pos[1] + j < 15; i -= offset[0], j -= offset[1])
        {
            if (ChessBoard.Instacne.grid[pos[0] + i, pos[1] + j] == chess)
            {
                str = "a" + str;

            }
            else if (ChessBoard.Instacne.grid[pos[0] + i, pos[1] + j] == 0)
            {
                str = "_" +str;
                break;
            }
            else
            {
                break;
            }
        }


        if (toScore.ContainsKey(str))
        {
            score[pos[0], pos[1]] += toScore[str];
        }

    }

    public void SetScore(int[] pos)
    {
        //先把分数置零
        score[pos[0], pos[1]] = 0;
        //对某一点的局势判断既要考虑攻 也要考虑守 
        //把下这一点所能得到的攻守效果对结果的综合贡献作为分数

        //攻 
        CheckOneLine(pos, new int[2] { 1, 0 },1);
        CheckOneLine(pos, new int[2] { 1, 1 },1);
        CheckOneLine(pos, new int[2] { 1, -1 },1);
        CheckOneLine(pos, new int[2] { 0, 1 },1);
        //守
        CheckOneLine(pos, new int[2] { 1, 0 }, 2);
        CheckOneLine(pos, new int[2] { 1, 1 }, 2);
        CheckOneLine(pos, new int[2] { 1, -1 },2);
        CheckOneLine(pos, new int[2] { 0, 1 }, 2);
    }

    public override void PlayeChess()
    {
        if(ChessBoard.Instacne.chessStack.Count == 0)
        {
            if (ChessBoard.Instacne.PlayChess(new int[2] { 7,7 }))
                ChessBoard.Instacne.timer = 0;
            return;
        }

        float maxScore = 0;
        int[] maxPos = new int[2] { 0, 0 };
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                if(ChessBoard.Instacne.grid[i,j] == 0)
                {
                    SetScore(new int[2] { i,j});
                    if(score[i,j]>= maxScore)
                    {
                        maxPos[0] = i;
                        maxPos[1] = j;
                        maxScore = score[i, j];
                    }
                }
            }
        }

        if (ChessBoard.Instacne.PlayChess(maxPos))
            ChessBoard.Instacne.timer = 0;

    }

    protected override void ChangeBtnColor()
    {
    }
}

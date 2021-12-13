using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMaxNode
{
    public int chess;
    public int[] pos;
    public List<MiniMaxNode> child;
    public float value;
}

public class AILevelThree : Player
{

    Dictionary<string, float> toScore = new Dictionary<string, float>();


    protected override void ChangeBtnColor()
    {
    }

    protected override void Start()
    {
        //三级AI  将各种棋形进行扩充 
        toScore.Add("aa___", 100);                      //眠二
        toScore.Add("a_a__", 100);
        toScore.Add("___aa", 100);
        toScore.Add("__a_a", 100);
        toScore.Add("a__a_", 100);
        toScore.Add("_a__a", 100);
        toScore.Add("a___a", 100);


        toScore.Add("__aa__", 500);                     //活二 
        toScore.Add("_a_a_", 500);
        toScore.Add("_a__a_", 500);

        toScore.Add("_aa__", 500);
        toScore.Add("__aa_", 500);


        toScore.Add("a_a_a", 1000);
        toScore.Add("aa__a", 1000);
        toScore.Add("_aa_a", 1000);
        toScore.Add("a_aa_", 1000);
        toScore.Add("_a_aa", 1000);
        toScore.Add("aa_a_", 1000);
        toScore.Add("aaa__", 1000);                     //眠三

        toScore.Add("_aa_a_", 9000);                    //跳活三
        toScore.Add("_a_aa_", 9000);

        toScore.Add("_aaa_", 10000);                    //活三       


        toScore.Add("a_aaa", 15000);                    //冲四
        toScore.Add("aaa_a", 15000);                    //冲四
        toScore.Add("_aaaa", 15000);                    //冲四
        toScore.Add("aaaa_", 15000);                    //冲四
        toScore.Add("aa_aa", 15000);                    //冲四        


        toScore.Add("_aaaa_", 1000000);                 //活四

        toScore.Add("aaaaa", float.MaxValue);           //连五


        if (chessColor != ChessType.Watch)
            Debug.Log(chessColor + "AILevelThree");
    }

    public float CheckOneLine(int[,] grid, int[] pos, int[] offset, int chess)
    {
        float score = 0;
        bool lfirst = true, lstop = false, rstop = false;
        int AllNum = 1;
        string str = "a";
        int ri = offset[0], rj = offset[1];
        int li = -offset[0], lj = -offset[1];
        while (AllNum < 7 && (!lstop || !rstop))
        {
            if (lfirst)
            {
                //左边
                if ((pos[0] + li >= 0 && pos[0] + li < 15) &&
            pos[1] + lj >= 0 && pos[1] + lj < 15 && !lstop)
                {
                    if (grid[pos[0] + li, pos[1] + lj] == chess)
                    {
                        AllNum++;
                        str = "a" + str;

                    }
                    else if (grid[pos[0] + li, pos[1] + lj] == 0)
                    {
                        AllNum++;
                        str = "_" + str;
                        if (!rstop) lfirst = false;
                    }
                    else
                    {
                        lstop = true;
                        if (!rstop) lfirst = false;
                    }
                    li -= offset[0]; lj -= offset[1];
                }
                else
                {
                    lstop = true;
                    if (!rstop) lfirst = false;
                }
            }
            else
            {
                if ((pos[0] + ri >= 0 && pos[0] + ri < 15) &&
          pos[1] + rj >= 0 && pos[1] + rj < 15 && !lfirst && !rstop)
                {
                    if (grid[pos[0] + ri, pos[1] + rj] == chess)
                    {
                        AllNum++;
                        str += "a";

                    }
                    else if (grid[pos[0] + ri, pos[1] + rj] == 0)
                    {
                        AllNum++;
                        str += "_";
                        if (!lstop) lfirst = true;
                    }
                    else
                    {
                        rstop = true;
                        if (!lstop) lfirst = true;
                    }
                    ri += offset[0]; rj += offset[1];
                }
                else
                {
                    rstop = true;
                    if (!lstop) lfirst = true;
                }
            }
        }

        string cmpStr = "";
        foreach (var keyInfo in toScore)
        {
            if (str.Contains(keyInfo.Key))
            {
                if (cmpStr != "")
                {
                    if (toScore[keyInfo.Key] > toScore[cmpStr])
                    {
                        cmpStr = keyInfo.Key;
                    }
                }
                else
                {
                    cmpStr = keyInfo.Key;
                }
            }
        }

        if (cmpStr != "")
        {
            score += toScore[cmpStr];
        }
        return score;
    }

    public float GetScore(int[,] grid, int[] pos)
    {
        float score = 0;

        score += CheckOneLine(grid, pos, new int[2] { 1, 0 }, 1);
        score += CheckOneLine(grid, pos, new int[2] { 1, 1 }, 1);
        score += CheckOneLine(grid, pos, new int[2] { 1, -1 }, 1);
        score += CheckOneLine(grid, pos, new int[2] { 0, 1 }, 1);

        score += CheckOneLine(grid, pos, new int[2] { 1, 0 }, 2);
        score += CheckOneLine(grid, pos, new int[2] { 1, 1 }, 2);
        score += CheckOneLine(grid, pos, new int[2] { 1, -1 }, 2);
        score += CheckOneLine(grid, pos, new int[2] { 0, 1 }, 2);

        return score;
    }

    public override void PlayeChess()
    {
        //如果当前没有子 不妨下在中间
        if (ChessBoard.Instacne.chessStack.Count == 0)
        {
            if (ChessBoard.Instacne.PlayChess(new int[2] { 7, 7 }))
                ChessBoard.Instacne.timer = 0;
            return;
        }

        MiniMaxNode node = null;
        //对当前棋盘局势计算4个最优下一步局势
        foreach (var item in GetList(ChessBoard.Instacne.grid, (int)chessColor, true))
        {
            //枚举每一个局势进行限定3次深度的搜索，返回该局势的极大极小值，进而选出最好局势
            CreateTree(item, (int[,])ChessBoard.Instacne.grid.Clone(),3,false);
           
            float a = float.MinValue;//记录极小值
            float b = float.MaxValue;//记录极大值
            item.value += AlphaBeta(item,3,false,a,b); //计算该局势的极大极小值
            if(node != null)
            {//挑选最大的下棋点
                if (node.value < item.value)
                    node = item;
            }
            else
            {
                node = item;
            }
        }
        //实例化这个棋子
        ChessBoard.Instacne.PlayChess(node.pos);
    }

    //返回节点 极大极小
    List<MiniMaxNode> GetList(int[,] grid, int chess, bool mySelf)
    {
        List<MiniMaxNode> nodeList = new List<MiniMaxNode>();
        MiniMaxNode node;
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                int[] pos = new int[2] { i, j };
                if (grid[pos[0], pos[1]] != 0) continue;

                node = new MiniMaxNode();
                node.pos = pos;
                node.chess = chess;
                //敌我双方 一正一负
                if (mySelf)
                    node.value = GetScore(grid, pos);
                else
                    node.value = -GetScore(grid, pos);

                if (nodeList.Count < 4) //只扩展4个结点，这四个结点是所有结点中的最优局势  事实上是局部最优 如果算力允许 可扩展至5，6，7，更多
                {
                    nodeList.Add(node);
                }
                else
                {
                    foreach (var item in nodeList)
                    {
                        if (mySelf)//自己要使分数最大 极大点
                        {
                            if (node.value > item.value)
                            {
                                nodeList.Remove(item);
                                nodeList.Add(node);
                                break;
                            }
                        }
                        else//对手要使分数最小  极小点
                        {
                            if (node.value < item.value)
                            {
                                nodeList.Remove(item);
                                nodeList.Add(node);
                                break;
                            }
                        }
                    }
                }
            }
        }

        return nodeList;
    }

    //对某一局势 创建博弈树  限定深度（思考步数）  
    public void CreateTree(MiniMaxNode node, int[,] grid, int deep, bool mySelf)
    {
        //深度计数为0或者值已经为无穷大了 直接返回
        if (deep == 0 || node.value == float.MaxValue)
        {
            return;
        }
        //当前点
        grid[node.pos[0], node.pos[1]] = node.chess;
        //得到当前点的孩子
        node.child = GetList(grid, node.chess, !mySelf);
        //对于每一个孩子，递归建立子树
        foreach (var item in node.child)
        {
            CreateTree(item, (int[,])grid.Clone(), deep - 1, !mySelf);
        }
    }

    //α-β剪枝
    public float AlphaBeta(MiniMaxNode node, int deep, bool mySelf, float alpha, float beta)
    {
        //如果当前结点是叶子结点（深度用完）或者已经是最大值或者最小值了 就可以直接返回

        if (deep == 0 || node.value == float.MaxValue || node.value == float.MinValue)
        {
            return node.value;
        }

        if (mySelf)
        {
            foreach (var child in node.child)
            {
                alpha = Mathf.Max(alpha, AlphaBeta(child, deep - 1, !mySelf, alpha, beta));

                //在Max层中 ， α >= β 发生alpha剪枝 返回α

                if (alpha >= beta)
                {
                    return alpha;
                }

            }
            return alpha;
        }
        else
        {
            foreach (var child in node.child)
            {
                beta = Mathf.Min(beta, AlphaBeta(child, deep - 1, !mySelf, alpha, beta));

                //在Min层中， α >= β 发生beta剪枝  返回beta
                if (alpha >= beta)
                {
                    return beta;
                }

            }
            return beta;
        }
    }



    
}

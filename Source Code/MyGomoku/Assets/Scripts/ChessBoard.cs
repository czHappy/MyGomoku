using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessBoard : MonoBehaviour {

    static ChessBoard _instacne;

    public ChessType turn = ChessType.Black;  //当前谁下棋 黑棋执行先手
    public int[,] grid;   //表示棋盘网格  0代表没有棋子 1 代表黑棋 2代表白棋
    public GameObject[] prefabs; //预置体数组
    public float timer = 0; //计时器 
    public bool gameStart = false;//游戏开始标志
    Transform parent;  //上一步棋
    public Text winner;
    public Stack<Transform> chessStack = new Stack<Transform>();//栈存储记录 做悔棋用

    public static ChessBoard Instacne
    {
        //单利模式 只读
        get
        {
            return _instacne;
        }

    }

    private void Awake()
    {
        //单例模式  当为空的时候赋值 否则就返回
        //因此只会存在一个棋盘对象
        if(Instacne == null)
        {
            _instacne = this;
        }
    }

    private void Start()
    {
        parent = GameObject.Find("Parent").transform;
        grid = new int[15, 15];
    }
    private void FixedUpdate()
    {
        timer += Time.deltaTime;
    }

    //下棋  传入坐标x,y  pos[0] pos[1]
    public bool PlayChess(int[] pos)
    {
        if (!gameStart) return false; //如果游戏没开始 下不了
        pos[0] = Mathf.Clamp(pos[0], 0, 14);//限定x坐标范围 0-14
        pos[1] = Mathf.Clamp(pos[1], 0, 14);//限定y坐标范围 0-14

        if (grid[pos[0], pos[1]] != 0) return false; //如果这一点已经被下过了 就不能再落子
        //如果当前是黑棋走
        if(turn == ChessType.Black)
        {
            //生成物体实例  注意棋盘中间点是0，0 ，而逻辑上是以左下角为0，0  故应该把坐标-7
            GameObject go =  Instantiate(prefabs[0], new Vector3(pos[0] - 7, pos[1] - 7), Quaternion.identity);
            chessStack.Push(go.transform); //当前步骤入栈
            go.transform.SetParent(parent); //父节点设置
            grid[pos[0], pos[1]] = 1; //赋值为1
            //判断胜负
            if (CheckWinner(pos))
            {
                GameEnd();// 如果胜负已分 则游戏结束
            }
            turn = ChessType.White; //否则下一步是白棋走
            
        }
        //如果当前是白棋走  同理
        else if(turn == ChessType.White)
        {
            GameObject go = Instantiate(prefabs[1], new Vector3(pos[0] - 7, pos[1] - 7), Quaternion.identity);
            chessStack.Push(go.transform);
            go.transform.SetParent(parent);
            grid[pos[0], pos[1]] = 2;
            //判断胜负
            if (CheckWinner(pos))
            {
                GameEnd();
            }
            turn = ChessType.Black;
        }

        return true;
    }
    //胜负显示文本设置
    void GameEnd()
    {
        winner.transform.parent .parent.gameObject.SetActive(true);
        switch (turn)
        {
            case ChessType.Watch:
                break;
            case ChessType.Black:
                winner.text = "黑棋胜！";
                break;
            case ChessType.White:
                winner.text = "白棋胜！";
                break;
            default:
                break;
        }       
        gameStart = false;
        Debug.Log(turn + "赢了");
    }
   
    //四个方向只要有一个5连珠成立即可判定胜负
    public bool CheckWinner(int [] pos)
    {
        if (CheckOneLine(pos, new int[2] { 1, 0 })) return true;
        if (CheckOneLine(pos, new int[2] { 0, 1 })) return true;
        if (CheckOneLine(pos, new int[2] { 1, 1 })) return true;
        if (CheckOneLine(pos, new int[2] { 1, -1})) return true;
        return false;
    }

    //检查一条线 通过offset调整方向  横 竖 斜 反斜
    //以当前棋子位置pos为中心 正方向数当前player的棋子个数  负方向也数
    //当在这条线上当前player有连续5子 即胜利
    public bool CheckOneLine(int[] pos,int[] offset)
    {
        int linkNum = 1;//中心子算一个子
        //正方向 数子  注意边界
        for (int i = offset[0],j = offset[1];(pos[0] + i >= 0 && pos[0] + i <15) &&
            pos[1] + j >= 0 && pos[1] + j < 15; i += offset[0],j += offset[1])
        {
            if(grid[pos[0] + i, pos[1] + j] == (int)turn)
            {
                linkNum++;
            }
            else //遇到其他颜色的子即可退出
            {
                break;
            }
        }
        //负方向 数子
        for (int i = -offset[0], j = -offset[1]; (pos[0] + i >= 0 && pos[0] + i < 15) &&
            pos[1] + j >= 0 && pos[1] + j < 15; i -= offset[0], j -= offset[1])
        {
            if (grid[pos[0] + i, pos[1] + j] == (int)turn)
            {
                linkNum++;
            }
            else
            {
                break;
            }
        }

        //五子连珠即代表胜利
        if (linkNum > 4) return true;

        return false;
    }

    //悔棋   如果是双人模式 那悔棋一步 弹出一个子 人机模式要弹出两个子 因为AI不会悔棋的
    public void RetractChess()
    {
        if(chessStack.Count == 1)//注意 要悔棋至少得有1个棋子
        {
            Transform pos = chessStack.Pop();
            grid[(int)(pos.position.x + 7), (int)(pos.position.y + 7)] = 0;
            Destroy(pos.gameObject);
            turn = turn == ChessType.Black ? ChessType.White : ChessType.Black; //只有一个棋子的时候 下一步还是这个颜色下棋
        }
        
        if (chessStack.Count > 1) // 大于1 则一次需要出栈两个棋子
        {
            Transform pos = chessStack.Pop();
            grid[(int)(pos.position.x + 7), (int)(pos.position.y + 7)] = 0;
            Destroy(pos.gameObject);
             pos = chessStack.Pop();
            grid[(int)(pos.position.x + 7), (int)(pos.position.y + 7)] = 0;
            Destroy(pos.gameObject);
        }
    }

}

// 枚举类型  白棋  黑棋 观战
public enum ChessType
{
    Watch ,
    Black,
    White
}

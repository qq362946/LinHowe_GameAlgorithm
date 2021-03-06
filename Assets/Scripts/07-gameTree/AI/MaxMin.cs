﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinHoweGameTree
{
    /// <summary>
    /// 极大极小值搜索算法
    /// </summary>
    public class MaxMin
    {
        private int[,] board;
        private Evaluation evaluation = new Evaluation();
        public Vector2Int Maxmin(int[,] _board,int deep)
        {
           
            
            board = _board;

            int best = int.MinValue;
            List<Vector2Int> openlist = generator(deep);
            List<Vector2Int> bestPoints = new List<Vector2Int>();
            for (int i = 0; i < openlist.Count; i++)
            {
                Vector2Int p = openlist[i];
                board[p.x,p.y] = -1;

                //找最大值
                var v = _max(deep - 1, int.MaxValue, best);

                evaluation.Evaluate(board);
                //如果输了
                if (evaluation.lose) continue;

                //都赢了，还算啥
                if (evaluation.win)
                {
                    return p;
                }

                //如果跟之前的一个好，则把当前位子加入待选位子
                if (v == best)
                {
                    bestPoints.Add(p);
                }

                //找到一个更好的分，就把以前存的位子全部清除
                if (v > best)
                {
                    best = v;
                    bestPoints = new List<Vector2Int>() { p};
                }

                //取消棋子的放置
                board[p.x,p.y] = 0;
            }
            int index = Random.Range(0, bestPoints.Count);

            return bestPoints[index];
        }

        /// <summary>
        /// 在每一步生成所有可以落子的点,优化性能的关键
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        private List<Vector2Int> generator(int deep)
        {
           
            //获取可以下棋的位置
            List<Vector2Int> openList = new List<Vector2Int>();
            for (int i = 0; i < 15; ++i)
            {
                for (int j = 0; j < 15; ++j)
                {
                    if (0 == board[i, j])
                    {
                        Vector2Int lazi = new Vector2Int(i, j);
                        if (HasNeighbor(lazi,1,1))
                            openList.Add(new Vector2Int(i, j));
                        else if(deep>=2&&HasNeighbor(lazi,2,2))
                            openList.Add(new Vector2Int(i, j));
                    }
                }
            }

            return openList;
        }

        /// <summary>
        /// 检测附近指定深度是否有指定数量的棋子
        /// </summary>
        /// <param name="board">棋盘信息</param>
        /// <param name="point">棋子节点</param>
        /// <param name="distance">检测棋子的深度</param>
        /// <param name="count">检测棋子的数量</param>
        /// <returns></returns>
        private bool HasNeighbor(Vector2Int point,int distance,int count)
        {
            const int len = 15;
            for (int i = point.x - distance; i <= point.x + distance; i++)
            {
                if (i < 0 || i >= len) continue;
                for (int j = point.y - distance; j <= point.y + distance; j++)
                {
                    if (j < 0 || j >= len) continue;
                    if (i == point.x && j == point.y) continue;
                    if (board[i,j] != 0)
                    {
                        count--;
                        if (count <= 0) return true;
                    }
                }
            }
            return false;
        }
        
        private int _max(int deep,int alpha,int beta)
        {
            //估值计算
            int maxv = evaluation.Evaluate(board);
            if (deep < 0) return maxv;
            
            

            int best = int.MaxValue;
            var points = generator(deep);

            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                board[p.x,p.y] = 1;
                var minv = _min(deep - 1, best < alpha ? best : alpha, beta);
                board[p.x,p.y] = 0;
                if (minv < best)
                {
                    best = minv;
                }
                if (minv < beta)
                {
                    break;
                }
            }
            return best;
        }

        private int _min(int deep, int alpha, int beta)
        {
            //估值计算
            int minv = evaluation.Evaluate(board);
            if (deep < 0 ) return minv;

            

            int best = int.MinValue;
            var points = generator(deep);

            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                board[p.x, p.y] = 1;
                var maxv = _max(deep - 1, best > alpha ? best : alpha, beta);
                board[p.x, p.y] = 0;
                if (maxv > best)
                {
                    best = maxv;
                }
                if (maxv > beta)
                {
                    break;
                }
            }
            return best;
        }
    }
}

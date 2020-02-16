using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Civilian : MonoBehaviour
{
    [Header("居民状态")]
    public bool masking;
    public bool cough;
    public bool heat;

    [Header("移动状态")]
    public bool moveAhead = true;
    public int pos_NextIndex = 0;

    [Space]
    public Vector2 origin;

    [Header("居民设定")]
    [Range(0, 1.0f)]
    public float probability_Infect = 0.1f;

    public EdgeCollider2D path;
    private List<Vector2> nodes = new List<Vector2>();
    private List<Vector2> simpleNodes = new List<Vector2>();

    private void Start()
    {
        origin = AmendPos(transform.position);

        path.hideFlags = HideFlags.HideInInspector;
        ResolutePath();
        // 修正初始位置
        Move();

    }

    private void Update()
    {
    }

    public void Move() {
        transform.localPosition = GetNode();
    }

    /// <summary>
    /// 解析路径转换成节点
    /// </summary>
    private void ResolutePath() {
        for (int i = 0; i < path.pointCount; i++) {
            simpleNodes.Add(GetPosWithoutIncrease(i));
        }

            // 冒泡检索
            for (int i = 0, j = i + 1; i < simpleNodes.Count - 1; i++,j++)
            {
                nodes.Add(simpleNodes[i]);

                if (Vector2.Distance(simpleNodes[i], simpleNodes[j]) <= 1)
                {
                    continue;
                }

                // x分量顶点补齐
                if (Mathf.Abs(simpleNodes[i].x - simpleNodes[j].x) > 1)
                {
                    float count = Vector2.Distance(simpleNodes[i], simpleNodes[j]) - 1;

                    int dir = simpleNodes[i].x > simpleNodes[j].x ? -1 : 1;
                    Vector2 startPoint = simpleNodes[i];

                    while (count != 0)
                    {
                        Vector2 newPos = startPoint;
                        newPos.x += dir;
                        nodes.Add(newPos);
                        startPoint = newPos;
                        count--;
                    }
                }
                // y分量顶点补齐
                else
                {
                    float count = Vector2.Distance(simpleNodes[i], simpleNodes[j]) - 1;

                    int dir = simpleNodes[i].y > simpleNodes[j].y ? -1 : 1;
                    Vector2 startPoint = simpleNodes[i];

                while (count != 0)
                    {
                        Vector2 newPos = startPoint;
                        newPos.y += dir;
                        nodes.Add(newPos);
                        startPoint = newPos;
                        count--;
                    }
                }
            }
            nodes.Add(simpleNodes[simpleNodes.Count - 1]);

            foreach (var i in nodes)
            {
                Debug.Log(i);
            }
        
    }

    /// <summary>
    /// 修正路线坐标
    /// </summary>
    /// <param name="_pos">需要转换的路径点</param>
    /// <returns></returns>
    private Vector2 AmendPos(Vector2 _pos) {
        Vector2 dis;

        if (_pos.x < 0)
        {
            float inter = Mathf.Abs(_pos.x);
            dis.x = -(Mathf.Floor(inter) + 0.5f);
        }
        dis.x = Mathf.Floor(_pos.x) + 0.5f;

        if (_pos.y < 0)
        {
            float inter = Mathf.Abs(_pos.y);
            dis.y = -(Mathf.Floor(inter) + 0.5f);
        }
        dis.y = Mathf.Floor(_pos.y) + 0.5f;

        return dis;
    }

    /// <summary>
    /// 获取下一坐标点的世界坐标(未解析路径)
    /// </summary>
    /// <returns></returns>
    private Vector2 GetNextPos() {
        Vector2 src = path.points[pos_NextIndex] + origin;
        Vector2 dis = AmendPos(src);

        // 修改前进方向
        if (moveAhead)
        {
            pos_NextIndex++;
            if (pos_NextIndex.Equals(path.pointCount))
            {
                pos_NextIndex-=2;
                ReverseDir();
            }
        }
        else {
            pos_NextIndex--;
            if (pos_NextIndex.Equals(-1))
            {
                pos_NextIndex+=2;
                ReverseDir();
            }
        }

        // 记录目前位置
        GameManager.instance.RecoredPosition(this, dis);

        // 向管理者提交下一次移动申请
        ApplyMovement();

        return dis;
    }

    /// <summary>
    /// 获取下一目标点但不增加下标
    /// </summary>
    /// <returns></returns>
    public Vector2 GetPosWithoutIncrease() {
        Vector2 src = path.points[pos_NextIndex] + origin;
        Vector2 dis = AmendPos(src);

        return dis;
    }
    public Vector2 GetPosWithoutIncrease(int _index)
    {
        Vector2 src = path.points[_index] + origin;
        Vector2 dis = AmendPos(src);

        return dis;
    }

    /// <summary>
    /// 获得解析后的的路径节点
    /// </summary>
    /// <returns></returns>
    public Vector2 GetNodeWithoutIncrease(int _index) {
        return nodes[_index];
    }
    public Vector2 GetNode()
    {
        Vector2 dis = GetNodeWithoutIncrease(pos_NextIndex);

        // 修改前进方向
        if (moveAhead)
        {
            pos_NextIndex++;
            if (pos_NextIndex.Equals(nodes.Count))
            {
                pos_NextIndex -= 2;
                ReverseDir();
            }
        }
        else
        {
            pos_NextIndex--;
            if (pos_NextIndex.Equals(-1))
            {
                pos_NextIndex += 2;
                ReverseDir();
            }
        }

        // 记录目前位置
        GameManager.instance.RecoredPosition(this, dis);

        // 向管理者提交下一次移动申请
        ApplyMovement();

        return dis;
    }

    /// <summary>
    /// 申请移动
    /// </summary>
    public void ApplyMovement() {
        Vector2 pos = GetNodeWithoutIncrease(pos_NextIndex);

        GameManager.instance.ApplyMovement(this, pos);
    }

    /// <summary>
    /// 反转居民移动方向
    /// </summary>
    public void ReverseDir() {
        moveAhead = !moveAhead;
    }

    private void OnDrawGizmos()
    {
        if (simpleNodes.Count == 0) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(simpleNodes[0], 0.3f);

        Gizmos.color = Color.yellow;
        for (int i = 0, j = i + 1; i < simpleNodes.Count - 1; i++,j++) {
            Gizmos.DrawLine(simpleNodes[i], simpleNodes[j]);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(simpleNodes[simpleNodes.Count - 1], 0.3f);
    }
}

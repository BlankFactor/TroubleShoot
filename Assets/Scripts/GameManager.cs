using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("游戏状态")]
    public bool gameStart;
    public bool round_Player = true;
    public bool round_Civilian;
    public bool inCorountine;

    public Dictionary<Civilian, Vector2> positionList = new Dictionary<Civilian, Vector2>();
    public Queue<KeyValuePair<Civilian, Vector2>> tempQeue = new Queue<KeyValuePair<Civilian, Vector2>>();
    public Queue<KeyValuePair<Civilian,Vector2>> movementApplication = new Queue<KeyValuePair<Civilian, Vector2>>();
    public List<Civilian> conflictObject = new List<Civilian>();

    [Header("游戏设定")]
    public float duraction_Movement = 0.2f;
    public int waitCount = 3;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
       
    }

    void Update()
    {
        // 居民移动
        if (round_Civilian && !inCorountine) {
            StartCoroutine(ExecuteMoveCommand());
        }
        if (Input.GetMouseButtonDown(0) && round_Player) {
            round_Civilian = true;
        }
    }

    /// <summary>
    /// 根据队列处理居民移动请求
    /// </summary>
    IEnumerator ExecuteMoveCommand() {
        inCorountine = true;
        List<Civilian> tempConflictList = new List<Civilian>();

        for (int i = 0,d = movementApplication.Count; i < d; i++) {
            tempQeue.Enqueue(movementApplication.Dequeue());
        }

        while (tempQeue.Count != 0) {
            KeyValuePair < Civilian,Vector2 >  kv = tempQeue.Dequeue();

            // 防死锁 限制出队次数
            bool moveable = true;
            int count = waitCount; 

            // 若位置冲突时 取出队列在重新排队
            while (!CheckMoveable(kv.Value)) {
                if (waitCount.Equals(0))
                {
                    moveable = false;

                    // 加入到冲突对象列表
                    tempConflictList.Add(kv.Key);

                    break;
                }

                tempQeue.Enqueue(kv);
                kv = tempQeue.Dequeue();

                waitCount--;
            }

            if (moveable)
            {
                // 移动且申请下一移动位置
                kv.Key.Move();
            }
            else {
                // 仅申请下一移动位置
                kv.Key.ApplyMovement();
            }

            yield return new WaitForSecondsRealtime(duraction_Movement);

        }
        
        /*
        if (tempConflictList.Count >= 2)
        {
            // 对比冲突对象 反转移动方向解决冲突
            if (tempConflictList.Count.Equals(conflictObject.Count))
            {
                Debug.Log("Conflicted");
                foreach (var i in tempConflictList)
                {
                    if (Random.Range(0, 1.0f) < 0.5f)
                    {
                         // 反转移动方向且重新申请移动队列
                        i.ReverseDir();

                    }
                }
            }
            conflictObject.Clear();
            for (int i = 0; i < tempConflictList.Count; i++)
            {
                conflictObject.Add(tempConflictList[i]);
            }
        }*/

        round_Civilian = false;
        round_Player = true;
        inCorountine = false;
    }

    public bool CheckMoveable(Vector2 _pos) {
        foreach (var i in positionList.Values) {
            if (i.Equals(_pos))
                return false;
        }
        return true;
    }

    /// <summary>
    /// 申请移动
    /// </summary>
    /// <param name="_c"></param>
    /// <param name="_pos"></param>
    public void ApplyMovement(Civilian _c, Vector2 _pos) {
        movementApplication.Enqueue(new KeyValuePair<Civilian, Vector2>(_c, _pos));
    }

    /// <summary>
    /// 重新记录居民位置
    /// </summary>
    /// <param name="_c"></param>
    /// <param name="_pos"></param>
    public void RecoredPosition(Civilian _c, Vector2 _pos) {
        if (!positionList.ContainsKey(_c))
        {
            positionList.Add(_c, _pos);
        }
        else {
            positionList[_c] = _pos;
        }
    }
}

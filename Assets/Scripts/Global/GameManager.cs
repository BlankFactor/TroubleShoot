using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("游戏状态")]
    public bool gameStart;
    public bool gameEnd;

    [Space]
    public int count_Civilian;
    public int count_Infecter;

    [Space]
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

        // 检测游戏是否初次启动 仅打包exe时去掉注释
        // CheckApplicationFirstStart();
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Victory();
        }
        else if (Input.GetKeyDown(KeyCode.W)) {
            Failed();
        }
  

        if (!gameStart) return;

        // 居民移动
        if (round_Civilian && !inCorountine) {
            StartCoroutine(ExecuteMoveCommand());
            WorldTimeManager.instance.FinishedRound();

            // 会结束检测感染数
            if (count_Infecter.Equals(count_Civilian))
            {
                Failed();
            }
            else if (count_Infecter.Equals(0))
            {
                Victory();
            }
        }
    }

    public void TurnToRound_Civilian() {
        round_Civilian = true;
        round_Player = false;
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
            if (kv.Key == null) continue;

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

            kv.Key.Move(moveable);

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

    public void AddInfecter() {
        count_Infecter++;
    }
    public void RemoveInfecter() {
        count_Infecter--;
        count_Civilian--;

       if (count_Infecter.Equals(0))
        {
            Victory();
        }
    }
    public void AddCivilian() {
        count_Civilian++;
    }

    /// <summary>
    /// 游戏胜利
    /// </summary>
    private void Victory() {
        EndGame();
        UIManager.instance.DisplayPanel_Endding(true);
        Debug.Log("游戏胜利");
    }

    /// <summary>
    /// 游戏失败
    /// </summary>
    private void Failed() {
        EndGame();
        UIManager.instance.DisplayPanel_Endding(false);
        Debug.Log("游戏结束");

    }

    public void StartGame() {
        gameStart = true;

        UIManager.instance.DisplayPanel_APRecorder();
        UIManager.instance.DisplayPanel_Command();
    }
    public void EndGame() {
        gameStart = false;
        gameEnd = true;

        AudioManager.instance.Stop_BGM();

        round_Player = round_Civilian = false;
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

    /// <summary>
    /// 检测程序是否初次启动
    /// </summary>
    public void CheckApplicationFirstStart() {
        if (PlayerPrefs.GetInt("AppFirstStart") == 0) {
            PlayerPrefs.SetInt("AppFirstStart", 1);
        }
    }

    public void RestartGame() {
        if(gameEnd)
            SceneManager.LoadScene(0);
    }

    public void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("AppFirstStart", 0);
    }
}

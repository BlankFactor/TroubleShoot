using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Civilian : MonoBehaviour
{
    [Header("居民状态")]
    public bool infected;
    [Space]
    public bool heat;
    public bool cough;

    [Space]
    public bool masking;

    [Space]
    public float temperature = 36.5f;

    [Space]
    public int day_Coma = -1;

    [Header("移动状态")]
    public bool moveable = true;
    public bool moveAhead = true;
    private bool loop = false;

    [Space]
    public bool moving = false;
    public float moveSpeed = 1.0f;

    [Space]
    public int pos_NextIndex = 0;

    [Space]
    public Vector2 origin;
    public Vector2 targetPos;

    [Header("居民设定")]
    [Range(0, 100f)]
    public float probability_Infect = 0;
    [Range(0, 100f)]
    public float probability_Immunity = 0f;

    [Space]
    public float increase_Masking = 90f;
    public float increase_Heat = 70f;
    public float increase_Cough = 50f;

    [Space]
    public float range_Infect = 1f;

    [Space]
    public LayerMask civilian_Layer;

    [Space]
    public EdgeCollider2D path;
    private List<Vector2> nodes = new List<Vector2>();
    private List<Vector2> simpleNodes = new List<Vector2>();
    public List<GameObject> contacts = new List<GameObject>();
    public List<GameObject> beContacts = new List<GameObject>();
    public GameObject edge;
    public SpriteRenderer spriteRender;
    private TextMeshPro text_Temper;

    private void Start()
    {
        text_Temper = GetComponentInChildren<TextMeshPro>();
        text_Temper.gameObject.SetActive(false);
        Initialize();
    }

    private void Update()
    {
        Moving();
    }

    void Initialize() {
        origin = AmendPos(transform.position);

        temperature = Random.Range(36f, 37f);

        ResolutePath();
        // 修正初始位置
        transform.localPosition = origin;
        pos_NextIndex++;

        if (moveable)
        {
            ApplyMovement();
        }
        else {
            ApplyMovement(transform.position);
            targetPos = origin;
        }

        // 传染状态初始化
        if (masking)
            WearMask();
        if (cough)
            AddProbability_Infect(increase_Cough);
        if (heat)
            AddProbability_Infect(increase_Heat);

        if (infected)
        {
            spriteRender.color = CivilianSpriteManagaer.instance.GetSpriteInfect();
            WorldTimeManager.instance.AddInfectedCivilian(this);
            temperature = Random.Range(38f, 42f);
            day_Coma = 14;

            GameManager.instance.AddInfecter();
        }
        else {
            spriteRender.color = CivilianSpriteManagaer.instance.GetSpriteNormal();
        }

        GameManager.instance.AddCivilian();

        // 记录目前位置
        GameManager.instance.RecoredPosition(this, transform.position);
    }

    public void Move(bool _moveable) {
        if (moveable)
        {

            if (_moveable)
            {
                targetPos = GetNode();
                moving = true;
            }
            else
                ApplyMovement();
        }
        else {
            ApplyMovement(transform.localPosition);
            GameManager.instance.RecoredPosition(this, transform.localPosition);
        }

        if (infected)
        {
            Infect();
        }
    }

    public void ReflashComaDay() {
        day_Coma--;
        if (day_Coma == 0)
        {
            moveable = false;
        }
    }
    public int GetDay_Coma() { return day_Coma; }

    private void Moving() {
        if (moving) {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, targetPos, moveSpeed * Time.deltaTime);

            if (transform.localPosition.Equals(targetPos)) {
                moving = false;
            }
        }
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

        loop = nodes[0].Equals(nodes[nodes.Count - 1]) ? true : false;

        Destroy(path);
        simpleNodes.Clear();
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
        if (moveable)
        {
            ApplyMovement();
        }

        return dis;
    }

    /// <summary>
    /// 申请移动
    /// </summary>
    public void ApplyMovement() {
        Vector2 pos = GetNodeWithoutIncrease(pos_NextIndex);

        GameManager.instance.ApplyMovement(this, pos);
    }
    public void ApplyMovement(Vector2 _pos) {
        GameManager.instance.ApplyMovement(this, _pos);
    }

    public void AddBeContacts(GameObject _g) {
        if(!beContacts.Contains(_g))
            beContacts.Add(_g);
    }
    public void RemoveContacts(GameObject _g) {
        contacts.Remove(_g);
    }
    public void RemoveBeContacts(GameObject _g) {
        beContacts.Remove(_g);
    }
    public void RemoveThisFromOtherContacts() {
        foreach (var i in beContacts) {
            i.GetComponent<Civilian>().RemoveContacts(gameObject);
        }
    }
    public void RemoveThisFromOtherBeContacts() {
        foreach (var i in contacts)
        {
            i.GetComponent<Civilian>().RemoveBeContacts(gameObject);
        }
    }
    /// <summary>
    /// 被检测出感染 销毁对象
    /// </summary>
    public void SendToHosipital() {
        RemoveThisFromOtherContacts();
        RemoveThisFromOtherBeContacts();

        GameManager.instance.RemoveInfecter();
        Destroy(gameObject);
    }

    /// <summary>
    /// 反转居民移动方向
    /// </summary>
    public void ReverseDir() {
        if (!loop)
            moveAhead = !moveAhead;
        else
            pos_NextIndex = 1;
    }

    public void SetMoveable(bool _v) {
        moveable = _v;

        // 重新发送移动请求
        if (moveable) {
            ApplyMovement();
        }
    }

    /// <summary>
    /// 感染周围设施或健康者
    /// </summary>
    private void Infect() {
        Vector2 origin = AmendPos(targetPos);

        Collider2D[] cols = Physics2D.OverlapBoxAll(origin, new Vector2(range_Infect, range_Infect), 0, civilian_Layer);
        
        if (cols.Length > 0) {
            foreach (var i in cols)
            {
                if (i.gameObject.Equals(gameObject))
                {
                    continue;
                }

                i.SendMessage("BeInfected",probability_Infect);

                // 记录接触接触的单位
                if (i.transform.tag.Equals("Civilian")) {
                    if (!contacts.Contains(i.gameObject)) {
                        contacts.Add(i.gameObject);
                        i.GetComponent<Civilian>().AddBeContacts(gameObject);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 被感染者感染
    /// </summary>
    /// <param name="_proba_Infect">感染概率</param>
    public void BeInfected(float _proba_Infect) {
        if (infected) return;

        float threadhold = _proba_Infect - probability_Immunity;

        // 概率感染
        if (Random.Range(0, 100) < threadhold) {
            infected = true;
            spriteRender.color = CivilianSpriteManagaer.instance.GetSpriteInfect();
            WorldTimeManager.instance.AddInfectedCivilian(this);
            day_Coma = Random.Range(1,15);

            // 重设体温
            temperature = Random.Range(38f, 42f);
            if (text_Temper.gameObject.activeSelf) {
                DisplayTemperature();
            }

            // 登记感染名单
            GameManager.instance.AddInfecter();

        }
    }

    public void WearMask() {
        masking = true;
        AddProbaliblity_Immunity(increase_Masking);
    }

    public void SetProbability_Infect(float _v) {
        probability_Infect = Mathf.Clamp(_v, 0, 100);
    }
    public void SetProbaliblity_Immunity(float _v) {
        probability_Immunity = Mathf.Clamp(_v, 0, 100);
    }
    public void AddProbability_Infect(float _v)
    {
        probability_Infect = Mathf.Clamp(probability_Infect + _v, 0, 100);
    }
    public void AddProbaliblity_Immunity(float _v)
    {
        probability_Immunity = Mathf.Clamp(probability_Immunity + _v, 0, 100);
    }

    // 显示/关闭 体温
    public void DisplayTemperature() {
        text_Temper.text = temperature.ToString("F1");
        text_Temper.transform.gameObject.SetActive(true);
    }
    public void DisableTemperature() {
        text_Temper.transform.gameObject.SetActive(false);
    }

    public bool Get_Infected() { return infected; }

    // 显示出所有接触者
    public void ShowContacts() {
        ContactsMarker.instance.Mark(gameObject, contacts);
    }
    // 设置描边
    public void SetExpand(bool _v) {
        edge.SetActive(_v);

        if (_v) {
            edge.GetComponent<SpriteRenderer>().sprite = spriteRender.sprite;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(range_Infect, range_Infect));

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

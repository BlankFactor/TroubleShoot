using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicPlace : MonoBehaviour
{
    [Header("当前状态")]
    public bool infected;
    [Range(0,100)]
    public float probability_Infect = 0;

    [Header("感染设置")]
    [Range(0,100)]
    public float basic_InfectProbability = 20.0f;

    [Header("触发设置")]
    [Range(1, 10)]
    public float width = 1;
    [Range(1, 10)]
    public float height = 1;
    public Vector2 offset;

    [Space]
    public LayerMask civilianLayer;

    void Start()
    {
        CreateTrigger();
        WorldTimeManager.instance.AddPublicPlace(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Infect() {
        if (!infected) return;

        Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position + new Vector3(offset.x, offset.y), new Vector2(width - 0.05f, height - 0.05f), 0, civilianLayer);
        
        if (cols.Length > 0) {
            foreach (var i in cols) {
                i.SendMessage("BeInfected", probability_Infect);
            }
        }
    }

    /// <summary>
    /// 被感染
    /// </summary>
    public void BeInfected() {
        if (infected) return;
        probability_Infect = basic_InfectProbability;
        infected = true;
    }
    /// <summary>
    /// 设施消毒
    /// </summary>
    public void Disnifect()
    {
        infected = false;
        probability_Infect = 0;
    }

    /// <summary>
    /// 修正路线坐标
    /// </summary>
    /// <param name="_pos">需要转换的路径点</param>
    /// <returns></returns>
    private Vector2 AmendPos(Vector2 _pos)
    {
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

    private void CreateTrigger() {
        transform.position = AmendPos(transform.position);

        BoxCollider2D bc = gameObject.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;

        bc.size = new Vector2(width - 0.05f, height - 0.05f);
        bc.offset = offset;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!infected)
        {
            if (collision.tag.Equals("Civilian"))
            {
                if (collision.GetComponent<Civilian>().Get_Infected())
                {
                    BeInfected();
                }
            }
        }
        else {
            if (collision.tag.Equals("Civilian"))
            {
                if (!collision.GetComponent<Civilian>().Get_Infected())
                {
                    collision.GetComponent<Civilian>().BeInfected(probability_Infect);
                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!infected)
        {
            if (collision.tag.Equals("Civilian"))
            {
                if (collision.GetComponent<Civilian>().Get_Infected())
                {
                    BeInfected();
                }
            }
        }
        else
        {
            if (collision.tag.Equals("Civilian"))
            {
                if (!collision.GetComponent<Civilian>().Get_Infected())
                {
                    collision.GetComponent<Civilian>().BeInfected(probability_Infect);
                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = infected ? new Color(1,0,0,0.5f) : new Color(0,1,0,0.5f);
        Gizmos.DrawCube(transform.position + new Vector3(offset.x, offset.y), new Vector3(width, height));
    }
}

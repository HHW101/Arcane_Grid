using StageEnum;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ArrowBase : MonoBehaviour
{
    [SerializeField]
    protected LineRenderer line;
    [SerializeField]
    protected GameObject head;
    [Header("Line setting")]

    [SerializeField]
    protected int pointNum=20;
    [Range(5,10)]
    [SerializeField]
    protected float offset=5.0f;
    bool isShoot = false;
    Vector2 a;
    Vector2 b;

    protected virtual void Awake()
    {
    
        Init(transform.position, new Vector2(transform.position.x, transform.position.y) + Vector2.up * 8);
    }
    public virtual void Init(Vector2 start, Vector2 end)
    {
        line.enabled = true;
        // line.positionCount = 2;
        // line.SetPosition(0, transform.position);
        // line.SetPosition(1, transform.position+Vector3.right*8);
        a = makeRandomVector(start, start + (end - start) / 2);
        b = makeRandomVector(start + (end - start) / 2, end);
        //DrawArrow(start,end);
        isShoot = false;
    }
    
    //베지에 곡선 사용
    public void DrawArrow(Vector2 start,Vector2 end)
    {
        if (isShoot)
            return;
        //ector2 pos=Vector2.zero;
        line.positionCount = pointNum;
        
        MakeHead(start,end);
        
        for (int i = 0; i < pointNum; i++) {
            float t = i / (float)(pointNum-1);
            Vector2 pos = Bezier(start, a,b,end,t);
            line.SetPosition(i, pos);
        }
    }
    public void MakeHead(Vector2 start,Vector2 end)
    {   
        
        head.transform.position = end;
        float t = 1.0f;
        Vector2 pos = Bezier(start, a, b, end, t);
    
        t =0.9f;
        Vector2 pos2 = Bezier(start, a, b, end, t);

       
        Vector2 dir = pos2-pos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        head.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

    }
    private Vector2 makeRandomVector(Vector2 start, Vector2 end)
    {
        float posX = Random.Range(start.x, end.x);
        float posY = Random.Range(start.y, end.y);
      //  float offset = Random.Range(minHeight, MaxHeight);

        Vector2 dir = (end - start).normalized;
        Vector2 normal = new Vector2(-dir.y, dir.x);

        return new Vector2(posX, posY) + normal * offset;
    }
    public Vector2 Bezier(Vector2 p0, Vector2 p1, Vector2 p2,Vector2 p3,float t)
    {
        Vector2 dir = (p3 - p0);
        float dist = dir.magnitude;
        Vector2 dirNorm = dir.normalized;
        Vector2 perp = new Vector2(-dirNorm.y, dirNorm.x);
        float offsetN = offset;
        if (Mathf.Abs(p0.x-p3.x)<Mathf.Abs(p0.y-p3.y)&&p0.y > p3.y)
            offsetN = -offset;
        else if(p0.x>p3.x)
            offsetN = -offset;

        p1 = p0 + dirNorm * (dist / 3f) + perp * offsetN;
        p2 = p0 + dirNorm * (dist * 2f / 3f) + perp * offsetN;

        Vector2 m0 = Vector2.Lerp(p0, p1, t);
        Vector2 m1 = Vector2.Lerp(p1, p2, t);
        Vector2 m2 = Vector2.Lerp(p2, p3, t);
        Vector2 b0 = Vector2.Lerp(m0, m1, t);
        Vector2 b1 = Vector2.Lerp(m1, m2, t);

        return Vector2.Lerp(b0, b1, t);
    }
    public virtual void Shoot(Vector2 targetPos)
    {
        isShoot = true;
       ParticleAnimationEvent fire = GameManager.Instance.poolManager.GetObject("FireBallEffect").GetComponent<ParticleAnimationEvent>();
        fire.Play();
      // fire.transform.position = transform.position;
        fire.gameObject.SetActive(true);
        StartCoroutine(ShootCoroutine(fire,targetPos));

    }
    public IEnumerator ShootCoroutine(ParticleAnimationEvent fire,Vector2 targetPos)
    {
        int index = 0;
        fire.gameObject.layer = 7;
        Vector3 targetPosition = line.GetPosition(index);
   
        fire.transform.position = line.GetPosition(index);
        while (index < line.positionCount)
        {

            fire.transform.position = Vector3.MoveTowards(fire.transform.position, targetPosition, 30 * StageTime.Instance.deltaTime);


            Vector3 direction = (targetPosition - fire.transform.position);
            if (direction != Vector3.zero)
            {
             
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                fire.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            if (Vector3.Distance(fire.transform.position, targetPosition) < 0.1f)
            {
          
                index++;

                {
                    if (index < line.positionCount)
                        targetPosition = line.GetPosition(index);

                }

               
            }
            yield return null;
        }
        fire.Bomb(targetPos);
        Destroy(gameObject);
    }
    public virtual void End()
    {
        if (isShoot)
            return;
        line.enabled = false;
        gameObject.SetActive(false);
    }
    public virtual void RealEnd()
    {
        if (isShoot)
            return;
        Logger.Log("발사 확인");
        line.enabled = false;
        Destroy(gameObject);
    }
}

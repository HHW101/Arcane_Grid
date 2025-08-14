using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class ParticleManagerParam
{
  
}

public class ParticleManager : MonoBehaviour
{
    private PoolManager poolManager;

    public void Init(PoolManager poolManager, ParticleManagerParam param)
    {
        this.poolManager = poolManager;
    }
    public void PlayAnimation(string key, Vector2 position, Quaternion rotation)
    {
        if (!poolManager)
        {
            //Debug.LogWarning("PoolManager가 연결되어 있지 않습니다!");
            return;
        }

        GameObject particle = poolManager.GetObject(key);
        if (!particle)
        {
           // Debug.LogWarning($"파티클 키({key})에 해당하는 풀 객체가 없습니다.");
            return;
        }
        particle.transform.SetPositionAndRotation(position, rotation);
        particle.SetActive(true);
        particle.GetComponent<ParticleAnimationEvent>().Play();
        //StartCoroutine(ReturnAfterDelay(key, particle, duration));
    }
   
    public void Play(string key, Vector3 position, Quaternion rotation, float scaleOffset, float customLifetime = -1f)
    {
        if (!poolManager)
        {
           // Debug.LogWarning("PoolManager가 연결되어 있지 않습니다!");
            return;
        }

        GameObject particle = poolManager.GetObject(key);
        if (!particle)
        {
            //Debug.LogWarning($"파티클 키({key})에 해당하는 풀 객체가 없습니다.");
            return;
        }

        particle.transform.SetPositionAndRotation(position, rotation);


        particle.SetActive(true);

        float duration = customLifetime;
        var ps = particle.GetComponent<ParticleSystem>();
        if (ps)
        {
            ps.Play();
            if (customLifetime <= 0f)
                duration = ps.main.duration;
        }

        StartCoroutine(ReturnAfterDelay(key, particle, duration));
    }

    private IEnumerator ReturnAfterDelay(string key, GameObject particle, float time)
    {
        yield return new WaitForSeconds(time);
        poolManager.ReturnObject(key, particle);
    }
}
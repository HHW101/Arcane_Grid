using System.Collections.Generic;
using UnityEngine;

using System;

[Serializable]
public class PoolManagerParam
{
    public List<PoolManager.PoolInfo> poolList = new();
}

public class PoolManager : MonoBehaviour
{
    [System.Serializable]
    public class PoolInfo
    {
        public string key;
        public GameObject prefab;
        public int poolSize = 10;
    }

    [SerializeField] private PoolManagerParam poolManagerParam = new(); // 인스펙터에서 Param으로 세팅
    private Dictionary<string, PoolData> poolDict = new();
    private Canvas popupCanvas;
    private StageScenePopupUI popupUI;

    private class PoolData
    {
        public GameObject prefab;
        public Queue<GameObject> pool = new();
        public Transform parent;

        public PoolData(GameObject prefab, int size, Transform parent)
        {
            this.prefab = prefab;
            this.parent = parent;
            for (int i = 0; i < size; i++)
            {
                GameObject obj = GameObject.Instantiate(prefab, parent);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }
        public GameObject Get()
        {
            if (pool.Count > 0)
                return pool.Dequeue();
            GameObject obj = GameObject.Instantiate(prefab, parent);
            obj.SetActive(false);
            return obj;
        }
        public void Return(GameObject obj)
        {
            obj.transform.SetParent(this.parent);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    // Param 기반 Init 방식
    public void Init(PoolManagerParam param)
    {
        poolManagerParam = param;
        InitializePools();
    }

    public void InitializePools()
    {
        poolDict.Clear(); // 초기화시 기존 풀 제거
        foreach (var p in poolManagerParam.poolList)
        {
            if (!poolDict.ContainsKey(p.key))
                poolDict[p.key] = new PoolData(p.prefab, p.poolSize, this.transform);
        }
    }

    public GameObject GetObject(string key)
    {
        if (!poolDict.ContainsKey(key))
        {
            //Debug.LogWarning($"[PoolManager] No pool for key {key}");
            return null;
        }
        return poolDict[key].Get();
    }

    public void ReturnObject(string key, GameObject obj)
    {
  
        if (!poolDict.ContainsKey(key))
        { 

            Destroy(obj);
            return;
        }
        poolDict[key].Return(obj);
    }
    
    public void RegisterPopupCanvas(Canvas canvas, StageScenePopupUI popupUI = null)
    {
        this.popupCanvas = canvas;
        if (popupUI)
            this.popupUI = popupUI;
    }

    public Canvas GetPopupCanvas()
    {
        if (!popupCanvas)
        {
            Logger.Log("Canvas Not Found");
        }
        return popupCanvas;
    }
    public StageScenePopupUI GetPopupUI()
    {
        if (!popupUI)
            Logger.Log("PopupUI Not Found");
        return popupUI;
    }
}

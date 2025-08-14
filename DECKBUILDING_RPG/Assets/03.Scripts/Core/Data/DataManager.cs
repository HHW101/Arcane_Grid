using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour
{
    [SerializeField]
    List<CardTemplate> cards = new List<CardTemplate>();
    Dictionary<int, CardTemplate> cardsDict = new Dictionary<int, CardTemplate>();
    [SerializeField]
    List<CardEffectTemplate> effects = new List<CardEffectTemplate>();
    Dictionary<int, CardEffectTemplate> effectsDict = new Dictionary<int, CardEffectTemplate>();
    [SerializeField]
    string folderPath = "Assets/11.AutoGenerate";

    protected void Awake()
    {
   
        Init();

    }
    
    private List<T> LoadData<T>()
    {
        List<T> list = new List<T>();
        string path = Path.Combine(Application.streamingAssetsPath, $"{typeof(T).Name}.json");
        if (!File.Exists(path))
        {
            Debug.LogWarning($"파일 없음: {path}");
            return list;
        }
        string json = File.ReadAllText(path);
        list = JsonConvert.DeserializeObject<List<T>>(json);
        return list;
    }


    private IEnumerator LoadData<T>(System.Action<List<T>> onComplete)
    {
        string fileName = $"{typeof(T).Name}.json";
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[WebGL] 파일 로드 실패: {request.error}, 경로: {path}");
            onComplete?.Invoke(new List<T>());
        }
        else
        {
            try
            {
                // BOM 제거
                string json = request.downloadHandler.text;
                if (!string.IsNullOrEmpty(json) && json[0] == '\uFEFF')
                    json = json.Substring(1);

                var list = JsonConvert.DeserializeObject<List<T>>(json);
                onComplete?.Invoke(list ?? new List<T>());
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[WebGL] JSON 파싱 실패: {e.Message}");
                onComplete?.Invoke(new List<T>());
            }
        }
    }


    private void Init()
    {

#if UNITY_WEBGL
         StartCoroutine(LoadDatas());
#else
        cards = LoadData<CardTemplate>();
        effects = LoadData<CardEffectTemplate>();
        foreach (CardTemplate cardTemplate in cards)
        {
            cardsDict[cardTemplate.ID] = cardTemplate;
        }
        foreach (CardEffectTemplate cardEffect in effects)
        {
            effectsDict[cardEffect.ID] = cardEffect;
        }
#endif
    }
    IEnumerator LoadDatas()
    {
        yield return StartCoroutine(LoadData<CardTemplate>((cardlist) => { cards = cardlist; }));
        yield return StartCoroutine(LoadData<CardEffectTemplate>((effectlist) => { effects= effectlist; }));
        foreach (CardTemplate cardTemplate in cards)
        {
            cardsDict[cardTemplate.ID] = cardTemplate;
        }
        foreach (CardEffectTemplate cardEffect in effects)
        {
            effectsDict[cardEffect.ID] = cardEffect;
        }
    }
    public T GetTemplate<T>(int id) where T : class
    {
        if (typeof(T) == typeof(CardTemplate) && cardsDict.TryGetValue(id, out var card))
        {
            return card as T;
        }
        else if (typeof(T) == typeof(CardEffectTemplate) && effectsDict.TryGetValue(id, out var effect))
        {
            return effect as T;
        }

        return null;
    }

}

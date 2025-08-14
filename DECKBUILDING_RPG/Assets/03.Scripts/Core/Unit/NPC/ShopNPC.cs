using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;
using UnityEngine.Networking;

public class ShopNPC : NPC
{
    [Header("상점진열 세팅")]
    [SerializeField] private int minCardsToDisplay = 1;
    [SerializeField] private int maxCardsToDisplay = 6;
    [SerializeField] private int minQuantityPerCard = 1;
    [SerializeField] private int maxQuantityPerCard = 1;

    private List<ShopCardItem> salesCards = new List<ShopCardItem>();

    protected override void Awake()
    {
        base.Awake();
        if (interactUI != null)
        {
            interactUI.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        base.Start();
        StartCoroutine(LoadAndInitShopCards());
    }

    private IEnumerator LoadAndInitShopCards()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "ShopSellCards.json");
        string jsonText = "";

        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            using (UnityWebRequest www = UnityWebRequest.Get(filePath))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    jsonText = www.downloadHandler.text;
                    Logger.Log($"[ShopNPC] ShopSellCards.json로드 성공: {filePath}");
                }
                else
                {
                    Logger.LogError($"[ShopNPC] ShopSellCards.json 로드 실패: {www.error} at {filePath}");
                    yield break;
                }
            }
        }
        else
        {
            if (File.Exists(filePath))
            {
                jsonText = File.ReadAllText(filePath);
                Logger.Log($"로드 성공: {filePath}");
            }
            else
            {
                Logger.LogError($" 파일을 찾을 수 없습니다: {filePath}");
                yield break;
            }
        }

        List<int> availableCardIDsFromJSON = new List<int>();
        try
        {
            ShopSellCardsData data = JsonUtility.FromJson<ShopSellCardsData>(jsonText);
            if (data != null && data.SellCardIDs != null)
            {
                foreach (string idStr in data.SellCardIDs)
                {
                    if (int.TryParse(idStr, out int idInt))
                    {
                        availableCardIDsFromJSON.Add(idInt);
                    }
                }
            }
            else
            {
                Logger.LogError("[ShopNPC] ShopSellCards.json (StreamingAssets) 파싱 실패 또는 'SellCardIDs' 배열이 비어 있습니다.");
                yield break;
            }
        }
        catch (Exception e)
        {
            Logger.LogError($"[ShopNPC] ShopSellCards.json (StreamingAssets) 파싱 중 오류 발생: {e.Message}");
            yield break;
        }

        salesCards.Clear();
        CardManager cardManager = GameManager.Instance.cardManager;


        int numCardsToDisplay = UnityEngine.Random.Range(minCardsToDisplay, maxCardsToDisplay + 1);
        numCardsToDisplay = Mathf.Min(numCardsToDisplay, availableCardIDsFromJSON.Count);

        List<int> shuffledCardIDs = new List<int>(availableCardIDsFromJSON);
        ShuffleList(shuffledCardIDs);

        List<int> selectedCardIDs = shuffledCardIDs.Take(numCardsToDisplay).ToList();

        foreach (int cardID in selectedCardIDs)
        {
            ACard cardInstance = cardManager.GetCard(cardID);

            if (cardInstance != null)
            {
                int quantity = UnityEngine.Random.Range(minQuantityPerCard, maxQuantityPerCard + 1);

                if (quantity < 1)
                {
                    Logger.LogWarning($"[ShopNPC:{gameObject.name}] '{cardInstance.Name}' 카드의 랜덤 수량이 1 미만({quantity})입니다. 1개로 조정");
                    quantity = 1;
                }

                salesCards.Add(new ShopCardItem(cardInstance, quantity));
                Logger.Log($"[ShopNPC:{gameObject.name}] 상점에 '{cardInstance.Name}' (ID: {cardID}, {quantity}개) 추가됨");
            }
        }

        if (salesCards.Count == 0)
        {
            Logger.LogError($"[ShopNPC:{gameObject.name}] ShopSellCards.json에 설정된 항목이 있지만, 유효한 카드가 없어 상점이 비어있습니다. ");
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public override void TakeInteract(Action onComplete)
    {
        ShopNPCInteractUI shopUIInstance = interactUI as ShopNPCInteractUI;

        if (shopUIInstance == null)
        {
            onComplete?.Invoke();
            return;
        }

        if (shopUIInstance.gameObject.activeSelf)
        {
            shopUIInstance.Disable();
        }
        else
        {
            shopUIInstance.OpenShop(new List<ShopCardItem>(salesCards), this, onComplete);
        }
    }

    public List<ShopCardItem> GetShopItems()
    {
        return salesCards;
    }

    public void OnCardPurchased(ACard purchasedCard)
    {
        for (int i = 0; i < salesCards.Count; i++)
        {
            if (salesCards[i].Card == purchasedCard)
            {
                ShopCardItem item = salesCards[i];
                item.Quantity--;

                if (item.Quantity <= 0)
                {
                    salesCards.RemoveAt(i);
                    Logger.Log($"[ShopNPC:{gameObject.name}] '{purchasedCard.Name}' 카드가 판매 목록에서 제거됨 (재고 없음)");
                }
                else
                {
                    salesCards[i] = item;
                    Logger.Log($"[ShopNPC:{gameObject.name}] '{purchasedCard.Name}' 카드 재고 {item.Quantity}개 남음");
                }
                return;
            }
        }
    }
}
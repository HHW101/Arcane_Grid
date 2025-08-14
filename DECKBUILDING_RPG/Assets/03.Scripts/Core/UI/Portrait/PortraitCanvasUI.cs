using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PortraitCanvasUI : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private RectTransform scrollViewRect;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private Transform portraitGroup;
    [SerializeField] private PortraitUI portraitPrefab;
    [SerializeField] private ScrollRect scrollRect;

    [Header("설정")]
    [SerializeField] private int maxVisiblePortraitCount = 8;
    [SerializeField] private float portraitWidth = 100f;
    [SerializeField] private float spacing = 8f;
    [SerializeField] private float minScrollViewWidth = 200f;
    [SerializeField] private float maxScrollViewWidth = 1200f;

    private readonly List<PortraitUI> portraitUIList = new();
    private readonly Dictionary<Unit, PortraitUI> unitToPortrait = new();
    private int lastPortraitCount = -1;
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    public int initialFocusIndex = 0;

    private GameContext _ctx;
    private Coroutine _observeCoro;
    private string _lastSnapshotHash = "";
    private PortraitUI _playerPortrait;

    [SerializeField] private bool includePlayerPortrait = true;       
    [SerializeField] private bool keepPlayerPortraitOnDeath = true;   

    private void Awake()
    {
        var turnManager = FindObjectOfType<TurnManager>();
        if (turnManager)
            turnManager.RegisterPortraitUI(this);
        else
            Logger.Log("TurnManager를 찾을 수 없습니다.");

        if (scrollRect)
        {
            return;
        }

        scrollRect = GetComponentInChildren<ScrollRect>();
        if (!scrollRect)
            Logger.Log("ScrollRect 컴포넌트를 찾을 수 없습니다!");
    }

    private void OnEnable()
    {
        _ctx = GameManager.Instance ? GameManager.Instance.gameContext : null;
        if (_ctx == null)
        {
            Logger.Log("GameContext를 찾을 수 없습니다.");
            return;
        }

        SyncFromContext();
        _observeCoro = StartCoroutine(ObserveRoster());
    }

    private void OnDisable()
    {
        if (_observeCoro != null) StopCoroutine(_observeCoro);
        _observeCoro = null;
    }

    private IEnumerator ObserveRoster()
    {
        var wait = new WaitForSeconds(0.2f);
        while (enabled)
        {
            if (_ctx == null) yield break;

            if (_ctx.isNPCListUpdateRequested)
            {
                _ctx.isNPCListUpdateRequested = false;
                SyncFromContext();
            }
            else
            {
                var h = ComputeSnapshotHash(_ctx);
                if (h != _lastSnapshotHash)
                {
                    _lastSnapshotHash = h;
                    SyncFromContext();
                }
            }

            yield return wait;
        }
    }

    private string ComputeSnapshotHash(GameContext ctx)
    {
        unchecked
        {
            int acc = 17;

            acc = acc * 31 + (ctx.player ? ctx.player.GetHashCode() : 0);
            
            acc = acc * 31 + (ctx.npcList?.Count ?? 0);
            acc = ctx.npcList.Aggregate(acc, (current, npc) => current * 31 + (npc ? npc.GetHashCode() : 0));

            acc = acc * 31 + (ctx.npcDatas?.Count ?? 0);
            foreach (var kv in ctx.npcDatas)
            {
                var npc = kv.Key;
                var data = kv.Value;
                acc = acc * 31 + (npc ? npc.GetHashCode() : 0);
                acc = acc * 31 + (data?.portraitPath?.GetHashCode() ?? 0);
            }
            return acc.ToString();
        }
    }

    private void SyncFromContext()
    {
        if (_ctx == null) return;

        if (includePlayerPortrait) SyncPlayerPortrait();
        else RemovePlayerPortrait();

        SyncNpcPortraits();

        AdjustScrollViewWidth(portraitUIList.Count);
        lastPortraitCount = portraitUIList.Count;
        UpdateAlignmentForContent();

        if (gameObject.activeInHierarchy)
            StartCoroutine(ApplyIndexAfterLayout(initialFocusIndex));

        _lastSnapshotHash = ComputeSnapshotHash(_ctx);
    }

    private void SyncPlayerPortrait()
    {
        var player = _ctx.player as Unit; 
        if (!player)
        {
            RemovePlayerPortrait();
            return;
        }

        if (_playerPortrait && unitToPortrait.TryGetValue(player, out var ui) && ui == _playerPortrait)
        {
            RefreshUnitPortraitSprite(player, _playerPortrait);
            return;
        }

        var sprite = LoadUnitPortraitSprite(player);
        if (!sprite) return;

        var obj = Instantiate(portraitPrefab, portraitGroup);
        var pui = obj.GetComponent<PortraitUI>();
        pui.Setup(sprite, player);

        var btn = pui.GetComponent<Button>();
        if (btn) btn.onClick.AddListener(pui.OnClickPortrait);

        portraitUIList.Insert(0, pui);
        unitToPortrait[player] = pui;
        _playerPortrait = pui;

        player.OnDied -= HandlePlayerDied; 
        player.OnDied += HandlePlayerDied;
    }

    private void RemovePlayerPortrait()
    {
        if (!_playerPortrait) return;

        var player = unitToPortrait.FirstOrDefault(kv => kv.Value == _playerPortrait).Key;
        if (player)
        {
            player.OnDied -= HandlePlayerDied;
            unitToPortrait.Remove(player);
        }

        portraitUIList.Remove(_playerPortrait);
        Destroy(_playerPortrait.gameObject);
        _playerPortrait = null;
    }

    private void HandlePlayerDied(Unit unit)
    {
        if (!keepPlayerPortraitOnDeath)
        {
            RemovePortrait(unit);
            return;
        }

        if (!_playerPortrait)
        {
            return;
        }

        _playerPortrait.SetHighlight(false);
        _playerPortrait.SetFrameColor(Color.gray);
    }

    private void SyncNpcPortraits()
    {
        var playerUnit = _ctx.player as Unit;

        var alive = new HashSet<Unit>(_ctx.npcList.Where(n => n).Cast<Unit>());

        var toRemove = unitToPortrait.Keys
            .Where(u => u && u != playerUnit && !alive.Contains(u))
            .ToList();
        foreach (var u in toRemove)
            RemovePortrait(u);

        foreach (var unit in alive.Where(unit => unit != playerUnit))
        {
            if (unitToPortrait.ContainsKey(unit))
            {
                RefreshUnitPortraitSprite(unit, unitToPortrait[unit]);
            }
            else
            {
                var sprite = LoadUnitPortraitSprite(unit);
                if (!sprite) continue;

                AddPortrait(sprite, unit);
            }
        }
    }

    private void RefreshUnitPortraitSprite(Unit unit, PortraitUI ui)
    {
        var sprite = LoadUnitPortraitSprite(unit);
        if (sprite) ui.SetPortraitSprite(sprite);
    }

    private Sprite LoadUnitPortraitSprite(Unit unit)
    {
        if (!unit) return null;

        string path = unit.PortraitPath;

        var anpc = unit as ANPC;
        if (!anpc || _ctx == null || !_ctx.npcDatas.TryGetValue(anpc, out var data))
        {
            return string.IsNullOrEmpty(path) ? null : Resources.Load<Sprite>(path);
        }

        if (!string.IsNullOrEmpty(data.portraitPath))
            path = data.portraitPath;

        return string.IsNullOrEmpty(path) ? null : Resources.Load<Sprite>(path);
    }

    private void Update()
    {
        int currentPortraitCount = portraitUIList.Count;
        if (currentPortraitCount == lastPortraitCount) return;

        AdjustScrollViewWidth(currentPortraitCount);
        lastPortraitCount = currentPortraitCount;
    }

    private void UpdateAlignmentForContent()
    {
        if (!layoutGroup) return;

        int count = portraitUIList.Count;
        float contentW = count * portraitWidth + Mathf.Max(0, count - 1) * spacing;
        float viewW = scrollViewRect.rect.width;

        layoutGroup.spacing = spacing;
        layoutGroup.childAlignment = (contentW <= viewW)
            ? TextAnchor.MiddleCenter
            : TextAnchor.MiddleLeft;
    }

    private void HandleUnitDied(Unit unit)
    {
        if (_ctx != null && unit == (_ctx.player as Unit)) return;

        RemovePortrait(unit);
    }

    private void AdjustScrollViewWidth(int portraitCount)
    {
        float targetWidth = portraitCount * portraitWidth + Mathf.Max(0, (portraitCount - 1)) * spacing;
        targetWidth = Mathf.Clamp(targetWidth, minScrollViewWidth, maxScrollViewWidth);
        scrollViewRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
    }

    private void RemovePortrait(Unit unit)
    {
        if (!unit) return;

        if (!unitToPortrait.TryGetValue(unit, out var ui))
            return;

        unit.OnDied -= HandleUnitDied;

        if (ui) Destroy(ui.gameObject);
        portraitUIList.Remove(ui);
        unitToPortrait.Remove(unit);

        AdjustScrollViewWidth(portraitUIList.Count);
        lastPortraitCount = portraitUIList.Count;

        if (scrollRect)
            scrollRect.horizontalNormalizedPosition = 0.5f;
    }

    private void AddPortrait(Sprite sprite, Unit unit)
    {
        var obj = Instantiate(portraitPrefab, portraitGroup);
        var ui = obj.GetComponent<PortraitUI>();
        ui.Setup(sprite, unit);
        portraitUIList.Add(ui);
        unitToPortrait[unit] = ui;
        unit.OnDied += HandleUnitDied;

        AdjustScrollViewWidth(portraitUIList.Count);
        lastPortraitCount = portraitUIList.Count;

        if (scrollRect)
            scrollRect.horizontalNormalizedPosition = 0.5f;
    }

    public void ClearAll()
    {
        foreach (var unit in unitToPortrait.Select(kv => kv.Key).Where(u => u))
            unit.OnDied -= HandleUnitDied;

        unitToPortrait.Clear();

        for (int i = portraitGroup.childCount - 1; i >= 0; i--)
            Destroy(portraitGroup.GetChild(i).gameObject);

        portraitUIList.Clear();
        _playerPortrait = null;

        AdjustScrollViewWidth(0);
        lastPortraitCount = 0;

        if (scrollRect)
            scrollRect.horizontalNormalizedPosition = 0.5f;
    }

    public void HighlightOnly(int index)
    {
        for (int i = 0; i < portraitUIList.Count; i++)
            portraitUIList[i].SetHighlight(i == index);
    }

    private void OnDestroy()
    {
        foreach (var unit in unitToPortrait.Select(kv => kv.Key).Where(u => u != null))
            unit.OnDied -= HandleUnitDied;

        unitToPortrait.Clear();
        _playerPortrait = null;
    }

    private void ScrollToIndex(int index)
    {
        if (!scrollRect) return;

        int count = portraitUIList.Count;
        if (count == 0) return;
        index = Mathf.Clamp(index, 0, count - 1);

        var content = scrollRect.content ? scrollRect.content : portraitGroup as RectTransform;
        if (!content) return;

        float contentW = content.rect.width;
        float viewW = scrollViewRect.rect.width;

        if (contentW <= viewW)
        {
            scrollRect.horizontalNormalizedPosition = 0f;
            return;
        }

        float step = portraitWidth + spacing;
        float itemLeft = index * step;
        float target = itemLeft / (contentW - viewW);
        target = Mathf.Clamp01(target);

        scrollRect.horizontalNormalizedPosition = target;
    }

    private IEnumerator ApplyIndexAfterLayout(int index)
    {
        yield return null;

        if (scrollRect && scrollRect.content)
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
        Canvas.ForceUpdateCanvases();

        yield return null;

        ScrollToIndex(index);
    }
}

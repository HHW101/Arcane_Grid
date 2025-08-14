using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HpUI : MonoSingleton<HpUI>
{
    [SerializeField] GameObject panel;
    public GameObject Panel => panel;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Image hpBar;
    [SerializeField] TMP_Text hpText;
    private UnitData unitData = null;
    private float lastHp = 0;
    private float lastMaxHp = 0;
    private EnumAssociatedResourceManager enumAssociatedResourceManager;

    private Camera mainCamera;
    private GameObject currentHovered = null;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        enumAssociatedResourceManager = GameManager.Instance.enumAssociatedResourceManager;
        mainCamera = Camera.main;
    }

    public void Show(UnitData unitData)
    {
        this.unitData = unitData;
        nameText.SetText(enumAssociatedResourceManager.GetUnitIdentifierName(unitData.unitIdentifierType));
        hpBar.fillAmount = (float)unitData.currentHealth / unitData.maxHealth;
        hpText.SetText($"{unitData.currentHealth} / {unitData.maxHealth}");
        lastHp = unitData.currentHealth;
        lastMaxHp = unitData.maxHealth;
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        bool isPointerOverPanel = IsPointerOverSelf();

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);

        GameObject highestObj = null;
        int highestOrder = int.MinValue;

        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;

            SpriteRenderer renderer = hit.collider.GetComponent<SpriteRenderer>();
            if (renderer != null && renderer.sortingOrder >= highestOrder)
            {
                highestOrder = renderer.sortingOrder;
                highestObj = hit.collider.gameObject;
            }
        }

        bool isHoveringUnit = highestObj != null && highestObj.TryGetComponent<Unit>(out Unit hoveredUnit);

        if (isPointerOverPanel && !isHoveringUnit)
        {
            Hide();
            return;
        }

        if (!isPointerOverPanel && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Hide();
            return;
        }

        if (highestObj != currentHovered && highestObj != null)
        {
            Hide();
            currentHovered = highestObj;

            if (highestObj.TryGetComponent<Unit>(out Unit unit))
            {
                unitData = unit.unitData;
                Show(unitData);
            }
        }

        if (unitData != null)
        {
            if (lastHp != unitData.currentHealth || lastMaxHp != unitData.maxHealth)
            {
                hpBar.fillAmount = (float)unitData.currentHealth / unitData.maxHealth;
                hpText.SetText($"{unitData.currentHealth} / {unitData.maxHealth}");
                lastHp = unitData.currentHealth;
                lastMaxHp = unitData.maxHealth;
            }
        }
    }
    public static bool IsPointerOverSelf()
    {
        if (Instance == null || EventSystem.current == null)
            return false;

        return Instance.IsPointerOverUIObject(Instance.panel);
    }

    private bool IsPointerOverUIObject(GameObject targetRoot)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject != null && result.gameObject.transform.IsChildOf(targetRoot.transform))
            {
                return true;
            }
        }

        return false;
    }
}

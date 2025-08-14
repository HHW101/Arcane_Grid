using System.Collections;
using UnityEngine;
using TMPro; 

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI text;
    private Transform target;
    private RectTransform canvasRect;
    private Vector3 offset;

    private Transform poolParent;

    public void Set(int damage, Transform target, RectTransform canvasRect, Vector3 offset, Transform poolParent)
    {
        text.text = damage.ToString();
        this.target = target;
        this.canvasRect = canvasRect;
        this.offset = offset;
        this.poolParent = poolParent;

        if (canvasRect)
            transform.SetParent(canvasRect.transform, false);

        StartCoroutine(PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        var t = 0f;
        const float duration = 1.0f;
        const float floatDistance = 60f;

        Vector3 baseOffset = offset;
        while (t < duration)
        {
            if (!this || !gameObject) yield break;
            if (target && canvasRect)
            {
                var floatY = Mathf.Lerp(0, floatDistance, t / duration);

                Vector3 worldPos = target.position + baseOffset + Vector3.up * (floatY * 0.01f);
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, screenPos, null, out var localPoint);
                GetComponent<RectTransform>().anchoredPosition = localPoint;
            }
            t += Time.deltaTime;
            yield return null;
        }

        if (this && gameObject)
        {
            gameObject.SetActive(false);

            if (poolParent)
                transform.SetParent(poolParent, false);

            GameManager.Instance.poolManager.ReturnObject("DamageText", gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        StopAllCoroutines();
    }
}

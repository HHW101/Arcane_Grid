using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PortraitHpFill : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private Image hpFillImage; 

    [Header("옵션")]
    [SerializeField] private bool fromBottom = true; 
    [SerializeField] private bool animate = true;
    [SerializeField] private float animDuration = 0.12f;

    private Unit unit;
    private Coroutine animCo;

    public void Bind(Unit target)
    {
        Unbind();
        unit = target;

        if (!hpFillImage) hpFillImage = GetComponent<Image>();
        if (hpFillImage) hpFillImage.fillOrigin = fromBottom ? (int)Image.OriginVertical.Bottom
                                                             : (int)Image.OriginVertical.Top;

        if (!unit || !hpFillImage)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        SetFillImmediate(CalcRatio());

        unit.OnHealthChanged += OnHealthChanged;
    }

    public void Unbind()
    {
        if (unit)
            unit.OnHealthChanged -= OnHealthChanged;

        unit = null;

        if (animCo == null)
        {
            return;
        }

        StopCoroutine(animCo);
        animCo = null;
    }

    private void OnDestroy() => Unbind();

    private void OnHealthChanged() => SetFill(CalcRatio());

    private float CalcRatio()
    {
        if (!unit) return 0f;
        float max = Mathf.Max(1f, unit.MaxHealth);
        float hpRatio = Mathf.Clamp01(unit.CurrentHealth / max);

        return 1f - hpRatio;
    }

    private void SetFill(float target)
    {
        if (!hpFillImage) return;

        if (!animate)
        {
            hpFillImage.fillAmount = target;
            return;
        }

        if (animCo != null) StopCoroutine(animCo);
        animCo = StartCoroutine(LerpFill(hpFillImage.fillAmount, target, animDuration));
    }

    private void SetFillImmediate(float v)
    {
        if (hpFillImage) hpFillImage.fillAmount = v;
    }

    private IEnumerator LerpFill(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            hpFillImage.fillAmount = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        hpFillImage.fillAmount = to;
        animCo = null;
    }
}

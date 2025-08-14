using UnityEngine;
using UnityEngine.UI;

public class PortraitUI : MonoBehaviour
{
    [SerializeField] private Image portraitImage;
    [SerializeField] private Image frameImage;
    [SerializeField] private GameObject highlightEffect;
    [SerializeField] private PortraitHpFill hpFill;
    [SerializeField] private Outline frameOutline;
    private Unit linkedUnit;
    
    [SerializeField] private bool usePixelSnap = true;  
    [SerializeField] private bool dimOnUnhighlight = false; 
    
    private void Awake()
    {
        if (!hpFill) hpFill = GetComponentInChildren<PortraitHpFill>(true);
    }
    
    public void Setup(Sprite portraitSprite,  Unit unit)
    {
        if (portraitImage && portraitSprite)
            portraitImage.sprite = portraitSprite;
        linkedUnit = unit;
        if (hpFill) hpFill.Bind(unit);
        ApplyDefaultVisual(); 
        if (usePixelSnap) PixelSnap();
    }
    public void SetPortraitSprite(Sprite portraitSprite)
    {
        if (portraitImage && portraitSprite)
            portraitImage.sprite = portraitSprite;
    }
    
    private void ApplyDefaultVisual()
    {
        transform.localScale = Vector3.one;

        if (portraitImage)
            portraitImage.color = dimOnUnhighlight ? new Color(1,1,1,0.65f) : Color.white;

        if (frameImage)
            frameImage.color = Color.white;

        if (frameOutline)
            frameOutline.effectColor = Color.clear;

        if (highlightEffect)
            highlightEffect.SetActive(false);
    }
    
    public void SetHighlight(bool active)
    {
        if (frameOutline)
            frameOutline.effectColor = active ? Color.blue : Color.clear;
        
        if (highlightEffect)
            highlightEffect.SetActive(active);

        if (frameImage)
            frameImage.color = active ? Color.green : Color.white;

        if (active)
            transform.localScale = Vector3.one * 1.2f;
        else
            transform.localScale = Vector3.one;

        if (portraitImage)
        {
            if (dimOnUnhighlight)
                portraitImage.color = active ? Color.white : new Color(1,1,1,0.65f);
            else
                portraitImage.color = Color.white; 
        }

        if (usePixelSnap) PixelSnap();
    }

    public void SetFrameColor(Color color)
    {
        if (frameImage)
            frameImage.color = color;
    }
    public void OnClickPortrait()
    {
        if (!linkedUnit) return;
        var cameraManager = GameManager.Instance.cameraManager;
        if (cameraManager)
        {
            cameraManager.SetCameraFollow(linkedUnit.transform);
        }
    }
    private void OnDestroy()
    {
        if (hpFill) hpFill.Unbind();
    }
    private void PixelSnap()
    {
        var rt = transform as RectTransform;
        var canvas = GetComponentInParent<Canvas>();
        if (!rt || !canvas) return;

        var scale = canvas.scaleFactor;
        var pos = rt.anchoredPosition;
        pos.x = Mathf.Round(pos.x * scale) / scale;
        pos.y = Mathf.Round(pos.y * scale) / scale;
        rt.anchoredPosition = pos;

        var size = rt.sizeDelta;
        size.x = Mathf.Round(size.x);
        size.y = Mathf.Round(size.y);
        rt.sizeDelta = size;
    }
}
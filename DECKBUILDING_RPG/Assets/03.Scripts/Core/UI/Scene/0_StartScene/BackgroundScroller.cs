
using UnityEngine;

[System.Serializable]
public class BackgroundLayer
{
    public RectTransform image1;
    public RectTransform image2;

    public float scrollSpeed;
}

public class BackgroundScroller : MonoBehaviour
{
    public BackgroundLayer[] layers;

    void Start()
    {
        foreach (var layer in layers)
        {
            if (layer.image1 != null && layer.image2 != null)
            {
                layer.image2.anchoredPosition = new Vector2(layer.image1.anchoredPosition.x + layer.image1.rect.width, layer.image2.anchoredPosition.y);
            }
        }
    }

    void Update()
    {
        foreach (var layer in layers)
        {
            ScrollLayer(layer.image1, layer.image2, layer.scrollSpeed);
        }
    }

    private void ScrollLayer(RectTransform img1, RectTransform img2, float speed)
    {
        if (img1 == null || img2 == null) return;

        float imageWidth = img1.rect.width;

        float movementAmount = speed * Time.deltaTime;
        Vector2 movement = new Vector2(-movementAmount, 0);
        img1.anchoredPosition += movement;
        img2.anchoredPosition += movement;

        if (img1.anchoredPosition.x <= -imageWidth)
        {
            float overshoot = Mathf.Abs(img1.anchoredPosition.x) - imageWidth;
            img1.anchoredPosition = new Vector2(img2.anchoredPosition.x + imageWidth - overshoot, img1.anchoredPosition.y);
        }

        if (img2.anchoredPosition.x <= -imageWidth)
        {
            float overshoot = Mathf.Abs(img2.anchoredPosition.x) - imageWidth;
            img2.anchoredPosition = new Vector2(img1.anchoredPosition.x + imageWidth - overshoot, img2.anchoredPosition.y);
        }
    }
}

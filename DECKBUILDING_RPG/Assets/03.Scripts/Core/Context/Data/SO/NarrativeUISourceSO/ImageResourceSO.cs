using UnityEngine;

[CreateAssetMenu(fileName = "ImageSource", menuName = "Custom/ImageSource")]
public class ImageResourceSO : NarrativeResourceSO
{ 
    public Sprite image;
    public float waitBeforeHideSecond;
}
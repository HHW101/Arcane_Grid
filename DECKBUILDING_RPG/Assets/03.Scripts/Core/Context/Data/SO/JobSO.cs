using UnityEngine;

[CreateAssetMenu(fileName ="Class", menuName = "Custom/New Job")]
public class JobSO : ScriptableObject
{
    public string jobName;
    public string jobDescription;
    public PlayerData jobDefaultPlayerData;
    public Sprite jobSprite;
    public Sprite jobIcon;
}
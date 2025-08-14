using System;

public interface IShowDetailInfoable
{
    public DetailInfoData GetDataForDetailInfoUI();
}

[Serializable]
public class DetailInfoData
{
    public string name;
    public string description;
    public string GetHeaderText()
    {
        return name;
    }
}
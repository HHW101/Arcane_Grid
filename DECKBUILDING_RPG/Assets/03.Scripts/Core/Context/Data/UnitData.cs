using System;
using System.Collections.Generic;

[Serializable]
public class UnitData
{
    public List<UnitEnum.UnitType> unitTypeList = new();
    public string portraitPath;

    public float currentHealth;
    public float maxHealth;

    public UnitEnum.UnitIdentifierType unitIdentifierType;

    [NonSerialized]
    private HashSet<UnitEnum.UnitType> unitTypeCache;

    public HashSet<UnitEnum.UnitType> unitTypeSet
    {
        get
        {
            if (unitTypeCache == null)
            {
                unitTypeCache = new HashSet<UnitEnum.UnitType>();
                foreach (var type in unitTypeList)
                {
                    unitTypeCache.Add(type);
                }
            }
            return unitTypeCache;
        }
    }

    public bool IsHaveThisType(UnitEnum.UnitType type)
    {
        return unitTypeSet.Contains(type);
    }

    public UnitData CloneUnitData()
    {
        return new UnitData
        {
            unitTypeList = new List<UnitEnum.UnitType>(this.unitTypeList),
            portraitPath = this.portraitPath,
            unitIdentifierType = this.unitIdentifierType,
            currentHealth = this.currentHealth,
            maxHealth = this.maxHealth
        };
    }
}
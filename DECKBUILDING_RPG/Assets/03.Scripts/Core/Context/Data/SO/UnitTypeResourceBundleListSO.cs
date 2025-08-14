using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UnitTypeResourceBundleList", menuName = "Custom/ResourceBundle/UnitType")]
public class UnitTypeResourceBundleListSO : ScriptableObject
{
    public List<UnitTypeResourceBundle> bundleList;
}

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UnitCampTypeResourceBundleList", menuName = "Custom/ResourceBundle/UnitCampType")]
public class UnitCampTypeResourceBundleListSO : ScriptableObject
{
    public List<UnitCampTypeResourceBundle> bundleList;
}

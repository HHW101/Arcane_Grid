using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UnitIdentifierTypeResourceList", menuName = "Custom/ResourceBundle/UnitIdentifierType")]
public class UnitIdentifierTypeResourceBundleListSO : ScriptableObject
{
    public List<UnitIdentifierTypeResourceBundle> bundleList;
}
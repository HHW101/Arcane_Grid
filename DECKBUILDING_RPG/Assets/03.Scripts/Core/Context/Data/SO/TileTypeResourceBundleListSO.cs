using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TileTypeResourceBundleList", menuName = "Custom/ResourceBundle/TileType")]
public class TileTypeResourceBundleListSO : ScriptableObject
{
    public List<TileTypeResourceBundle> bundleList;
}
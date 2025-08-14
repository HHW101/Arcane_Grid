using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "JobTypeResourceBundleList", menuName = "Custom/ResourceBundle/JobType")]
public class JobTypeResourceBundleListSO : ScriptableObject
{
    public List<JobTypeResourceBundle> bundleList;
}
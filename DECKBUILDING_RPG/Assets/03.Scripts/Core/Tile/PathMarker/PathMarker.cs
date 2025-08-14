using UnityEngine;

public class PathMarker : MonoBehaviour
{
    [SerializeField]
    public FromSourcePathMarker fromSourcePathMarker;
    [SerializeField]
    public ToDestPathMarker toDestPathMarker;
    [SerializeField]
    public GameObject markerRoot;

    public void Show(HexCoord prev, HexCoord cur, HexCoord next, bool isReachable)
    {
        markerRoot.SetActive(true);
        fromSourcePathMarker.Show(prev, cur, isReachable);
        toDestPathMarker.Show(cur, next, isReachable);
    }

    public void Hide()
    {
        markerRoot.SetActive(false);
        fromSourcePathMarker.Hide();
        toDestPathMarker.Hide();
    }
}
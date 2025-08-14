using UnityEngine;

public class ToDestPathMarker : MonoBehaviour
{
    public GameObject unreachableMarker;
    public GameObject reachableMarker;

    public void Start()
    {
        unreachableMarker.SetActive(false);
        reachableMarker.SetActive(false);
    }

    public void Show(HexCoord cur, HexCoord next, bool isReachable)
    {
        unreachableMarker.SetActive(!isReachable);
        reachableMarker.SetActive(isReachable);

        HexCoord dir = HexCoord.GetDirection(cur, next);
        float angle = HexCoord.GetAngleFromDirection(dir);
        transform.rotation = Quaternion.Euler(0f, 0f, (360f - angle));
    }

    public void Hide()
    {
        unreachableMarker.SetActive(false);
        reachableMarker.SetActive(false);
    }
}
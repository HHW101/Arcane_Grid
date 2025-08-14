
using UnityEngine;

public class FromSourcePathMarker : MonoBehaviour
{
    public GameObject unreachableMarker;
    public GameObject reachableMarker;

    public void Start()
    {
        unreachableMarker.SetActive(false);
        reachableMarker.SetActive(false);
    }

    public void Show(HexCoord prev, HexCoord cur, bool isReachable)
    {
        unreachableMarker.SetActive(!isReachable);
        reachableMarker.SetActive(isReachable);

        HexCoord dir = HexCoord.GetDirection(prev, cur);
        float angle = HexCoord.GetAngleFromDirection(dir);
        transform.rotation = Quaternion.Euler(0f, 0f, (540f - angle) % 360f);
    }

    public void Hide()
    {
        unreachableMarker.SetActive(false);
        reachableMarker.SetActive(false);
    }
}
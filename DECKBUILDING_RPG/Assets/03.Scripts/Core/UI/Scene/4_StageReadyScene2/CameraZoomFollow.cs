using UnityEngine;

public class CameraZoomFollw : MonoBehaviour
{
    //
    public Transform followTarget;
    public float zoomSpeed = 5f;
    public float minY = 2f;
    public float maxY = 20f;

    private void Awake()
    {
        var virtualCam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (virtualCam != null && virtualCam.Follow == null)
        {
            virtualCam.Follow = followTarget;
        }
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            Vector3 pos = followTarget.position;
            pos.y += scroll * zoomSpeed;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            followTarget.position = pos;
        }
    }
}

using System;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

[Serializable]
public class CameraManagerParam
{
    public Camera mainCamera;
    public CinemachineBrain brain;
    public CinemachineVirtualCamera defaultCamera;

}
public class CameraManager : MonoBehaviour
{
    GameContext gameContext;
    CameraManagerParam cameraManagerParam = new CameraManagerParam();
    
    public float zoomSpeed = 2f;
    public float minOrthoSize = 5f;
    public float maxOrthoSize = 15f;
    public float panSpeed = 1f;

    Vector3 lastMousePos;
    bool isPanning = false;
    Transform prevFollowTarget; 
    
    public void Init(GameContext gameContexxt, CameraManagerParam cameraManagerParam)
    {
        this.gameContext = gameContexxt;
        this.cameraManagerParam = cameraManagerParam;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        HandleZoom();
        HandlePan();
    }
    
    public void SetCameraFollow(Transform target)
    {
        if (cameraManagerParam.defaultCamera != null && target != null)
        {
            cameraManagerParam.defaultCamera.Follow = target;
        }
        else
        {
            Debug.LogWarning("VirtualCamera 또는 target이 null입니다.");
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);
        cameraManagerParam.defaultCamera.Follow = null;
    }

    private void HandleZoom()
    {
        if (ScrollViewWheelBlocker.IsPointerOverScrollView)
            return;
        
        var vcam = cameraManagerParam.defaultCamera;
        if (!vcam || !vcam.m_Lens.Orthographic)
        {
            return;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (!(Mathf.Abs(scroll) > 0.01f))
        {
            return;
        }

        float newSize = vcam.m_Lens.OrthographicSize - scroll * zoomSpeed;
        vcam.m_Lens.OrthographicSize = Mathf.Clamp(newSize, minOrthoSize, maxOrthoSize);
    }

    private void HandlePan()
    {
        var cam = cameraManagerParam.mainCamera;

        if (Input.GetMouseButtonDown(2))
        {
            lastMousePos = Input.mousePosition;
            isPanning = true;

            var vcam = cameraManagerParam.defaultCamera;
            if (vcam.Follow)
            {
                prevFollowTarget = vcam.Follow;
                vcam.Follow = null;
            }
        }
        if (Input.GetMouseButtonUp(2))
        {
            isPanning = false;
        }

        if (isPanning)
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            var move = new Vector3(-delta.x, -delta.y, 0) * (panSpeed * cam.orthographicSize) / (Screen.height * 0.5f);
            cam.transform.position += move;
            lastMousePos = Input.mousePosition;
        }
    }
    public void RestoreFollow()
    {
        if (!cameraManagerParam.defaultCamera || !prevFollowTarget)
        {
            return;
        }

        cameraManagerParam.defaultCamera.Follow = prevFollowTarget;
        prevFollowTarget = null;
    }

}

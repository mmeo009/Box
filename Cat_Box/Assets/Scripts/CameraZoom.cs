using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float moveSpeed = 10f;
    public LayerMask groundLayer;

    public float zoomSpeed = 5f;
    public float minZoom = 10f;
    public float maxZoom = 50f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        ZoomCamera();
    }

    void ZoomCamera()
    {
        float scrollData = Input.GetAxis("Mouse ScrollWheel");

        if (scrollData != 0.0f)
        {
            float newCameraHeight = transform.position.y - scrollData * zoomSpeed;
            newCameraHeight = Mathf.Clamp(newCameraHeight, minZoom, maxZoom);
            transform.position = new Vector3(transform.position.x, newCameraHeight, transform.position.z);
        }
    }
}
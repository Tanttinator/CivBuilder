using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    [Header("WASD")]
    public float kbSpeed;
    [Header("Zoom")]
    public float zoomSensitivity;
    public float minZoom;
    public float maxZoom;
    [Header("Drag Move")]
    public float dragMoveSpeed;
    [Header("Drag Rotation")]
    public float dragRotationSpeed;

    Vector3 origin;
    Vector3 difference;

    Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void MoveCamera(Vector3 dir, Space space)
    {
        transform.Translate(dir * kbSpeed * (cam.fieldOfView / maxZoom) * Time.deltaTime, space);
    }

    public void MoveForward()
    {
        MoveCamera(new Vector3(transform.forward.x, 0, transform.forward.z).normalized, Space.World);
    }

    public void MoveBack()
    {
        MoveCamera(new Vector3(-transform.forward.x, 0, -transform.forward.z).normalized, Space.World);
    }

    public void MoveRight()
    {
        MoveCamera(Vector3.right, Space.Self);
    }

    public void MoveLeft()
    {
        MoveCamera(-Vector3.right, Space.Self);
    }

    public void DragMove()
    {
        transform.Translate(new Vector3(transform.forward.x, 0, transform.forward.z).normalized * -Input.GetAxis("Mouse Y") * dragMoveSpeed * (cam.fieldOfView / maxZoom) * Time.deltaTime, Space.World);
        transform.Translate(Vector3.right * -Input.GetAxis("Mouse X") * dragMoveSpeed * (cam.fieldOfView / maxZoom) * Time.deltaTime);
    }

    public void DragRotate()
    {
        transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * dragRotationSpeed * (cam.fieldOfView / maxZoom) * Time.deltaTime, Input.GetAxis("Mouse X") * dragRotationSpeed * (cam.fieldOfView / maxZoom) * Time.deltaTime, 0));
        float x = transform.localRotation.eulerAngles.x;
        float y = transform.localRotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(x, y, 0);
    }

    public void Zoom(float amount)
    {
        cam.fieldOfView -= amount * zoomSensitivity * (cam.fieldOfView / maxZoom);
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);
    }
}

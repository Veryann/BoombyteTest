using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] GameObject target;
    [SerializeField] float distance;
    [SerializeField] float xSpeed;
    [SerializeField] float ySpeed;
    [SerializeField] float scrollSensitivity;
    [SerializeField] float minDistance;
    [SerializeField] float maxDistance;
    [SerializeField] float smoothTime = 5f;
    float yMinLimit = 0f;
    float yMaxLimit = 90f;
    float yRotation;
    float xRotation;
    float xVelocity;
    float yVelocity;


    void Start()
    {
        Vector3 angles = cam.transform.eulerAngles;
        yRotation = angles.y;
        xRotation = angles.x;
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            xVelocity += xSpeed * Input.GetAxis("Mouse X") * distance * 0.02f;
            yVelocity += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
        }

        distance -= scrollSensitivity * Input.mouseScrollDelta.y;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        yRotation += xVelocity;
        xRotation -= yVelocity;
        xRotation = ClampAngle(xRotation, yMinLimit, yMaxLimit);
        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0);

        Vector3 position = rotation * new Vector3(0f, 0f, -distance) + target.transform.position;

        cam.transform.rotation = rotation;
        cam.transform.position = position;
        xVelocity = Mathf.Lerp(xVelocity, 0, Time.deltaTime * smoothTime);
        yVelocity = Mathf.Lerp(yVelocity, 0, Time.deltaTime * smoothTime);
    }

    public float ClampAngle(float angle, float min, float max)
    {
        angle = angle < -360 ? angle + 360f : angle;
        angle = angle > 360 ? angle - 360f : angle;

        return Mathf.Clamp(angle, min, max);
    }

}

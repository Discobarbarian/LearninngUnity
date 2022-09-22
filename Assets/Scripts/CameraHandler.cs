using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public Transform TargetTransform;
    public Transform CameraTransform;
    public Transform CameraPivotTransform;
    private Transform _transform;
    private Vector3 _cameraTransformPosition;
    private LayerMask _ignoreLayers;

    public static CameraHandler _singleton;

    public float LookSpeed = 0.1f;
    public float FollowSpeed = 0.1f;
    public float PivotSpeed = 0.03f;

    private float _defaultPosition;
    private float _lookAngle;
    private float _pivotAngle;
    private float _minPivot = -35f;
    private float _maxPivot = 35f;

    private void Awake()
    {
        _singleton = this;
        _transform = transform;
        _defaultPosition = CameraTransform.localPosition.z;
        _ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
    }

    public void FollowTarget(float delta)
    {
        Vector3 targetPosition = Vector3.Lerp(_transform.position, TargetTransform.position, delta / FollowSpeed);
        _transform.position = targetPosition;
    }

    public void HandleCameraRotation (float delta, float mouseInputX, float mouseInputY)
    {
        _lookAngle += (mouseInputX * + LookSpeed) / delta;
        _pivotAngle -= (mouseInputY * PivotSpeed) / delta;
        _pivotAngle = Mathf.Clamp(_pivotAngle, _minPivot, _maxPivot);

        Vector3 rotation = Vector3.zero;
        rotation.y = _lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        _transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = _pivotAngle;

        targetRotation = Quaternion.Euler(rotation);
        CameraPivotTransform.localRotation = targetRotation;
    }
}

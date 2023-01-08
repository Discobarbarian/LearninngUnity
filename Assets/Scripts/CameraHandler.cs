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
    private Vector3 _cameraFollowVelocity = Vector3.zero;

    public static CameraHandler _singleton;

    public float LookSpeed = 0.1f;
    public float FollowSpeed = 0.1f;
    public float PivotSpeed = 0.03f;

    private float _defaultPosition;
    private float _targetPosition;
    private float _lookAngle;
    private float _pivotAngle;
    private float _minPivot = -35f;
    private float _maxPivot = 35f;

    public float CameraSphereRadius = 0.2f;
    public float CameraCollisionOffset = 0.2f;
    public float MinimumCollisionOffset = 0.2f;

    private void Awake()
    {
        _singleton = this;
        _transform = transform;
        _defaultPosition = CameraTransform.localPosition.z;
        _ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
    }

    public void FollowTarget(float delta)
    {
        Vector3 targetPosition = Vector3.SmoothDamp
            (_transform.position, TargetTransform.position, ref _cameraFollowVelocity, delta / FollowSpeed);
        _transform.position = targetPosition;

        HandleCameraCollisions(delta);
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

    private void HandleCameraCollisions(float delta)
    {
        _targetPosition = _defaultPosition;
        RaycastHit hit;
        Vector3 direction = CameraTransform.position - CameraPivotTransform.position;
        direction.Normalize();

        if (Physics.SphereCast
            (CameraPivotTransform.position, CameraSphereRadius, direction, out hit, Mathf.Abs(_targetPosition),
            _ignoreLayers))
        {
            float dist = Vector3.Distance(CameraPivotTransform.position, hit.point);
            _targetPosition = -(dist - CameraCollisionOffset);
        }

        if (Mathf.Abs(_targetPosition) < MinimumCollisionOffset)
        {
            _targetPosition = -MinimumCollisionOffset;
        }

        _cameraTransformPosition.z = Mathf.Lerp(CameraTransform.localPosition.z, _targetPosition, delta / 0.2f);
        CameraTransform.localPosition = _cameraTransformPosition;

    }
}

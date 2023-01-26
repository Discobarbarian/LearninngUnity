using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public Transform Target;
    public Transform Camera;
    public Transform Pivot;
    private Vector3 _cameraTransformPosition;
    private LayerMask _ignoreLayers;
    private Vector3 _cameraFollowVelocity = Vector3.zero;

    public static CameraHandler SINGLETON;

    public float LookSpeed = 0.1f;
    public float FollowSpeed = 0.1f;
    public float PivotSpeed = 0.03f;

    private float _defaultPosition;
    private float _targetPosition;
    private float _lookAngle;
    private float _pivotAngle;
    private float _minPivot = -35f;
    private float _maxPivot = 35f;

    public float SphereRadius = 0.2f;
    public float CameraCollisionOffset = 0.2f;
    public float MinimumCollisionOffset = 0.2f;

    private void Awake()
    {
        SINGLETON = this;
        _defaultPosition = Camera.localPosition.z;
        _ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
    }

    public void FollowTarget(float delta)
    {
        Vector3 targetPosition 
            = Vector3.SmoothDamp(transform.position, Target.position, ref _cameraFollowVelocity, delta / FollowSpeed);
        transform.position = targetPosition;

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
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = _pivotAngle;

        targetRotation = Quaternion.Euler(rotation);
        Pivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions(float delta)
    {
        _targetPosition = _defaultPosition;
        RaycastHit hit;
        Vector3 direction = Camera.position - Pivot.position;
        direction.Normalize();
        bool cast = Physics.SphereCast(Pivot.position, SphereRadius, direction, out hit, Mathf.Abs(_targetPosition), _ignoreLayers);

        if (cast)
        {
            float dist = Vector3.Distance(Pivot.position, hit.point);
            _targetPosition = -(dist - CameraCollisionOffset);
        }

        if (Mathf.Abs(_targetPosition) < MinimumCollisionOffset)
        {
            _targetPosition = -MinimumCollisionOffset;
        }

        _cameraTransformPosition.z = Mathf.Lerp(Camera.localPosition.z, _targetPosition, delta / 0.2f);
        Camera.localPosition = _cameraTransformPosition;
    }
}

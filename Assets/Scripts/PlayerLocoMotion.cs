using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocoMotion : MonoBehaviour
{
    Transform cameraObject;
    InputHandler inputHandler;
    Vector3 moveDirection;

    [HideInInspector] public Transform _transform;
    [HideInInspector] public AnimatorHandler _animatorHandler;

    public Rigidbody Rigidbody;
    public GameObject NormalCamera;

    [Header("Options")]
    [SerializeField] float _movementSpeed = 5f;
    [SerializeField] float _rotationSpeed = 10f;
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        inputHandler = GetComponent<InputHandler>();
        _animatorHandler = GetComponentInChildren<AnimatorHandler>();
        cameraObject = Camera.main.transform;
        _transform = transform;
        _animatorHandler.Init();
    }

    public void Update()
    {
        float delta = Time.deltaTime;
        inputHandler.TickInput(delta);

        moveDirection = cameraObject.forward * inputHandler.vertical;
        moveDirection += cameraObject.right * inputHandler.horizontal;
        moveDirection.Normalize();
        moveDirection.y = 0f;

        float speed = _movementSpeed;
        moveDirection *= speed;

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, _normalVector);
        Rigidbody.velocity = projectedVelocity;
        _animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0f);

        if (_animatorHandler.CanRotate)
        {
            HandleRotation(delta);
        }
    }

    #region Movement
    Vector3 _normalVector;
    Vector3 _targerPosition;

    private void HandleRotation(float delta)
    {
        Vector3 targetDirection = Vector3.zero;
        float moveOverride = inputHandler.moveAmount;

        targetDirection = cameraObject.forward * inputHandler.vertical;
        targetDirection += cameraObject.right * inputHandler.horizontal;
        targetDirection.Normalize();
        targetDirection.y = 0f;

        if(targetDirection == Vector3.zero) targetDirection = _transform.forward;

        float rotationSpeed = _rotationSpeed;
        Quaternion tr = Quaternion.LookRotation(targetDirection);
        Quaternion targetRotation = Quaternion.Slerp(_transform.rotation, tr, rotationSpeed * delta);

        _transform.rotation = targetRotation;
    }
    #endregion
}

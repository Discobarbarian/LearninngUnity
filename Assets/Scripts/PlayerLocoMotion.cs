using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocoMotion : MonoBehaviour
{
    private Transform _cameraObject;
    private InputHandler _inputHandler;
    private Vector3 _normalVector;
    private Vector3 _targetPosition;
    private Vector3 _moveDirection;

    private Transform _transform;
    private AnimatorHandler _animatorHandler;

    public Rigidbody Rigidbody;

    [Header("Options")]
    [SerializeField] float _movementSpeed = 5f;
    [SerializeField] float _rotationSpeed = 10f;

    private void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        _inputHandler = GetComponent<InputHandler>();
        _animatorHandler = GetComponentInChildren<AnimatorHandler>();
        _cameraObject = Camera.main.transform;
        _transform = transform;
        _animatorHandler.Init();
    }

    private void Update()
    {
        float delta = Time.deltaTime;

        _inputHandler.TickInput(delta);
        HandleMovement(delta);
        HandleRollingAndSprinting(delta);
    }

    private void HandleRotation(float delta)
    {
        Vector3 targetDirection = Vector3.zero;
        float moveOverride = _inputHandler.MoveAmount;

        targetDirection = _cameraObject.forward * _inputHandler.Vertical;
        targetDirection += _cameraObject.right * _inputHandler.Horizontal;
        targetDirection.Normalize();
        targetDirection.y = 0f;

        if(targetDirection == Vector3.zero) targetDirection = _transform.forward;

        Quaternion tr = Quaternion.LookRotation(targetDirection);
        Quaternion targetRotation = Quaternion.Slerp(_transform.rotation, tr, _rotationSpeed * delta);

        _transform.rotation = targetRotation;
    }

    private void HandleMovement(float delta)
    {
        _moveDirection = _cameraObject.forward * _inputHandler.Vertical;
        _moveDirection += _cameraObject.right * _inputHandler.Horizontal;
        _moveDirection.Normalize();
        _moveDirection.y = 0f;

        _moveDirection *= _movementSpeed;

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(_moveDirection, _normalVector);
        Rigidbody.velocity = projectedVelocity;
        _animatorHandler.UpdateAnimatorValues(_inputHandler.MoveAmount, 0f);

        if (_animatorHandler.CanRotate)
        {
            HandleRotation(delta);
        }
    }

    private void HandleRollingAndSprinting(float delta)
    {
        if (_animatorHandler.Animator.GetBool("isInteracting")) return;
        if (!_inputHandler.RollFlag) return;
        
        _moveDirection = _cameraObject.forward * _inputHandler.Vertical;
        _moveDirection += _cameraObject.right * _inputHandler.Horizontal;

        if (_inputHandler.MoveAmount > 0)
        {
            _animatorHandler.PlayTargetAnimation("Roll", true);
            _moveDirection.y = 0;
            Quaternion rollRotation = Quaternion.LookRotation(_moveDirection);
            transform.rotation = rollRotation;
            return;
        }

        _animatorHandler.PlayTargetAnimation("StepBack", true);
    }
}

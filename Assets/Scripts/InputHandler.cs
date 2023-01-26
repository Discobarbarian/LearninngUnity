using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float Horizontal;
    public float Vertical;
    public float MoveAmount;
    public float MouseX;
    public float MouseY;

    public bool Input;
    public bool RollFlag;
    public bool IsInteracting;

    private PlayerControls _inputActions;
    private CameraHandler _cameraHandler;

    private Vector2 _movementInput;
    private Vector2 _cameraInput;

    private void Awake()
    {
        _cameraHandler = CameraHandler.SINGLETON;    
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;

        if (_cameraHandler != null)
        {
            _cameraHandler.FollowTarget(delta);
            _cameraHandler.HandleCameraRotation(delta, MouseX, MouseY);
        }
    }

    private void OnEnable()
    {
        if (_inputActions == null)
        {
            _inputActions = new PlayerControls();
            _inputActions.PlayerMovement.Movement.performed += inputActions => _movementInput = inputActions.ReadValue<Vector2>();
            _inputActions.PlayerMovement.Camera.performed += i => _cameraInput = i.ReadValue<Vector2>();
        }
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    public void TickInput(float delta)
    {
        MoveInput(delta);
        HandleRollInput(delta);
    }

    private void MoveInput(float delta)
    {
        Horizontal = _movementInput.x;
        Vertical = _movementInput.y;
        MoveAmount = Mathf.Clamp01(Mathf.Abs(Horizontal) + Mathf.Abs(Vertical));
        MouseX = _cameraInput.x;
        MouseY = _cameraInput.y;
    }

    private void HandleRollInput(float delta)
    {
        Input = _inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;

        if (Input)
        {
            RollFlag = true;
        }
    }
}

using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    public Animator Animator;
    private InputHandler _inputHandler;
    private PlayerLocoMotion _playerLocoMotion;

    private int _vertical;
    private int _horizontal;

    public bool CanRotate;

    public void Init()
    {
        Animator = GetComponent<Animator>();
        _inputHandler = GetComponentInParent<InputHandler>();
        _playerLocoMotion = GetComponentInParent<PlayerLocoMotion>();
        _vertical = Animator.StringToHash("Vertical");
        _horizontal = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement)
    {
        verticalMovement = ValidateInput(verticalMovement);
        horizontalMovement = ValidateInput(horizontalMovement);

        Animator.SetFloat(_vertical, verticalMovement, 0.1f, Time.deltaTime);
        Animator.SetFloat(_horizontal, horizontalMovement, 0.1f, Time.deltaTime);
    }

    private float ValidateInput (float parameter)
    {
        float abs = Mathf.Abs(parameter);
        float sign = Mathf.Sign(parameter);

        if (abs > 0.55f) abs = 1f;
        else if (abs > 0f) abs = 0.5f;

        return abs * sign;
    }

    public void PlayTargetAnimation(string targetAnim, bool isInteracting)
    {
        Animator.applyRootMotion = isInteracting;
        Animator.SetBool("isInteracting", isInteracting);
        Animator.CrossFade(targetAnim, 0.2f);
    }
    private void AllowRotation()
    {
        CanRotate = true;
    }

    private void BanRotation()
    {
        CanRotate = false;
    }

    private void OnAnimatorMove()
    {
        if (_inputHandler.IsInteracting == false)
            return;

        float delta = Time.deltaTime;
        _playerLocoMotion.Rigidbody.drag = 0;
        Vector3 deltaPosition = Animator.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / delta;
        //velocity.z = 2f;
        _playerLocoMotion.Rigidbody.velocity = velocity; //this line disables player model movement wlile rolling, but i need to make little thrust
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private InputHandler _inputHandler;
    private Animator _animator;
    void Start()
    {
        _inputHandler = GetComponent<InputHandler>();
        _animator = GetComponentInChildren<Animator>();
    }


    void Update()
    {
        _inputHandler.IsInteracting = _animator.GetBool("isInteracting");
        _inputHandler.RollFlag = false;
    }
}

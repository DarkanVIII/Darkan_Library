using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
#if UNITY_EDITOR
    public static InputManager I { get; private set; }
#else
    public static InputManager I;
#endif

    public static event Action OnJumpStarted;

    PlayerInputActions _inputActions;

    void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(gameObject);

        _inputActions = new();
        _inputActions.Player.Enable();

        _inputActions.Player.Jump.started += Jump_Started;
    }

    void OnDestroy()
    {
        _inputActions.Player.Jump.started -= Jump_Started;
    }

    void Jump_Started(InputAction.CallbackContext context) => OnJumpStarted?.Invoke();

    public Vector2 GetMovementVectorNormalized()
    {
        return _inputActions.Player.Move.ReadValue<Vector2>().normalized;
    }

    public float GetHorizontalAxis()
    {
        return _inputActions.Player.Move.ReadValue<Vector2>().x;
    }

    public float GetVerticalAxis()
    {
        return _inputActions.Player.Move.ReadValue<Vector2>().y;
    }

}

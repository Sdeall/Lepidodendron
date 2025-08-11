using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum MovementState {Static, Forward, Left, Right}

    [SerializeField] float _moveSpeed, _rotationSpeed, _gravity;

    MovementState currentState = MovementState.Static;

    CharacterController controller;

    Transform playerControllerTransform;

    void Awake()
    {
        SetComponents();
    }

    void FixedUpdate()
    {
        controller.Move(new Vector3(0, -_gravity * Time.fixedDeltaTime, 0));
        if (currentState != MovementState.Static)
            Move();
    }

    void Move()
    {
        switch (currentState)
        {
            case MovementState.Forward:
                controller.Move(playerControllerTransform.forward * _moveSpeed * Time.fixedDeltaTime);
                break;
            case MovementState.Left:
                playerControllerTransform.Rotate(Vector3.up, -_rotationSpeed * Time.fixedDeltaTime);
                break;
            case MovementState.Right:
                playerControllerTransform.Rotate(Vector3.up, _rotationSpeed * Time.fixedDeltaTime);
                break;
        }
    }

    public void SetMovementState(MovementState state)
    {
        currentState = state;
    }

    void SetComponents()
    {
        controller = GetComponentInChildren<CharacterController>();
        playerControllerTransform = controller.transform;
        
        if (controller == null)
            Debug.LogError("No CharacterController found on Player.");
    }
}

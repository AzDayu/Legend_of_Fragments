using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool SprintPressed { get; private set; }
    public bool ToggleCameraTriggered { get; private set; }
    public bool FreeCamPressed { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool WeaponPressed { get; private set; }

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction toggleAction;
    private InputAction lookAction;
    private InputAction freeCamAction;
    private InputAction AttackAction;
    private InputAction WeaponAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
        toggleAction = playerInput.actions["Next"];
        lookAction = playerInput.actions["Look"];
        freeCamAction = playerInput.actions["FreeCam"];
        AttackAction = playerInput.actions["Attack"];
        WeaponAction = playerInput.actions["Weapon"];
    }

    void Update()
    {
        MoveInput = moveAction.ReadValue<Vector2>();
        LookInput = lookAction.ReadValue<Vector2>();

        JumpTriggered = jumpAction.WasPressedThisFrame();
        SprintPressed = sprintAction.IsPressed();
        ToggleCameraTriggered = toggleAction.WasPressedThisFrame();
        FreeCamPressed = freeCamAction.IsPressed();
        AttackPressed = AttackAction.IsPressed();
        WeaponPressed = WeaponAction.WasPressedThisFrame();
    }
}

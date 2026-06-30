using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 150.0f;
    public float jumpForce = 5.0f;
    public int maxJumpCount = 2;

    [Header("Ground Check Settings")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Obstacle Settings")]
    public float wallCheckDistance = 0.5f;
    public LayerMask wallLayer;

    private Rigidbody rigidBody;
    private Animator anim;

    private PlayerInputHandler inputHandler;
    private PlayerCamera playerCamera;
    private PlayerStats playerStats;
    private PlayerCombat playerCombat;

    private int addJump;
    private bool isGrounded;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        inputHandler = GetComponent<PlayerInputHandler>();
        playerCamera = GetComponent<PlayerCamera>();
        playerStats = GetComponent<PlayerStats>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    void Start()
    {
        rigidBody.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (inputHandler.ToggleCameraTriggered)
        {
            playerCamera.ToggleCamera();
        }

        if (inputHandler.JumpTriggered && addJump > 0)
        {
            PerformJump();
        }

        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance, groundLayer);
        anim.SetBool("isGrounded", isGrounded);

        if (isGrounded && rigidBody.linearVelocity.y <= 0.01f)
        {
            addJump = maxJumpCount - 1;
            anim.SetTrigger("doLanding");
        }

        playerCamera.UpdateCameraPivotPosition(transform.position);
    }

    void FixedUpdate()
    {
        if (inputHandler.FreeCamPressed)
        {
            rigidBody.linearVelocity = new Vector3(0, rigidBody.linearVelocity.y, 0);
            anim.SetBool("isWalking", false);
            playerCamera.HandleLook(inputHandler.LookInput, transform);
            return;
        }

        if (inputHandler.FreeCamPressed)
        {
            playerCamera.HandleLook(inputHandler.LookInput, transform);
        }

        if (playerCombat != null && playerCombat.isAttacking)
        {
            rigidBody.linearVelocity = new Vector3(0, rigidBody.linearVelocity.y, 0);
            anim.SetBool("isWalking", false);
            return;
        }

        Vector2 input = inputHandler.MoveInput;
        Transform camTransform = playerCamera.GetActiveCameraTransform();

        Vector3 forward = camTransform.forward;
        Vector3 right = camTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (forward * input.y + right * input.x).normalized;

        bool isLanding = anim.GetCurrentAnimatorStateInfo(0).IsTag("Landing");

        if (anim.GetBool("isGrounded") && isLanding)
        {
            rigidBody.linearVelocity = new Vector3(0, rigidBody.linearVelocity.y, 0);
            anim.SetBool("isWalking", false);
        }
        else
        {
            bool isBlocked = false;
            if (moveDir.magnitude > 0.1f)
            {
                isBlocked = Physics.Raycast(transform.position + Vector3.up * 0.2f, moveDir, wallCheckDistance, wallLayer);
            }

            if (inputHandler.SprintPressed && playerStats.stamina > 0 && moveDir.magnitude > 0.1f)
            {
                moveSpeed = 10.0f;
                playerStats.UseStamina(0.5f);
            }
            else
            {
                moveSpeed = 5.0f;
                playerStats.RecoverStamina(0.3f);
            }

            anim.SetFloat("speed", moveSpeed);

            if (isBlocked)
            {
                rigidBody.linearVelocity = new Vector3(0, rigidBody.linearVelocity.y, 0);
                anim.SetBool("isWalking", false);
            }
            else
            {
                rigidBody.linearVelocity = new Vector3(moveDir.x * moveSpeed, rigidBody.linearVelocity.y, moveDir.z * moveSpeed);

                if (moveDir.magnitude > 0.1f)
                {
                    anim.SetBool("isWalking", true);

                    if (playerCamera.isFirstPerson)
                    {
                        Quaternion targetRot = Quaternion.LookRotation(forward);
                        rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, targetRot, rotateSpeed * Time.fixedDeltaTime));
                    }
                    else
                    {
                        Quaternion targetRot = Quaternion.LookRotation(moveDir);
                        rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, targetRot, rotateSpeed * Time.fixedDeltaTime));
                    }
                }
                else
                {
                    anim.SetBool("isWalking", false);
                }
            }
        }

        float vely = isGrounded ? 0f : rigidBody.linearVelocity.y;
        anim.SetFloat("yVelocity", vely);

    }

    void PerformJump()
    {
        anim.SetTrigger("doJump");

        rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, 0, rigidBody.linearVelocity.z);
        rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        addJump--;
        isGrounded = false;
        anim.SetBool("isGrounded", false);
    }

}
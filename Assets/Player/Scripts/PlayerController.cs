using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerSoundType
{
    None = 0,
    Walk,
    Sprint,
    Landing
}

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 150.0f;
    public float jumpForce = 5.0f;
    public int maxJumpCount = 2;
    public float maxStemina = 150.0f;
    public float stemina;

    [Header("Ground Check Settings")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Obstacle Settings")]
    public float wallCheckDistance = 0.5f;
    public LayerMask wallLayer;

    [Header("Camera Settings")]
    public CinemachineCamera firstPersonCam;
    public CinemachineCamera thirdPersonCam;
    public Transform cameraPivot;

    [Header("Audio Settings")]
    public AudioClip walkSound;
    public AudioClip sprintSound;
    public AudioClip landSound;
    public float sound = 0.6f;
    private AudioSource audioSource;

    public float mouseSensitivity = 100f;
    private float xRotation = 0f;
    private float yRotation = 0f;

    private bool isFirstPerson = false;

    private Rigidbody rigidBody;
    private Animator anim;
    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction toggleAction;
    private InputAction lookAction;
    private InputAction freeCamAction;

    private int addJump;
    private bool isGrounded;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
        toggleAction = playerInput.actions["Next"];
        lookAction = playerInput.actions["Look"];
        freeCamAction = playerInput.actions["FreeCam"];

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        rigidBody.freezeRotation = true;
        stemina = maxStemina;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (jumpAction.WasPressedThisFrame() && addJump > 0)
        {
            PerformJump();
        }

        if (toggleAction.WasPressedThisFrame())
        {
            ToggleCamera();
        }

        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance, groundLayer);

        anim.SetBool("isGrounded", isGrounded);

        if (isGrounded && !wasGrounded && rigidBody.linearVelocity.y <= 0.01f)
        {
            addJump = maxJumpCount - 1;
            anim.SetTrigger("doLanding");
        }
        cameraPivot.position = transform.position;

    }

    void HandleLook()
    {
        Vector2 look = lookAction.ReadValue<Vector2>();

        float mouseX = look.x * mouseSensitivity * Time.deltaTime;
        float mouseY = look.y * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -70f, 70f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        if (isFirstPerson)
        {
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
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

    void FixedUpdate()
    {
        if (freeCamAction.IsPressed())
        {
            rigidBody.linearVelocity = new Vector3(0, rigidBody.linearVelocity.y, 0);
            anim.SetBool("isWalking", false);
            HandleLook();
        }
        cameraPivot.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        Vector2 input = moveAction.ReadValue<Vector2>();
        float h = input.x;
        float v = input.y;

        Transform camTransform = isFirstPerson ? firstPersonCam.transform : thirdPersonCam.transform;

        Vector3 forward = camTransform.forward;
        Vector3 right = camTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (forward * v + right * h).normalized;

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

            if (sprintAction.IsPressed() && stemina > 0 && moveDir.magnitude > 0.1f)
            {
                moveSpeed = 10.0f;
                stemina -= 0.5f;
            }
            else
            {
                moveSpeed = 5.0f;
                if (stemina < maxStemina) stemina += 0.3f;
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

                    if (isFirstPerson)
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

    void ToggleCamera()
    {
        isFirstPerson = !isFirstPerson;
        firstPersonCam.Priority = isFirstPerson ? 20 : 10;
        thirdPersonCam.Priority = isFirstPerson ? 10 : 20;
    }

    PlayerSoundType currentType;
    public void PlayPlayerSound(PlayerSoundType type)
    {
        if (audioSource == null) return;

        // 타입 바뀌면 기존 사운드 끊기
        if (currentType != type)
        {
            audioSource.Stop();
            currentType = type;
        }

        AudioClip clip = null;

        switch (type)
        {
            case PlayerSoundType.Walk: clip = walkSound; break;
            case PlayerSoundType.Sprint: clip = sprintSound; break;
            case PlayerSoundType.Landing: clip = landSound; break;
        }

        audioSource.PlayOneShot(clip, sound);
    }

    public void PlayerSoundAllStop()
    {
        audioSource.Stop();
    }

}
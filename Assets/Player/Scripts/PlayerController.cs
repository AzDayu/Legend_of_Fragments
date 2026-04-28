using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //이동 관련 조절 부분
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 150.0f;
    public float jumpForce = 5.0f;
    public int maxJumpCount = 2;
    public float maxStemina = 150.0f;
    public float stemina;

    //착지 확인 부분
    [Header("Ground Check Settings")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    // 벽을 감지 부분
    [Header("Obstacle Settings")]
    public float wallCheckDistance = 0.5f; 
    public LayerMask wallLayer;

    // 사운드 부분
    [Header("Audio Settings")]
    public AudioClip walkSound;
    public AudioClip sprintSound;
    public AudioClip landSound;
    public float sound = 0.6f;
    private AudioSource audioSource;

    private Rigidbody rigidBody;
    private Animator anim;
    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction toggleAction;

    private int addJump;
    private bool isGrounded;

    public CinemachineCamera firstPersonCam;
    public CinemachineCamera thirdPersonCam;
    private bool isFirstPerson = false;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
        toggleAction = playerInput.actions["Next"];

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

    }

    void PerformJump()
    {
        anim.SetTrigger("doJump");

       // if (audioSource != null && audioSource.isPlaying)
       // {
       //     audioSource.Stop();
       // }

        rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, 0, rigidBody.linearVelocity.z);
        rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        addJump--;
        isGrounded = false;
        anim.SetBool("isGrounded", false);
    }

    void FixedUpdate()
    {
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

    public void PlayFootstep(string type)
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (type == "Walk")
        {
            audioSource.PlayOneShot(walkSound, sound);
        }
        else if (type == "Sprint")
        {
            audioSource.PlayOneShot(sprintSound, sound);
        }
        else if (type == "Landing")
        {
            audioSource.PlayOneShot(landSound, sound);
        }
    }
}
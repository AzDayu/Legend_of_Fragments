using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //이동 속도 및 점프 높이 조절 부분
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 150.0f;
    public float jumpForce = 5.0f;

    //스테미나 조절 부분
    [Header("Stamina Settings")]
    public float maxStemina = 150.0f;
    public float stemina;

    private Rigidbody rigidBody;
    private Animator anim;
    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction toggleAction;

    public int maxJumpCount = 2;
    private int remainJumpCount;
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
        if (jumpAction.WasPressedThisFrame() && remainJumpCount > 0)
        {
            PerformJump();
        }

        if (toggleAction.WasPressedThisFrame())
        {
            ToggleCamera();
        }
    }

    void PerformJump()
    {
        anim.ResetTrigger("doJump");
        anim.SetTrigger("doJump");

        rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, 0, rigidBody.linearVelocity.z);
        rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        remainJumpCount--;
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
        }
        else
        {
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

        float vely = isGrounded ? 0f : rigidBody.linearVelocity.y;
        anim.SetFloat("yVelocity", vely);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
            remainJumpCount = maxJumpCount;
            anim.SetBool("isGrounded", true);
            anim.SetTrigger("doLanding");
        }
    }

    void ToggleCamera()
    {
        isFirstPerson = !isFirstPerson;
        firstPersonCam.Priority = isFirstPerson ? 20 : 10;
        thirdPersonCam.Priority = isFirstPerson ? 10 : 20;
    }
}
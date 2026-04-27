using UnityEngine;
using UnityEngine.InputSystem;

public class MyPlayer : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 150.0f;
    public float jumpForce = 5.0f;
    public float maxStemina = 150.0f;
    public float stemina;

    private Rigidbody rigidBody;
    private Animator anim;

    public int maxJumpCount = 2;
    private int remainJumpCount;
    private bool isGrounded;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        rigidBody.freezeRotation = true;
        stemina = maxStemina;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && remainJumpCount > 0)
        {
            PerformJump();
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
        Debug.Log("현재 스테미나 :" + stemina);
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = new Vector3(h, 0, v).normalized;

        bool isLanding = anim.GetCurrentAnimatorStateInfo(0).IsTag("Landing");

        if (anim.GetBool("isGrounded") && isLanding)
        {
            rigidBody.linearVelocity = new Vector3(0, rigidBody.linearVelocity.y, 0);
            anim.SetBool("isWalking", false);
            
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                moveSpeed = 10.0f;
                Debug.Log("LeftShift 눌림 :");

            }

            anim.SetFloat("speed", moveSpeed);
            rigidBody.linearVelocity = new Vector3(moveDir.x * moveSpeed, rigidBody.linearVelocity.y, moveDir.z * moveSpeed);

            if (moveDir.magnitude > 0.1f)
            {
                anim.SetBool("isWalking", true);
                Quaternion newRotation = Quaternion.LookRotation(moveDir);
                rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, newRotation, rotateSpeed * Time.fixedDeltaTime));

                if (stemina > 0 && moveSpeed >= 10) stemina--;
                else moveSpeed = 5.0f;
            }
            else
            {
                anim.SetBool("isWalking", false);
                if (stemina < maxStemina) stemina++;
            }
        }

        float vely = isGrounded ? 0f : rigidBody.linearVelocity.y;
        anim.SetFloat("yVelocity", vely);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, 0, rigidBody.linearVelocity.z);

            isGrounded = true;
            remainJumpCount = maxJumpCount;
            anim.ResetTrigger("doJump");
            anim.SetBool("isGrounded", true);
            anim.SetTrigger("doLanding");
        }
    }
}
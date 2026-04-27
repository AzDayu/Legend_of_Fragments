using UnityEngine;

public class MyPlayer : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 150.0f; 
    private Rigidbody rigidBody;
    private Animator anim;
    public float jumpForce = 5.0f;
    private bool isGrounded = true;
    bool jumpRequested = false;


    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        rigidBody.freezeRotation = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequested = true;
            anim.SetBool("IsJumping", true);
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(h, 0, v).normalized * moveSpeed;

        rigidBody.linearVelocity = new Vector3(moveDir.x, rigidBody.linearVelocity.y, moveDir.z);

        if (moveDir.magnitude > 0.1f)
        {
            anim.SetBool("isWalking", true);

            Quaternion newRotation = Quaternion.LookRotation(moveDir);
            rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, newRotation, rotateSpeed * Time.fixedDeltaTime));
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

        if (jumpRequested)
        {
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            jumpRequested = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.name.Contains("Plane"))
        {
            isGrounded = true;
            Debug.Log("바닥 착지 성공!");
            anim.SetBool("IsJumping", false);
        }
    }
}


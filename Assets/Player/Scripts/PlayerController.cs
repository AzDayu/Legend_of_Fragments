using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Vector3 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1. 키보드 입력 받기
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // 2. 이동 방향 설정
        moveInput = new Vector3(x, 0, z).normalized;

        // 디버그: 입력이 들어오는지 콘솔에 표시
        if (moveInput.magnitude > 0) Debug.Log("달리는 중!");
    }

    void FixedUpdate()
    {
        // 3. 물리 엔진으로 이동 (이 방식이 가장 확실합니다)
        rb.linearVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, moveInput.z * moveSpeed);
    }
}
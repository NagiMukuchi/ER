using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float speed;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;
    private Vector3 startPosition;

    // Các hash animation để tránh việc tạo string nhiều lần
    static int s_DeadHash = Animator.StringToHash("Dead");
    static int s_RunStartHash = Animator.StringToHash("runStart");
    static int s_MovingHash = Animator.StringToHash("Moving");
    static int s_JumpingHash = Animator.StringToHash("Jumping");
    static int s_JumpingSpeedHash = Animator.StringToHash("JumpSpeed");
    static int s_SlidingHash = Animator.StringToHash("Sliding");

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    void Update()
    {
        // Kiểm tra nhân vật có đang chạm đất không
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        // Nhảy
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // reset trục Y
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Gọi animation "Jumping" khi nhảy
            animator.SetBool(s_JumpingHash, true);
        }

        // Nếu nhân vật đã chạm đất, dừng animation "Jumping"
        if (isGrounded)
        {
            animator.SetBool(s_JumpingHash, false);
        }

        // Chặn rơi mãi
        if (transform.position.y < -20f)
        {
            Respawn();
        }
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Tính toán tốc độ hiện tại
        float speed = GetCurrentSpeed();

        if (direction.magnitude >= 0.1f)
        {
            // Nếu tốc độ >= 1, kích hoạt animation "runStart"
            if (speed >= 1f)
            {
                animator.SetBool(s_RunStartHash, true);  // Bật animation "runStart"
            }

            // Chuyển sang animation "Moving"
            animator.SetBool(s_MovingHash, true);

            // Xoay nhân vật về hướng di chuyển
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            // Di chuyển nhân vật
            Vector3 move = direction * moveSpeed;
            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
        }
        else
        {
            // Dừng animation "Moving" và "runStart" khi không di chuyển
            animator.SetBool(s_RunStartHash, false);
            animator.SetBool(s_MovingHash, false);

            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }
    }

    float GetCurrentSpeed()
    {
        // Tính toán và trả về tốc độ hiện tại (bỏ qua trục Y)
        return new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
    }

    void Respawn()
    {
        transform.position = startPosition + Vector3.up * 2f;
        rb.velocity = Vector3.zero;
    }

    // Hiển thị hình tròn kiểm tra Ground trong Scene
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float startMoveSpeed = 0f;   // tốc độ lúc idle
    public float moveSpeed;
    public float maxMoveSpeed = 15f;    // tốc độ tối đa
    public float accelerationRate = 2f; // tốc độ tăng tốc

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    [Range(0f, 1f)]
    public float jumpSpeedReduceFactor = 0.5f; // khi CHẠM ĐẤT mới giảm còn % này
    public int maxJumps = 2;           // ✅ Số lần nhảy tối đa (1 = nhảy thường, 2 = double jump)

    private int jumpCount = 0;         // ✅ Đếm số lần nhảy đã thực hiện

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundLayer;

    [Header("Lane Settings")]
    public float laneDistance = 3f;      // khoảng cách giữa các lane
    private int currentLane = 0;         // 0 = giữa, -1 = trái, +1 = phải
    public float laneChangeSpeed = 10f;  // tốc độ chuyển lane

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;
    private bool isStartingRun = false;
    private Vector3 startPosition;

    // Animation hash
    static int s_RunStartHash = Animator.StringToHash("runStart");
    static int s_MovingHash = Animator.StringToHash("Moving");
    static int s_JumpingHash = Animator.StringToHash("Jumping");

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();
        startPosition = transform.position;

        moveSpeed = startMoveSpeed;

        animator.SetBool(s_MovingHash, false);
        animator.SetBool(s_RunStartHash, false);

        // ✅ Ban đầu quay mặt về phía camera (Idle nhìn ra màn hình)
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    void Update()
    {
        // Kiểm tra chạm đất
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        // Reset khi chạm đất
        if (isGrounded && rb.velocity.y <= 0.01f)
        {
            if (jumpCount > 0)   // vừa rơi xuống xong
            {
                ApplyLandingSlowdown();
            }

            jumpCount = 0;
            animator.SetBool(s_JumpingHash, false);
            animator.speed = AnimSpeedFromMove();
        }

        // Bắt đầu chạy từ Idle sang runStart
        if (!isStartingRun && Input.anyKeyDown)
        {
            isStartingRun = true;
            animator.SetBool(s_RunStartHash, true);

            // ✅ Khi bắt đầu chạy thì quay lại hướng chạy
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        // Tăng tốc chỉ khi đang chạy và không quá max
        if (isStartingRun && moveSpeed < maxMoveSpeed)
        {
            moveSpeed += accelerationRate * Time.deltaTime;
            if (moveSpeed > maxMoveSpeed) moveSpeed = maxMoveSpeed;
        }

        // ✅ Nhảy (cho phép nhảy nếu chưa vượt maxJumps)
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            jumpCount++;
            animator.SetBool(s_JumpingHash, true);

            // 🛑 DỪNG animation trong lúc nhảy
            animator.speed = 0f;
        }

        // ✅ Điều khiển lane bằng A/D hoặc mũi tên
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentLane > -1) currentLane--;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentLane < 1) currentLane++;
        }

        if (transform.position.y < -20f)
        {
            Respawn();
        }
    }

    void FixedUpdate()
    {
        float forwardSpeed = isStartingRun ? moveSpeed : 0f;

        // Tính X mục tiêu dựa trên lane
        float targetX = currentLane * laneDistance;
        float newX = Mathf.Lerp(transform.position.x, targetX, Time.fixedDeltaTime * laneChangeSpeed);

        // Duy trì tốc độ chạy + trọng lực
        rb.velocity = new Vector3(0f, rb.velocity.y, forwardSpeed);

        // Dịch dần sang lane
        rb.position = new Vector3(newX, rb.position.y, rb.position.z);

        animator.SetBool(s_MovingHash, isStartingRun);
    }

    void ApplyLandingSlowdown()
    {
        moveSpeed *= jumpSpeedReduceFactor;
        if (moveSpeed < 1f) moveSpeed = 1f;
    }

    float AnimSpeedFromMove()
    {
        return Mathf.Lerp(1f, 1.5f, moveSpeed / maxMoveSpeed);
    }

    void Respawn()
    {
        transform.position = startPosition + Vector3.up * 2f;
        rb.velocity = Vector3.zero;

        moveSpeed = startMoveSpeed;
        isStartingRun = false;
        jumpCount = 0;

        animator.speed = 1f;
        animator.SetBool(s_RunStartHash, false);
        animator.SetBool(s_MovingHash, false);
        animator.SetBool(s_JumpingHash, false);

        // ✅ Quay lại Idle nhìn ra màn hình
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        // Reset lane
        currentLane = 0;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }

    public bool IsRunning()
    {
        return isStartingRun;
    }
}

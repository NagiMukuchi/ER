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
    public float sideMoveSpeed = 5f;    // tốc độ di chuyển ngang
    private float currentSideSpeed;     // tốc độ ngang hiện tại

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    [Range(0f, 1f)]
    public float jumpSpeedReduceFactor = 0.5f; // khi CHẠM ĐẤT mới giảm còn % này

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;
    private bool isStartingRun = false;
    private bool isJumping = false;     // true: đang ở trên không
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
        currentSideSpeed = sideMoveSpeed;

        animator.SetBool(s_MovingHash, false);
        animator.SetBool(s_RunStartHash, false);

        // ✅ Ban đầu quay mặt về phía camera (Idle nhìn ra màn hình)
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    void Update()
    {
        // Kiểm tra chạm đất
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        // Bắt đầu chạy từ Idle sang runStart
        if (!isStartingRun && Input.anyKeyDown)
        {
            isStartingRun = true;
            animator.SetBool(s_RunStartHash, true);

            // ✅ Khi bắt đầu chạy thì quay lại hướng chạy
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        // Tăng tốc chỉ khi đang chạy, ĐANG ở mặt đất và không trong trạng thái nhảy
        if (isStartingRun && !isJumping && moveSpeed < maxMoveSpeed)
        {
            moveSpeed += accelerationRate * Time.deltaTime;
            if (moveSpeed > maxMoveSpeed) moveSpeed = maxMoveSpeed;
        }

        // Nhảy (chỉ khi đang chạm đất)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            isJumping = true;
            animator.SetBool(s_JumpingHash, true);

            // 🛑 DỪNG animation trong lúc đang nhảy
            animator.speed = 0f;
        }

        // Vừa CHẠM ĐẤT sau khi nhảy
        if (isGrounded && isJumping)
        {
            isJumping = false;
            animator.SetBool(s_JumpingHash, false);

            ApplyLandingSlowdown();

            animator.speed = AnimSpeedFromMove();
        }

        if (!isJumping)
        {
            animator.speed = AnimSpeedFromMove();
        }

        if (transform.position.y < -20f)
        {
            Respawn();
        }
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        float forwardSpeed = isStartingRun ? moveSpeed : 0f;
        Vector3 velocity = new Vector3(horizontal * currentSideSpeed, rb.velocity.y, forwardSpeed);
        rb.velocity = velocity;

        animator.SetBool(s_MovingHash, isStartingRun);

        // ❌ Không cần ép luôn nhìn thẳng ở đây nữa
        // transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void ApplyLandingSlowdown()
    {
        moveSpeed *= jumpSpeedReduceFactor;
        if (moveSpeed < 1f) moveSpeed = 1f;

        currentSideSpeed = sideMoveSpeed * jumpSpeedReduceFactor;
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
        currentSideSpeed = sideMoveSpeed;
        isStartingRun = false;
        isJumping = false;

        animator.speed = 1f;
        animator.SetBool(s_RunStartHash, false);
        animator.SetBool(s_MovingHash, false);
        animator.SetBool(s_JumpingHash, false);

        // ✅ Quay lại Idle nhìn ra màn hình
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }

    public bool IsRunning()
    {
        return isStartingRun; // hoặc animator.GetBool(s_MovingHash);
    }
}

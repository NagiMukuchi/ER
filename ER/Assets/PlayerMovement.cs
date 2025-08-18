using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float startMoveSpeed = 0f;         // bắt đầu từ 0 để Idle
    public float moveSpeed;
    public float targetMoveSpeed = 5f;        // tốc độ sau khi chạy
    public float maxMoveSpeed = 15f;
    public float accelerationRate = 0.2f;
    public float jumpForce = 7f;
    public float sideMoveSpeed = 5f;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundLayer;

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
    }

    void Update()
    {
        // Tăng tốc dần khi đã bắt đầu chạy
        if (isStartingRun && moveSpeed < maxMoveSpeed)
        {
            moveSpeed += accelerationRate * Time.deltaTime;
        }

        // Bắt đầu chạy từ Idle sang runStart
        if (!isStartingRun && Input.anyKeyDown) // hoặc bạn có thể thay bằng auto-start
        {
            isStartingRun = true;
            animator.SetBool(s_RunStartHash, true);
            moveSpeed = targetMoveSpeed; // bắt đầu chạy với tốc độ mục tiêu
        }

        // Điều chỉnh tốc độ animation
        float animSpeedMultiplier = Mathf.Max(moveSpeed / targetMoveSpeed, 0.1f);
        animator.speed = animSpeedMultiplier;

        // Kiểm tra chạm đất
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        // Nhảy
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool(s_JumpingHash, true);
        }

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

        // Luôn chạy thẳng nếu đã bắt đầu
        float forwardSpeed = isStartingRun ? moveSpeed : 0f;
        Vector3 direction = new Vector3(horizontal * sideMoveSpeed, rb.velocity.y, forwardSpeed);
        rb.velocity = direction;

        // Chuyển animation chạy
        if (isStartingRun)
        {
            animator.SetBool(s_MovingHash, true);
        }
        else
        {
            animator.SetBool(s_MovingHash, false);
        }

        // Nhân vật luôn nhìn thẳng
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void Respawn()
    {
        transform.position = startPosition + Vector3.up * 2f;
        rb.velocity = Vector3.zero;

        moveSpeed = startMoveSpeed;
        isStartingRun = false;

        animator.speed = 1f;
        animator.SetBool(s_RunStartHash, false);
        animator.SetBool(s_MovingHash, false);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
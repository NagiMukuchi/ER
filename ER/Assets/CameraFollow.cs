using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;                // Nhân vật

    [Header("Camera Offsets")]
    public Vector3 idleOffset = new Vector3(0f, 3f, 5f);   // Nhìn từ trước (Idle)
    public Vector3 runOffset = new Vector3(0f, 5f, -7f);  // Nhìn từ sau (Run)

    [Header("Smooth Settings")]
    public float positionSmoothTime = 0.15f; // Thời gian làm mượt vị trí
    public float rotationSmoothSpeed = 5f;   // Tốc độ làm mượt xoay
    public float offsetLerpSpeed = 2f;       // Tốc độ chuyển offset

    private Vector3 velocity = Vector3.zero;
    private Vector3 currentOffset;
    private bool isRunning = false;

    void Start()
    {
        // Ban đầu camera nhìn từ trước
        currentOffset = idleOffset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Kiểm tra trạng thái chạy của Player (PlayerMovement script)
        PlayerMovement pm = target.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            if (pm.IsRunning()) // ✅ Hàm public kiểm tra chạy
                isRunning = true;
            else
                isRunning = false;
        }

        // Chuyển offset dần dần (Idle ↔ Run)
        Vector3 desiredOffset = isRunning ? runOffset : idleOffset;
        currentOffset = Vector3.Lerp(currentOffset, desiredOffset, offsetLerpSpeed * Time.deltaTime);

        // --- Làm mượt vị trí ---
        Vector3 desiredPosition = target.position + target.rotation * currentOffset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            positionSmoothTime
        );
        transform.position = smoothedPosition;

        // --- Làm mượt xoay ---
        Vector3 lookDirection = target.position - transform.position;
        Quaternion desiredRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotationSmoothSpeed * Time.deltaTime
        );
    }
}

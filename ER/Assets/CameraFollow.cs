using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow_Subway : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;                // Nhân vật

    [Header("Camera Offset")]
    public Vector3 offset = new Vector3(0f, 7f, -10f); // Vị trí camera so với player

    [Header("Smooth Settings")]
    public float positionSmoothTime = 0.15f; // Thời gian làm mượt vị trí
    public float rotationSmoothSpeed = 5f;   // Tốc độ làm mượt xoay

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // --- Làm mượt vị trí ---
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            positionSmoothTime
        );
        transform.position = smoothedPosition;

        // --- Camera luôn nhìn thẳng về player ---
        Vector3 lookDirection = target.position - transform.position;
        lookDirection.y = 0; // Khóa trục Y để tránh rung lắc dọc
        Quaternion desiredRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotationSmoothSpeed * Time.deltaTime
        );
    }
}

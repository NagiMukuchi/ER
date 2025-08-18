using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;                // Nhân vật

    [Header("Camera Offset")]
    public Vector3 offset = new Vector3(0f, 5f, -7f);

    [Header("Smooth Settings")]
    public float positionSmoothTime = 0.15f; // Thời gian làm mượt vị trí
    public float rotationSmoothSpeed = 5f;   // Tốc độ làm mượt xoay

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // --- Làm mượt vị trí ---
        Vector3 desiredPosition = target.position + target.rotation * offset;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;           // Nhân vật
    public Vector3 offset = new Vector3(0f, 5f, -7f); // Khoảng cách camera so với nhân vật
    public float smoothSpeed = 10f;    // Độ mượt

    void LateUpdate()
    {
        // Tính vị trí mong muốn của camera
        Vector3 desiredPosition = target.position + target.rotation * offset;

        // Di chuyển camera mượt tới vị trí đó
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Camera luôn nhìn về nhân vật
        transform.LookAt(target);
    }
}

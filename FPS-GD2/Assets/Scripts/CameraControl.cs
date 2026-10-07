using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform camTransform;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float camMove = camTransform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0f, camMove, 0f);
    }
}

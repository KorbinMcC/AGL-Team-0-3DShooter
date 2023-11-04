using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderCameraFollow : MonoBehaviour
{
    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderCameraFollow : MonoBehaviour
{
    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        //transform.forward = Camera.main.transform.forward;
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}

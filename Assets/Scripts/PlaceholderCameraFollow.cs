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
        Vector3 lookDir = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
        //transform.rotation = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), 100f);
    }
}

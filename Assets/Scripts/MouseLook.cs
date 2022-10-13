using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    public float mouseSensitivity = 100f;
    public Transform planeBody;
    public Transform cameraLook;
    public Transform playerBody;

    float xRotation = 0f;
    float yRotation = 0f;

    #region FirstPerson/ThirdPerson
    private bool firstPerson, thirdPerson;
    #endregion
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        #region Start in FirstPerson
        transform.position.Set(0f, .0167f, -0.05f);
        firstPerson = true;
        thirdPerson = false;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region FirstPerson/ThirdPerson Swap
        //Go to FirstPerson
        if(Input.GetKeyDown(KeyCode.V) && thirdPerson)
        {
            transform.position.Set(0f, .0167f, -0.05f);
            firstPerson = true;
            thirdPerson = false;
        }
        //Go to ThirdPerson
        if(Input.GetKeyDown(KeyCode.V) && firstPerson)
        {
            transform.position.Set(0f, .5f, -1f);
            thirdPerson = true;
            firstPerson = false;
        }
        #endregion
        #region Camera Movement
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (Input.GetKey(KeyCode.C))
        {
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            playerBody.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.transform.Rotate(Vector3.up * mouseX);
        }
        else
        {
            xRotation -= mouseY;
            //xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            yRotation += mouseX;

            planeBody.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
            planeBody.Rotate(Vector3.up * mouseX);
            planeBody.Rotate(Vector3.right * mouseY);

        }
        #endregion

    }
}

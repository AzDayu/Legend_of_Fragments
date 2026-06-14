using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public CinemachineCamera firstPersonCam;
    public CinemachineCamera thirdPersonCam;
    public Transform cameraPivot;
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    public bool isFirstPerson { get; private set; } = false;

    public void HandleLook(Vector2 lookInput, Transform playerTransform)
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.fixedDeltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.fixedDeltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -70f, 70f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        if (isFirstPerson)
        {
            playerTransform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }

    public void UpdateCameraPivotPosition(Vector3 position)
    {
        cameraPivot.position = position;
    }

    public void ToggleCamera()
    {
        isFirstPerson = !isFirstPerson;
        firstPersonCam.Priority = isFirstPerson ? 20 : 10;
        thirdPersonCam.Priority = isFirstPerson ? 10 : 20;
    }

    public Transform GetActiveCameraTransform()
    {
        return isFirstPerson ? firstPersonCam.transform : thirdPersonCam.transform;
    }
}

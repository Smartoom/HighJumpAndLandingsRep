using UnityEngine;

public class PlayerLookingScript : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private float mouseSensitivityX = 1;
    [SerializeField] private float mouseSensitivityY = 1;
    private float mouseX, mouseY;

    private float xRot, yRot;

    [Header("References")]
    [SerializeField] private Transform camRotationTransform;
    [SerializeField] private Transform Orientation;

    private void OnEnable()
    {
        xRot = camRotationTransform.rotation.eulerAngles.x;
        yRot = camRotationTransform.rotation.eulerAngles.y;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        xRot += -mouseY * mouseSensitivityX * Time.deltaTime;
        xRot = Mathf.Clamp(xRot, -90, 90);
        yRot += mouseX * mouseSensitivityY * Time.deltaTime;
        camRotationTransform.rotation = Quaternion.Euler(xRot, yRot, 0);
        Orientation.rotation = Quaternion.Euler(0, yRot, 0);
    }
}

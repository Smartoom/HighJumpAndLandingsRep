using UnityEngine;

public class PlayerCameraLookingScript : MonoBehaviour
{
    [SerializeField] private Vector3 headCameraOffset;
    [SerializeField] private Transform camPosition;
    private void LateUpdate()
    {
        camPosition.position = transform.position + headCameraOffset;
    }
}

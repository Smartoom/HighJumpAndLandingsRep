using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    public static MiniMapCamera instance;
    [SerializeField] private Camera cam;
    [SerializeField] private Vector3 offset;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            Debug.Log("!!!!!!!!!!!!!!!!######!!!!!!!!!!!!");
            return;
        }
        instance = this;
    }

    public void SetFollowTarget(Transform newTarget)
    {
        if (newTarget != null)
            transform.position = newTarget.position + offset;
        transform.parent = newTarget;
    }
    /// <summary>
    /// for zooming in and out
    /// </summary>
    /// <param name="zoom"></param>
    public void SetCamSize(float camSize)
    {
        cam.orthographicSize = camSize;
    }
}

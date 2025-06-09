using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    public static MiniMapCamera instance;
    [SerializeField] private Camera cam;


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

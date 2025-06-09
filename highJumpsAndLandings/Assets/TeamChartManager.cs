using UnityEngine;

public class TeamChartManager : MonoBehaviour
{
    //less a manger and more of an input enabler. ....
    [SerializeField] KeyCode teamChartKey = KeyCode.Tab;
    [SerializeField] float normalSize = 50;
    [SerializeField] float zoomedOutSize = 100;
    void Update()
    {
        bool keyPressed = Input.GetKey(teamChartKey);
        CanvasReferenceManager.instance.teamUIChart.SetActive(keyPressed);

        if (keyPressed)
            MiniMapCamera.instance.SetCamSize(zoomedOutSize);//maybe use a ? thing idk
        else
            MiniMapCamera.instance.SetCamSize(normalSize);
    }
}

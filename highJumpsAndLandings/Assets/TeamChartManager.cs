using UnityEngine;

public class TeamChartManager : MonoBehaviour
{
    //less a manger and more of an input enabler 
    [SerializeField] KeyCode teamChartKey = KeyCode.Tab;
    void Update()
    {
        CanvasReferenceManager.instance.teamUIChart.SetActive(Input.GetKey(teamChartKey));
    }
}

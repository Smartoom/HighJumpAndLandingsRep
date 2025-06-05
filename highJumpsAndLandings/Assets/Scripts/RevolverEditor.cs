using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Revolver))]
public class RevolverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Revolver revolver = (Revolver)target;

        /*revolver.SetTimeSinceLastShot(GUILayout.HorizontalSlider(revolver.GetTimeSinceLastShot(), 0, 2));*/
        //GUILayout.Space(20);
        float b = revolver.GetCrosshairOffsetCurve().Evaluate(revolver.GetTimeSinceLastShot());
        CanvasReferenceManager canvasInstance = GameObject.Find("ScreenSpaceCanvas").GetComponent<CanvasReferenceManager>();
        for (int i = 0; i < canvasInstance.plusCrosshairBars.Length; i++)
        {
            canvasInstance.plusCrosshairBars[i].localPosition = Vector3.right * b;
        }
    }
}

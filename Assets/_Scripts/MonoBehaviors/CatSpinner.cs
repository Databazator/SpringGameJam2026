using DG.Tweening;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CatSpinner))]
public class CatSpinnerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var tgt = target as CatSpinner;
        if (GUILayout.Button("Start Spinning"))
        {
            tgt.StartSpin();
        }
        if (GUILayout.Button("Stop Spinning"))
        {
            tgt.StopSpin();
        }
    }
}
#endif

public class CatSpinner : MonoBehaviour
{
    public Transform CatVisualPivot; 

    public float SpinSide;
    public float SpinDuration;
    public int SpinRevolutions;
    public float RiseMult;
    public int RiseTimes;
    bool spinActive = true;
    float spinTimer = 0f;


    private void Start()
    {
        StartSpin();
    }
    public void StartSpin()
    {
        spinActive = true;
    }

    public void StopSpin()
    {
        spinActive = false;
    }

    private void Update()
    {
        if(spinTimer <= SpinDuration)
        {
            spinTimer += Time.deltaTime;

            float currFactor = spinTimer / SpinDuration;
            float currRot = (currFactor * SpinRevolutions * 360f * Mathf.Sign(SpinSide));
            

            float currRise = Mathf.Sin(currFactor * RiseTimes * 4 * Mathf.PI) * RiseMult;
            Vector3 currentPos = CatVisualPivot.position + Vector3.up * currRise;

            CatVisualPivot.rotation = Quaternion.Euler(0f, currRot, 0f);
            CatVisualPivot.position = currentPos;
        }
        else
        {
            if(spinActive)
            {
                spinTimer = 0f;
                CatVisualPivot.rotation = Quaternion.identity;
            }
        }
        
    }
}





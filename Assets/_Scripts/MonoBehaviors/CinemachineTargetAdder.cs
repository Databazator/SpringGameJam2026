using Unity.Cinemachine;
using UnityEngine;

public class CinemachineTargetAdder : MonoBehaviour
{
    private CinemachineTargetGroup targets;

    private void Awake()
    {
        targets = GetComponent<CinemachineTargetGroup>();
    }

    public void AddMember(Transform transform)
    {
        targets.AddMember(transform, 1.0f, 1.0f);
    }
}

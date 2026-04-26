using EasyButtons;
using UnityEngine;
using UnityEngine.Events;

public class TrebuchetAnimationController : MonoBehaviour
{
    private Animator animator;

    [SerializeField] UnityEvent OnFiring;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    [Button]
    [ContextMenu("Fire")]
    public void PlayTrebuchetFireAnim()
    {
        animator.SetTrigger("Fire");
        OnFiring?.Invoke();
    }
}

using UnityEngine;

public class TrebuchetAnimationController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    [ContextMenu("Fire")]
    public void PlayTrebuchetFireAnim()
    {
        animator.SetTrigger("Fire");
    }
}

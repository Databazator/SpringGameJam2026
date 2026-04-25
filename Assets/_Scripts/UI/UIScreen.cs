using DG.Tweening;
using UnityEngine;

public class UIScreen : MonoBehaviour
{
    //public bool StartActive;

    public float ScreenFadeDuration = 1f;

    protected CanvasGroup canvasGroup;

    protected bool screenActive;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
             
    }

    [ContextMenu("Show Screen")]
    public void InspectorShowScreen()
    {
        ShowScreen(0f);
    }
    
    public virtual void ShowScreen(float showElemsDelay = 0f)
    {
        ResetElements();

        canvasGroup.DOFade(1f, showElemsDelay);
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        DOVirtual.DelayedCall(showElemsDelay, () =>
        {
            ShowElementsSequence();
        });

    }

    [ContextMenu("Hide Screen")]
    public void InspectorHideScreen()
    {
        HideScreen(0f);
    }
    
    public virtual void HideScreen(float hidePanelDelay = -1f)
    {
        HideElementsSequence();

        DOVirtual.DelayedCall(hidePanelDelay, () =>
        {
            canvasGroup.DOFade(0f, ScreenFadeDuration);
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        });
    }

    public virtual void HideScreenImmediate()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public virtual void ResetElements()
    {

    }

    public virtual void ShowElementsSequence()
    {

    }

    public virtual void HideElementsSequence()
    {

    }
}

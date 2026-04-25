using DG.Tweening;
using UnityEngine;

public class CatSpinSequence : MonoBehaviour
{
    CatSpinner spinner;

    private void Awake()
    {
        spinner = GetComponent<CatSpinner>();
    }

    private void Start()
    {
        //Sequence spinSeq = DOTween.Sequence();
        //spinSeq.PrependCallback(() => spinner.StartSpin())
        //    .AppendInterval(2f).AppendCallback(() => spinner.StopSpin())
        //.AppendInterval(1f)
        //.SetLoops(-1, LoopType.Restart);
    }
}

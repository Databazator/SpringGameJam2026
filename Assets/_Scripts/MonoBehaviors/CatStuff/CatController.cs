using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MarkusSecundus.Utils.Extensions;
using MarkusSecundus.Utils.Primitives;
using UnityEngine;

public class CatController : MonoBehaviour
{
    [SerializeField] Vector3 _eatingFishScale = new Vector3(2, 4, 2);
    [SerializeField] float _fishScaleGain = 1.1f;
    [SerializeField] float _fishWeightGain = 1.1f;

    CatSpinner _spinner;
    Rigidbody2D _rb;

    Vector3 _ogScale;
    void Start()
    {
		_rb = GetComponentInChildren<Rigidbody2D>();
        _spinner = GetComponentInChildren<CatSpinner>();
        _ogScale = transform.localScale;
    }

    public void DoShoot()
    {
        _spinner.SetFlying();
    }
    public void PrepareShooting(float intensity)
    {
        _spinner.SetPreparation(intensity);
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
        var fish = collision.GetComponentInParent<FishController>();
        if (fish) DoEatFish(fish);
	}

    TweenerCore<Vector3, Vector3, VectorOptions> _scaleTween;
    void DoEatFish(FishController fish)
	{
        if (_scaleTween.IsNotNil() && _scaleTween.IsPlaying()) _scaleTween.Kill();

        this._ogScale *= _fishScaleGain;
        this._rb.mass *= _fishWeightGain;
        

        float totalEffectDuration = fish._getEatenDuration_seconds;
        _scaleTween = transform.DOScale(transform.localScale.MultiplyElems(_eatingFishScale), totalEffectDuration * 0.4f).OnComplete(() =>
        {
            _scaleTween = transform.DOScale(_ogScale, totalEffectDuration * 0.6f);
        });
        
		Debug.Log($"Eating the fish '{fish.name}'", this);
		fish._runEatenEffect();
	}
}

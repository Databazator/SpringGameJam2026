using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MarkusSecundus.Utils.Extensions;
using MarkusSecundus.Utils.Primitives;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CatController : MonoBehaviour
{
    [SerializeField] Vector3 _eatingFishScale = new Vector3(2, 4, 2);
    [SerializeField] float _fishScaleGain = 1.1f;
    [SerializeField] float _fishWeightGain = 1.1f;

    private InputSystem_Actions _actions;
    private InputAction _moveAction;
    private float _input;

    public UnityEvent OnDeath;

    [SerializeField] float _airNudgeForce = 10f;
    [SerializeField] float _airNudgeCooldown = 0.25f;
    [SerializeField] float _airNudgeScalePopMult = 0.9f;
    [SerializeField] float _airNudgeScalePopDuration = 0.25f;
    private bool _canAirNudge = true;

    public bool HasAirControl = true;

    CatSpinner _spinner;
    Rigidbody2D _rb;

    Vector3 _ogScale;

    private void Awake()
    {
        _actions = new InputSystem_Actions();
        _moveAction = _actions.Player.Move;
    }
    void Start()
    {
		_rb = GetComponentInChildren<Rigidbody2D>();
        _spinner = GetComponentInChildren<CatSpinner>();
        _ogScale = transform.localScale;
    }   

    private void Update()
    {
        _input = _moveAction.ReadValue<Vector2>().y;
    }

    private void FixedUpdate()
    {
        if (!HasAirControl) return;

        if(Mathf.Abs(_input) > Mathf.Epsilon)
        {
            if(_canAirNudge)
            {
                _canAirNudge = false;

                _rb.AddForce(Vector2.up * Mathf.Sign(_input) * _airNudgeForce, ForceMode2D.Impulse);

                transform.DOScale(_ogScale * _airNudgeScalePopMult, _airNudgeScalePopDuration * 0.25f).OnComplete(() =>
                {
                    transform.DOScale(_ogScale, _airNudgeScalePopDuration * 0.75f);
                });

                DOVirtual.DelayedCall(_airNudgeCooldown, () => _canAirNudge = true);
            }
        }
    }
    public void SetAirNudgeControl(bool value)
    {
        HasAirControl = value;
    }

    public void DoShoot()
    {
        _spinner.SetFlying();
    }
    public void PrepareShooting(float intensity)
    {
        _spinner.SetPreparation(intensity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Debug.Log("Cat Death");
            OnDeath.Invoke();
            SetAirNudgeControl(false);
        }
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

    private void OnEnable()
    {
        _moveAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
    }
}

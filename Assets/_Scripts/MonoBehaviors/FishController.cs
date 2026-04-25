using DG.Tweening;
using MarkusSecundus.Utils.Primitives;
using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class FishController : MonoBehaviour
{
    [SerializeField] Transform _rotatableBody;
    [SerializeField] SplineContainer _path;
    [SerializeField] float _moveSpeed = 1.0f;

    [System.Serializable]
    struct FlyingEffect
	{
		public Transform Wing;
        public Quaternion StartRotation;
        public Quaternion EndRotation;
        public float Speed;
        public Ease Ease;
	}
    [SerializeField] FlyingEffect _flyEffect;



    void Start()
    {
        _moveSpeed /= _path.CalculateLength();
        _runMovementAnimation();
        _runFlyingAnimation();
    }

    float _t = 0f;

    Vector2? _lastPos = null;
	private void Update()
	{
        _t += Time.deltaTime * _moveSpeed;
        while (_t >= 1f) _t -= 1f;

        transform.position = _path.EvaluatePosition(_t);

		if(_lastPos != null)
        {
            var delta = transform.position.xy() - _lastPos.Value;
            if(delta.x != 0f)
				_rotatableBody.localScale = _rotatableBody.localScale.WithX(delta.x < 0f ? 1f : -1f);
		}
        _lastPos = this.transform.position.xy();
	}

	void _runMovementAnimation()
	{
		var seq = DOTween.Sequence().Append(_flyEffect.Wing.DORotateQuaternion(_flyEffect.EndRotation, _flyEffect.Speed).SetEase(_flyEffect.Ease)).Append(_flyEffect.Wing.DORotateQuaternion(_flyEffect.StartRotation, _flyEffect.Speed).SetEase(_flyEffect.Ease));
		seq.SetLoops(-1, LoopType.Restart);
	}

    void _runFlyingAnimation()
    {
        _flyEffect.Wing.rotation = _flyEffect.StartRotation;
        var seq = DOTween.Sequence().Append(_flyEffect.Wing.DOLocalRotateQuaternion(_flyEffect.EndRotation, _flyEffect.Speed).SetEase(_flyEffect.Ease)).Append(_flyEffect.Wing.DOLocalRotateQuaternion(_flyEffect.StartRotation, _flyEffect.Speed).SetEase(_flyEffect.Ease));
        seq.SetLoops(-1, LoopType.Restart);
        
    }
}

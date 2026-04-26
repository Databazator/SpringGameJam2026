using DG.Tweening;
using UnityEngine;
using System.Runtime.CompilerServices;
using MarkusSecundus.Utils.Primitives;
using MarkusSecundus.Utils.Randomness;




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
    public bool SpinOnStart = true;
    public Transform CatVisualPivot;
    private Vector3 startCatVisPivotLocalPos;
    private Vector3 startCatPosition;

    public float SpinSide;
    bool spinActive = true;
    float spinTimer = 0f;

    [System.Serializable]
    public struct SpinConfig
    {
        public float SpinDuration;
        public float SpinRevolutions;
        public float RiseMultiplier;
        public float RiseTimes;

        public SpinConfig Lerp(in SpinConfig o, float t) => new SpinConfig
        {
            SpinDuration = Mathf.Lerp(SpinDuration, o.SpinDuration, t),
            SpinRevolutions = Mathf.Lerp(SpinRevolutions, o.SpinRevolutions, t),
            RiseMultiplier = Mathf.Lerp(RiseMultiplier, o.RiseMultiplier, t),
            RiseTimes = Mathf.Lerp(RiseTimes, o.RiseTimes, t)
        };
    }

    public SpinConfig IdleSpin;
    public Interval<float> IdleRestDuration;
    public Interval<SpinConfig> PreparationSpin;
    public SpinConfig FlyingSpin;

    public enum CurrentState
    {
        Idle, Preparation, Flying
    }
    CurrentState _currentState = CurrentState.Idle;
    float _preparationIntensity = 0f;
    public SpinConfig DesiredSpin => _currentState switch
    {
        CurrentState.Idle => IdleSpin,
        CurrentState.Preparation => PreparationSpin.Min.Lerp(PreparationSpin.Max, _preparationIntensity),
        CurrentState.Flying => FlyingSpin,
        _ => throw new System.ArgumentException("This should not happen!")
    };

    public SpinConfig CurrentSpin;

    private void Start()
    {
        CurrentSpin = IdleSpin;
        if (SpinOnStart)
        {
            StartSpin();
        }

        startCatVisPivotLocalPos = CatVisualPivot.localPosition;
        startCatPosition = transform.position;
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
        CurrentSpin = CurrentSpin.Lerp(DesiredSpin, Time.deltaTime);

        if(! spinActive)
        {
            if(_currentState != CurrentState.Idle)
            {
                spinActive = true;
                spinTimer = 0;
            }
            else
			{
				spinTimer -= Time.deltaTime;
				if (spinTimer < 0)
				{
					spinTimer = 0f;
					spinActive = true;
				}
                return;
			}
        }
        Debug.Assert(spinActive);
        spinTimer += Time.deltaTime;

        if (spinTimer <= CurrentSpin.SpinDuration)
        {

            float currFactor = spinTimer / CurrentSpin.SpinDuration;
            float currRot = (currFactor * CurrentSpin.SpinRevolutions * 360f * Mathf.Sign(SpinSide));


            float currRise = Mathf.Cos(currFactor * CurrentSpin.RiseTimes * 4f * Mathf.PI + startCatPosition.x) * CurrentSpin.RiseMultiplier;
            Vector3 currentPos = CatVisualPivot.localPosition + Vector3.up * currRise;

            CatVisualPivot.rotation = Quaternion.Euler(0f, currRot, 0f);
            CatVisualPivot.localPosition = currentPos;
        }
		else  {
            if (_currentState == CurrentState.Idle)
            {
                spinActive = false;
                spinTimer = RandomHelpers.Rand.Next(this.IdleRestDuration);
			}
            else
            {
				spinTimer = 0f;
			}

			//CatVisualPivot.rotation = Quaternion.identity;
			//CatVisualPivot.localPosition = startCatVisPivotLocalPos;
		}
        
    }

    public void SetIdle() => this._currentState = CurrentState.Idle;
    public void SetPreparation(float intensity) => (this._currentState, this._preparationIntensity) = (CurrentState.Preparation, intensity);
    public void SetFlying() => this._currentState = CurrentState.Flying;
    
    public void ResetTransform()
    {
        CatVisualPivot.localPosition = Vector2.zero.Extend(CatVisualPivot.localPosition.z);
        CatVisualPivot.localRotation = Quaternion.identity;
    }

    public void StartSpinSequence()
    {
		Sequence spinSeq = DOTween.Sequence().PrependCallback(() => this.StartSpin())
			.AppendInterval(2f).AppendCallback(() => this.StopSpin())
		//.AppendInterval(1f)
		.SetLoops(-1, LoopType.Restart);
	}
}





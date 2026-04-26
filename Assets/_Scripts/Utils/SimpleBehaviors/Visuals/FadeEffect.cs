

using DG.Tweening;
using MarkusSecundus.Utils.Assets._Scripts.Utils.SimpleBehaviors.Visuals;
using MarkusSecundus.Utils.Datastructs;
using MarkusSecundus.Utils.Extensions;
using MarkusSecundus.Utils.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace MarkusSecundus.Utils.Behaviors.Cosmetics
{
    public class FadeEffect : MonoBehaviour, IStoppable
    {
        [SerializeField] string _comment;
        [SerializeField] float duration_seconds = 1f;
        [SerializeField] float alphaLow = 0f;
        [SerializeField] float alphaHigh = 1f;
        [SerializeField] ActionOnStart _onStart;

        [SerializeField] Ease ease = Ease.Unset;

        [SerializeField] UnityEvent onComplete;


        [System.Serializable]
        public enum ActionOnStart
        {
            DoNothing = 0, FadeIn, FadeOut
        }

        private void Start()
		{
			if (GetComponentInChildren<Renderer>())
			{
				if (_onStart == ActionOnStart.FadeIn) FadeInRenderer(onComplete.Invoke);
				else if (_onStart == ActionOnStart.FadeOut) FadeOutRenderer(onComplete.Invoke);
			}
			if (GetComponentInChildren<AudioSource>())
			{
				if (_onStart == ActionOnStart.FadeIn) FadeInAudio(onComplete.Invoke);
				else if (_onStart == ActionOnStart.FadeOut) FadeOutAudio(onComplete.Invoke);
			}
			if (GetComponentInChildren<Graphic>())
			{
				if (_onStart == ActionOnStart.FadeIn) FadeIn(onComplete.Invoke);
				else if (_onStart == ActionOnStart.FadeOut) FadeOut(onComplete.Invoke);
			}
        }
        public void FadeOut() => FadeOut(duration_seconds);
        public void FadeOut(float duration) => FadeOut(null, duration_seconds);
        public void FadeOut(System.Action onFinished, float? duration = null, bool includeDefaultAction = true) => _runGraphicsEffect(alphaHigh, alphaLow, duration ?? duration_seconds, ease, onFinished, includeDefaultAction);
        public void FadeIn() => FadeIn(duration_seconds);
        public void FadeIn(float duration) => FadeIn(null, duration);
        public void FadeIn(System.Action onFinished, float? duration = null, bool includeDefaultAction = true) => _runGraphicsEffect(alphaLow, alphaHigh, duration ?? duration_seconds, ease, onFinished, includeDefaultAction);



		public void FadeOutAudio() => FadeOutAudio(duration_seconds);
		public void FadeOutAudio(float duration) => FadeOutAudio(null, duration_seconds);
		public void FadeOutAudio(System.Action onFinished, float? duration = null, bool includeDefaultAction = true) => _runVolumeEffect(alphaHigh, alphaLow, duration ?? duration_seconds, ease, onFinished, includeDefaultAction);
		public void FadeInAudio() => FadeInAudio(duration_seconds);
		public void FadeInAudio(float duration) => FadeInAudio(null, duration);
		public void FadeInAudio(System.Action onFinished, float? duration = null, bool includeDefaultAction = true) => _runVolumeEffect(alphaLow, alphaHigh, duration ?? duration_seconds, ease, onFinished, includeDefaultAction);



		public void FadeOutRenderer() => FadeOutRenderer(duration_seconds);
		public void FadeOutRenderer(float duration) => FadeOutRenderer(null, duration_seconds);
		public void FadeOutRenderer(System.Action onFinished, float? duration = null, bool includeDefaultAction = true) => _runRendererEffect(alphaHigh, alphaLow, duration ?? duration_seconds, ease, onFinished, includeDefaultAction);
		public void FadeInRenderer() => FadeInRenderer(duration_seconds);
		public void FadeInRenderer(float duration) => FadeInRenderer(null, duration);
		public void FadeInRenderer(System.Action onFinished, float? duration = null, bool includeDefaultAction = true) => _runRendererEffect(alphaLow, alphaHigh, duration ?? duration_seconds, ease, onFinished, includeDefaultAction);


        void _runGraphicsEffect(float alphaBegin, float alphaEnd, float duration, Ease ease, System.Action onFinished = null, bool includeDefaultAction = true)
            => _runEffect<Graphic>(alphaBegin, alphaEnd, duration, ease, onFinished, includeDefaultAction,
                (Graphic g, float alphaEnd, float duration_seconds) => g.DOFade(alphaEnd, duration_seconds),
                (Graphic g, float toSet)=> g.color = g.color.With(a: toSet)
                );

        void _runRendererEffect(float alphaBegin, float alphaEnd, float duration, Ease ease, System.Action onFinished = null, bool includeDefaultAction = true)
			=> _runEffect<Renderer>(alphaBegin, alphaEnd, duration, ease, onFinished, includeDefaultAction,
				(Renderer g, float alphaEnd, float duration_seconds) => g.material.DOFade(alphaEnd, duration_seconds),
				(Renderer g, float toSet) => g.material.color = g.material.color.With(a: toSet)
				);


		void _runVolumeEffect(float alphaBegin, float alphaEnd, float duration, Ease ease, System.Action onFinished = null, bool includeDefaultAction = true)
            => _runEffect<AudioSource>(alphaBegin, alphaEnd, duration, ease, onFinished, includeDefaultAction,
				(AudioSource g, float alphaEnd, float duration_seconds) => g.DOVolume(alphaEnd, duration_seconds),
				(AudioSource g, float toSet) => g.volume= toSet
				);

		void _runEffect<T>(
            float alphaBegin, float alphaEnd, float duration, Ease ease, System.Action onFinished, bool includeDefaultAction
            , Func<T, float, float, Tween> doFade, Action<T,  float> setter
            ) where T: Component
        {
            _tweens.Clear();

            gameObject.SetActive(true);
            Tween last = null;
            foreach (var rend in GetComponentsInChildren<T>(true))  
            {
                rend.gameObject.SetActive(true);
                setter(rend, alphaBegin);

                var tween = last = doFade(rend, alphaEnd, duration_seconds).SetEase(ease);
				_tweens.Add(tween);
                if (alphaEnd <= 0f) tween.onComplete += () => rend.gameObject.SetActive(false);
            }
            if (last != null) last.onComplete += () =>
            {
                _tweens.Clear();
                this.onComplete.Invoke();
                onFinished?.Invoke();
            };
            else
            {
                _tweens.Clear();
                this.onComplete.Invoke();
                onFinished?.Invoke();
            }
        }
        List<Tween> _tweens = new();
        public bool IsRunning => (! _tweens.IsNullOrEmpty()) && _tweens.Any(t => t.IsPlaying());
        public void Stop()
        {
            foreach (var t in _tweens) if (t.IsPlaying()) t.Kill();
            _tweens.Clear();
		}
	}
}
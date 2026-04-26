using MarkusSecundus.Utils.Primitives;
using MarkusSecundus.Utils.Randomness;
using System.Collections;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [System.Serializable]
    public struct SoundOption{
        public AudioClip Clip;
        public Interval<float> PitchRange;
        public Interval<float> VolumeRange;
    }

    [SerializeField] SoundOption[] Sounds = new SoundOption[] { new SoundOption { PitchRange = new Interval<float>(1f, 1f), VolumeRange = new Interval<float>(1f, 1f)} };
    [SerializeField] double MinTimeBetweenPlaying = 0;

    AudioSource _src;

    void Start()
    {
        _src = GetComponent<AudioSource>();
        if(!_src)
        {
			_src = this.gameObject.AddComponent<AudioSource>();
            _src.loop = false;
            _src.playOnAwake = false;
        }
    }

    System.Random rand => RandomHelpers.Rand;

    double _lastPlayingTimestamp = double.NegativeInfinity;
    double _nextPlayingTimestamp => _lastPlayingTimestamp + MinTimeBetweenPlaying;
    public void DoPlay()
    {
        if(! (this.enabled && this.gameObject.activeInHierarchy)) 
            return;
        if (MinTimeBetweenPlaying > 0.0 && (Time.timeAsDouble < _nextPlayingTimestamp))
            return;

        _lastPlayingTimestamp = Time.timeAsDouble;

        var sound = rand.Choice(Sounds);
        float volume = rand.Next(sound.VolumeRange);
        float pitch = rand.Next(sound.PitchRange);
        _src.pitch = pitch;
        _src.PlayOneShot(sound.Clip, volume);
        Debug.Log($"Playing {sound.Clip}, vol: {volume}, pitch: {pitch}", this);
    }

    Coroutine _playing = null;
    public void StartPlaying()
    {
        _playing = StartCoroutine(impl());
        IEnumerator impl()
        {
            while (true)
            {
                DoPlay();
                yield return null;
            }
        }
    }
    public void StopPlaying()
    {
        if (_playing != null)
        {
            StopCoroutine(_playing);
            _playing = null;
        }
    }
}

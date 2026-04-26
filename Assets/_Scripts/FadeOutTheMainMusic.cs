using MarkusSecundus.Utils.Behaviors.Cosmetics;
using UnityEngine;

public class FadeOutTheMainMusic : MonoBehaviour
{
	[SerializeField] string MusicObjectName;
	public void DoFadeOutTheMusic()
	{
		foreach(var src in Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
		{
			if (src.gameObject.name == MusicObjectName)
			{
				var fader = src.GetComponent<FadeEffect>();
				if (fader)
				{
					fader.FadeOutAudio();
				}
			}
		}
	}
}

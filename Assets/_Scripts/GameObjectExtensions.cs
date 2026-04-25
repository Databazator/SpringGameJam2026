using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Extension methods for <see cref="GameObject"/> class.
/// </summary>
public static class GameObjectExtensions
{
	/// <summary>
	/// Execute an action with specifed amount of seconds delay.
	/// </summary>
	public static void ExecuteDelayed(this GameObject gameObject, float delaySeconds, UnityAction action)
	{
		gameObject.GetComponent<MonoBehaviour>().StartCoroutine(ExecuteAfterWait(delaySeconds, action));
	}

    /// <summary>
    /// Coroutine whihc executes an action with specifed amount of seconds delay.
    /// </summary>
    private static IEnumerator ExecuteAfterWait(float seconds, UnityAction action)
	{
		yield return new WaitForSeconds(seconds);

		action.Invoke();
	}
}

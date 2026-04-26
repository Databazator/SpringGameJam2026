using MarkusSecundus.Utils.Physics;
using System.Collections;
using UnityEngine;

public class CatSuckTrigger : MonoBehaviour
{
	[SerializeField] Transform _destination;
	[SerializeField] float _steerForce = 1.0f;
	[SerializeField] ForceMode2D _steerMode = ForceMode2D.Impulse;

	Coroutine _catSucker = null;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.attachedRigidbody.GetComponentInChildren<CatController>())
		{
			if(_catSucker == null)
				_catSucker = StartCoroutine(_steerCatToDestination(collision.attachedRigidbody));
		}
	}

	public void StopCatSucker()
	{
		Debug.Log("Stop Cucker command");
		if(_catSucker != null)
		{
			Debug.Log("STOPPING Cat Sucker!");
			StopCoroutine(_catSucker);
			_catSucker = null;
		}	
	}

	IEnumerator _steerCatToDestination(Rigidbody2D cat)
	{
		Debug.Log("STARTING Cat Sucker.");
		while (true)
		{
			var positionDelta = _destination.position - cat.transform.position;
			cat.SteerToVelocity(positionDelta, _steerForce, _steerMode);
			yield return null;
		}
	}
}

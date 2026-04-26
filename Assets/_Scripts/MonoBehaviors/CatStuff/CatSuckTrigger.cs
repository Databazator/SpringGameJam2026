using MarkusSecundus.Utils.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatSuckTrigger : MonoBehaviour
{
	[SerializeField] Transform _destination;
	[SerializeField] float _steerForce = 1.0f;
	[SerializeField] ForceMode2D _steerMode = ForceMode2D.Impulse;

	Coroutine _catSucker = null;


	CatController _currentCat = null;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		var cat = collision?.attachedRigidbody?.GetComponentInChildren<CatController>();
		if (cat && collision.attachedRigidbody.bodyType == RigidbodyType2D.Dynamic && _currentCat != cat)
		{
			if(_catSucker == null)
			{
				_currentCat = cat;
				_catSucker = StartCoroutine(_steerCatToDestination(collision.attachedRigidbody));
			}
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
			if (_currentCat != null)
			{
				_currentCat.GetComponentInParent<Rigidbody2D>().gravityScale = 1f;
			}
		}	
	}

	IEnumerator _steerCatToDestination(Rigidbody2D cat)
	{
		Debug.Log("STARTING Cat Sucker.");
		cat.gravityScale = 0f;
		while (true)
		{
			var positionDelta = _destination.position - cat.transform.position;
			cat.SteerToVelocity(positionDelta, _steerForce, _steerMode);
			yield return null;
		}
	}
}

using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class CatQueue : MonoBehaviour
{
    public List<CatController> Cats;
    public Transform TrebuchetSlingPos;

    [SerializeField] private Vector3 _positionOffsets;
    [SerializeField] private float _repositionDuration;
    [SerializeField] private float _repositionRandomOffset;
    [SerializeField] private float _positionXRandomOffset;

    public UnityEvent OnCatLoaded;

    private void Start()
    {
        PositionCatsInQueue();
    }

    public void PositionCatsInQueue()
    {
        int index = 0;
        //position cats into the queue
        foreach (var c in Cats)
        {
            c.transform.position = GetQueuePosForIndex(index);

            index++;
        }
    }

    public bool Empty()
    {
        return Cats.Count == 0;
    }

    // Front cat jumps to the trebuchet sling and then the queue is shuffled forward. Returnes dequeued cat
    [ContextMenu("Pop Front Cat")]
    public CatController DequeueCat()
    {
        if (Empty()) return null;

        CatController c = Cats.First();
        Cats.RemoveAt(0);

        c.transform.DOMove(TrebuchetSlingPos.position, _repositionDuration).SetEase(Ease.InOutBack).OnComplete(() =>
        {
            OnCatLoaded.Invoke();
        });

        Vector3 ogScale = c.transform.localScale;
        c.transform.DOScale(ogScale * 0.8f, _repositionDuration * 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            c.transform.DOScale(ogScale, _repositionDuration * 0.8f).SetEase(Ease.InOutQuad);
        });

        MoveQueue();

        return c;
    }

    private void MoveQueue()
    {
        int index = 0;
        foreach (CatController c in Cats) 
        {
            float delay = UnityEngine.Random.Range(0, _repositionRandomOffset);

            Sequence seq = DOTween.Sequence();

            seq.PrependInterval(delay);
            seq.Append(c.transform.DOShakeRotation(_repositionDuration, 25, 20));
            seq.Append(c.transform.DOMove(GetQueuePosForIndex(index), _repositionDuration).SetEase(Ease.InOutQuad));

            index++;
        }
        
    }

    private Vector3 GetQueuePosForIndex(int index)
    {
        if(index < 0 || index >= Cats.Count)
        {
            Debug.LogError("GetQueuePos called with invalid index");
            return Vector3.zero;
        }

        return transform.position + index * _positionOffsets + Vector3.right * UnityEngine.Random.Range(-_positionXRandomOffset, _positionXRandomOffset);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        int count = Cats.Count + 1;
        for(int i = 0; i < count; i++)
        {
            Gizmos.DrawSphere(transform.position + i * _positionOffsets, 0.25f);
        }

        if(TrebuchetSlingPos != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(TrebuchetSlingPos.position, 0.25f);
        }
    }
}

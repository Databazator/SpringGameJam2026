using DG.Tweening;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    enum GameState
    {
        Invalid,
        LoadingCat,
        Launch,
        Flight,
        Catch
    }

    private GameState currentState = GameState.Invalid;

    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private List<CatController> _cats;

    public bool Active = true;

    [Header("System References")]
    public UIManager UIManager;
    public CatQueue CatQueue;
    public TrebuchetController Trebuchet;
    public TrebuchetAnimatorEvents TrebuchetAnimEvents;
    public ShooterController Shooter;
    public Transform CatHolderTransform;

    [Header("VirtualCameras")]
    public CinemachineCamera TrebuchetLoadCam;
    public CinemachineCamera FocusGroupCam;

    [SerializeField]
    private CinemachineTargetAdder CameraFocusAdder;
    public CinemachineTargetGroup TargetGroup;

    public Transform FocusGroupStartTarget1;
    public Transform FocusGroupStartTarget2;

    [Header("State Timing")]
    public float CatLoadStateEnterLogicDelay = 2f; // wait for camera tween to resolve before doing stuff

    public int LivesCount = 9;
    private int _lives;

    private int _catCount;
    private int _catsCaught;

    /// <summary>
    /// Occurs when cat is changed.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<GameObject> OnCatChanged { get; private set; } = new();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        if (Shooter != null) Shooter.IsControlActive = false;

        LoadCatsIntoQueue();

        if (UIManager == null || !UIManager.gameObject.activeInHierarchy)
        {
            StartGame();
        }



    }

    void LoadCatsIntoQueue()
    {
        // load cats that are to be transported this game into the queue
        if (CatQueue != null && CatHolderTransform != null)
        {
            CatQueue.Cats.Clear();

            CatController cat;
            foreach (Transform child in CatHolderTransform)
            {
                if (child.TryGetComponent<CatController>(out cat))
                {
                    CatQueue.Cats.Add(cat);
                }
            }

            CatQueue.PositionCatsInQueue();
        }
    }

    public void StartGame()
    {
        _lives = LivesCount;

        LoadCatsIntoQueue();

        if (CatQueue != null)
        {
            _catCount = CatQueue.Cats.Count;
        }
        _catsCaught = 0;

        DOVirtual.DelayedCall(1f, () =>
        {
            TargetGroup.Targets.Clear();
            TargetGroup.AddMember(FocusGroupStartTarget1, 1, 1);
            TargetGroup.AddMember(FocusGroupStartTarget2, 1, 1);

            SetState(GameState.LoadingCat);

            Debug.Log("Start Game Called");
        });
    }

    public void CatLoaded()
    {
        SetState(GameState.Launch);
    }

    public void CatLandedInBox()
    {
        Debug.Log("Cat Landed in a Box! Huzzah");

        _catsCaught++;
        if (CatQueue.Empty() && _catsCaught/2f == _catCount) // all cats made it -> Victory screen
        {
            UIManager.ShowVictoryScreen();
            return;
        }

        SetState(GameState.LoadingCat);
    }

    private void SetState(GameState state)
    {
        if (!Active) return;

        if (currentState == state) return;

        Debug.Log($"Set state to {state.ToString()}");

        currentState = state;

        if (currentState == GameState.LoadingCat)
        {
            TrebuchetLoadCam.Priority = 10;
            FocusGroupCam.Priority = 0;

            DOVirtual.DelayedCall(CatLoadStateEnterLogicDelay, () =>
            {
                var c = CatQueue.DequeueCat();
                // setup trebuchet and shooter refs after cat loading tween finishes
                DOVirtual.DelayedCall(CatQueue.RepositionDuration, () =>
                {
                    Trebuchet.Cat = c.gameObject;
                    Shooter.SetCatAsProjectile(c);
                    TrebuchetAnimEvents.ProjectileTransform = c.transform;
                    OnCatChanged.Invoke(c.gameObject);
                    CameraFocusAdder.AddMember(c.transform);

                    Debug.Log("Cat Changed");
                });
            });
        }
        else if (currentState == GameState.Launch)
        {
            Shooter.IsControlActive = true;
            TrebuchetLoadCam.Priority = 0;
            FocusGroupCam.Priority = 10;
        }
        else if (currentState == GameState.Flight)
        {

        }
        else if (currentState == GameState.Catch)
        {

        }

    }

}

using DG.Tweening;
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

        if(CatQueue != null)
        {
            _catCount = CatQueue.Cats.Count;
        }
        _catsCaught = 0;

        SetState(GameState.LoadingCat);
    }

    public void CatLoaded()
    {
        SetState(GameState.Launch);
    }

    public void CatLandedInBox()
    {
        _catsCaught++;
        if(CatQueue.Empty()) // all cats made it -> Victory screen
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

        currentState = state;

        if (currentState == GameState.LoadingCat)
        {
            TrebuchetLoadCam.Priority = 10;
            FocusGroupCam.Priority = 0;

            DOVirtual.DelayedCall(CatLoadStateEnterLogicDelay, () =>
            {
                var c = CatQueue.DequeueCat();
                Shooter.SetCatAsProjectile(c);
                TrebuchetAnimEvents.ProjectileTransform = c.transform;
                Trebuchet.Cat = c.gameObject;
                OnCatChanged.Invoke(c.gameObject);
                CameraFocusAdder.AddMember(c.transform);

                Debug.Log("Cat Changed");
            });
        }
        else if (currentState == GameState.Launch)
        {
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

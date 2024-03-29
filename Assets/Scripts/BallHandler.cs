using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float detachDelay = 0.5f;
    [SerializeField] private float respawnDelay = 0.5f;
    [SerializeField] private Vector2 maxDestanceFromPovit = new(10,10);

    private Rigidbody2D currentBallRibigbody;
    private SpringJoint2D currentBallSpringJoin;

    private Camera mainCamera;
    private bool isDragging = false;

    private void Start()
    {
        mainCamera = Camera.main;
        SpwanNewBall();
    }

    // For multi touch
    private void OnEnable() => EnhancedTouchSupport.Enable();

    private void OnDisable() => EnhancedTouchSupport.Disable();

    private void Update()
    {
        if(currentBallRibigbody is null)
            return;

        //if(!Touchscreen.current.primaryTouch.press.isPressed)
        if(Touch.activeTouches.Count == 0)
        {
            if(isDragging)
                LaunchBall();

            isDragging = false;
            return;
        }

        isDragging = true;
        currentBallRibigbody.isKinematic = true;

        Vector2 touchPositions = new();
        foreach(Touch touch in Touch.activeTouches)
        {
            touchPositions += touch.screenPosition;
        }
        touchPositions /= Touch.activeTouches.Count;

        //Vector3 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPositions);

        currentBallRibigbody.position = worldPosition;
    }

    private void SpwanNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab,pivot.position,Quaternion.identity);

        currentBallRibigbody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoin = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSpringJoin.connectedBody = pivot;
    }

    private void LaunchBall()
    {
        currentBallRibigbody.isKinematic = false;
        currentBallRibigbody = null;

        Invoke(nameof(DetachBall),detachDelay);
    }

    private void DetachBall()
    {
        currentBallSpringJoin.enabled = false;
        currentBallSpringJoin = null;

        Invoke(nameof(SpwanNewBall),respawnDelay);
    }
}
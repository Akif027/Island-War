using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class MapScroll : MonoBehaviour
{
    [Header("Cinemachine References")]
    public CinemachineCamera cinemachineCamera; // Updated reference to CinemachineCamera
    public CinemachineConfiner2D confiner2D;    // Reference to the Cinemachine Confiner 2D component
    public Collider2D boundingShape;           // Collider used as the bounding shape for the confiner

    [Header("Scroll Settings")]
    public float scrollSpeed = 0.5f; // Speed of the scroll
    public float FollowSpeed = 2f; // Speed of the follow

    [Header("Zoom Settings")]
    public float minZoom = 5f; // Minimum orthographic size for the camera
    public float maxZoom = 15f; // Maximum orthographic size for the camera
    public float zoomSpeed = 0.1f; // Speed of zooming

    private Camera mainCamera; // Main Unity Camera
    private Vector3 dragOrigin; // Stores the position where dragging started

    public static MapScroll Instance { get; private set; }

    private bool isScrollEnabled = true;
    private GameObject followTarget; // The target the camera will follow
    private bool isFollowing = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        mainCamera = Camera.main;

        if (cinemachineCamera == null)
        {
            Debug.LogError("Cinemachine camera is not assigned!");
            return;
        }

        if (confiner2D == null)
        {
            Debug.LogError("Cinemachine Confiner 2D is not assigned! ");
            return;
        }

        if (boundingShape != null)
        {
            confiner2D.BoundingShape2D = boundingShape;
        }
    }

    void Update()
    {
        if (isFollowing && followTarget != null)
        {
            FollowTarget();
        }
        if (isScrollEnabled)
        {
            HandleDrag();
            HandleZoom();
        }
    }

    private Coroutine transitionCoroutine;

    public void SmoothTransitionToPosition(Vector3 targetPosition, float duration)
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(SmoothTransitionCoroutine(targetPosition, duration));
    }

    private IEnumerator SmoothTransitionCoroutine(Vector3 targetPosition, float duration)
    {
        Transform camTransform = cinemachineCamera.transform;
        Vector3 startPosition = camTransform.position;
        targetPosition.z = startPosition.z; // Keep the Z value unchanged

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Interpolate position
            camTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        camTransform.position = targetPosition; // Ensure the final position is exactly the target
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = dragOrigin - currentPoint;
            MoveCamera(difference);
        }
    }

    private void MoveCamera(Vector3 offset)
    {
        Transform camTransform = cinemachineCamera.transform;
        Vector3 newPosition = camTransform.position + offset * scrollSpeed;

        if (confiner2D != null && boundingShape != null)
        {
            Bounds bounds = boundingShape.bounds;
            newPosition.x = Mathf.Clamp(newPosition.x, bounds.min.x, bounds.max.x);
            newPosition.y = Mathf.Clamp(newPosition.y, bounds.min.y, bounds.max.y);
        }

        camTransform.position = new Vector3(newPosition.x, newPosition.y, camTransform.position.z);
    }

    public void StartFollowing(GameObject target)
    {
        followTarget = target;
        isFollowing = true;
    }

    public void StopFollowing()
    {
        followTarget = null;
        isFollowing = false;
    }

    private void FollowTarget()
    {
        if (followTarget == null) return;

        Vector3 targetPosition = followTarget.transform.position;
        targetPosition.z = cinemachineCamera.transform.position.z; // Keep the Z value unchanged

        Transform camTransform = cinemachineCamera.transform;
        camTransform.position = Vector3.Lerp(camTransform.position, targetPosition, FollowSpeed * Time.deltaTime);
    }

    private void HandleZoom()
    {
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float difference = prevMagnitude - currentMagnitude;

            // Adjust the orthographic size of the CinemachineCamera
            float currentZoom = cinemachineCamera.Lens.OrthographicSize;
            float newZoom = Mathf.Clamp(currentZoom + difference * zoomSpeed, minZoom, maxZoom);

            // Ensure the zoom level does not exceed confiner bounds
            if (boundingShape != null)
            {
                float maxPossibleZoom = CalculateMaxZoomForConfiner();
                newZoom = Mathf.Clamp(newZoom, minZoom, maxPossibleZoom);
            }

            cinemachineCamera.Lens.OrthographicSize = newZoom;
        }
    }
    private float CalculateMaxZoomForConfiner()
    {
        if (boundingShape == null) return maxZoom;

        Bounds bounds = boundingShape.bounds;
        float cameraAspect = (float)Screen.width / Screen.height;

        // Calculate the max orthographic size that fits within the confiner
        float maxOrthographicSize = Mathf.Min(bounds.extents.y, bounds.extents.x / cameraAspect);

        return Mathf.Min(maxOrthographicSize, maxZoom);
    }
    public void EnableScroll()
    {
        isScrollEnabled = true;
    }

    public void DisableScroll()
    {
        isScrollEnabled = false;
    }
}

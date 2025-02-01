using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlacementManager : MonoBehaviour
{
    private CharacterType characterType;

    [Header("References")]
    public GameObject MainSpriteObj;
    public GameObject CannotPlaceInvalidObj;
    public GameObject BlueBorder;
    public GameObject YellowBorder;

    [Header("Settings")]
    [SerializeField] private bool isValidPlacement = false;
    public float rayLength = 10f; // Length of the ray
    public Color rayColor = Color.red; // Visualization color for ray
    public LayerMask raycastMask; // Mask for raycast

    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private Vector3 offset;
    private bool isBeingDragged = false;
    public bool isTokenPlaced = false;
    private bool dragEnabled = false;

    private bool KingRecivedBonusAtStart = false;
    public Token token;



    private Dictionary<PolygonCollider2D, Vector2[]> polygonVerticesCache = new Dictionary<PolygonCollider2D, Vector2[]>();

    void Start()
    {

        token = GetComponent<Token>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the token's first child.");
        }

        EnableDrag(true);

        characterType = token.characterData.characterType;
        HandleBorder();

        // Delay placement rule check for initialization
        StartCoroutine(DelayedCheckPlacementRules());
    }

    private IEnumerator DelayedCheckPlacementRules()
    {
        yield return new WaitForEndOfFrame();
        CheckPlacementRules();
    }

    private void EnableDrag(bool isEnabled)
    {
        dragEnabled = isEnabled;
    }

    private void Update()
    {
        HandleDrag();

        GamePhase currentPhase = GameManager.Instance.getCurrentPhase();

        if (currentPhase == GamePhase.GamePlay && token.characterData.characterType != CharacterType.Golem && GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Dynamic)
        {
            SetRigidbodyType(RigidbodyType2D.Dynamic);
        }
        else if (currentPhase == GamePhase.Placement)
        {
            SetRigidbodyType(RigidbodyType2D.Kinematic);
        }

        if (token.characterData.characterType == CharacterType.Golem && !token.IsImmobile() && GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Dynamic)
        {
            SetRigidbodyType(RigidbodyType2D.Dynamic);
            Debug.LogError("Golem is now dynamic.");
        }
        if (token.characterData.characterType == CharacterType.Enchantress && GameManager.Instance.getCurrentPhase() == GamePhase.Placement && MainSpriteObj.GetComponentInChildren<CircleCollider2D>() != null)
        {
            MainSpriteObj.GetComponentInChildren<CircleCollider2D>().enabled = false;

        }
        else
        {
            if (MainSpriteObj.GetComponentInChildren<CircleCollider2D>() == null) return;
            MainSpriteObj.GetComponentInChildren<CircleCollider2D>().enabled = true;
        }
        if (isBeingDragged)
        {
            CheckPlacementRules();
        }

        DebugDrawRay();
    }

    private void HandleDrag()
    {
        if (!dragEnabled || isTokenPlaced) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);

            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject && token.IsCurrentPlayerOwner())
                {
                    isBeingDragged = true;
                    MapScroll.Instance.DisableScroll();
                    Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    offset = transform.position - new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
                    break;
                }
            }
        }

        if (Input.GetMouseButton(0) && isBeingDragged)
        {
            UIManager.Instance.OkButton.gameObject.SetActive(false);
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = transform.position.z;
            transform.position = mousePosition + offset;
        }

        if (Input.GetMouseButtonUp(0) && isBeingDragged)
        {
            isBeingDragged = false;
            MapScroll.Instance.EnableScroll();
            CheckPlacementRules();

            if (isValidPlacement)
            {
                EventManager.TriggerEvent<PlacementManager>("TokenPlaced", this);
            }
            else
            {
                ResetPosition();
            }
        }
    }

    private void SetRigidbodyType(RigidbodyType2D type)
    {
        GetComponent<Rigidbody2D>().bodyType = type;
    }

    public void CheckPlacementRules()
    {
        isValidPlacement = false;

        switch (characterType)
        {
            case CharacterType.Mermaid:

                isValidPlacement = token.characterData.ability.CheckPlacementForMermaid(this);

                break;

            default:
                isValidPlacement = CheckPlacement("Base");
                break;
        }
        if (isValidPlacement && !isBeingDragged)
        {
            CheckAndRepositionToken();
        }
        MainSpriteObj.SetActive(isValidPlacement);
        CannotPlaceInvalidObj.SetActive(!isValidPlacement);
    }
    private void CheckAndRepositionToken()
    {
        float minDistance = 0.2f; // Minimum distance between tokens to avoid overlap
        int maxAttempts = 10; // Maximum number of attempts to find a valid position

        Collider2D[] colliders;
        int attempts = 0;
        bool isOverlapping = false;

        do
        {
            // Check for overlapping tokens
            colliders = Physics2D.OverlapCircleAll(transform.position, minDistance);
            isOverlapping = false;

            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject != gameObject && collider.CompareTag("Token"))
                {
                    isOverlapping = true;
                }
            }

            attempts++;
        } while (isOverlapping && attempts < maxAttempts);

        if (isOverlapping)
        {
            Debug.LogWarning($"{name} could not find a non-overlapping position after {maxAttempts} attempts.");
            isValidPlacement = false; // Mark placement as invalid if overlapping
        }
        else
        {
            Debug.Log($"{name} successfully repositioned to a valid position.");
        }
    }

    private void DebugDrawRay()
    {
        Debug.DrawRay(transform.position, Vector3.back * rayLength, rayColor);
    }

    private bool CheckPlacement(string tag)
    {
        float radius = GetComponent<CircleCollider2D>().radius;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, raycastMask);

        foreach (Collider2D collider in colliders)
        {
            Base playerBase = collider.GetComponent<Base>();
            if (playerBase != null && (playerBase.ownerID != token.owner && playerBase.ownerID != 0))
            {
                return false;
            }

            if (collider.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
    }

    public void ConfirmPlacement()
    {
        Debug.Log($"{characterType} placed successfully at  {transform.position} ");
        EnableDrag(false);
        isTokenPlaced = true;
        SetRigidbodyType(RigidbodyType2D.Kinematic);
        token.OnTokenPlaced();
    }
    private void DebugDrawEdge(Vector2 closestPoint)
    {
        Debug.DrawLine(transform.position, closestPoint, Color.green, 0.1f);
    }
    private void ResetPosition()
    {
        Debug.Log("Invalid placement. Resetting token position.");
        Handheld.Vibrate();
        SoundManager.Instance?.PlayNotPossiblePlacement();
        EventManager.TriggerEvent<bool>("TokenPlaced", false);

    }

    private void HandleBorder()
    {
        Color playerColor = GameManager.Instance.GetPlayerColor(token.owner);
        YellowBorder.SetActive(playerColor == Color.yellow);
        BlueBorder.SetActive(playerColor != Color.yellow);
    }



}

using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Ability", menuName = "Abilities/Generic Character Ability")]
public class CharacterAbility : ScriptableObject
{
    [Header("Ability Details")]
    public string abilityName;
    [TextArea] public string description;

    [Header("General Abilities")]
    public bool canMoveOnWater;

    public bool becomesImmobileOnPlacement;

    [Header("Vault Interaction")]
    public bool isVaultInteraction; // Flag for vault/base interaction

    public int coinsPerCaptureBase = 0; // Coins gained for capturing a vault/base

    [Header("Placement Bonus")]
    public bool bonusPointsOnPlaced;
    public int bonusCoins = 30; // Default bonus coins for placement

    [Header("Modifiers")]
    public float speedMultiplier = 1f;
    public float weightMultiplier = 1f;





    /// <summary>
    /// Called when the token is activated.
    /// </summary>
    public virtual void Activate(Token token)
    {

        // Handle immobility on placement for Golem
        if (becomesImmobileOnPlacement && token.characterData.characterType == CharacterType.Golem)
        {
            token.SetImmobile(); // Call the Token's method to make it immobile
            Debug.Log($"{token.characterData.characterName} becomes immobile on placement.");
        }

    }
    public void AddKingBonus(Token token)
    {
        if (bonusPointsOnPlaced && bonusCoins > 0)
        {
            EventManager.TriggerCoinAdd(token.owner, bonusCoins); SoundManager.Instance.PlayCoinCollect();
            Debug.Log($"{token.name} awarded {bonusCoins} bonus coins on placement.");


        }
    }


    /// <summary>
    /// Called when the token interacts with a vault or base.
    /// </summary>
    public virtual void OnVaultInteraction(Token token)
    {
        if (GameManager.Instance.currentPhase != GamePhase.GamePlay) return;
        if (isVaultInteraction)
        {

            float radius = token.GetComponent<CircleCollider2D>().radius; // Adjust this value as needed
            LayerMask layerMask = LayerMask.GetMask("Vault", "VaultMid");

            // Use OverlapCircle to detect colliders within the specified radius
            Collider2D hitCollider = Physics2D.OverlapCircle(token.transform.position, radius, layerMask);

            if (hitCollider != null)
            {
                if (hitCollider.CompareTag("Vault"))
                {
                    if (token.characterData.characterType != CharacterType.Thief)
                    {
                        EventManager.TriggerCoinAdd(token.owner, 20);
                    }
                    else
                    {
                        EventManager.TriggerCoinAdd(token.owner, 5);
                    }

                    SoundManager.Instance.PlayCoinCollect();
                }
                else if (hitCollider.CompareTag("VaultMid"))
                {
                    if (token.characterData.characterType != CharacterType.Thief)
                    {
                        EventManager.TriggerCoinAdd(token.owner, 10);
                    }
                    else
                    {
                        EventManager.TriggerCoinAdd(token.owner, 5);
                    }
                    SoundManager.Instance.PlayCoinCollect();
                }
            }


        }
    }
    bool Basealreadycaptured = false;
    /// <summary>
    /// Called when the token interacts with a vault or base.
    /// </summary>
    public virtual void OnBaseCapture(Token token)
    {
        if (GameManager.Instance.currentPhase != GamePhase.GamePlay && token.characterData.characterType == CharacterType.Thief) return;
        if (coinsPerCaptureBase > 0)
        {

            float radius = token.GetComponent<CircleCollider2D>().radius; // Adjust based on your token's size
            LayerMask layerMask = LayerMask.GetMask("BaseIcon", "Base");

            // Use OverlapCircle to detect colliders within the specified radius
            Collider2D hitCollider = Physics2D.OverlapCircle(token.transform.position, radius, layerMask);

            if (hitCollider != null && hitCollider.CompareTag("BaseIcon"))
            {


                Base ownerBase = hitCollider.GetComponentInParent<Base>();

                if (!Basealreadycaptured && ownerBase.ownerID != token.owner && token.characterData.characterType != CharacterType.Thief) // Only capture opponent bases
                {
                    EventManager.TriggerGloryPointAdd(token.owner, coinsPerCaptureBase);
                    Basealreadycaptured = true;
                    Debug.LogError($"Player {token.owner} has captured the base. ");
                    SoundManager.Instance?.PlayScore();
                }
            }

            {
                Basealreadycaptured = false;

            }
        }

    }


    public void OutOfBound(Token token, Vector2 boundarySize)
    {

        Rigidbody2D rb = token.GetRigidbody2D();
        if (rb == null) return;

        Vector2 position = rb.position;

        // Check if the token is out of the horizontal bounds
        if (Mathf.Abs(position.x) > boundarySize.x / 2f || Mathf.Abs(position.y) > boundarySize.y / 2f)
        {
            Debug.LogError($"{token.name} is out of bounds and will be eliminated.  ");
            token.EliminateToken();
            token.ChangeTurn();
        }
    }
    public void HandleReflection(Token token, Vector2 boundarySize)
    {
        Rigidbody2D rb = token.GetRigidbody2D();
        if (rb == null) return;

        Vector2 position = rb.position;
        Vector2 velocity = rb.velocity;

        bool reflected = false;

        // Horizontal boundary check
        if (Mathf.Abs(position.x) >= boundarySize.x / 2f)
        {
            velocity.x *= -1; // Reverse X velocity
            position.x = Mathf.Clamp(position.x, -boundarySize.x / 2f, boundarySize.x / 2f); // Clamp position
            reflected = true;
        }

        // Vertical boundary check
        if (Mathf.Abs(position.y) >= boundarySize.y / 2f)
        {
            velocity.y *= -1; // Reverse Y velocity
            position.y = Mathf.Clamp(position.y, -boundarySize.y / 2f, boundarySize.y / 2f); // Clamp position
            reflected = true;
        }

        if (reflected)
        {
            rb.velocity = velocity; // Apply reflected velocity
            rb.position = position; // Update position
            Debug.Log($"{token.name} (Satyr) reflected with velocity: {velocity} ");
            SoundManager.Instance?.PlayFlickTheChip();
        }
    }
    /// <summary>
    /// Validates the token's final position based on its ability.
    /// </summary>
    /// <summary>
    /// Validates the token's final position based on its ability and terrain interaction.
    /// </summary>
    public virtual bool ValidateFinalPosition(Token token)
    {
        // Check if the character is not a Mermaid
        if (token.characterData.characterType != CharacterType.Mermaid)
        {
            // Define the circle's radius
            float radius = token.GetComponent<CircleCollider2D>().radius;
            LayerMask layerMask = LayerMask.GetMask("Water", "Land", "Base");

            // Use OverlapCircle to detect colliders within the specified radius
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(token.transform.position, radius, layerMask);

            foreach (Collider2D collider in hitColliders)
            {
                // Check for valid terrain for non-water characters
                if (collider.CompareTag("Land") || collider.CompareTag("Base"))
                {
                    Debug.Log($"{token.characterData.characterName} is safely on land or base. ");
                    return true; // Valid for land-based characters
                }
                if (collider.CompareTag("Water"))
                {
                    Debug.LogError($"{token.characterData.characterName} cannot be on water.");
                    return false; // Invalid for non-water characters
                }
            }

            Debug.LogError($"{token.characterData.characterName} is not on any valid terrain.");
            return false; // Default invalid if no valid terrain is found
        }
        else
        {
            // For Mermaid, use the specific placement check
            return CheckPlacementForMermaid(token);
        }
    }

    public bool CheckPlacementForMermaid(PlacementManager token)
    {
        float radius = token.GetComponent<CircleCollider2D>().radius;
        Vector2 position = token.transform.position;

        int numRays = 360; // Number of rays to cast
        bool isTouchingWater = false;
        bool isTouchingLand = false;
        bool isTouchingBase = false;
        bool isOwnersBase = false;

        for (int i = 0; i < numRays; i++)
        {
            float angle = i * Mathf.Deg2Rad * (360f / numRays);
            Vector2 rayOrigin = position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero, 0f, token.raycastMask);

            if (hit.collider != null)
            {
                Base ownerBase = hit.collider.GetComponent<Base>();
                if (ownerBase != null && ownerBase.ownerID == token.token.owner && ownerBase.ownerID != 0)
                {
                    isOwnersBase = true;
                }

                if (hit.collider.CompareTag("Water"))
                {
                    isTouchingWater = true;
                }
                else if (hit.collider.CompareTag("Land"))
                {
                    isTouchingLand = true;
                }
                else if (hit.collider.CompareTag("Base"))
                {
                    isTouchingBase = true;
                }
            }
        }

        // Disallow placement if touching both land and water
        if (isTouchingWater && isTouchingLand)
        {
            return false;
        }

        // Disallow placement if only on water
        if (isTouchingWater && !isTouchingLand && !isTouchingBase)
        {
            return false;
        }

        // Allow placement if touching water and either it's the owner's base or not touching any base
        if (isTouchingWater && (isOwnersBase || !isTouchingBase))
        {
            return true;
        }

        // Disallow placement in all other cases
        return false;
    }
    public bool CheckPlacementForMermaid(Token token)
    {
        float radius = token.GetComponent<CircleCollider2D>().radius;
        Vector2 position = (Vector2)token.transform.position;
        LayerMask layerMask = LayerMask.GetMask("Water", "Land", "Base", "Vault", "VaultMid", "BaseIcon");

        // Create an array of points around the perimeter of the circle
        int numRays = 360; // Number of rays to cast (you can adjust this for more precision)
        bool isTouchingWater = false;
        bool isTouchingBase = false;
        bool isTouchingLand = false;
        bool isTouchingVault = false;
        bool midVault = false;
        bool isTouchingbaseIcon = false;
        bool isOwnersBase = false;
        for (int i = 0; i < numRays; i++)
        {
            float angle = i * Mathf.Deg2Rad * (360f / numRays);
            Vector3 rayOrigin = (Vector3)position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            // Raycast along Vector3.forward (Z-axis)
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector3.forward, -3, layerMask); // Raycast along the Z-axis

            if (hit.collider != null)
            {
                Base OwnerBase = hit.collider.GetComponent<Base>();
                if (OwnerBase != null && (OwnerBase.ownerID == token.owner && OwnerBase.ownerID != 0))
                {
                    isOwnersBase = true;
                }
                if (hit.collider.CompareTag("Water"))
                {
                    isTouchingWater = true; // If any point touches Water, it's valid
                }
                else if (hit.collider.CompareTag("Land"))
                {
                    isTouchingLand = true; // If any point touches Water, it's valid
                }
                else if (hit.collider.CompareTag("Vault"))
                {
                    isTouchingVault = true;
                }
                else if (hit.collider.CompareTag("BaseIcon"))
                {
                    isTouchingbaseIcon = true;
                }
                else if (hit.collider.CompareTag("VaultMid"))
                {
                    midVault = true;
                }
                else if (hit.collider.CompareTag("Base"))
                {
                    isTouchingBase = true; // If any point touches Base, mark it as touching base
                }
            }
        }

        if (isTouchingBase && isTouchingbaseIcon && !isOwnersBase)
        {
            return true;
        }
        if (isTouchingLand && isTouchingVault)
        {
            return true;
        }
        if (isTouchingLand && midVault)
        {
            return true;
        }
        if (isTouchingLand && !isTouchingWater)
        {
            return false; // Invalid if fully touching the Base without Water
        }

        // If the mermaid is touching both Water and Base, it's valid
        // But if it's fully on top of a Base and not touching Water, it's invalid
        if (isTouchingBase && !isTouchingWater)
        {
            return false; // Invalid if fully touching the Base without Water
        }

        return isTouchingWater || isTouchingBase || isTouchingLand || midVault || isTouchingVault || isTouchingbaseIcon; // Valid if touching Water or Base (with condition)
    }
}

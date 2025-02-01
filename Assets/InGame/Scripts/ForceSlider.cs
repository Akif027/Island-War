using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ForceSlider : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    public float maxForce = 100f; // Maximum force value
    public GameObject fillParent; // Parent object containing the Fill children
    public Sprite[] fillSprites;  // Array of sprites for each fill step
    public Sprite emptySprite;    // Sprite for empty state
    public RectTransform dragArea; // Area to calculate drag percentage
    public RectTransform pointer; // Pointer that follows the fill force

    private Image[] fillImages;
    private float currentFillPercent = 0.02f; // Current fill percentage (0 to 1)
    private bool isDragging = false;

    void Start()
    {
        // Get all Image components under the parent
        fillImages = fillParent.GetComponentsInChildren<Image>();

        // Set all images to empty initially
        foreach (var image in fillImages)
        {
            image.sprite = emptySprite;
        }

        Debug.Log("ForceArea initialized!");
    }

    void UpdateFill()
    {
        int totalImages = fillSprites.Length;
        int filledSteps = Mathf.FloorToInt(currentFillPercent * totalImages); // Determine fill steps

        for (int i = 0; i < fillImages.Length; i++)
        {
            if (i < filledSteps)
            {
                int spriteIndex = i % totalImages;
                fillImages[i].sprite = fillSprites[spriteIndex];
            }
            else
            {
                fillImages[i].sprite = emptySprite; // Empty state
            }
        }

        UpdatePointerPosition();
        Debug.Log($"Fill updated: {currentFillPercent * 100}%   ");
    }

    public float GetForce()
    {
        // Map currentFillPercent (0 to 1) to the desired range (10 to 100)
        return Mathf.Lerp(0.2f, maxForce, currentFillPercent);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return; // Ignore non-left clicks

        isDragging = true;
        Debug.Log("Pointer down!");
        UpdateDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            UpdateDrag(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        Debug.Log("Pointer up!");
    }

    private void UpdateDrag(PointerEventData eventData)
    {
        // Convert the pointer position to a local position within the drag area
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragArea,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        // Calculate the drag percentage based on the local X position
        float normalizedX = Mathf.Clamp01((localPoint.x - dragArea.rect.xMin) / dragArea.rect.width);
        currentFillPercent = normalizedX;

        Debug.Log($"Dragging: {currentFillPercent * 100}%");
        UpdateFill();
    }

    private void UpdatePointerPosition()
    {
        if (pointer == null) return;

        // Calculate the pointer's position based on the fill percentage
        float pointerX = Mathf.Lerp(0, dragArea.rect.width, currentFillPercent);
        pointer.anchoredPosition = new Vector2(pointerX, pointer.anchoredPosition.y);

        Debug.Log($"Pointer moved to X: {pointerX}");
    }
}

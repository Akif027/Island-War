using UnityEngine;
using UnityEngine.UI;

public class RulesManager : MonoBehaviour
{
   [Header("Content Panels")]
   public GameObject rulesContent;    // Reference to the Rules content GameObject
   public GameObject charactersContent; // Reference to the Characters content GameObject
   public GameObject controlsContent;  // Reference to the Controls content GameObject

   [Header("Buttons")]
   public Button rulesButton;         // Button for switching to Rules content
   public Button charactersButton;    // Button for switching to Characters content
   public Button controlsButton;      // Button for switching to Controls content

   [Header("Scroll Rect")]
   public ScrollRect scrollRect;      // Reference to the Scroll Rect component

   private void Start()
   {
      // Attach button click listeners
      rulesButton.onClick.AddListener(() => ShowContent(rulesContent));
      charactersButton.onClick.AddListener(() => ShowContent(charactersContent));
      controlsButton.onClick.AddListener(() => ShowContent(controlsContent));

      // Initialize by showing the first content (optional: default to RulesContent)
      ShowContent(rulesContent);
   }

   /// <summary>
   /// Shows the specified content and hides the others.
   /// </summary>
   /// <param name="contentToShow">The content to display</param>
   private void ShowContent(GameObject contentToShow)
   {
      SoundManager.Instance?.PlayButtonTap();
      // Hide all content panels
      rulesContent.SetActive(false);
      charactersContent.SetActive(false);
      controlsContent.SetActive(false);

      // Show the selected content
      contentToShow.SetActive(true);

      // Reset the scroll position to the top
      scrollRect.content = contentToShow.GetComponent<RectTransform>();
      scrollRect.verticalNormalizedPosition = 1f;
   }
}

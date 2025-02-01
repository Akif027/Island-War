using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HiringManager : MonoBehaviour
{
    [SerializeField] private GameObject characterCardPrefab;
    [SerializeField] private Transform gridLayout;
    [SerializeField] private Transform infoGridLayout;
    [SerializeField] private Transform playerIcon1;
    [SerializeField] private Transform playerIcon2;
    [SerializeField] private ScoreManager scoreManager;

    /// <summary>
    /// Opens the hiring panel and displays the current player's characters.
    /// </summary>
    public void OpenHiringPanel()
    {
        GameManager.Instance.GetPlayerInfo(GameManager.Instance.GetCurrentPlayer()).PopulateAndUpdateUnlockedCharacter();
        ShowPlayerCharacters(GameManager.Instance.GetCurrentPlayer(), gridLayout, false);
        UIManager.Instance.OpenHiringPanel();
    }

    /// <summary>
    /// Opens the information panel for the next player.
    /// </summary>
    public void OpenInfoPanel()
    {
        int currentPlayer = GameManager.Instance.GetCurrentPlayer();
        int nextPlayer = currentPlayer == 1 ? 2 : 1;
        GameManager.Instance.GetPlayerInfo(nextPlayer).PopulateAndUpdateUnlockedCharacter();
        UIManager.Instance.OpenInfoPanel();

        // Clear existing info cards
        foreach (Transform child in infoGridLayout)
        {
            Destroy(child.gameObject);
        }

        // Update player icons and display characters for the next player
        UpdatePlayerIcons(currentPlayer);
        ShowPlayerCharacters(nextPlayer, infoGridLayout, true); // Info panel has no buttons
    }

    /// <summary>
    /// Displays the characters of the specified player in the provided grid layout.
    /// </summary>
    private void ShowPlayerCharacters(int playerNumber, Transform layout, bool isForInfoPanel)
    {
        if (layout == null)
        {
            Debug.LogError("Grid layout is missing!");
            return;
        }

        // Clear existing cards in the grid layout
        foreach (Transform child in layout)
        {
            Destroy(child.gameObject);
        }

        // Get player characters
        Dictionary<CharacterData, bool> playerCharacters = GameManager.Instance.GetPlayerInfo(playerNumber).GetPlayerCards();

        if (playerCharacters == null || playerCharacters.Count == 0) return;

        // Create character cards for the player
        foreach (var entry in playerCharacters)
        {
            CharacterData characterData = entry.Key;
            bool isUnlocked = entry.Value;

            GameObject card = Instantiate(characterCardPrefab, layout);
            InitializeCharacterCard(card, characterData, isUnlocked, isForInfoPanel);
        }
    }

    /// <summary>
    /// Initializes a character card UI element.
    /// </summary>
    private void InitializeCharacterCard(GameObject card, CharacterData characterData, bool isUnlocked, bool isForInfoPanel)
    {
        if (card == null || characterData == null)
        {
            Debug.LogError("Card or CharacterData is null!");
            return;
        }

        card.name = characterData.characterName;

        // Set card image
        Image cardImage = card.GetComponent<Image>();
        if (cardImage != null) cardImage.sprite = characterData.characterCardSprite;

        if (isForInfoPanel)
        {
            // Remove any button components for info panel
            Button cardButton = card.GetComponent<Button>();
            if (cardButton != null)
            {
                Destroy(cardButton); // Remove button functionality
            }
            if (!isUnlocked)
            {
                cardImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Greyed-out color
            }
            else
            {
                cardImage.color = Color.white; // Normal color
            }
        }
        else
        {
            // Configure button functionality for hiring panel
            Button cardButton = card.GetComponent<Button>();
            if (cardButton != null)
            {
                cardButton.onClick.RemoveAllListeners();

                if (!isUnlocked)
                {
                    cardButton.onClick.AddListener(() => PurchaseCharacter(characterData));
                }
                else
                {
                    cardButton.interactable = false;
                }
            }
        }
    }

    /// <summary>
    /// Handles purchasing a character for the current player.
    /// </summary>
    private void PurchaseCharacter(CharacterData character)
    {
        int playerNumber = GameManager.Instance.GetCurrentPlayer();
        int playerCoins = scoreManager.GetCoins(playerNumber);

        if (playerCoins < character.CharacterCost)
        {
            Debug.Log($"Player {playerNumber} doesn't have enough coins to purchase {character.characterName}. ");
            return;
        }

        // Deduct coins and assign character
        EventManager.TriggerCoinDeduct(playerNumber, (int)character.CharacterCost);
        GameManager.Instance.InstantiateSinglePlayerToken(character, playerNumber);
        UIManager.Instance.CloseHiringPanel();
        Debug.Log($"{character.characterName} purchased by Player {playerNumber}.");
    }

    /// <summary>
    /// Updates the player icons in the info panel.
    /// </summary>
    private void UpdatePlayerIcons(int currentPlayer)
    {
        Sprite player1Icon = GameManager.Instance.GetPlayerIcon(1);
        Sprite player2Icon = GameManager.Instance.GetPlayerIcon(2);

        Image icon1Image = playerIcon1.GetComponent<Image>();
        Image icon2Image = playerIcon2.GetComponent<Image>();

        if (icon1Image != null && icon2Image != null)
        {
            icon1Image.sprite = currentPlayer == 1 ? player2Icon : player1Icon;
            icon2Image.sprite = currentPlayer == 2 ? player1Icon : player2Icon;
        }
    }

    /// <summary>
    /// Closes the information panel.
    /// </summary>
    public void CloseInfoPanel()
    {
        UIManager.Instance.CloseInfoPanel();
    }

    /// <summary>
    /// Closes the hiring panel.
    /// </summary>
    public void CloseHiringPanel()
    {
        UIManager.Instance.CloseHiringPanel();
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class TextMapper : MonoBehaviour
{
    [Header("Number Sprites")]
    [Tooltip("Array of sprites for 0-9.")]
    public Sprite[] numberSprites; // Array for glory points and turn
    public Sprite[] coinNumberSprites; // Array for coins

    [Header("Players Glory Points")]
    public Transform player1Container; // Parent with child Images (Horizontal Layout Group)
    public Transform player2Container; // Parent with child Images (Horizontal Layout Group)

    [Header("Turn Indicators")]
    public Transform turnContainer; // Parent with child Images (Horizontal Layout Group)
    public GameObject player1CoatOfArm;
    public GameObject player2CoatOfArm;

    [Header("Coins Display")]
    public Transform coinContainer; // Parent with child Images (Horizontal Layout Group)
    public Transform coinContainerOnHiringPanel; // Parent with child Images (Horizontal Layout Group)
    public Transform InfoContainer; // Parent with child Images (Horizontal Layout Group)
    public ScoreManager scoreManager;
    public Transform WinnercoinContainerPlayerOne; // Parent with child Images (Horizontal Layout Group)
    public Transform WinnerGloryContainerPlayerOne; // Parent with child Images (Horizontal Layout Group)
    public Transform WinnercoinContainerPlayerTwo; // Parent with child Images (Horizontal Layout Group)
    public Transform WinnerGloryContainerPlayerTwo; // Parent with child Images (Horizontal Layout Group)
    private void Start()
    {
        UpdateTurn(0);
        UpdateTurnIcon(1);
        AddPlayerGloryPoints(1, 0);
        AddPlayerGloryPoints(2, 0);

    }

    public void AddPlayerCoinsPoints(int amount)
    {
        UpdateNumberImages(amount, coinContainer, coinNumberSprites);
        UpdateNumberImages(amount, coinContainerOnHiringPanel, coinNumberSprites);
    }

    public void AddPlayerGloryPoints(int playerNo, int amount)
    {
        Transform targetContainer = playerNo == 1 ? player1Container : player2Container;
        UpdateNumberImages(amount, targetContainer, numberSprites);
        Transform targetWinnerContainer = playerNo == 1 ? WinnerGloryContainerPlayerOne : WinnerGloryContainerPlayerTwo;
        UpdateNumberImages(amount, targetWinnerContainer, numberSprites);
    }

    public void UpdateTurnIcon(Int32 playerNo)
    {
        SetActiveState(player1CoatOfArm, playerNo == 1);
        SetActiveState(player2CoatOfArm, playerNo == 2);

        UpdateNumberImages(scoreManager.GetCoins(playerNo), coinContainer, coinNumberSprites);
        UpdateNumberImages(scoreManager.GetCoins(playerNo), coinContainerOnHiringPanel, coinNumberSprites);
        UpdateNumberImages(scoreManager.GetCoins(playerNo == 1 ? 2 : 1), InfoContainer, coinNumberSprites);
        UpdateNumberImages(scoreManager.GetCoins(1), WinnercoinContainerPlayerOne, coinNumberSprites);
        UpdateNumberImages(scoreManager.GetCoins(2), WinnercoinContainerPlayerTwo, coinNumberSprites);
    }

    public void UpdateTurn(int amount)
    {
        if (amount == 0) return;
        UpdateNumberImages(amount, turnContainer, numberSprites);
    }

    /// <summary>
    /// Updates the images in the container to display the integer as individual digits.
    /// </summary>
    /// <param name="number">The integer to display.</param>
    /// <param name="container">The container with child Image components.</param>
    /// <param name="sprites">The sprite array corresponding to the digits 0-9.</param>
    private void UpdateNumberImages(int number, Transform container, Sprite[] sprites)
    {
        if (container == null)
        {
            Debug.LogError("Container is null! Ensure it is assigned in the Inspector.");
            return;
        }

        if (sprites == null || sprites.Length != 10)
        {
            Debug.LogError("Sprite array is null or not correctly set up! Ensure it contains exactly 10 sprites (0-9). ");
            return;
        }

        string numberString = number.ToString();
        for (int i = 0; i < container.childCount; i++)
        {
            Transform child = container.GetChild(i);
            Image childImage = child.GetComponent<Image>();

            if (i < numberString.Length)
            {
                int digit = numberString[i] - '0';
                if (digit >= 0 && digit < sprites.Length)
                {
                    childImage.sprite = sprites[digit];
                    childImage.enabled = true;
                }
                else
                {
                    Debug.LogError($"Invalid digit {digit} for number {number}. Check the sprite array.");
                }
            }
            else
            {
                childImage.enabled = false;
            }
        }
    }

    /// <summary>
    /// Sets the active state of a GameObject.
    /// </summary>
    /// <param name="obj">The GameObject to modify.</param>
    /// <param name="state">The desired active state.</param>
    private void SetActiveState(GameObject obj, bool state)
    {
        if (obj != null)
        {
            obj.SetActive(state);
        }
        else
        {
            Debug.LogError("GameObject reference is null!");
        }
    }
}

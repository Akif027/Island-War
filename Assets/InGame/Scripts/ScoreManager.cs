using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    private Dictionary<int, int> playerCoins = new Dictionary<int, int>();
    private Dictionary<int, int> playerGloryCoins = new Dictionary<int, int>(); // New dictionary for glory coins

    public TextMapper textMapper;

    public string coinTextFormat = "{0}"; // Customizable format string

    private void OnEnable()
    {
        EventManager.CoinAdd += OnCoinAdd;
        EventManager.CoinDeduct += OnCoinDeduct;
        EventManager.GloryPointAdd += OnGloryPointAdded;
        EventManager.GloryPointDeduct += OnGloryPointDeducted;
    }

    private void OnDisable()
    {
        EventManager.CoinAdd -= OnCoinAdd;
        EventManager.CoinDeduct -= OnCoinDeduct;
        EventManager.GloryPointAdd -= OnGloryPointAdded;
        EventManager.GloryPointDeduct -= OnGloryPointDeducted;
    }

    private void Start()
    {
        InitializePlayers();
    }

    private void InitializePlayers()
    {
        // Initialize coins and glory coins for both players
        for (int playerNumber = 1; playerNumber <= 2; playerNumber++) // Adjust for more players
        {
            if (!playerCoins.ContainsKey(playerNumber))
            {
                playerCoins[playerNumber] = 30; // Starting coins
                textMapper.AddPlayerCoinsPoints(playerCoins[playerNumber]);
            }

            if (!playerGloryCoins.ContainsKey(playerNumber))
            {
                playerGloryCoins[playerNumber] = 0; // Starting glory coins
                textMapper.AddPlayerGloryPoints(playerNumber, playerGloryCoins[playerNumber]);
            }
        }
    }

    public void OnCoinAdd(int playerNumber, int coinAmount)
    {

        SetupPlayer(playerNumber);
        playerCoins[playerNumber] += coinAmount;
        textMapper.AddPlayerCoinsPoints(playerCoins[playerNumber]);
        Debug.Log($"Player {playerNumber} added {coinAmount} coins. Total: {playerCoins[playerNumber]}.");
        GameManager.Instance.UpdateTurnIfo();
    }

    public void OnCoinDeduct(int playerNumber, int coinAmount)
    {
        SetupPlayer(playerNumber);
        playerCoins[playerNumber] = Mathf.Max(0, playerCoins[playerNumber] - coinAmount);
        textMapper.AddPlayerCoinsPoints(playerCoins[playerNumber]);
        Debug.Log($"Player {playerNumber} deducted {coinAmount} coins. Total: {playerCoins[playerNumber]}.");
    }

    public void OnGloryPointAdded(int playerNumber, int gloryPoints)
    {
        SetupGloryPlayer(playerNumber);
        playerGloryCoins[playerNumber] += gloryPoints;
        textMapper.AddPlayerGloryPoints(playerNumber, playerGloryCoins[playerNumber]);
        Debug.Log($"Player {playerNumber} added {gloryPoints} glory coins. Total: {playerGloryCoins[playerNumber]}.");
    }

    public void OnGloryPointDeducted(int playerNumber, int gloryPoints)
    {
        SetupGloryPlayer(playerNumber);
        playerGloryCoins[playerNumber] = Mathf.Max(0, playerGloryCoins[playerNumber] - gloryPoints);
        textMapper.AddPlayerGloryPoints(playerNumber, playerGloryCoins[playerNumber]);
        Debug.Log($"Player {playerNumber} deducted {gloryPoints} glory coins. Total: {playerGloryCoins[playerNumber]}.");
    }

    public int GetCoins(int playerNumber)
    {
        return playerCoins.TryGetValue(playerNumber, out int coins) ? coins : 0;
    }

    public int GetGloryCoins(int playerNumber)
    {
        return playerGloryCoins.TryGetValue(playerNumber, out int gloryCoins) ? gloryCoins : 0;
    }

    private void SetupPlayer(int playerNumber)
    {
        if (!playerCoins.ContainsKey(playerNumber))
        {
            playerCoins[playerNumber] = 0;
        }
    }

    private void SetupGloryPlayer(int playerNumber)
    {
        if (!playerGloryCoins.ContainsKey(playerNumber))
        {
            playerGloryCoins[playerNumber] = 0;
        }
    }
}

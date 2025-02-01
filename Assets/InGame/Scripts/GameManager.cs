using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance for global access
    public GameData gameData; // Reference to the GameData ScriptableObject
    public DraftManager draftManager;

    public Int32 currentPlayer; // Current player's turn (1 or 2)

    [Header("Phase Management")]
    public GamePhase currentPhase; // Current phase of the game

    [Header("Player Info")]
    public Dictionary<int, Sprite> playerIcons = new Dictionary<int, Sprite>(); // Maps player numbers to their icons
    public Dictionary<int, Color> SetColors = new Dictionary<int, Color>(); // Maps player numbers to their icons
    public List<PlayerInfo> playerInfos = new List<PlayerInfo>();
    public TextMapper textMapper;
    public Transform spawnTokenPositionPlayer1;
    public Transform spawnTokenPositionPlayer2;
    public Transform spawnTokenPositionIsland2Player1;
    public Transform spawnTokenPositionIsland3Player1;
    public Transform spawnTokenPositionIsland2Player2;
    public Transform spawnTokenPositionIsland3Player2;
    public Transform MermaidSpawnAreaPlayer1;
    public Transform MermaidSpawnAreaPlayer2;
    bool isGameover = false;
    public ScoreManager scoreManager;
    int winner;

    void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Start with the Coin Toss phase
        currentPhase = GamePhase.CoinToss;
        InitializeCoinToss();
        EventManager.Subscribe("OnTurnEnd", HandleTurn);
    }
    void OnDisable()
    {
        EventManager.Unsubscribe("OnTurnEnd", HandleTurn);
        playerIcons.Clear();

    }
    private void HandleTurn()
    {
        Debug.LogWarning("Player turn Changed");
        if (IsTurnLimitReachedOrTokensEmpty())
        {

            return;
        }
        SoundManager.Instance?.PlayTurnChange();
        incrementCurrentPlayerTurn();
        CheckAndRewardThiefVaultInteraction();


        ChangePlayerTurn(currentPlayer == 1 ? 2 : 1);

        MapScroll.Instance.SmoothTransitionToPosition(
            currentPlayer == 1
                ? spawnTokenPositionPlayer1.position
                : spawnTokenPositionPlayer2.position,
            0.5f);

        if (currentPhase == GamePhase.GamePlay) UIManager.Instance.EnablePlayLoyOut();
        MapScroll.Instance.EnableScroll();
    }

    private void TriggerGameOver()
    {

        winner = GetWinnerPlayerNumber();
        Debug.Log($"Game Over! Winner is Player {winner}");
        ChangePhase(GamePhase.Gameover);
    }

    void Update()
    {

        if (IsTurnLimitReachedOrTokensEmpty() && !isGameover)
        {
            Debug.LogError("GameOVer");
            TriggerGameOver();
            isGameover = true;
        }
    }

    private void InitializeCoinToss()
    {
        Debug.Log("Phase 1: Coin Toss");

    }

    public void OnTossResult(Sprite winnerSprite, Sprite loserSprite)
    {
        SetPlayerIcons(1, winnerSprite);
        SetPlayerIcons(2, loserSprite);

        ChangePhase(GamePhase.Elimination);
        draftManager?.TransitionToPhase(GamePhase.Elimination);


    }

    public Sprite GetCurrentPlayerIcon()
    {
        // PrintAllPlayerIcons();
        return GetPlayerIcon(currentPlayer);
    }

    public void ChangePlayerTurn(int playerNumber)
    {

        Debug.LogWarning("Chaning player turn to " + playerNumber);
        if (!playerIcons.ContainsKey(playerNumber))
        {
            Debug.LogError($"Invalid player number: {playerNumber}. Must be a valid key in playerIcons ");
            return;
        }


        currentPlayer = playerNumber;

        UpdateTurnIfo();

        Debug.Log($"Player turn changed to: Player {currentPlayer}");
    }
    public void PrintAllPlayerIcons()
    {
        foreach (var entry in playerIcons)
        {
            string iconName = entry.Value != null ? entry.Value.name : "None";
            Debug.Log($"Player {entry.Key}: Icon {iconName}");
        }
    }
    public void UpdateTurnIfo()
    {
        if (currentPhase == GamePhase.GamePlay || playerInfos.Count != 0)
        {
            textMapper.UpdateTurnIcon(currentPlayer);
            //  textMapper.UpdateTurn(GetPlayerHasTurnCount());
        }
    }
    /// <summary>
    /// Checks if a thief token is still triggering the vault.
    /// Awards 5 coins to the player if a thief token is interacting with the vault.
    /// </summary>
    public void CheckAndRewardThiefVaultInteraction()
    {

        foreach (PlayerInfo playerInfo in playerInfos)
        {
            foreach (Token token in playerInfo.tokens)
            {
                if (token == null || token.characterData == null) continue;

                // Check if the token is a thief and is interacting with a vault
                if (token.characterData.characterType == CharacterType.Thief && IsTokenTouchingBase(token)/* && token.IsCurrentPlayerOwner()*/)
                {
                    EventManager.TriggerGloryPointAdd(token.owner, 5);
                    Debug.LogError($"Player {token.owner} has captured the base.");
                    SoundManager.Instance?.PlayScore();

                }
            }
        }
    }

    /// <summary>
    /// Determines if a token is currently touching a vault.
    /// </summary>
    /// <param name="token">The token to check.</param>
    /// <returns>True if the token is touching a vault; otherwise, false.</returns>
    private bool IsTokenTouchingBase(Token token)
    {
        float radius = token.GetComponent<CircleCollider2D>().radius;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(token.transform.position, radius);

        foreach (Collider2D collider in colliders)
        {
            Base baseS = collider.GetComponentInParent<Base>();

            if (collider.CompareTag("BaseIcon") && baseS.ownerID != token.owner)
            {
                return true; // Token is interacting with a vault
            }
        }
        return false;
    }

    public void SetPlayerIcons(int playerNumber, Sprite icon)
    {
        if (playerIcons.ContainsKey(playerNumber))
        {
            playerIcons[playerNumber] = icon;

            // Assign color based on the icon's name
            SetColors[playerNumber] = icon.name == "Yellow" ? Color.yellow : Color.blue;
        }
        else
        {
            playerIcons.Add(playerNumber, icon);

            // Ensure the color is added for the new player as well
            SetColors[playerNumber] = icon.name == "Yellow" ? Color.yellow : Color.blue;
        }
    }
    public Color GetPlayerColor(int playerNumber)
    {
        if (SetColors.TryGetValue(playerNumber, out var Col))
        {
            return Col;
        }

        throw new Exception($"Player {playerNumber} not found!");
    }
    public void PrintAllSetColors()
    {
        foreach (var entry in SetColors)
        {
            Debug.Log($"Player {entry.Key}: Color {entry.Value}");
        }
    }

    public Sprite GetPlayerIcon(int playerNumber)
    {
        if (playerIcons.TryGetValue(playerNumber, out var icon))
        {
            return icon;
        }

        throw new Exception($"Player {playerNumber} not found!");
    }

    public Int32 GetCurrentPlayer()
    {
        return currentPlayer;
    }

    /// <summary>
    /// Set or update PlayerInfo for a specific player.
    /// </summary>
    public void SetPlayerInfo(int playerNumber, PlayerInfo playerInfo)
    {
        var existingPlayerInfo = playerInfos.Find(p => p.PlayerNumber == playerNumber);

        if (existingPlayerInfo != null)
        {
            // Ensure tokens from the new PlayerInfo are added to the existing list
            if (playerInfo.tokens != null)
            {
                foreach (var token in playerInfo.tokens)
                {
                    if (!existingPlayerInfo.tokens.Contains(token))
                    {
                        existingPlayerInfo.tokens.Add(token);
                    }
                }
            }

            Debug.Log($"Updated PlayerInfo for Player {playerNumber}.");
        }
        else
        {
            playerInfos.Add(playerInfo);
            Debug.Log($"Added PlayerInfo for Player {playerNumber}.");
        }
    }


    /// <summary>
    /// Get PlayerInfo by player number.
    /// </summary>
    public PlayerInfo GetPlayerInfo(int playerNumber)
    {



        var playerInfo = playerInfos.Find(p => p.PlayerNumber == playerNumber);

        if (playerInfo == null)
        {
            Debug.LogError($"PlayerInfo for Player {playerNumber} not found! or not initialized");
        }

        if (playerInfo == null)
        {
            Debug.LogError($"PlayerInfo for Player {playerNumber} not found! or not initilzed");
        }

        return playerInfo;
    }

    public CharacterData GetCharacter(string name)
    {
        if (gameData == null)
        {
            Debug.LogError("GameData is not assigned!");
            return null;
        }

        return gameData.GetCharacterByName(name);
    }

    public CharacterData GetCharacter(int index)
    {
        if (gameData == null)
        {
            Debug.LogError("GameData is not assigned!");
            return null;
        }

        return gameData.GetCharacterByIndex(index);
    }



    public void ChangePhase(GamePhase newPhase)
    {

        currentPhase = newPhase;
        switch (newPhase)
        {

            case GamePhase.Gameover:
                gameFinised();
                break;
        }

    }
    private void gameFinised()
    {
        Debug.LogWarning("Winner is " + winner);

        if (winner != 0)
        {

            UIManager.Instance.OnWinningPanel(winner);
        }


    }
    public GamePhase getCurrentPhase()
    {
        return currentPhase;
    }

    public void InstantiateSinglePlayerToken(CharacterData character, int playerNumber)
    {
        Transform spawnPosition;

        if (character.characterType != CharacterType.Mermaid)
        {
            // Define an array of spawn positions based on the player number
            Transform[] spawnPositions = playerNumber == 1
                ? new[] { spawnTokenPositionPlayer1, spawnTokenPositionIsland2Player1, spawnTokenPositionIsland3Player1 }
                : new[] { spawnTokenPositionPlayer2, spawnTokenPositionIsland2Player2, spawnTokenPositionIsland3Player2 };

            // Randomly pick one from the available positions
            spawnPosition = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Length)];
        }
        else
        {
            // Use specific spawn areas for mermaids
            spawnPosition = playerNumber == 1 ? MermaidSpawnAreaPlayer1 : MermaidSpawnAreaPlayer2;
        }


        // Instantiate the token at the adjusted spawn position
        GameObject token = Instantiate(character.TokenPrefab, spawnPosition.position, Quaternion.identity, spawnPosition);

        // Force the Z-position after instantiation
        Vector3 enforcedPosition = new Vector3(token.transform.position.x, token.transform.position.y, 0);
        token.transform.position = enforcedPosition;

        var placementManager = token.GetComponent<PlacementManager>();
        var tokenComponent = token.GetComponent<Token>();

        if (tokenComponent != null)
        {
            tokenComponent.characterData = character;
            tokenComponent.owner = playerNumber;

        }

        if (placementManager != null)
        {

            placementManager.isTokenPlaced = false;
        }
     ;
        if (tokenComponent != null)
        {
            tokenComponent.IsUnlocked = true;
            Debug.LogError($"Token instantiated for Player   {playerNumber}: {character.characterName}");
        }

        PlayerInfo playerInfo = GetPlayerInfo(playerNumber);
        if (playerInfo == null)
        {
            playerInfo = new PlayerInfo(playerNumber, tokenComponent);
            SetPlayerInfo(playerNumber, playerInfo);
        }

        playerInfo.tokens.Add(tokenComponent);
        GameManager.Instance.ChangePhase(GamePhase.Placement);
        //    draftManager.HandleSingleTokenPlaced(placementManager);

        MapScroll.Instance.SmoothTransitionToPosition(token.transform.position, 0.5f);
    }


    public void AddTokenToPlayer(int playerNumber, CharacterType characterType)
    {
        PlayerInfo playerInfo = GetPlayerInfo(playerNumber);
        if (playerInfo == null)
        {
            Debug.LogError($"PlayerInfo for player {playerNumber} not found!");
            return;
        }

        CharacterData character = gameData.characters.FirstOrDefault(c => c.characterType == characterType);
        if (character == null)
        {
            Debug.LogError($"Character with type {characterType} not found in gameData! ");
            return;
        }

        Token newToken = new Token { characterData = character };
        playerInfo.tokens.Add(newToken);
        Debug.Log($"Added token of type {characterType} to Player {playerNumber}.");
    }
    public void RemoveTokenFromPlayer(int playerNumber, CharacterType characterType)
    {
        PlayerInfo playerInfo = GetPlayerInfo(playerNumber);
        if (playerInfo == null)
        {
            Debug.LogError($"PlayerInfo for player {playerNumber} not found!");
            return;
        }

        Token tokenToRemove = playerInfo.tokens.FirstOrDefault(t => t.characterData != null && t.characterData.characterType == characterType);
        if (tokenToRemove == null)
        {
            Debug.LogError($"Token with type {characterType} not found for Player {playerNumber}!");
            return;
        }
        playerInfo.AddremoveCharterToLockedList(tokenToRemove);
        playerInfo.tokens.Remove(tokenToRemove);

        // Immediately check for game over
        // if (IsTurnLimitReachedOrTokensEmpty())
        // {
        //     TriggerGameOver();
        // }
        Debug.Log($"Removed token of type {characterType} from Player {playerNumber}.");
    }


    public List<Token> GetAllCurrentTokensOfPlayer(int playerNumber)
    {
        // Get the PlayerInfo for the specified player
        PlayerInfo playerInfo = GetPlayerInfo(playerNumber);
        if (playerInfo == null)
        {
            Debug.LogError($"PlayerInfo for player {playerNumber} not found!");
            return new List<Token>(); // Return empty list if PlayerInfo doesn't exist
        }

        // Return the tokens of the specified player
        Debug.Log($"Player {playerNumber} has {playerInfo.tokens.Count} tokens.");
        return playerInfo.tokens;
    }
    public int GetPlayerHasTurnCount()
    {
        PlayerInfo playerInfo = GetPlayerInfo(currentPlayer);
        if (playerInfo == null)
        {
            Debug.LogError($"PlayerInfo for current player not found!");
            return 0; // Return a default value, such as 0
        }
        return playerInfo.HasTurn;
    }

    public int GetWinnerPlayerNumber()
    {
        // Retrieve PlayerInfo for both players
        PlayerInfo player1Info = GetPlayerInfo(1);
        PlayerInfo player2Info = GetPlayerInfo(2);



        // Check if any player has zero tokens (loser)
        if (player1Info.tokens.Count == 0)
        {
            Debug.LogError(player1Info.tokens.Count);
            return 2; // Player 2 wins
        }
        if (player2Info.tokens.Count == 0)
        {
            Debug.LogError(player2Info.tokens.Count);
            return 1; // Player 1 wins
        }

        // Check if both players have reached the turn limit

        int player1Glory = scoreManager.GetGloryCoins(1);
        int player2Glory = scoreManager.GetGloryCoins(2);

        if (player1Glory > player2Glory) return 1; // Player 1 wins
        if (player2Glory > player1Glory) return 2; // Player 2 wins

        // Handle tie in glory points
        Debug.LogError("It's a tie in glory points!");
        return -1; // Indicate a tie or decide another tie-breaking logic

    }
    public bool IsTurnLimitReachedOrTokensEmpty()
    {
        // Get the PlayerInfo for the current player
        PlayerInfo playerInfo = GetPlayerInfo(1);
        PlayerInfo playerInfo2 = GetPlayerInfo(2);
        if (playerInfo == null || playerInfo2 == null)
        {
            Debug.LogError($"PlayerInfo for Player {currentPlayer} not found!");
            return false;
        }

        // Check if HasTurn has reached 20 or the token list is empty
        if (playerInfo.HasTurn >= 20 || playerInfo.tokens.Count <= 0 || playerInfo2.HasTurn >= 20 || playerInfo2.tokens.Count <= 0)
        {
            return true;
        }


        return false;
    }
    public void incrementCurrentPlayerTurn()
    {
        if (currentPhase != GamePhase.GamePlay && playerInfos != null) return;
        PlayerInfo playerInfo = GetPlayerInfo(currentPlayer);

        playerInfo.HasTurn++;
        textMapper.UpdateTurn(playerInfo.HasTurn);

    }
}

[System.Serializable]
public class PlayerInfo
{
    public Int32 PlayerNumber;
    public List<Token> tokens = new List<Token>();
    public List<CharacterData> UnlockedCharacter = new List<CharacterData>();
    public List<CharacterData> lockedCharacter = new List<CharacterData>();
    private Dictionary<CharacterData, bool> PlayerCards = new Dictionary<CharacterData, bool>();
    [SerializeField] private int _hasTurn;

    /// <summary>
    /// Gets or sets the HasTurn value with a maximum cap of 20.
    /// </summary>
    public int HasTurn
    {
        get => _hasTurn;
        set
        {
            _hasTurn = Mathf.Clamp(value, 0, 20); // Ensure HasTurn stays between 0 and 20
            if (value > 20)
            {
                Debug.LogWarning($"Player {PlayerNumber}'s HasTurn capped at 20.");
            }
        }
    }
    public void PopulateAndUpdateUnlockedCharacter()
    {

        UnlockedCharacter.Clear();
        foreach (var item in tokens)
        {
            UnlockedCharacter.Add(item.characterData);
        }
        for (int i = lockedCharacter.Count - 1; i >= 0; i--)
        {
            // Check if any token has a matching characterData
            if (tokens.Any(token => token.characterData == lockedCharacter[i]))
            {
                // Remove the matching characterData from lockedCharacter
                lockedCharacter.RemoveAt(i);
            }
        }
    }

    public void PopulateLockedList()
    {
        foreach (var item in tokens)
        {

            UnlockedCharacter.Add(item.characterData);
        }
    }
    public void AddremoveCharterToLockedList(Token token)
    {
        lockedCharacter.Add(token.characterData);

    }

    public PlayerInfo(Int32 PlayerNumber_, Token _tokens)
    {

        PlayerNumber = PlayerNumber_;
        tokens.Add(_tokens);// Initialize tokens if null
        Debug.Log($"{tokens.Count} tokens initialized for Player {PlayerNumber_}");



    }

    public Dictionary<CharacterData, bool> GetPlayerCards()
    {
        UpdatePlayerCards();
        // Debug.LogError($"playerCharacters {PlayerCards.Count} : ");
        return PlayerCards;
    }
    public void UpdatePlayerCards()
    {
        PlayerCards.Clear(); // Clear the dictionary before adding new entries

        // Add unlocked characters with status 'true'
        foreach (var unlockedCharacter in UnlockedCharacter)
        {
            PlayerCards[unlockedCharacter] = true;
        }

        // Add locked characters with status 'false'
        foreach (var lockedCharacter in lockedCharacter)
        {
            PlayerCards[lockedCharacter] = false;
        }

        // Optional: Debug log to show the current state of PlayerCards
        foreach (var entry in PlayerCards)
        {
            Debug.Log($"Character: {entry.Key.ability}, Status: {entry.Value}");
        }
    }

    // public List<CharacterData> GetAllCharacterDataOfPlayer(int playerNumber)
    // {
    //     List<CharacterData> playerTokens = new List<CharacterData>();


    // }
}

public enum GamePhase
{
    CoinToss,
    Elimination,
    Selection,
    Placement,
    GamePlay,
    Gameover
}

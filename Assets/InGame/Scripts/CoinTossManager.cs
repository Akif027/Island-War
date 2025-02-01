using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CoinTossManager : MonoBehaviour
{
    public GameObject coin1;
    public GameObject coin2;
    public float tossDuration = 2f; // Duration of the flip animation
    public float rotationSpeed = 2f; // Speed of the coin rotation
    public GameObject coinPanel;

    [Header("Text Prefab Settings")]
    public GameObject textPrefab; // Assign your Text prefab in the Inspector
    public Canvas parentCanvas; // Parent Canvas for the text prefab
    public Vector3 spawnPosition; // Position where the text will appear
    public float textAnimationDuration = 2f; // Duration of the text animation

    private GameObject activeCoin;
    private GameObject losingCoin;
    private int winner; // Integer for the winner (1 or 2)
    private Sprite winnerSprite; // Sprite of the winning coin
    private Sprite loserSprite; // Sprite of the losing coin

    void Start()
    {
        // Offset coins and set unique rotations
        coin1.transform.position += new Vector3(-0.5f, 0, 0);
        coin2.transform.position += new Vector3(0.5f, 0, 0);

        coin1.transform.rotation = Quaternion.Euler(0, 0, 0);
        coin2.transform.rotation = Quaternion.Euler(0, 180, 0);

        StartToss();
    }

    public void StartToss()
    {
        // Determine the winning coin and set winner as an integer
        activeCoin = Random.Range(0, 2) == 0 ? coin1 : coin2;
        losingCoin = activeCoin == coin1 ? coin2 : coin1;
        winner = activeCoin == coin1 ? 1 : 2;
        SoundManager.Instance?.PlayCoinToss();
        // Get the sprite from the Image component of the winning coin
        winnerSprite = activeCoin.GetComponent<Image>().sprite;
        loserSprite = losingCoin.GetComponent<Image>().sprite;

        coin1.SetActive(true);
        coin2.SetActive(true);

        // Start the toss animation
        StartTossAnimation();
    }

    private void StartTossAnimation()
    {
        // Animate both coins
        DoTweenHelper.CoinFlip(coin1, tossDuration, rotationSpeed);
        DoTweenHelper.CoinFlip(coin2, tossDuration, rotationSpeed);

        // Highlight the winning coin after the animation
        DOVirtual.DelayedCall(tossDuration, () =>
        {
            HighlightWinner();
            SoundManager.Instance?.PlayCoinTossStopped();
        });
    }

    private void HighlightWinner()
    {
        // Ensure only the active coin is visible
        coin1.SetActive(false);
        coin2.SetActive(false);
        activeCoin.SetActive(true);

        // Add a bounce effect to the winning coin
        DoTweenHelper.Bounce(activeCoin, 1.2f, 1f);

        // Animate the winner text
        DoTweenHelper.AnimateText(
            textPrefab,
            parentCanvas,
            spawnPosition,
            "Winner!!",
            textAnimationDuration,
            3
        );

        // Trigger the event after a short delay
        DOVirtual.DelayedCall(1.5f, () =>
        {
            GameManager.Instance.OnTossResult(winnerSprite, loserSprite);
            coinPanel.SetActive(false);
        });
    }

}


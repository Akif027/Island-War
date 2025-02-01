using DG.Tweening;
using TMPro;
using UnityEngine;
using System;

public static class DoTweenHelper
{
    /// <summary>
    /// Animates a coin flip by rotating it around its Y-axis.
    /// </summary>
    /// <param name="coin">The coin GameObject to animate.</param>
    /// <param name="duration">The duration of the flip animation.</param>
    /// <param name="rotationSpeed">The speed of the rotation.</param>
    public static void CoinFlip(GameObject coin, float duration, float rotationSpeed)
    {
        coin.transform.DORotate(new Vector3(0, 360 * rotationSpeed, 0), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // Ensure the coin is perfectly aligned after the flip
                coin.transform.rotation = Quaternion.Euler(0, 0, 0);
            });
    }

    /// <summary>
    /// Scales an object up and down to create a bounce effect.
    /// </summary>
    /// <param name="target">The target GameObject to scale.</param>
    /// <param name="scaleMultiplier">The scale multiplier for the bounce.</param>
    /// <param name="duration">The duration of the bounce animation.</param>
    public static void Bounce(GameObject target, float scaleMultiplier, float duration)
    {
        target.transform.DOScale(target.transform.localScale * scaleMultiplier, duration / 2)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                target.transform.DOScale(target.transform.localScale / scaleMultiplier, duration / 2).SetEase(Ease.InQuad);
            });
    }

    /// <summary>
    /// Moves an object to a target position with easing.
    /// </summary>
    /// <param name="target">The target GameObject to move.</param>
    /// <param name="targetPosition">The position to move to.</param>
    /// <param name="duration">The duration of the move.</param>
    public static void MoveTo(GameObject target, Vector3 targetPosition, float duration)
    {
        target.transform.DOMove(targetPosition, duration).SetEase(Ease.InOutQuad);
    }

    /// <summary>
    /// Fades the alpha of a UI Image.
    /// </summary>
    /// <param name="image">The UI Image to fade.</param>
    /// <param name="targetAlpha">The target alpha value.</param>
    /// <param name="duration">The duration of the fade animation.</param>
    public static void FadeImage(UnityEngine.UI.Image image, float targetAlpha, float duration)
    {
        image.DOFade(targetAlpha, duration).SetEase(Ease.InOutQuad);
    }
    /// <summary>
    /// Animates a TextMeshProUGUI prefab with scaling and fading effects.
    /// </summary>
    /// <param name="textPrefab">The text prefab to animate.</param>
    /// <param name="parentCanvas">The parent canvas to instantiate the text under.</param>
    /// <param name="spawnPosition">The position where the text will appear.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="animationDuration">The total duration of the animation.</param>
    /// <param name="scaleFactor">The maximum scale factor during the animation.</param>
    public static void AnimateText(GameObject textPrefab, Canvas parentCanvas, Vector3 spawnPosition, string message, float animationDuration, float scaleFactor)
    {
        // Instantiate the text prefab under the parent canvas
        GameObject textInstance = UnityEngine.Object.Instantiate(textPrefab, parentCanvas.transform);

        // Set the position within the canvas
        RectTransform textRect = textInstance.GetComponent<RectTransform>();
        if (textRect != null)
        {
            textRect.anchoredPosition = spawnPosition;
        }

        // Set the text message
        TextMeshProUGUI textComponent = textInstance.GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = message;
            textComponent.alpha = 0; // Start fully transparent
        }

        // Create the animation sequence
        Sequence textSequence = DOTween.Sequence();
        textSequence.Append(textInstance.transform.DOScale(Vector3.one * scaleFactor, animationDuration / 2).SetEase(Ease.OutBack));
        if (textComponent != null)
        {
            textSequence.Join(textComponent.DOFade(1f, animationDuration / 2).SetEase(Ease.InOutQuad));
        }
        textSequence.Append(textInstance.transform.DOScale(Vector3.one, animationDuration / 2).SetEase(Ease.InBack));
        if (textComponent != null)
        {
            textSequence.Append(textComponent.DOFade(0f, animationDuration / 2).SetEase(Ease.InQuad));
        }

        // Destroy the text instance after the animation completes
        textSequence.OnComplete(() =>
        {
            UnityEngine.Object.Destroy(textInstance);
        });
    }

    /// <summary>
    /// Animates a GameObject: scales it up, moves it to the target, and scales it down to zero.
    /// </summary>
    /// <param name="obj">The object to animate.</param>
    /// <param name="target">The target position.</param>
    /// <param name="duration">The duration of the animation.</param>
    /// <param name="scaleFactor">The scale factor during the animation.</param>
    public static void FlyToTarget(GameObject obj, Transform target, float duration, float scaleFactor, System.Action onComplete = null)
    {
        if (obj == null || target == null) return;

        // Create a sequence for the animation
        Sequence sequence = DOTween.Sequence();

        // Scale up at the start
        sequence.Append(obj.transform.DOScale(Vector3.one * scaleFactor, duration / 3).SetEase(Ease.OutBack));

        // Move to the target while maintaining scale
        sequence.Append(obj.transform.DOMove(target.position, duration / 3).SetEase(Ease.InOutQuad));

        // Scale down to zero at the end
        sequence.Append(obj.transform.DOScale(Vector3.zero, duration / 3).SetEase(Ease.InBack));

        // Destroy the object after the animation completes
        sequence.OnComplete(() =>
        {
            UnityEngine.Object.Destroy(obj);
            onComplete?.Invoke(); // Invoke the callback if provided
        });
    }

    public static void ScaleToZero(GameObject target, float duration, Action onComplete = null)
    {
        if (target == null) return;

        Debug.Log($"Scaling {target.name} to zero over {duration} seconds.");

        target.transform.DOScale(Vector3.zero, duration)
            .SetEase(Ease.InBack) // Use a smooth easing for better animation
            .OnComplete(() =>
            {
                Debug.Log($"{target.name} scaled to zero. Invoking onComplete callback.");
                onComplete?.Invoke();
            });
    }


    public static void FadePanel(CanvasGroup panel, bool fadeIn, float duration, System.Action onComplete = null)
    {
        if (panel == null) return;

        // Set the target alpha
        float targetAlpha = fadeIn ? 1f : 0f;

        // Enable the panel if fading in
        if (fadeIn) panel.gameObject.SetActive(true);

        // Animate the alpha value
        panel.DOFade(targetAlpha, duration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            // Disable the panel after fading out
            if (!fadeIn) panel.gameObject.SetActive(false);

            // Invoke the callback if provided
            onComplete?.Invoke();
        });
    }
    /// <summary>
    /// Fades the alpha of a UI TextMeshPro.
    /// </summary>
    /// <param name="text">The TextMeshPro component to fade.</param>
    /// <param name="targetAlpha">The target alpha value.</param>
    /// <param name="duration">The duration of the fade animation.</param>
    public static void FadeText(TMPro.TextMeshProUGUI text, float targetAlpha, float duration)
    {
        text.DOFade(targetAlpha, duration).SetEase(Ease.InOutQuad);
    }
}

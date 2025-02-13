using System.Collections;
using UnityEngine;
using TMPro;              
using DG.Tweening;     

public class CountdownTimer : MonoBehaviour
{
    
    [SerializeField, Tooltip("Number of seconds to count down from.")]
    private int countdownSeconds = 5;

    [SerializeField, Tooltip("Reference to the TextMeshProUGUI component displaying the countdown.")]
    private TextMeshProUGUI countdownText;

    /// <summary>
    /// Validates dependencies and starts the countdown coroutine.
    /// </summary>
    private void Start()
    {
        if (countdownText == null)
        {
            Debug.LogError($"{nameof(CountdownTimer)}: Countdown text reference is not assigned.", this);
        }
        StartCoroutine(CountdownCoroutine());
    }

    /// <summary>
    /// Coroutine that updates the countdown display every second using integer values,
    /// plays a scaling effect on each update, invokes an event when finished, and
    /// destroys the GameObject.
    /// </summary>
    private IEnumerator CountdownCoroutine()
    {
        int remainingSeconds = countdownSeconds;

        while (remainingSeconds >= 0)
        {
            UpdateCountdownText(remainingSeconds);
            yield return new WaitForSeconds(1f);
            remainingSeconds--;
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Updates the countdown text UI element with the current remaining seconds
    /// and triggers a scaling animation.
    /// </summary>
    /// <param name="time">The current time (in seconds) to display.</param>
    private void UpdateCountdownText(int time)
    {
        if (countdownText != null)
        {
            countdownText.text = time.ToString();
            PlayScaleEffect();
        }
    }

    /// <summary>
    /// Plays a scaling effect on the countdown text using DOTween.
    /// The text scales up briefly and then returns to its original size.
    /// </summary>
    private void PlayScaleEffect()
    {
        if (countdownText != null)
        {
            // Cancel any running tween on the rectTransform to avoid overlap
            countdownText.rectTransform.DOKill();

            // Ensure starting scale is 1
            countdownText.rectTransform.localScale = Vector3.one;

            // Create a tween sequence: scale up then back down
            Sequence scaleSequence = DOTween.Sequence();
            scaleSequence.Append(countdownText.rectTransform.DOScale(1.5f, 0.15f).SetEase(Ease.InOutSine));
            scaleSequence.Append(countdownText.rectTransform.DOScale(1f, 0.15f).SetEase(Ease.InOutSine));
        }
    }
}

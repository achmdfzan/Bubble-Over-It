using UnityEngine;
using DG.Tweening; // DOTween namespace

public class StartMenuAnimation : MonoBehaviour
{
    [SerializeField] private GameObject startCanvas; // Reference to the start canvas
    [SerializeField] private float fadeDuration = 1f; // Duration of the fade-in animation
    [SerializeField] private float moveDistance = 100f; // Distance to move upwards
    [SerializeField] private float moveDuration = 1f; // Duration of the movement animation

    [SerializeField] private Ease moveEase = Ease.OutQuad; // Easing for movement
    [SerializeField] private Ease fadeEase = Ease.Linear; // Easing for fade-in

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // Get the CanvasGroup component from startCanvas
        canvasGroup = startCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup component is missing on the startCanvas GameObject.");
        }
    }

    public void AnimateStartCanvas()
    {
        if (canvasGroup == null) return;

        // Ensure the CanvasGroup is initially invisible
        canvasGroup.alpha = 1f;

        // Animate position and fade-in
        startCanvas.transform.DOMoveY(startCanvas.transform.position.y + moveDistance, moveDuration)
            .SetEase(moveEase); // Use chosen easing for movement

        canvasGroup.DOFade(0f, fadeDuration)
            .SetEase(fadeEase) // Use chosen easing for fade
            .OnComplete(() =>
            {
                // Disable the startCanvas GameObject after animation
                gameObject.SetActive(false);
            });
    }
}

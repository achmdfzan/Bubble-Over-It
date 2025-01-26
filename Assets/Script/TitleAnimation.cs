using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Import DOTween namespace

public class TitleAnimation : MonoBehaviour
{
    public Image[] images; // Array of images to animate
    public float bounceHeight = 50f; // Height of the bounce
    public float animationDuration = 0.5f; // Duration of the jump animation
    public float delayBetweenImages = 0.2f; // Delay between animations for each image
    public float idleScaleAmount = 0.05f; // Amount to scale during idle animation
    public float idleDuration = 1f; // Duration of the idle animation

    void Start()
    {
        // Start idle animations for all images
        foreach (Image img in images)
        {
            StartIdleAnimation(img);
        }

        // Start the looping jump animation
        StartAnimationLoop();
    }

    void StartAnimationLoop()
    {
        float delay = 0f; // Initial delay

        // Loop through each image in the array and animate with delay
        foreach (Image img in images)
        {
            AnimateBounce(img, delay);
            delay += delayBetweenImages; // Increment the delay for the next image
        }

        // Restart the animation for all images after the last animation finishes
        float totalDuration = delay + animationDuration * 2; // Total time including delays and animation duration
        Invoke(nameof(StartAnimationLoop), totalDuration); // Recursively restart the loop
    }

    void AnimateBounce(Image img, float startDelay)
    {
        // Sequence to create the bounce animation
        Sequence bounceSequence = DOTween.Sequence();

        // Add a delay at the start of the animation
        bounceSequence.AppendInterval(startDelay);

        // Move the image up by bounceHeight
        bounceSequence.Append(img.rectTransform.DOAnchorPosY(img.rectTransform.anchoredPosition.y + bounceHeight, animationDuration)
            .SetEase(Ease.OutQuad));

        // Move the image back to its original position
        bounceSequence.Append(img.rectTransform.DOAnchorPosY(img.rectTransform.anchoredPosition.y, animationDuration)
            .SetEase(Ease.InQuad));
    }

    void StartIdleAnimation(Image img)
    {
        // Looping idle animation: scale up and down subtly
        img.rectTransform.DOScale(1f + idleScaleAmount, idleDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo); // Infinite loop, alternating between scale up and down
    }
}

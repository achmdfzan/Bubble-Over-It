using UnityEngine;
using DG.Tweening;

public class OnEnableIndicatorAnim : MonoBehaviour
{
    [SerializeField]
    private float scaleUpFactor = 1.2f; // The factor to scale up by
    [SerializeField]
    private float animationDuration = 0.5f; // Duration of the scaling animation

    private Vector3 originalScale; // To store the original scale

    void Awake()
    {
        // Store the original scale at the start
        originalScale = transform.localScale;
    }

    void OnEnable()
    {
        // Reset to the original scale
        transform.localScale = originalScale;

        // Scale up and then back to the original size
        transform.DOScale(originalScale * scaleUpFactor, animationDuration / 2)
                 .OnComplete(() =>
                     transform.DOScale(originalScale, animationDuration / 2));
    }
}

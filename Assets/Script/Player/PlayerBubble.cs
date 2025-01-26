using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Bubble
{
    public class PlayerBubble : MonoBehaviour
    {
        [Header("Component")]
        [SerializeField] private PlayerCamera _playerCamera;
        [SerializeField] private GameObject _sliderGameobject;
        [SerializeField] private CanvasGroup _canvasEndGame;


        [Header("Bubble Configuration")]
        [SerializeField] private Transform _bubbleObject;
        [SerializeField] public float _speedBubbleValue;
        [SerializeField] private float _currentBubbleValue;

        [Header("Animation Broken")]
        public GameObject vfxBrokenGameobject;

        [SerializeField]
        private float scaleUpFactor = 1.2f; // The factor to scale up by
        [SerializeField]
        private float animationDuration = 0.5f; // Duration of the scaling animation

        private Vector3 originalScale; // To store the original scale

        private void Awake()
        {
            originalScale = _sliderGameobject.transform.localScale;
        }
        public void StartAnimation()
        {
            _bubbleObject.gameObject.SetActive(true);
            _currentBubbleValue = 0;
            UpdateBubbleValue();
        }

        public void StopAnimation()
        {
            AnimateBroken();
            _playerCamera.ZoomInCamera();
        }

        public void AddBubbleValue()
        {
            _currentBubbleValue += _speedBubbleValue;
            UpdateBubbleValue();
            _playerCamera.ZoomOutCamera();
            ScaleUp();


            if(_currentBubbleValue >= 250)
            {
                _canvasEndGame.DOFade(0.96f, 5f);
            }
        }

        private void UpdateBubbleValue()
        {
            _bubbleObject.DOScale(_currentBubbleValue, 0.3f);
        }

        private void AnimateBroken()
        {
            var vfxBreak = Instantiate(vfxBrokenGameobject, _bubbleObject.position, Quaternion.identity);
            vfxBreak.transform.localScale = _bubbleObject.localScale;
            Destroy(vfxBreak, 1f);

            _currentBubbleValue = 0;
            UpdateBubbleValue();
            _bubbleObject.gameObject.SetActive(false);
        }

        void ScaleUp()
        {
            // Reset to the original scale
            _sliderGameobject.transform.localScale = originalScale;

            // Scale up and then back to the original size
            _sliderGameobject.transform.DOScale(originalScale * scaleUpFactor, animationDuration / 2)
                     .OnComplete(() =>
                         _sliderGameobject.transform.DOScale(originalScale, animationDuration / 2));
        }
    }
}

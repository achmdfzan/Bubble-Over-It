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

        [Header("Bubble Configuration")]
        [SerializeField] private Transform _bubbleObject;
        [SerializeField] private float _speedBubbleValue;
        [SerializeField] private float _currentBubbleValue;

        [Header("Animation Broken")]
        public GameObject vfxBrokenGameobject;

        public void StartAnimation()
        {
            _bubbleObject.gameObject.SetActive(true);
            _currentBubbleValue = 0;
            UpdateBubbleValue();
        }

        public void StopAnimation()
        {
            AnimateBroken();
            _playerCamera.ZoomOut();
        }

        public void AddBubbleValue()
        {
            _currentBubbleValue += _speedBubbleValue;
            UpdateBubbleValue();
            _playerCamera.ZoomIn();
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
    }
}

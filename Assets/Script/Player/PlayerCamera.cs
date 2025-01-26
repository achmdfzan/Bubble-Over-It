using DG.Tweening;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public CinemachineCamera virtualCamera;
    public CinemachineHardLookAt rotationComposer;
    public Transform playerTransform;
    public float zoomSpeed = 1f;
    public float zoomSize = 2f;
    private float defaultSize = 6.3f;
    [SerializeField] private Transform _targetedTransform;
    [SerializeField] private Transform _endGameTargetTransform;
    private float _defaultTargetTransform;
    [SerializeField] private float _takeDownTargetTransform;
    public float rayDistance;
    public LayerMask groundLayer;


    private bool _isFlying = false;
    private bool _hasTakeDown = false;
    private Tween _takeDownTween;

    private void Start()
    {
        defaultSize = virtualCamera.Lens.OrthographicSize;
        _defaultTargetTransform = _targetedTransform.localPosition.y;
    }

    public void ZoomOutCamera()
    {
        StopAllCoroutines();

        _isFlying = true;

        float amount = virtualCamera.Lens.OrthographicSize + zoomSize;
        amount = Mathf.Clamp(amount, 0, 10);
        StartCoroutine(SmoothZoom(amount));
    }

    public void ZoomInCamera()
    {
        StopAllCoroutines();
        StartCoroutine(SmoothZoomOut());

        RaycastHit2D hitLeft = Physics2D.Raycast(playerTransform.position, Vector2.down, rayDistance, groundLayer);
        Debug.DrawRay(playerTransform.position, Vector2.down * rayDistance, Color.red);

        if (hitLeft)
        {
            Debug.Log("Hit");
            HasTakeDown();
            _hasTakeDown = true;
        }
    }

    public void HasTakeDown()
    {
        //_takeDownTween = DOTween.To(
        //           () => rotationComposer.LookAtOffset.y ,    // Nilai awal (getter)
        //           x => rotationComposer.LookAtOffset.y = x, // Setter untuk mengubah nilai
        //           -3.5f,                   // Nilai target
        //           2f
        //).SetEase(Ease.InOutQuad);

    }

    private IEnumerator SmoothZoom(float target)
    {
        float startSize = virtualCamera.Lens.OrthographicSize;
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            virtualCamera.Lens.OrthographicSize = Mathf.Lerp(startSize, target, elapsedTime / zoomSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator SmoothZoomOut()
    {
        float startSize = virtualCamera.Lens.OrthographicSize;
        while (virtualCamera.Lens.OrthographicSize > defaultSize)
        {
            virtualCamera.Lens.OrthographicSize -= Time.deltaTime * zoomSpeed;
            yield return null;
        }
    }

    public void HasGrounded()
    {

        if(_takeDownTween != null)
        {
            _takeDownTween.Kill();
            _takeDownTween = null;
        }

        //if (_hasTakeDown)
        //{
        //    _takeDownTween = DOTween.To(
        //           () => rotationComposer.LookAtOffset.y,    // Nilai awal (getter)
        //           x => rotationComposer.LookAtOffset.y = x, // Setter untuk mengubah nilai
        //           0f,                   // Nilai target
        //           1f
        //    ).SetEase(Ease.InOutQuad);
        //}
        _isFlying = false;
        _hasTakeDown = false;
    }

    public void ChangeTargetedEndGame()
    {
        virtualCamera.LookAt = _endGameTargetTransform;
    }
}

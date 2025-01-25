using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public CinemachineCamera virtualCamera;
    public float zoomSpeed = 1f;
    public float zoomSize = 2f;
    private float defaultSize = 6.3f;

    private void Start()
    {
        defaultSize = virtualCamera.Lens.OrthographicSize;
    }

    public void ZoomIn()
    {
        StopAllCoroutines();

        float amount = virtualCamera.Lens.OrthographicSize + zoomSize;
        StartCoroutine(SmoothZoom(amount));
    }

    public void ZoomOut()
    {
        StopAllCoroutines();
        StartCoroutine(SmoothZoomOut());
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
}

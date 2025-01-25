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
        StartCoroutine(SmoothZoom(defaultSize));
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
}

using UnityEngine;
using System.Collections;

public class CinemachineTargetController : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private float duration;
    [SerializeField] private GameObject target;
    [SerializeField] private Transform topPointReference;
    [SerializeField] private Transform bottomPointReference;
    [SerializeField] private Transform endGamePointReference;


    [Header("States")]
    [SerializeField] private bool isOnTop;
    [SerializeField] private bool isTransitioning;
    [SerializeField] private Vector3 startPositionSnapshot;
    private float elapsedTime = 0f; // Tracks interpolation time
    private bool movingToTop = false; // Controls movement direction
    private bool toEndGame = false;

    private void Start()
    {
        isTransitioning = false;
        isOnTop = true;

        //StartCoroutine(Test());
    }

    //private IEnumerator Test()
    //{
    //    MoveToBottom();

    //    while (true)
    //    {
    //        yield return new WaitForSeconds(5f);
    //        MoveToTop();

    //        yield return new WaitForSeconds(5f);
    //        MoveToBottom();
    //    }
    //}

    public void MoveToTop()
    {
        //if (isTransitioning || isOnTop) return;
        //if (isOnTop) return;
        if (movingToTop) return;


        startPositionSnapshot = target.transform.position;
        isTransitioning = true;
        movingToTop = true;
        elapsedTime = 0f; // Reset elapsed time for movement
    }

    public void MoveToBottom()
    {
        //if (isTransitioning || !isOnTop) return;
        //if (!isOnTop) return;
        if (!movingToTop) return;

        startPositionSnapshot = target.transform.position;

        isTransitioning = true;
        movingToTop = false;
        elapsedTime = 0f; // Reset elapsed time for movement
    }

    public void MoveToEndGame()
    {
        startPositionSnapshot = target.transform.position;

        toEndGame = true;
        isTransitioning = true;
        movingToTop = false;
        elapsedTime = 0f; // Reset elapsed time for movement
    }

    private void Update()
    {
        if (!isTransitioning) return;

        // Increment elapsed time
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);

        // Smooth interpolation with ease-in/ease-out
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        // Determine target positions

        //Vector3 startPosition = movingToTop ? bottomPointReference.position : topPointReference.position;
        Vector3 startPosition = startPositionSnapshot;

        Vector3 targetPosition = Vector3.zero;
        if(toEndGame)
        {
            targetPosition = endGamePointReference.transform.position;
        } else
        {
            targetPosition = movingToTop ? topPointReference.position : bottomPointReference.position;
        }

        // Interpolate target's position
        target.transform.position = Vector3.Lerp(startPosition, targetPosition, smoothT);

        if (toEndGame) return;
        // Check if the transition is complete
        if (t >= 1f)
        {
            isTransitioning = false;
            isOnTop = movingToTop; // Update state
        }
    }
}

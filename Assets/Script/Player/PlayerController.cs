using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG;
using DG.Tweening;


namespace Bubble
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerInputs _playerInputs;

        [Header("Flags")]
        public bool isMovement = false;
        public bool isFlying = false;

        private Coroutine leftHoldCoroutine;
        private Coroutine rightHoldCoroutine;
        private float leftHoldDuration;
        private float rightHoldDuration;

        [Header("Component")]
        [SerializeField] private Rigidbody2D _PlayerRb;
        [SerializeField] private PlayerBubble _playerBubble;
        [SerializeField] private PlayerCamera _playerCamera;

        [Header("Slider Mechanic")]
        [SerializeField] private GameObject _sliderPanel;
        [SerializeField] private Slider _sliderIndicator;  // Referensi ke _sliderIndicator di UI
        [SerializeField] private float _sliderBackgroundValue;
        [SerializeField] private RectTransform _areaClick;

        [SerializeField] private RectTransform _indicatorImg;
        [SerializeField] private Image _barImage;
        [SerializeField] private float _offsetBar = 7f;

        [Header("Mechanic Settings")]
        [SerializeField] private float startClickArea;
        [SerializeField] private float endClickArea;
        [SerializeField] private int _defaultPassChange = 2;
        private float _passChange = 2;
        [SerializeField] private int passValue;
        private int _passRequiriment;
        [SerializeField] private int _currentLevelBubble = 0;
        public float inAirTime = 0;
        private Tween _applyGravityTween;

        [Header("Settings")]
        public float sliderSpeed = 100f;
        public float moveMultipiler = 1f;
        public float gravityForce = -0.1f;
        [SerializeField] private PlayerBubbleLevel[] _playerBubbleLevel;

        private Coroutine sliderCoroutine;
        private bool isIncreasing = true;

        [Header("Raycast Settings")]
        public float rayDistance = 2f; 
        public LayerMask groundLayer;   
        public float rayOffset = 1f;    
        public bool isGroundedLeft;
        public bool isGroundedRight;
        public bool isPlunging;
        private Coroutine _groundCheckerCoroutine;

        public bool isreversed = false;

        private void Awake()
        {
            _playerInputs = new PlayerInputs();
        }

        private void OnEnable()
        {
            _passChange = _defaultPassChange;

            _playerInputs.Enable();
            _playerInputs.Movement.Left.started += left => StartLeftHold();
            _playerInputs.Movement.Left.canceled += left => CancelLeftHold();

            _playerInputs.Movement.Right.started += right => StartRightHold();
            _playerInputs.Movement.Right.canceled += right => CancelRightHold();

            _playerInputs.Movement.Mouse.performed += mouse => MouseClicked();
        }

        private void OnDisable()
        {
            _playerInputs.Movement.Left.started -= left => StartLeftHold();
            _playerInputs.Movement.Left.canceled -= left => CancelLeftHold();

            _playerInputs.Movement.Right.started -= right => StartRightHold();
            _playerInputs.Movement.Right.canceled -= right => CancelRightHold();

            _playerInputs.Movement.Mouse.performed -= mouse => MouseClicked();

            _playerInputs.Disable();
        }


        #region Movement
        private void StartLeftHold()
        {
            if (leftHoldCoroutine == null)
            {
                leftHoldCoroutine = StartCoroutine(HoldLeftButton());
            }
        }

        private void CancelLeftHold()
        {
            if (leftHoldCoroutine != null)
            {
                MovePlayer(leftHoldDuration, false);
                Debug.Log("Left button held for: " + leftHoldDuration.ToString("F2") + " seconds");
                leftHoldDuration = 0;
                leftHoldCoroutine = null;
            }
        }

        private IEnumerator HoldLeftButton()
        {
            while (true)
            {
                leftHoldDuration += Time.deltaTime;

                if (leftHoldDuration >= 0.5f)
                {
                    MovePlayer(leftHoldDuration, false);
                    leftHoldDuration = 0;
                }

                yield return null;
            }
        }

        private void StartRightHold()
        {
            if (rightHoldCoroutine == null)
            {
                rightHoldCoroutine = StartCoroutine(HoldRightButton());
            }
        }

        private void CancelRightHold()
        {
            if (rightHoldCoroutine != null)
            {
                MovePlayer(rightHoldDuration, true);
                rightHoldDuration = 0;
                Debug.Log("Right button held for: " + rightHoldDuration.ToString("F2") + " seconds");
                rightHoldCoroutine = null;
            }
        }
        

        private IEnumerator HoldRightButton()
        {
            while (true)
            {
                rightHoldDuration += Time.deltaTime;

                if (rightHoldDuration >= 0.5f)
                {
                    MovePlayer(rightHoldDuration, true);
                    rightHoldDuration = 0;
                }
                yield return null;
            }
        }

        private void MovePlayer(float holdValue, bool isRight)
        {
            float moveValue = holdValue * moveMultipiler;
            Vector2 forceDirection = isRight ? Vector2.left : Vector2.right;

            if (!isreversed)
            {
                forceDirection = isRight ? Vector2.left : Vector2.right;
            }
            else
            {
                forceDirection = isRight ? Vector2.right : Vector2.left;
            }

            _PlayerRb.AddForce(forceDirection * moveValue, ForceMode2D.Impulse);
            Debug.Log("Force applied: " + moveValue.ToString("F2") + " to " + (isRight ? "Right" : "Left"));

            if (isRight)
            {
                if (rightHoldCoroutine != null)
                {
                    StopCoroutine(rightHoldCoroutine);
                    rightHoldCoroutine = null;
                }
            }
            else
            {
                if (leftHoldCoroutine != null)
                {
                    StopCoroutine(leftHoldCoroutine);
                    leftHoldCoroutine = null;
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                isreversed = !isreversed;   
            }
        }

        #endregion

        #region Mouse Action

        private void MouseClicked()
        {
            if (isFlying)
            {
                if(isPlunging)
                    return;

                ChekingClicker();
            }
            else
            {
                _PlayerRb.gravityScale = 0;
                passValue = 0;
                _passChange = _defaultPassChange + 1; ;
                isFlying = true;
                _currentLevelBubble = 0;
                isGroundedLeft = false;
                isGroundedRight = false;
                SetLevelBubble();

                _playerBubble.StartAnimation();
                SetSliderBackground();
                StartSliderMovement();
            }

        }

        private void SetLevelBubble()
        {
            _passRequiriment = _playerBubbleLevel[_currentLevelBubble].nextLevelRequiriment;
            sliderSpeed = _playerBubbleLevel[_currentLevelBubble].sliderSpeed;
        }

        private void SetSliderBackground()
        {
            _sliderBackgroundValue = Random.Range(10f, 90f);
            float normalizedFill = Mathf.Clamp(_sliderBackgroundValue / 100f, 0f, 1f);

            float rotationZ = Mathf.Lerp(0f, 360f, normalizedFill);
            Debug.Log("rotationZ : " + rotationZ);

            _areaClick.localEulerAngles = new Vector3(0, 0, -rotationZ);
        }

        public void StartSliderMovement()
        {
            if (sliderCoroutine == null)
            {
                _sliderPanel.SetActive(true);
                sliderCoroutine = StartCoroutine(MoveSlider());
            }
        }

        public void StopSliderMovement()
        {
            if (sliderCoroutine != null)
            {
                StopCoroutine(sliderCoroutine);
                sliderCoroutine = null;
            }
        }

        private IEnumerator MoveSlider()
        {
            while (true)
            {
                if (isIncreasing)
                {
                    _sliderIndicator.value += sliderSpeed * Time.deltaTime;
                    UpdateIndicatorValue();
                    if (_sliderIndicator.value >= _sliderIndicator.maxValue)
                    {
                        _sliderIndicator.value = _sliderIndicator.maxValue;
                        _sliderIndicator.value = 0;

                        _passChange--;

                        if(_passChange <= 0)
                        {
                            BubbleBroken();
                        }
                    }
                }

                yield return null;
            }
        }

        private void UpdateIndicatorValue()
        {
            float buttonAngle = _barImage.fillAmount * 360;
            _indicatorImg.localEulerAngles = new Vector3(0, 0, -buttonAngle); 
        }

        private void ChekingClicker()
        {
            if(_sliderIndicator.value >= _sliderBackgroundValue - 10 && _sliderIndicator.value <= _sliderBackgroundValue + 10)
            {
                _passChange = _defaultPassChange;
                passValue++;

                SetSliderBackground();

                _playerBubble.AddBubbleValue();

                if (passValue > _passRequiriment)
                {
                    _currentLevelBubble++;
                    SetLevelBubble();
                }

                ApplyGravity();
            }
            else
            {
                BubbleBroken();
            }
        }

        private void ApplyGravity()
        {
            _PlayerRb.gravityScale += gravityForce;
        }

        public void BubbleBroken()
        {
            if (sliderCoroutine != null)
            {
                StopCoroutine(sliderCoroutine);
                sliderCoroutine = null;
            }

            _playerBubble.StopAnimation();

            _applyGravityTween = DOTween.To(
                   () => _PlayerRb.gravityScale,    // Nilai awal (getter)
                   x => _PlayerRb.gravityScale = x, // Setter untuk mengubah nilai
                   1,                   // Nilai target
                   1f                        
               ).SetEase(Ease.InOutQuad);         
            _sliderPanel.SetActive(false);

            _groundCheckerCoroutine = StartCoroutine(IECheckGround());
        }

        private IEnumerator IECheckGround()
        {
            inAirTime = 0;
            isPlunging = true;
            while (true)
            {
                CheckGround();

                inAirTime += Time.deltaTime;
                yield return null;
            }
        }


        private void CheckGround()
        {
            Vector2 leftOrigin = new Vector2(transform.position.x - rayOffset, transform.position.y);
            RaycastHit2D hitLeft = Physics2D.Raycast(leftOrigin, Vector2.down, rayDistance, groundLayer);
            isGroundedLeft = hitLeft.collider != null;

            Vector2 rightOrigin = new Vector2(transform.position.x + rayOffset, transform.position.y);
            RaycastHit2D hitRight = Physics2D.Raycast(rightOrigin, Vector2.down, rayDistance, groundLayer);
            isGroundedRight = hitRight.collider != null;

            Debug.DrawRay(leftOrigin, Vector2.down * rayDistance, isGroundedLeft ? Color.green : Color.red);
            Debug.DrawRay(rightOrigin, Vector2.down * rayDistance, isGroundedRight ? Color.green : Color.red);

            if (isGroundedLeft || isGroundedRight)
            {
                if (_groundCheckerCoroutine != null)
                {
                    isPlunging = false;
                    StopCoroutine(_groundCheckerCoroutine);
                    _groundCheckerCoroutine = null;
                    isFlying = false;

                    _playerCamera.HasGrounded();

                    _applyGravityTween.Kill();
                }
            }
        }

        #endregion
    }
}


[System.Serializable]
public class PlayerBubbleLevel
{
    public int nextLevelRequiriment;
    public int sliderSpeed;
}

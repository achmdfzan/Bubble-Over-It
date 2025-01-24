using System.Collections;
using UnityEngine;
using UnityEngine.UI;


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

        [Header("Slider Mechanic")]
        [SerializeField] private GameObject _sliderPanel;
        [SerializeField] private Slider _sliderIndicator;  // Referensi ke _sliderIndicator di UI
        [SerializeField] private float _sliderBackgroundValue;
        [SerializeField] private Slider _sliderBackground;

        [Header("Mechanic")]
        [SerializeField] private float startClickArea;
        [SerializeField] private float endClickArea;
        [SerializeField] private int _defaultPassChange = 2;
        private float _passChange = 2;
        [SerializeField] private int passValue;
        private int _passRequiriment;
        [SerializeField] private int _currentLevelBubble = 0;

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

                if (leftHoldDuration >= 2)
                {
                    MovePlayer(leftHoldDuration, false);
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
                Debug.Log("Right button held for: " + rightHoldDuration.ToString("F2") + " seconds");
                rightHoldDuration = 0;
                rightHoldCoroutine = null;
            }
        }
        

        private IEnumerator HoldRightButton()
        {
            while (true)
            {
                rightHoldDuration += Time.deltaTime;

                if (rightHoldDuration >= 2)
                {
                    MovePlayer(rightHoldDuration, true);
                }
                yield return null;
            }
        }

        private void MovePlayer(float holdValue, bool isRight)
        {
            float moveValue = holdValue * moveMultipiler;
            Vector2 forceDirection = isRight ? Vector2.right : Vector2.left;

            _PlayerRb.AddForce(forceDirection * moveValue, ForceMode2D.Impulse);
            Debug.Log("Force applied: " + moveValue.ToString("F2") + " to " + (isRight ? "Right" : "Left"));

            if (isRight)
            {
                StopCoroutine(rightHoldCoroutine);
                rightHoldCoroutine = null;
            }
            else
            {
                StopCoroutine(leftHoldCoroutine);
                leftHoldCoroutine = null;
            }
        }

        #region Mouse Action

        private void MouseClicked()
        {
            if (isFlying)
            {
                if(isPlunging)
                    return;

                ChekingClicker();
                Debug.Log("Cheking");
            }
            else
            {
                Debug.Log("Blowing");
                _PlayerRb.gravityScale = 0;
                passValue = 0;
                _passChange = _defaultPassChange + 1; ;
                isFlying = true;
                _currentLevelBubble = 0;
                isGroundedLeft = false;
                isGroundedRight = false;
                SetLevelBubble();

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

            _sliderBackground.value = _sliderBackgroundValue;
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
                    if (_sliderIndicator.value >= _sliderIndicator.maxValue)
                    {
                        _sliderIndicator.value = _sliderIndicator.maxValue;

                        _passChange--;

                        if(_passChange <= 0)
                        {
                            BubbleBroken();
                        }

                        isIncreasing = false;
                    }
                }
                else
                {
                    _sliderIndicator.value -= sliderSpeed * Time.deltaTime;
                    if (_sliderIndicator.value <= _sliderIndicator.minValue)
                    {
                        _sliderIndicator.value = _sliderIndicator.minValue;

                        _passChange--;

                        if (_passChange <= 0)
                        {
                            BubbleBroken();
                        }

                        isIncreasing = true;
                    }
                }

                yield return null;
            }
        }

        private void ChekingClicker()
        {
            if(_sliderIndicator.value >= _sliderBackgroundValue - 10 && _sliderIndicator.value <= _sliderBackgroundValue + 10)
            {
                Debug.Log("Passed");
                _passChange = _defaultPassChange;
                passValue++;

                if(passValue > _passRequiriment)
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

        private void BubbleBroken()
        {
            if (sliderCoroutine != null)
            {
                StopCoroutine(sliderCoroutine);
                sliderCoroutine = null;
            }


            _PlayerRb.gravityScale = 1;
            Debug.Log("Buuble Broken");
            _sliderPanel.SetActive(false);

            _groundCheckerCoroutine = StartCoroutine(IECheckGround());
        }

        private IEnumerator IECheckGround()
        {
            isPlunging = true;
            while (true)
            {
                CheckGround();
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
                if(_groundCheckerCoroutine != null)
                {
                    isPlunging = false;
                    StopCoroutine(_groundCheckerCoroutine);
                    _groundCheckerCoroutine = null;
                    isFlying = false;
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

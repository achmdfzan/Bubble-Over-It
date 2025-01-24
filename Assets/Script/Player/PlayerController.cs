using UnityEngine;


namespace Bubble
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerInputs _playerInputs;


        private void Awake()
        {
            _playerInputs = new PlayerInputs();
        }

        private void OnEnable()
        {
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
            Debug.Log("starthold");
        }

        private void CancelLeftHold()
        {

            Debug.Log("EndHold");
        }

        private void StartRightHold()
        {

        }

        private void CancelRightHold()
        {

        }

        private void MouseClicked()
        {

        }

    }
}

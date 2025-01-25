using UnityEngine;


namespace Bubble
{
    public class BubbleTrigger : MonoBehaviour
    {
        public PlayerController playerController;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            playerController.BubbleBroken();
        }
    }
}

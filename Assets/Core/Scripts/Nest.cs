using UnityEngine;
using managers;

namespace interactables
{
    [RequireComponent(typeof(Collider))]
    public class Nest : MonoBehaviour
    {
        [SerializeField]
        private int sleepRestore = 100; // Amount of health to restore
        [SerializeField]
        private KeyCode interactKey = KeyCode.X;
        [SerializeField]
        private GameObject pickupPrompt;

        private bool playerInRange = false;

        private void Start()
        {
            HidePickupPrompt();
        }

        private void Update()
        {
            // Check if player is in range and presses the interact key
            if (playerInRange && Input.GetKeyDown(interactKey))
            {
                RestoreSleep();
                QuestManager.Instance.ProgressQuest(QuestType.FindNest, 1);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check if the object that entered the trigger has the "Player" tag
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
                ShowPickupPrompt();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
                HidePickupPrompt();
            }
        }

        private void RestoreSleep()
        {
            SnapToNest();
            MainGameManager.Instance.Player.playerTiredness.Sleep(sleepRestore);
            HidePickupPrompt(); // Hide prompt after use
        }

        private void SnapToNest()
        {
            MainGameManager.Instance.SnapPlayer(transform.position);
        }

        private void ShowPickupPrompt()
        {
            if (pickupPrompt != null)
            {
                pickupPrompt.SetActive(true); // Show UI prompt
            }
        }

        private void HidePickupPrompt()
        {
            if (pickupPrompt != null)
            {
                pickupPrompt.SetActive(false); // Hide UI prompt
            }
        }
    }
}

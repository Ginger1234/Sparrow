using UnityEngine;
using managers;

namespace interactables
{
    [RequireComponent(typeof(Collider))]
    public class PickableObject : MonoBehaviour
    {
        [SerializeField]
        private int healthRestoreAmount = 20;
        private FoodManager manager;
        private Transform spawnPoint;

        public void Initialize(FoodManager managerRef, Transform point)
        {
            manager = managerRef;
            spawnPoint = point;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                HandlePlayerPickup();
            }
        }

        private void HandlePlayerPickup()
        {
            MainGameManager.Instance.Player.playerHunger.Increase(healthRestoreAmount);
            MainGameManager.Instance.Player.Eat();
            QuestManager.Instance.ProgressQuest(QuestType.FindFood, 1);

            if (manager != null)
            {
                manager.OnFoodCollected(gameObject, spawnPoint);
            }

            Destroy(gameObject);
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace interactables
{
    public class FoodManager : MonoBehaviour
    {
        [Header("Food Settings")]
        [Tooltip("List of possible food items with weights")]
        [SerializeField] private FoodItem[] foodItems;
        [Tooltip("Potential spawn positions")]
        [SerializeField] private Transform[] spawnPoints;
        [Tooltip("Minimum time between spawns")]
        [SerializeField] private float minSpawnInterval = 5f;
        [Tooltip("Maximum time between spawns")]
        [SerializeField] private float maxSpawnInterval = 15f;
        [Tooltip("Maximum food items active at once")]
        [SerializeField] private int maxActiveFood = 10;
        [Tooltip("Cooldown after the food was collected")]
        [SerializeField] private float cooldownTime = 7f;
        
        [Header("Debug")]
        public bool showSpawnPoints = true;
        public Color spawnPointColor = Color.green;

        private List<Transform> availableSpawnPoints = new List<Transform>();
        private List<GameObject> activeFood = new List<GameObject>();
        private float nextSpawnTime;

        void Start()
        {
            ValidateSettings();
            InitializeAvailablePoints();
            SetNextSpawnTime();
        }

        void Update()
        {
            if (ShouldSpawnFood())
            {
                SpawnFood();
                SetNextSpawnTime();
            }
        }

        bool ShouldSpawnFood()
        {
            return Time.time >= nextSpawnTime && 
                activeFood.Count < maxActiveFood && 
                availableSpawnPoints.Count > 0;
        }

        void InitializeAvailablePoints()
        {
            availableSpawnPoints.Clear();
            availableSpawnPoints.AddRange(spawnPoints);
        }

        void SetNextSpawnTime()
        {
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }

        void ValidateSettings()
        {
            if (foodItems.Length == 0)
            {
                Debug.LogError("No food items assigned in FoodManager!");
                enabled = false;
            }

            if (spawnPoints.Length == 0)
            {
                Debug.LogError("No spawn points assigned in FoodManager!");
                enabled = false;
            }
        }

        void SpawnFood()
        {
            // Select random spawn point and remove it from available points
            int spawnIndex = Random.Range(0, availableSpawnPoints.Count);
            Transform spawnPoint = availableSpawnPoints[spawnIndex];
            availableSpawnPoints.RemoveAt(spawnIndex);

            // Select food item based on weights
            GameObject foodPrefab = SelectFoodItemByWeight();
            
            // Instantiate food and set up tracking
            GameObject newFood = Instantiate(foodPrefab, spawnPoint.position, Quaternion.identity);
            activeFood.Add(newFood);

            // Attach component to handle collection and cleanup
            PickableObject foodScript = newFood.GetComponent<PickableObject>();
            foodScript.Initialize(this, spawnPoint);
        }

        GameObject SelectFoodItemByWeight()
        {
            int totalWeight = 0;
            foreach (FoodItem item in foodItems)
            {
                totalWeight += item.spawnWeight;
            }

            int randomValue = Random.Range(0, totalWeight);
            int cumulativeWeight = 0;

            foreach (FoodItem item in foodItems)
            {
                cumulativeWeight += item.spawnWeight;
                if (randomValue < cumulativeWeight)
                {
                    return item.foodPrefab;
                }
            }

            return foodItems[0].foodPrefab; // fallback
        }

        public void OnFoodCollected(GameObject food, Transform spawnPoint)
        {
            // Cleanup and return spawn point to available pool
            activeFood.Remove(food);
            Destroy(food);
            StartCoroutine(FoodSpawnCoolDownn(spawnPoint));
        }


        IEnumerator FoodSpawnCoolDownn(Transform spawnPoint)
        {
            yield return new WaitForSeconds(cooldownTime);
            availableSpawnPoints.Add(spawnPoint);
        }

        void OnDrawGizmos()
        {
            if (!showSpawnPoints || spawnPoints == null) return;

            Gizmos.color = spawnPointColor;
            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.3f);
                }
            }
        }
    }
}
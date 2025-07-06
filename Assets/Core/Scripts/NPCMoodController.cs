using UnityEngine;

namespace npc_system
{
    [System.Serializable]
    public class MoodState
    {
        public HumanMood mood;
        public GameObject moodObject;
    }

    public class NPCMoodController : MonoBehaviour
    {
        [Header("Mood Settings")]
        [Tooltip("List of possible mood states")]
        public MoodState[] moodStates;

        [Header("Current State")]
        public HumanMood currentMood = HumanMood.Neutral;

        private void Start()
        {
            SetAllMoodsInactive();
            // Set initial mood
            SetMood(currentMood);
        }

        public void SetMood(HumanMood newMood)
        {
            // First disable all mood objects
            SetAllMoodsInactive();

            // Find and enable the new mood object
            bool foundMood = false;
            foreach (MoodState mood in moodStates)
            {
                if (mood.mood == newMood && mood.moodObject != null)
                {
                    mood.moodObject.SetActive(true);
                    foundMood = true;
                    currentMood = newMood;
                    break;
                }
            }

            if (!foundMood)
            {
                Debug.LogWarning($"Mood state '{newMood}' not found in moodStates array!");
            }
        }

        public void SetHappy()
        {
            SetMood(HumanMood.Rewarding);
        }

        public void SetNeutral()
        {
            SetMood(HumanMood.Neutral);
        }

        public void SetAngry()
        {
            SetMood(HumanMood.Angry);
        }

        private void SetAllMoodsInactive()
        {
            foreach (MoodState mood in moodStates)
            {
                if (mood.moodObject != null)
                {
                    mood.moodObject.SetActive(false);
                }
            }
        }
    }
}

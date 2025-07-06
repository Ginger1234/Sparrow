using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using interactables;
using managers;

namespace npc_system
{
    public enum HumanMood
    {
        Neutral,
        Angry,
        Rewarding
    }

    public class HumanNPC : MonoBehaviour
    {
        [Header("NPC Settings")]
        [SerializeField] private HumanMood currentState = HumanMood.Neutral;
        [SerializeField] private GameObject reward;
        [SerializeField] private Transform throwPoint;
        [SerializeField] private float throwForce = 5f;
        [SerializeField] private Animator[] animators;
        [SerializeField] private InteractableNew NPCPosessionPrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private string positiveInteractionTrigger;
        [SerializeField] private string negativeInteractionTrigger;
        [SerializeField] private GameObject interactPrompt;

        private InteractableNew NPCPosession;
        private NPCMoodController moodController;
        private NPCMovement movement;
        protected bool inRadius = false;

        private const float PositiveInteractionDelay = 4f;
        private const float NegativeInteractionDelay = 3f;

        protected void Start()
        {
            InitializeInteractPrompt();
            InitializeNPCPosession();
            InitializeComponents();
        }

        private void InitializeInteractPrompt()
        {
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }

        private void InitializeNPCPosession()
        {
            if (NPCPosessionPrefab != null)
            {
                NPCPosession = Instantiate(NPCPosessionPrefab);
                NPCPosession.transform.position = spawnPoint.position;
                NPCPosession.SetOwner(this);
            }
        }

        private void InitializeComponents()
        {
            if (TryGetComponent(out NPCMovement _movement))
            {
                movement = _movement;
            }
            moodController = GetComponent<NPCMoodController>();
        }

        protected void Update()
        {
            if (inRadius)
            {
                WaitForPositiveBehavior();
                WaitForNegativeBehavior();
            }
        }

        protected void WaitForPositiveBehavior()
        {
            if (Input.GetKeyDown(KeyCode.R) && currentState != HumanMood.Angry)
            {
                if (currentState != HumanMood.Rewarding)
                {
                    QuestManager.Instance.ProgressQuest(QuestType.EntratainHuman, 1);
                }

                currentState = HumanMood.Rewarding;
                moodController?.SetHappy();
                TriggerAnimation(positiveInteractionTrigger);
                StopMovement(PositiveInteractionDelay);
                StartCoroutine(GiveRewardDelay(PositiveInteractionDelay*0.5f));
            }
        }

        protected void WaitForNegativeBehavior()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentState = HumanMood.Angry;
                TriggerAnimation(negativeInteractionTrigger);
                StopMovement(NegativeInteractionDelay);
            }
        }

        private void TriggerAnimation(string triggerName)
        {
            foreach (Animator animator in animators)
            {
                if(animator.gameObject.activeInHierarchy)
                {
                    animator.SetTrigger(triggerName);
                }
            }
        }

        private void StopMovement(float duration)
        {
            if (movement)
            {
                movement.StopFor(duration);
            }
        }

        public void GetAngry()
        {
            QuestManager.Instance.ProgressQuest(QuestType.AnnoyHuman, 1);
            currentState = HumanMood.Angry;
            moodController?.SetAngry();
        }

        private IEnumerator GiveRewardDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            GiveReward();
        }

        protected void GiveReward()
        {
            if (reward != null && throwPoint != null)
            {
                GameObject _reward = Instantiate(reward, throwPoint.position, Quaternion.identity);
                Rigidbody rb = _reward.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 throwDirection = (throwPoint.forward + Vector3.up).normalized;
                    rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
                }
            }
        }

        public void ResetState()
        {
            currentState = HumanMood.Neutral;
            moodController?.SetNeutral();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            inRadius = true;
            if (NPCPosession != null && NPCPosession.IsInteracted())
            {
                NegativeInteraction();
            }
            if (currentState == HumanMood.Neutral)
            {
                ShowInteractPrompt();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                inRadius = false;
                HideInteractPrompt();
            }
        }

        private void ShowInteractPrompt()
        {
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }
        }

        private void HideInteractPrompt()
        {
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }

        private void NegativeInteraction()
        {
            TriggerAnimation(negativeInteractionTrigger);
            StopMovement(NegativeInteractionDelay);
        }
    }
}

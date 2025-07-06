using System.Collections;
using UnityEngine;
using npc_system;
using managers;

namespace interactables
{
    [RequireComponent(typeof(Collider))]
    public class InteractableNew : MonoBehaviour
    {
        private const float InteractCooldownDuration = 0.2f;

        private bool isInteracted = false;
        private bool isInRadius = false;
        private bool isHeld = false;

        [SerializeField]
        private HumanNPC human;

        private void Start()
        {
            isInteracted = false;
            isHeld = false;
        }

        private void Update()
        {
            if (isInRadius && !isHeld && Input.GetKeyDown(KeyCode.X))
            {
                Interact();
            }
        }

        private void Interact()
        {
            isInteracted = true;
            isHeld = true;

            human?.GetAngry();

            MainGameManager.Instance.Player.Hold(this);
        }

        public void Drop()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
            transform.parent = null;
            StartCoroutine(InteractCooldown());
        }

        public void SetOwner(HumanNPC _npc)
        {
            if(!human) human = _npc;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isInRadius = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isInRadius = false;
            }
        }

        public bool IsInteracted()
        {
            return isInteracted;
        }

        private IEnumerator InteractCooldown()
        {
            yield return new WaitForSeconds(InteractCooldownDuration);
            isHeld = false;
        }
    }
}
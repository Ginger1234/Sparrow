using UnityEngine;
using System.Collections;

namespace npc_system
{
    public class NPCMovement : MonoBehaviour
    {
        [Header("Waypoints")]
        [Tooltip("Array of waypoints the NPC will patrol between")]
        public Transform[] waypoints;
        
        [Header("Movement Settings")]
        [Tooltip("Movement speed of the NPC")]
        public float moveSpeed = 3.5f;
        [Tooltip("How close the NPC needs to be to a waypoint to consider it reached")]
        public float waypointThreshold = 0.5f;
        [Tooltip("Time in seconds the NPC waits at each waypoint")]
        public float waitTime = 2f;
        
        [Header("Animation")]
        [Tooltip("Animator controller for the NPC")]
        public Animator[] animators;
        [Tooltip("Name of the walking parameter in the animator")]
        public string walkParameter = "walk";
        
        private int currentWaypointIndex = 0;
        private bool isWaiting = false;
        private bool isPatrolling = true;
        
        void Start()
        {
            // Validate waypoints
            if (waypoints == null || waypoints.Length == 0)
            {
                Debug.LogError("No waypoints assigned to NPCPatrol script!");
                isPatrolling = false;
                return;
            }
            
            // Face the first waypoint initially
            if (waypoints.Length > 0)
            {
                transform.LookAt(waypoints[0].position);
            }
        }
        
        void Update()
        {
            if (!isPatrolling || isWaiting) return;
            
            // Get current waypoint
            Transform currentWaypoint = waypoints[currentWaypointIndex];
            
            // Move towards the waypoint
            transform.position = Vector3.MoveTowards(
                transform.position, 
                currentWaypoint.position, 
                moveSpeed * Time.deltaTime
            );
            
            // Rotate to face the waypoint
            Vector3 direction = (currentWaypoint.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            
            // Update animator
            foreach (Animator _anim in animators)
            {
                _anim.SetBool(walkParameter, true);
            }
            
            // Check if reached the waypoint
            if (Vector3.Distance(transform.position, currentWaypoint.position) < waypointThreshold)
            {
                StartCoroutine(WaitAtWaypoint());
            }
        }

        public void StopFor(float time)
        {
            StartCoroutine(UnexpectedStop(time));
        }

        IEnumerator UnexpectedStop(float time)
        {
            isWaiting = true;
            // Stop movement animation
            foreach (Animator _anim in animators)
            {
                _anim.SetBool(walkParameter, false);
            }
            
            // Wait for the specified time
            yield return new WaitForSeconds(time);

            isWaiting = false;
        }
        
        IEnumerator WaitAtWaypoint()
        {
            isWaiting = true;
            
            // Stop movement animation
            foreach (Animator _anim in animators)
            {
                _anim.SetBool(walkParameter, false);
            }
            
            // Wait for the specified time
            yield return new WaitForSeconds(waitTime);
            
            // Move to next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            isWaiting = false;
        }
        
        // Draw gizmos for waypoints in editor
        void OnDrawGizmos()
        {
            if (waypoints == null) return;
            
            Gizmos.color = Color.cyan;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    Gizmos.DrawSphere(waypoints[i].position, 0.3f);
                    if (i < waypoints.Length - 1 && waypoints[i+1] != null)
                    {
                        Gizmos.DrawLine(waypoints[i].position, waypoints[i+1].position);
                    }
                    else if (i == waypoints.Length - 1 && waypoints[0] != null)
                    {
                        Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
                    }
                }
            }
        }
    }
}


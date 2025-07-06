using UnityEngine;

namespace npc_system
{
    public class ChefNPC : HumanNPC
    {
        private void Update()
        {
            if(inRadius)
            {
                WaitForPositiveBehavior();
            }
        }

    }
}


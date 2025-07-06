using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace npc_system
{
    public class KidNPC : HumanNPC
    {
        private void Update()
        {
            if(inRadius)
            {
                WaitForPositiveBehavior();
                WaitForNegativeBehavior();
            }
        }
    }
}

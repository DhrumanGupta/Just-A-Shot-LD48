using System;
using System.Collections;
using System.Collections.Generic;
using Game.Management;
using UnityEngine;

namespace Game.Interaction
{
    public class OldMan : Npc
    {
        private void Awake()
        {
            base.OnInteractionComplete += HandleInteractionComplete;
        }

        private void HandleInteractionComplete()
        {
            KeyManager.Instance.AddKey();
        }
    }
}

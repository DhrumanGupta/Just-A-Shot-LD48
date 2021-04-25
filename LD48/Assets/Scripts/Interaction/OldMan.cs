using Game.GameManagement;

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
            if (HasInteracted) return;
            KeyManager.Instance.AddKeys(1);
        }
    }
}

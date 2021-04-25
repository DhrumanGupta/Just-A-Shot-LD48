using Game.GameManagement;

namespace Game.Environment
{
    public class OldMan : Npc
    {
        private void Awake()
        {
            OnInteractionComplete += HandleInteractionComplete;
        }

        private void HandleInteractionComplete()
        {
            if (HasInteracted) return;
            KeyManager.Instance.AddKeys(1);
        }
    }
}

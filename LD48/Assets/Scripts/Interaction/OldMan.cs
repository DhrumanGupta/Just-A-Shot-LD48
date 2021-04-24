using Game.GameManagement;

namespace Game.Interaction
{
    public class OldMan : Npc
    {
        private bool _hasInteracted = false;
        
        private void Awake()
        {
            base.OnInteractionComplete += HandleInteractionComplete;
        }

        private void HandleInteractionComplete()
        {
            if (_hasInteracted) return;
            
            _hasInteracted = true;
            KeyManager.Instance.AddKeys(1);
        }
    }
}

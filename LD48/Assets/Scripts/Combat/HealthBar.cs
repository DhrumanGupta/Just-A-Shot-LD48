using UnityEngine;
using UnityEngine.UI;

namespace Game.Combat
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private Transform _healthBar = null;
        [SerializeField] private Sprite[] _hearts;
        private Image[] _images;

        private void Awake()
        {
            _health.OnHealthChanged += UpdateHealthBar;
            
            _images = new Image[_healthBar.childCount];
            for (int i = 0; i < _healthBar.childCount; i++)
            {
                var child = _healthBar.GetChild(i);
                _images[i] = child.GetComponent<Image>();
            }
        }

        public void UpdateHealthBar(int health)
        {
            var fullHearts = health / 2;
            
            var hasHalfHeart = IntToBool(health % 2);

            foreach (var child in _images)
            {
                child.sprite = null;
            }

            for (var i = 0; i < _images.Length; i++)
            {
                if (i < fullHearts) 
                    _images[i].sprite = _hearts[0];
                else if (hasHalfHeart && i == fullHearts)
                    _images[i].sprite = _hearts[1];
                else
                    _images[i].sprite = _hearts[2];
            }
        }

        private bool IntToBool(int value) => value == 1;
    }
}
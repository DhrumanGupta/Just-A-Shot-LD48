using System;
using UnityEngine;

namespace Game.Obstacles
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Door : MonoBehaviour
    {
        private Animator _animator;
        private Collider2D _collider;
        
        private int _animatorOpenId;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _collider = GetComponent<BoxCollider2D>();
            _animatorOpenId = Animator.StringToHash("isOpen");
        }
        
        /// <summary>
        /// Set's the doors state
        /// </summary>
        /// <param name="isVisible">The state of the door</param>
        public void SetState(bool isVisible)
        {
            _collider.isTrigger = !isVisible;
            _animator.SetBool(_animatorOpenId, isVisible);
        }
    }
}

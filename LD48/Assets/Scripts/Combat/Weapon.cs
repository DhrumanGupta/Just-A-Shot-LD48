using System;
using Game.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Just a Shot/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [Header("References")]
        [SerializeField] private AnimatorOverrideController _animatorOverride = null;
        [SerializeField] private Projectile _projectile = null;
        [field: SerializeField] public Transform ProjectileSpawnTransform { get; private set; }
        
        [Header("Stats")]
        [SerializeField] private float _weaponDamage = 5f;
        [SerializeField] private float _weaponRange = 1.55f;

        private const string _weaponName = "Weapon";

        public void Spawn(Animator animator)
        {
            // If there is no animator override given and animator is an override controller,
            // set the animator's controller as the override's runtime controller
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (_animatorOverride != null)
            {
                animator.runtimeAnimatorController = _animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
        }

        public bool HasProjectile()
        {
            return _projectile != null;
        }

        public void LaunchProjectile(Transform spawnLocation, Health target)
        {
            var projectileInstance = Instantiate(_projectile, spawnLocation.position, Quaternion.identity);
            projectileInstance.SetTarget(target, _weaponDamage);
        }

        public float GetDamage()
        {
            return _weaponDamage;
        }

        public float GetRange()
        {
            return _weaponRange;
        }
    }
}
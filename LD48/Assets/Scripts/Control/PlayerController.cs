using System.Collections.Generic;
using UnityEngine;

namespace Game.Control
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerController : MonoBehaviour
    {
        #region Private Variables

        #region References

        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private Camera _camera;
        [SerializeField] private LayerMask _whatIsGround;

        #endregion

        #region Movement Data

        private float _input;

        [Space] [Header("Movement")] [SerializeField]
        private float _moveSpeed;

        [SerializeField] private float _jumpForce;
        [SerializeField] private float _frictionForce;
        [SerializeField] private Transform _groundCheck;

        private bool _isJumping;
        private bool _isGrounded;

        #endregion

        private int _animatorRunId;
        private int _animatorIdleId;
        private int _animatorJumpId;

        [SerializeField] private GameObject _jumpEffectPrefab = null;
        private bool _isJumpEffectPrefabNotNull;

        #endregion

        #region Unity Events

        private void Awake()
        {
            this._camera = Camera.main;
            this._rigidbody = GetComponent<Rigidbody2D>();
            this._spriteRenderer = GetComponent<SpriteRenderer>();
            this._animator = GetComponent<Animator>();

            _animatorRunId = Animator.StringToHash("isRunning");
            _animatorIdleId = Animator.StringToHash("isIdle");
            _animatorJumpId = Animator.StringToHash("isJumping");

            _isJumpEffectPrefabNotNull = _jumpEffectPrefab != null;
        }

        private void Update()
        {
            GetInput();
            FlipSpriteBasedOnDirection();
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            CheckIfGrounded();
            Move();
            Jump();
        }

        #endregion

        private void GetInput()
        {
            this._input = Input.GetAxis("Horizontal");
            this._isJumping = Input.GetButtonDown("Jump") && this._isGrounded;
        }

        private void FlipSpriteBasedOnDirection()
        {
            if (_input == 0) return;
            this._spriteRenderer.flipX = this._input > 0;
        }

        private void UpdateAnimator()
        {
            bool isPlayerMoving = _input == 0;
            this._animator.SetBool(_animatorIdleId, isPlayerMoving);
            if (isPlayerMoving) return;

            this._animator.SetBool(_animatorRunId, Mathf.Abs(_input) > 0.1f);
        }

        private void CheckIfGrounded()
        {
            _isGrounded = Physics2D.OverlapCircle(this._groundCheck.position, .1f, _whatIsGround);
        }

        private void Move()
        {
            var currentVelocity = this._rigidbody.velocity;

            currentVelocity = new Vector2(this._input * _moveSpeed, currentVelocity.y);
            this._rigidbody.velocity = currentVelocity;

            // Apply some friction
            Vector2 friction = currentVelocity.normalized * _frictionForce;
            friction.x *= -1;
            friction.y = 0;
            this._rigidbody.AddForce(friction, ForceMode2D.Force);
        }

        private void Jump()
        {
            if (!this._isJumping) return;

            if (_isJumpEffectPrefabNotNull)
                Destroy(Instantiate(_jumpEffectPrefab, this._groundCheck.transform.position, Quaternion.identity), 4f);

            this._rigidbody.AddForce(new Vector2(0, this._jumpForce), ForceMode2D.Impulse);
            this._isJumping = false;
            this._isGrounded = false;
        }
    }
}
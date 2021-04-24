using System.Collections.Generic;
using UnityEngine;

namespace Game.Control
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(BoxCollider2D))]
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

        private float _inputX;

        [Space] [Header("Movement")] [SerializeField]
        private float _moveSpeed;

        [SerializeField] private float _jumpForce;
        [SerializeField] private float _frictionForce;
        
        [Header("Jumping")]
        [SerializeField] private float _checkRadius = 0.2f;
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private int _airJumps = 1;
        private int _airJumpsLeft;
        private bool _isGrounded;
        private bool _isJumping;
        
        [Header("Wall Jumping")]
        [SerializeField] private Transform _frontCheck;
        [SerializeField] private float _wallSlideSpeed = 1f;
        [SerializeField] private int _wallJumps = 1;

        private bool _isTouchingFront;
        private bool _isWallSliding;
        private bool _isWallJumping;
        private int _wallJumpsLeft;
        private Transform _lastWallTouched;

        #endregion

        private int _animatorRunId;
        private int _animatorIdleId;
        private int _animatorJumpId;

        [SerializeField] private GameObject _jumpEffectPrefab = null;
        private bool _isJumpEffectPrefabNotNull;

        private new Transform transform;

        #endregion

        #region Unity Events

        private void Awake()
        {
            transform = GetComponent<Transform>();
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
            FlipBasedOnDirection();
            UpdateAnimator();
            print(_wallJumpsLeft);
        }

        private void FixedUpdate()
        {
            CheckTransforms();
            Move();
            Jump();
            WallSlide();
            WallJump();
        }

        #endregion

        private void GetInput()
        {
            _inputX = Input.GetAxis("Horizontal");
            var jump = Input.GetButtonDown("Jump");
            _isJumping = jump && (this._isGrounded || _airJumpsLeft > 0);
            _isWallJumping = jump && _isWallSliding && _wallJumpsLeft > 0;
        }

        private void FlipBasedOnDirection()
        {
            if (_inputX == 0) return;
            
            var localScale = transform.localScale;
            localScale.x = _inputX < 0 ? -1f : 1f;
            transform.localScale = localScale;
        }

        private void UpdateAnimator()
        {
            bool isPlayerMoving = _inputX == 0;
            this._animator.SetBool(_animatorIdleId, isPlayerMoving);
            if (isPlayerMoving) return;

            this._animator.SetBool(_animatorRunId, Mathf.Abs(_inputX) > 0.1f);
        }

        private void CheckTransforms()
        {
            _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _checkRadius, _whatIsGround);
            var newWallExists = Physics2D.OverlapCircle(_frontCheck.position, _checkRadius, _whatIsGround);

            _isTouchingFront = newWallExists;
            if (!_isTouchingFront) return;
            
            if (newWallExists.transform != _lastWallTouched)
            {
                _wallJumpsLeft = _wallJumps;
                print("WADAWDWADWA");
            }
            
            _lastWallTouched = newWallExists.transform;
        }

        private void Move()
        {
             var currentVelocity = new Vector2(this._inputX * _moveSpeed, _rigidbody.velocity.y);
            _rigidbody.velocity = currentVelocity;

            // Apply some friction
            Vector2 friction = currentVelocity.normalized * _frictionForce;
            friction.x *= -1;
            friction.y = 0;
            _rigidbody.AddForce(friction, ForceMode2D.Force);
        }

        private void Jump()
        {
            if (_isGrounded)
            {
                _airJumpsLeft = _airJumps;
                _wallJumpsLeft = _wallJumps;
            }
            
            if (!_isJumping) return;

            if (_isJumpEffectPrefabNotNull)
                Destroy(Instantiate(_jumpEffectPrefab, this._groundCheck.transform.position, Quaternion.identity), 4f);

            _rigidbody.velocity =
                new Vector2(_rigidbody.velocity.x, Mathf.Clamp(_rigidbody.velocity.y, -1, float.MaxValue));
            _rigidbody.AddForce(new Vector2(0, this._jumpForce), ForceMode2D.Impulse);
            _animator.SetTrigger(_animatorJumpId);
            _isJumping = false;

            if (_isGrounded)
                _isGrounded = false;
            else
                _airJumpsLeft--;
        }

        private void WallSlide()
        {
            _isWallSliding = _isTouchingFront && !_isGrounded && _inputX != 0;

            if (_isWallSliding)
            {
                var velocity = _rigidbody.velocity;
                _rigidbody.velocity = new Vector2(velocity.x,
                    Mathf.Clamp(velocity.y, -_wallSlideSpeed, float.MaxValue));
            }
        }

        private void WallJump()
        {
            if (!_isWallJumping) return;

            if (_isJumpEffectPrefabNotNull)
                Destroy(Instantiate(_jumpEffectPrefab, this._groundCheck.transform.position, Quaternion.identity), 4f);

            _rigidbody.velocity =
                new Vector2(_rigidbody.velocity.x, Mathf.Clamp(_rigidbody.velocity.y, -1, float.MaxValue));
            _rigidbody.AddForce(new Vector2(0, this._jumpForce), ForceMode2D.Impulse);
            
            _animator.SetTrigger(_animatorJumpId);
            _isWallJumping = false;
            _wallJumpsLeft--;
        }
    }
}
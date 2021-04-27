using System;
using System.Linq;
using Game.Combat;
using Game.Environment;
using Game.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Control
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Fighter))]
    [RequireComponent(typeof(Health))]
    public class PlayerController : MonoBehaviour
    {
        #region Private Variables

        #region References

        private Rigidbody2D _rigidbody;
        private Animator _animator;
        private Camera _camera;
        private Fighter _fighter;
        
        #endregion

        #region Movement Data

        private float _inputX;

        [Space] [Header("Movement")] 
        [SerializeField] private LayerMask _whatIsGround;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _frictionForce;

        [Header("Jumping")] [SerializeField] private float _checkRadius = 0.2f;
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private int _airJumps = 1;
        private int _airJumpsLeft;
        private bool _isGrounded;
        private bool _isJumping;

        [Header("Wall Jumping")] [SerializeField]
        private Transform _frontCheck;

        [SerializeField] private float _wallSlideSpeed = 1f;
        [SerializeField] private int _wallJumps = 1;

        private bool _isTouchingFront;
        private bool _isWallSliding;
        private bool _isWallJumping;
        private int _wallJumpsLeft;

        #endregion

        private int _animatorRunId;
        private int _animatorGroundId;
        private int _animatorWallSlideId;

        [SerializeField] private GameObject _jumpEffectPrefab = null;
        private bool _isJumpEffectPrefabNotNull;

        [Header("Interaction")]
        [SerializeField] private GameObject _interactSprite = null;

        private new Transform transform;

        #endregion

        #region Unity Events

        private void Awake()
        {
            transform = GetComponent<Transform>();
            _camera = Camera.main;
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _fighter = GetComponent<Fighter>();

            _animatorRunId = Animator.StringToHash("isWalking");
            _animatorGroundId = Animator.StringToHash("isGrounded");
            _animatorWallSlideId = Animator.StringToHash("isWallsliding");

            _isJumpEffectPrefabNotNull = _jumpEffectPrefab != null;
        }

        private void Update()
        {
            GetInput();
            FlipBasedOnDirection();
            UpdateAnimator();
            Interact();
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

        #region Input and Visuals

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
            _animator.SetBool(_animatorRunId, Mathf.Abs(_rigidbody.velocity.x) > 0.1f);
            _animator.SetBool(_animatorGroundId, _isGrounded);
            _animator.SetBool(_animatorWallSlideId, _isWallSliding);
        }

        private void CheckTransforms()
        {
            _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _checkRadius, _whatIsGround);
            var newWallExists = Physics2D.OverlapCircle(_frontCheck.position, _checkRadius, _whatIsGround);

            _isTouchingFront = newWallExists;
            if (_isTouchingFront)
            {
                _wallJumpsLeft = _wallJumps;
            }
        }

        #endregion

        #region Movement

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
                Instantiate(_jumpEffectPrefab, this._groundCheck.transform.position, Quaternion.identity);

            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
            _rigidbody.AddForce(new Vector2(0, this._jumpForce), ForceMode2D.Impulse);

            _isJumping = false;

            if (_isGrounded)
                _isGrounded = false;
            else
                _airJumpsLeft--;
        }

        private void WallSlide()
        {
            _isWallSliding = _isTouchingFront && !_isGrounded; // && _inputX != 0;

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

            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);

            _rigidbody.AddForce(new Vector2(0, _jumpForce), ForceMode2D.Impulse);

            _isWallJumping = false;
            _wallJumpsLeft--;
        }

        #endregion

        // ReSharper disable Unity.PerformanceAnalysis
        private void Interact()
        {
            var results = new RaycastHit2D[3];
            var size = Physics2D.RaycastNonAlloc(transform.position, transform.right * transform.localScale.x, results,
                3f);

            if (size == 0) return;

            var result = results.Where(x => x.transform != null && x.transform.TryGetComponent(out IInteractable _))
                .ToArray().FirstOrDefault();
            IInteractable interactable =
                result.transform != null ? result.transform.GetComponent<IInteractable>() : null;

            var isInteractableNull = interactable == null;

            if (_interactSprite.activeSelf && isInteractableNull) _interactSprite.SetActive(false);
            else if (!_interactSprite.activeSelf && !isInteractableNull) _interactSprite.SetActive(true);

            if (isInteractableNull) return;
            _interactSprite.transform.position = (result.transform.position + transform.position) / 2f;
            _interactSprite.GetComponent<SpriteRenderer>().flipX = transform.localScale.x < 0;

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(interactable.Interact());
            }
        }

        public void Die()
        {
            GameObject.FindObjectOfType<SavingWrapper>().LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // If collision wasn't from the bottom, dont do anything
            if (Vector3.Dot(other.GetContact(0).normal, Vector3.up) <= 0.75f) return;

            if (!other.collider.TryGetComponent(out Health target)) return;
            _fighter.Attack(target);
        }

        private void OnEnable()
        {
            _rigidbody.isKinematic = false;
        }
        
        private void OnDisable()
        {
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.isKinematic = true;
        }
    }
}
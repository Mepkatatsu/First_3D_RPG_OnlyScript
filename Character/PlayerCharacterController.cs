using System.Collections.Generic;
using UnityEngine;

namespace SingletonPattern
{
    public class PlayerCharacterController : Singleton<PlayerCharacterController>, IAttackable, IDamageable
    {
        #region Variables

        private Animator _animator;
        private CharacterController _controller;
        private AttackStateController _attackStateController;
        private PlayerStat _playerStat;

        [SerializeField] private Transform _hitPoint;
        [HideInInspector] public GameObject target;

        private readonly int _isMovingHash = Animator.StringToHash("IsMoving");
        private readonly int _specialIdleTriggerHash = Animator.StringToHash("SpecialIdleTrigger");
        private readonly int _specialIdleNumberHash = Animator.StringToHash("SpecialIdleNumber");
        private readonly int _attackAnimationIndexHash = Animator.StringToHash("AttackAnimationIndex");
        private readonly int _attackTriggerHash = Animator.StringToHash("AttackTrigger");
        private readonly int _comboAttackTriggerHash = Animator.StringToHash("ComboAttackTrigger");
        private readonly int _isAliveHash = Animator.StringToHash("IsAlive");
        private readonly int _hitTriggerHash = Animator.StringToHash("HitTrigger");

        private Vector3 _moveDirection = new();
        private Vector3 _previousMoveDirection = new();
        private Vector3 _idleStartPosition = new();
        private Quaternion _lookRotation = new();
        private Quaternion _idleStartRotation = new();

        private float _defaultMoveSpeed = 7f;
        private float _normalAttackComboTime = 0;
        private float _idleElapsedTime = 0;
        private float _stepSoundCooltime = 0;
        private float _inputX = 0;
        private float _inputZ = 0;

        private float _joystickInputX = 0;
        private float _joystickInputY = 0;

        private bool _isPlayedSpecialIdle = false;

        private const float TwistDanceTime = 9.433f;
        private const float SwingDanceTime = 4.4f;

        #endregion Variables

        #region Properties
        public bool IsInAttackState => _attackStateController.IsInAttackState;
        public bool IsInNornalAttackState { get; set; }
        public int Level => _playerStat.playerStats.level;

        public LayerMask targetMask;

        #endregion Properties

        #region Unity Methods

        // Start is called before the first frame update
        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _attackStateController = GetComponent<AttackStateController>();
            _playerStat = GetComponent<PlayerStat>();

            InitAttackBehaviour();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!IsAlive) return;

            _inputX = Input.GetAxis("Horizontal");
            _inputZ = Input.GetAxis("Vertical");

            _previousMoveDirection = _moveDirection;
            _moveDirection = new Vector3(_inputX, 0, _inputZ).normalized;
            if (_joystickInputX != 0 || _joystickInputY != 0)
            {
                _moveDirection = new Vector3(_joystickInputX, 0, _joystickInputY);
            }

            // 자연스러운 애니메이션 연결을 위해 공격 중에도 이동 애니메이션 체크
            if (_moveDirection == Vector3.zero)
            {
                // 반대로 방향을 전환할 때 Idle 모션으로 전환되지 않도록 직전 프레임 상태 확인
                if (_previousMoveDirection == Vector3.zero)
                {
                    _animator.SetBool(_isMovingHash, false);
                }
            }
            else
            {
                _animator.SetBool(_isMovingHash, true);

                if (_stepSoundCooltime <= 0)
                {
                    _stepSoundCooltime = 0.3f;
                    AudioManager.Instance.PlaySFX("Step");
                }
            }

            if (IsInAttackState)
            {
                if(target == null)
                {
                    return;
                }

                Vector3 direction = (target.transform.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

                return;
            }

            if (_moveDirection == Vector3.zero)
            {
                _idleElapsedTime += Time.deltaTime;
                CheckIdleElapsedTime();
            }
            else
            {
                MoveCharacter(_moveDirection);
                ResetIdleTime();
            }

            // 특수 대기 모션의 종료 지점이 원점이 아니기 때문에 시작 지점의 rotation과 position을 맞춰줌
            if (_isPlayedSpecialIdle && _idleElapsedTime > 0)
            {
                _controller.enabled = false;
                transform.rotation = Quaternion.Slerp(transform.rotation, _idleStartRotation, Time.deltaTime * 5);
                transform.position = Vector3.Lerp(transform.position, _idleStartPosition, 0.1f);
                _controller.enabled = true;
            }

            if (_normalAttackComboTime > 0)
            {
                _normalAttackComboTime -= Time.deltaTime;

                if (_normalAttackComboTime <= 0)
                {
                    ResetNormalAttackCooltime();
                }
            }

            if (_stepSoundCooltime > 0)
            {
                _stepSoundCooltime -= Time.deltaTime;
            }
            
        }

        #endregion Unity Methods

        #region Methods

        private void InitAttackBehaviour()
        {
            foreach (AttackBehaviour behaviour in _attackBehaviours)
            {
                behaviour.targetMask = targetMask;
            }
        }

        private void ResetIdleTime()
        {
            _isPlayedSpecialIdle = false;
            _idleElapsedTime = 0;
        }

        private void ResetNormalAttackCooltime()
        {
            foreach (AttackBehaviour behaviour in _attackBehaviours)
            {
                if (behaviour.attackIndex == 0)
                {
                    behaviour.ResetCooltime();
                }
            }
        }

        public void ExecuteAttackButton(int buttonIndex)
        {
            if (!IsAlive) return;

            CurrentAttackBehaviour = null;

            foreach (AttackBehaviour behaviour in _attackBehaviours)
            {
                if (behaviour.attackIndex == buttonIndex)
                {
                    if (behaviour.IsAvailable)
                    {
                        if ((CurrentAttackBehaviour == null) || (CurrentAttackBehaviour.priority < behaviour.priority))
                        {
                            CurrentAttackBehaviour = behaviour;
                        }
                    }
                }
            }

            Attack();
        }

        private void Attack()
        {
            if (CurrentAttackBehaviour == null) return;

            // 기본 공격 콤보 연결
            if (IsInNornalAttackState && CurrentAttackBehaviour.attackIndex == 0 && CurrentAttackBehaviour.attackAnimationIndex != 0 && !_animator.GetBool(_comboAttackTriggerHash))
            {
                _animator.SetTrigger(_comboAttackTriggerHash);
                CurrentAttackBehaviour.StartCooltime();

                // 3연속 기본공격을 성공하면 다시 처음부터 수행할 수 있도록 1, 2번째 기본 공격의 쿨타임을 초기화
                if (CurrentAttackBehaviour.attackAnimationIndex == 2)
                {
                    ResetNormalAttackCooltime();
                }

                return;
            }

            if (!IsInAttackState && CurrentAttackBehaviour.IsAvailable)
            {
                _attackStateController.IsInAttackState = true;

                CurrentAttackBehaviour.StartCooltime();
                _animator.SetTrigger(_attackTriggerHash);
                _animator.SetInteger(_attackAnimationIndexHash, CurrentAttackBehaviour.attackAnimationIndex);

                ResetIdleTime();

                // 기본 공격을 일정 시간 이내에 연결하지 않으면 첫 번째 기본공격부터 다시 공격하도록 함
                if (CurrentAttackBehaviour.attackIndex == 0)
                {
                    _normalAttackComboTime = 1;
                    IsInNornalAttackState = true;

                    // 3연속 기본공격을 성공하면 다시 처음부터 수행할 수 있도록 1, 2번째 기본 공격의 쿨타임을 초기화
                    if (CurrentAttackBehaviour.attackAnimationIndex == 2)
                    {
                        ResetNormalAttackCooltime();
                    }
                }
            }
        }

        private void MoveCharacter(Vector3 moveDirection)
        {
            _controller.Move(_defaultMoveSpeed * Time.deltaTime * moveDirection);
            _lookRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 10);
        }

        public void SetJoystickInput(float joystickInputX, float joystickInputY)
        {
            _joystickInputX = joystickInputX;
            _joystickInputY = joystickInputY;
        }

        private void CheckIdleElapsedTime()
        {
            if (_idleElapsedTime > 10)
            {
                _isPlayedSpecialIdle = true;
                _idleStartRotation = transform.rotation;
                _idleStartPosition = transform.position;

                int randomNumber = Random.Range(0, 2);

                _animator.SetTrigger(_specialIdleTriggerHash);
                _animator.SetInteger(_specialIdleNumberHash, randomNumber);

                if (randomNumber == 0) _idleElapsedTime = -SwingDanceTime;
                else if (randomNumber == 1) _idleElapsedTime = -TwistDanceTime;
            }
        }

        #endregion Methods

        #region IDamageable Interface
        public bool IsAlive => _playerStat.playerStats.CurrentHP > 0;

        public void TakeDamage(int damage, GameObject hitEffect, Transform attackFrom)
        {
            if (!IsAlive)
            {
                return;
            }

            _playerStat.playerStats.AddHealth(-damage);

            AudioManager.Instance.PlaySFX("Hit");

            if (hitEffect)
            {
                Instantiate<GameObject>(hitEffect, _hitPoint);
            }

            if (IsAlive)
            {
                if (!IsInAttackState)
                {
                    _animator.SetTrigger(_hitTriggerHash);
                }
            }
            else
            {
                _animator.SetBool(_isAliveHash, false);
            }
        }
        #endregion IDamageable Interface

        #region IAttackable Interface
        [SerializeField] private List<AttackBehaviour> _attackBehaviours = new();
        public AttackBehaviour CurrentAttackBehaviour
        {
            get;
            private set;
        }
        public void OnExecuteAttack(int attackIndex)
        {
            AudioManager.Instance.PlaySFX("Attack" +  (attackIndex + 1));
            foreach (AttackBehaviour behaviour in _attackBehaviours)
            {
                if (behaviour.attackAnimationIndex == attackIndex)
                {
                    behaviour.ExecuteAttack();
                    return;
                }
            }
        }
        #endregion IAttackable Interface
    }

}
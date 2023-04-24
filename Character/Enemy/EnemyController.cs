using SingletonPattern;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(CharacterController))]
public abstract class EnemyController : MonoBehaviour
{
    #region Variables
    protected StateMachine<EnemyController> stateMachine;
    public string enemyName = "";
    public Sprite enemyIcon;
    public virtual float AttackRange => 3.0f;
    public bool isInBattle = false;

    private EnemyAreaController _enemyAreaController;
    protected Transform target;
    protected NavMeshAgent agent;
    protected Animator animator;
    public Vector3 patrolPosition;

    #endregion Variables

    #region Properties

    public Transform Target
    {
        get
        {
            if (target.GetComponent<PlayerCharacterController>().IsAlive && _enemyAreaController.IsPlayerInArea && isInBattle)
            {
                return target;
            }
            else return null;
        }
    }
    public LayerMask TargetMask;
    public bool isPatrol = false;

    #endregion Properties

    #region Unity Methods

    // Start is called before the first frame update
    protected virtual void Start()
    {
        stateMachine = new StateMachine<EnemyController>(this, new IdleState());
        target = PlayerCharacterController.Instance.transform;
        _enemyAreaController = transform.parent.GetComponent<EnemyAreaController>();

        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = true;

        animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        stateMachine.Update(Time.deltaTime);
        if (stateMachine.CurrentState is not MoveToTargetState && stateMachine.CurrentState is not DeadState)
        {
            FaceTarget();
        }
    }

    private void OnMouseDown()
    {
        OnDrawEnemyOutline();
        PlayerCharacterController.Instance.target = gameObject;
        UIManager.Instance.UpdateEnemyUI(gameObject);
    }

    public void OnDrawEnemyOutline()
    {
        if (gameObject.name.Equals("Lich(Clone)")) OutlineManager.Instance.OnDrawOutline(transform.GetChild(1).gameObject);
        else if (gameObject.name.Equals("Skeleton(Clone)")) OutlineManager.Instance.OnDrawOutline(transform.GetChild(2).gameObject);
    }

    #endregion Unity Methods

    #region Helper Methods
    public virtual bool IsAvailableAttack => false;

    public R ChangeState<R>() where R : State<EnemyController>
    {
        return stateMachine.ChangeState<R>();
    }

    public void SetPatrolPosition()
    {
        while(true)
        {
            float randomX = Random.Range(-5f, 5);
            float randomZ = Random.Range(-5f, 5);

            Vector3 patrolPosition = new Vector3(randomX, 0, randomZ) + transform.position;

            if (_enemyAreaController.CheckEnemyMoveRange(patrolPosition) == true)
            {
                this.patrolPosition = patrolPosition;
                break;
            }
        }
    }

    void FaceTarget()
    {
        if (Target)
        {
            Vector3 direction = (Target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    #endregion Helper Methods
}

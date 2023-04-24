public abstract class State<T>
{
    protected int mecanimStateHash;
    protected StateMachine<T> stateMachine;
    protected T context;

    public State()
    {
    }

    internal void SetMachineAndContext(StateMachine<T> stateMachine, T context)
    {
        this.stateMachine = stateMachine;
        this.context = context;

        OnInitialized();
    }

    public virtual void OnInitialized()
    { }

    public virtual void OnEnter()
    { }

    public virtual void PreUpdate()
    { }

    public abstract void Update(float deltaTime);

    public virtual void OnExit()
    { }
}

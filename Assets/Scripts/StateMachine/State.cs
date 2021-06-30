/// <summary>
/// This class represents and is superclass to all States a StateMachine can be in
/// </summary>
public abstract class State {

	/// <summary>
	/// The StateMachine that owns this State
	/// </summary>
	public StateMachine owner;


	/// <summary>
	/// This is used to do preparation work after a transition happened
	/// </summary>
	public virtual void StartState() {
	}

	/// <summary>
	/// Translation for the unity method
	/// </summary>
	public virtual void UpdateState() {
	}

	/// <summary>
	/// This is used to do possible clean up work before a transition happens
	/// </summary>
	public virtual void FinalizeState() {
	}
}

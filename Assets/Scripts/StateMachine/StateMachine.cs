using UnityEngine;
using UnityEngine.XR.ARFoundation;
    
/// <summary>
/// A StateMachine containing a State
/// </summary>
public class StateMachine : MonoBehaviour {

	public GameObject circlePrefab;
	public LineRenderer lineRendererPrefab;
	public ARSessionOrigin sessionOrigin;

	public GameObject navPoint;


	/// <summary>
	/// The current State of the StateMachine
	/// </summary>
	public State state;

	private void Start() {
		this.state = new MainScreenState(this);
		this.state.StartState();
	}

	private void Update() {
		// If there is a state, tell it that Unity called an Update.
		if (this.state != null) {
			this.state.UpdateState();
		}
	}

    /// <summary>
	/// This is used to change the current State of the StateMachine.
	/// It should always be called from States within this StateMachine.
	/// </summary>
	public void ChangeState(State newState) {
		// if there is a previous state, do possible clean up work
		if (this.state != null) {
			this.state.FinalizeState();
		}

		// assign new state
		this.state = newState;

		// initialize the new state
		this.state.StartState();
	}
}

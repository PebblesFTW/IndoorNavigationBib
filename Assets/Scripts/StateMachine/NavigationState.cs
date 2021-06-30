using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class NavigationState : State {
    
    private Vector3 startPoint;
    private GameObject animationCircle; // TODO ?
    private LineRenderer lineRenderer;

    private Button menuButton;
    private Button rescanButton;
    private GameObject panel;
    private Transform[] highlighter;

    
    public NavigationState(StateMachine owner, Vector3 startPoint) {
        this.owner = owner;
        this.startPoint = startPoint;
        this.startPoint.y = 0;
        this.highlighter = new Transform[0];
    }

    public override void StartState() {
        // panel
        this.panel = this.owner.transform.Find("Canvas").Find("Nav Panel").gameObject;
        this.panel.SetActive(true);
        // Buttons
        this.menuButton = this.panel.transform.Find("ButtonMenu").gameObject.GetComponent<Button>();
        this.rescanButton = this.panel.transform.Find("ButtonRestart").gameObject.GetComponent<Button>();
        // listeners for buttons
        this.menuButton.onClick.AddListener(delegate {OnMenuClick();} );
        this.rescanButton.onClick.AddListener(delegate {OnRescanClick();} );

        // instantiate the line renderer without any points for now
        this.lineRenderer = GameObject.Instantiate(this.owner.lineRendererPrefab);
        this.lineRenderer.positionCount = 0;

        // calculate and draw path to goal position
        drawNavArrow();
    }

    private void drawNavArrow() {
        // set start position
        NavMeshHit startPosition;
        NavMesh.SamplePosition(this.startPoint, out startPosition, 2.5f, NavMesh.AllAreas);
        this.lineRenderer.positionCount = 0;
        
        // create indicator for the start position
        this.animationCircle = GameObject.Instantiate(this.owner.circlePrefab, startPosition.position, Quaternion.identity);
        // translate circle down a bit to render arrow/line above it
        this.animationCircle.transform.Translate(new Vector3(0, -0.01f, 0));
        this.animationCircle.transform.Rotate(new Vector3(90, 0, 0));

        // set goal position
        NavMeshHit goalPosition;
        Vector3 goal = this.owner.navPoint.transform.Find("NavPos").position;
        NavMesh.SamplePosition(goal, out goalPosition, 2.5f, NavMesh.AllAreas);

        // get highlighters
        this.highlighter = this.owner.navPoint.transform
                .GetComponentsInChildren<Transform>(true)
                .Where(transform => transform.gameObject.name.Equals("Highlighter")) // TODO tags?
                .ToArray();

        // activate highlighters
        if (this.highlighter.Length > 0) {
            Array.ForEach(this.highlighter, highlighter => highlighter.gameObject.SetActive(true));
        }
        
        // calculate path
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(startPosition.position, goalPosition.position, NavMesh.AllAreas, path)) {                
            // render line
            this.lineRenderer.positionCount = path.corners.Length;
            this.lineRenderer.SetPositions(path.corners);
        }
    }

    public override void UpdateState() {
        if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape)) {
            this.owner.ChangeState(new MarkerState(this.owner));
        }
    }

    public override void FinalizeState() {
        this.panel.SetActive(false);
        
        // remove listeners
        this.menuButton.onClick.RemoveListener(delegate {OnMenuClick();} );
        this.rescanButton.onClick.RemoveListener(delegate {OnRescanClick();} );

        // delete start position indicator
        if (this.animationCircle) {
            GameObject.Destroy(this.animationCircle);
        }

        // delete line
        if (this.lineRenderer) {
            GameObject.Destroy(this.lineRenderer);
        }
        
        // deactivate highlighters
        if (this.highlighter.Length > 0) {
            Array.ForEach(this.highlighter, highlighter => highlighter.gameObject.SetActive(false));
        }
    }

    private void OnMenuClick() {
        this.owner.ChangeState(new MainScreenState(this.owner));
    }

    private void OnRescanClick() {
        this.owner.ChangeState(new MarkerState(this.owner));
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MarkerState : State {
    private Button debugButton; // TODO remove

    private Button menuButton;
    private Button startButton;
    private GameObject panel;

    private ARTrackedImageManager trackedImageManager;

    
    public MarkerState(StateMachine owner) {
        this.owner = owner;
    }

    public override void StartState() {        
        // panel
        this.panel = this.owner.transform.Find("Canvas").Find("PosReset Panel").gameObject;
        this.panel.SetActive(true);

        // buttons
        this.debugButton = this.panel.transform.Find("DebugButton").gameObject.GetComponent<Button>();
        this.menuButton = this.panel.transform.Find("ButtonMenu").gameObject.GetComponent<Button>();
        this.startButton = this.panel.transform.Find("ButtonStart").gameObject.GetComponent<Button>();
        // add listeners for buttons
        this.menuButton.onClick.AddListener(delegate {OnMenuClick();} );
        this.startButton.onClick.AddListener(delegate {OnStartClick();} );
        this.debugButton.onClick.AddListener(delegate {OnStartClick();} );
        // disable start button, since there is no marker initially
        this.startButton.interactable = false;

        // tracked image manager
        this.trackedImageManager = this.owner.sessionOrigin.GetComponent<ARTrackedImageManager>();
        this.trackedImageManager.trackedImagesChanged += this.AdjustPosition;
    }

    public override void UpdateState() {
        if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape)) {
            this.owner.ChangeState(new MainScreenState(this.owner));
        }
    }

    public override void FinalizeState() {
        this.panel.SetActive(false);

        // remove listeners
        this.menuButton.onClick.RemoveListener(delegate {OnMenuClick();} );
        this.startButton.onClick.RemoveListener(delegate {OnStartClick();} );
        this.trackedImageManager.trackedImagesChanged -= this.AdjustPosition;
    }

    private void OnMenuClick() {
        this.owner.ChangeState(new MainScreenState(this.owner));
    }

    private void OnStartClick() {
        this.owner.ChangeState(new NavigationState(this.owner, this.owner.sessionOrigin.camera.transform.position));
    }

    private void AdjustPosition(ARTrackedImagesChangedEventArgs imagesChanged) {       
        // get all ar tracked images that are currently tracked 
        List<ARTrackedImage> markers = imagesChanged.added;

        List<ARTrackedImage> updatedImages = imagesChanged.updated;
        foreach (ARTrackedImage img in updatedImages) {
            if (img.trackingState == TrackingState.Tracking) {
                markers.Add(img);
            }
        }

        if (markers.Count == 0) {
            // there is no more marker visible
            this.startButton.interactable = false;
            return;
        }
        
        this.startButton.interactable = true;

        // find marker closest to camera position
        ARTrackedImage closest = markers[0];
        foreach (ARTrackedImage img in markers) {
            float currentDistance = Vector3.Distance(this.owner.sessionOrigin.camera.transform.position, img.transform.position);
            float closestDistance = Vector3.Distance(this.owner.sessionOrigin.camera.transform.position, closest.transform.position);
            if (currentDistance < closestDistance) {
                closest = img;
            }
        }
        
        // move ar session origin to match reality with model
        GameObject[] markerObjects = GameObject.FindGameObjectsWithTag("Marker");
        GameObject marker = Array.Find(markerObjects, marker => marker.name == closest.referenceImage.name);

        // Set rotation of sessionOrigin to align with world rotation
        //this.owner.sessionOrigin.transform.rotation = Quaternion.identity;
        // account for possible rotation of marker (to adjust camera perspective) so it doesn't appear tilted
        //this.owner.sessionOrigin.transform.rotation = Quaternion.Inverse(this.owner.sessionOrigin.camera.transform.localRotation) * marker.transform.rotation;
        
        //Quaternion desRot = closest.transform.localRotation * Quaternion.Euler(90, 0, 0); // works somewhat (tilt)

        // move ar session origin to match view on device
        this.owner.sessionOrigin.MakeContentAppearAt(marker.transform, closest.transform.position, closest.transform.localRotation);
        this.owner.sessionOrigin.transform.RotateAround(marker.transform.position, Vector3.right, -90);
    }

/* 
    private void BuildEscalationSets(GameObject[] navPointObjects, string tag) {
        EscalationSet escalationSet = new EscalationSet(tag);

        foreach (GameObject element in navPointObjects) {
            string[] splitName = element.name.Split(' ');
            EscalationSet intermediate = escalationSet;
            
            foreach (string step in splitName) {
                intermediate.Children.Add(new EscalationSet(step));
                intermediate.Children.TryGetValue(step, out intermediate);
            }
        }
    }
    
    private class EscalationSet : IComparable<EscalationSet> {
        private string name;
        public string Name {get;}
        private SortedSet<EscalationSet> children;
        public SortedSet<EscalationSet> Children {get;}

        public EscalationSet(string name) {
            this.Name = name;
            this.Children = new SortedSet<EscalationSet>();
        }

        public int CompareTo(EscalationSet other) {
            return this.Name.CompareTo(other.Name);
        }
    }
*/
}

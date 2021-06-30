using UnityEngine;

public class AttachDepthMask : MonoBehaviour {
    public Material depthMaskMaterial;

    private void Awake() {
        foreach (Transform g in transform.GetComponentsInChildren<Transform>()) {
            Renderer mat = g.gameObject.GetComponent<Renderer>();
            if (mat != null) {
                mat.material = this.depthMaskMaterial;
                g.gameObject.AddComponent<SetRenderQueue>();
            }
        }
    }    
}

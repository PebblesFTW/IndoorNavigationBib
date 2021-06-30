using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPoint : MonoBehaviour {
    private void Update()
    { }


    [TextArea]
    public string TxtInfoPoint;

    [SerializeField] Text txt;
    [SerializeField] GameObject panel;

    void Start() {
        txt.text = TxtInfoPoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            panel.SetActive(true);

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            panel.SetActive(false);

        }
    }

}


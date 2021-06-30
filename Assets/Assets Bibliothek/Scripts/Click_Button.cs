using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Click_Button : MonoBehaviour
{

	public string Url;

    public void Open()
    {
        Application.OpenURL(Url);
    }

    [SerializeField] GameObject Button; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Button.SetActive(true);

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Button.SetActive(false);

        }
    }

}

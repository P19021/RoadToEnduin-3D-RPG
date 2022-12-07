using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speech : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject indicator;
    [SerializeField] Camera mainCamera;
    [TextArea(3, 10)]
    public string text;


    void Update()
    {
        //Prompt always looks towards the camera
        if (indicator.activeInHierarchy)
        {
            indicator.transform.LookAt(mainCamera.transform);
        }
    }

    public void OnInteract(Interactor interactor)
    {
        indicator.SetActive(false); //Hide prompt
        //call interactor's public method ReceiveInteract with override method that gets a string as a parameter
        interactor.ReceiveInteract(text);
    }

    public void OnEndInteract()
    {
        Debug.LogWarning("Not handling End Interact");
        //Handle end of Interaction in later versions..
    }

    public void OnCancelInteract()
    {
        indicator.SetActive(false); //Hide prompt
    }

    public void OnReadyInteract()
    {
        indicator.SetActive(true); //Show prompt
    }
}

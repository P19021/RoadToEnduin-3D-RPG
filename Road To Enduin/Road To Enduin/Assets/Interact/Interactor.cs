using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    //------------------------------------------------------------------------- DECLARATIONS SECTION-----------------------------------------------------------

    //Setup Section----------------------------------------------------------------------------------------------------------------------------------------
    [Header("Setup")]

    [Tooltip("Tag used to identify interactable objects")]
    public string targetTag = "Interactable";

    [Tooltip("How far will the player look for interactable objects")]
    public float maxTargetRange = 5.0f;

    [Tooltip("All layers the player will search for interactable objects")]
    public LayerMask layerMask = ~0;

    [Tooltip("The UI that will be used by interactable items")]
    public InteractionUI interactionUI;

    [Tooltip("The main camera object")]
    public GameObject mainCamera;

    [Header("Input")]
    [Tooltip("All the keys that will be able to cancel an interaction")]
    public KeyCode interactKey = KeyCode.E;
    public KeyCode[] interactCancelKeys;


    //Interactable Objects----------------------------------------------------------------------------------------------------------------------------------
    IInteractable previousInteractSelection; //Object previously selected for interaction - Used for calculations between frames
    public IInteractable currentInteractSelection; //Object currently selected for interaction

    //State Booleans----------------------------------------------------------------------------------------------------------------------------------------
    bool readyToInteract; //States if the player is ready to interact
    bool isInteracting; //States if the player is currently interacting

//------------------------------------------------------------------------END OF DECLARATIONS SECTION--------------------------------------------------------

//------------------------------------------------------------------------------METHODS SECTION-------------------------------------------------------------
   
    
    // Start is called before the first frame update
    void Start()
    {
        //Reseting variables
        previousInteractSelection = null;
        currentInteractSelection = null;

        //Console warning in case variables dont have reference
        if (targetTag == null)
        {
            Debug.LogWarning("Interactor: Target tag is not set");
        }

        if(interactionUI == null)
        {
            Debug.LogWarning("Interactor: Interaction UI is not set");
        }

        //Call Abort method which will reset the rest of our variables
        CancelInteract();
    }

    // Update is called once per frame
    void Update()
    {
        currentInteractSelection = null;

        RaycastHit hit;

        //Checks if there is an object hit by the raycast
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxTargetRange, layerMask)) 
        {
            //Checks if the object hit is of tag "Interactable"
            if (hit.collider.CompareTag(targetTag))
            {
                //Get object components
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    //Sets the object to the current Interaction Available Object   
                    currentInteractSelection = interactable;
                }
            }
        }

        //Cases in which there was no check in the previous frame
        if(currentInteractSelection != null && !readyToInteract)
        {
            ReadyInteract();
        }else if(currentInteractSelection == null && readyToInteract)
        {
            CancelInteract();
        }

        //In case we are not currently interacting
        if (!isInteracting)
        {
            if(readyToInteract && Input.GetKeyDown(interactKey))
            {
                Interact();
            }
        }
        //In case we are currently interacting
        else
        {
            //In case we press a cancel button
            if (CancelButtonPressed())
            {
                EndInteract();
            }
            //In case the object we are interacting with is changed since previous frame
            else if (currentInteractSelection == null || currentInteractSelection != previousInteractSelection)
            {
                EndInteract();
            }
        }

        //In case current interactable object is not null -> matches current and last object
        //Used if current and last objects are different
        if (currentInteractSelection != null)
        {
            previousInteractSelection = currentInteractSelection;
        }


       //Handling UI position and rotation according to camera and interacted object
        if (interactionUI.isActiveAndEnabled)
        {
            //positions the UI a little bit above the interactable/interacted object
            interactionUI.gameObject.transform.position = hit.collider.transform.position
                                                               + new Vector3(0, 0.21f, 0);

            //Locks the x and z axis on the UI rotation, so it seems more smooth when the camera rotates
            Vector3 targetPostition = new Vector3(interactionUI.transform.rotation.x,
                                          mainCamera.transform.position.y,
                                         interactionUI.transform.rotation.z);

            //Changes rotation of InteractorUI according to the camera so it's always facing the player..
            interactionUI.gameObject.transform.LookAt(targetPostition);
        }

    }

    private void ReadyInteract()
    {
        readyToInteract = true;
        //Sets InteractorUI enabled
        interactionUI.enabled = true;
        interactionUI.gameObject.SetActive(true);

        currentInteractSelection.OnReadyInteract(); //When selected, calls the interface method 
    }

    private void Interact()
    {
        isInteracting = true;
        readyToInteract = true;

        currentInteractSelection.OnInteract(this); //Calls the interface interact method on current object
        Debug.Log("Interact");
    }

    private void CancelInteract()
    {
        readyToInteract = false;

        //Disables InteractorUI
        interactionUI.gameObject.SetActive(false);

        if (previousInteractSelection != null)
        {
            previousInteractSelection.OnCancelInteract(); //Calls the cancel interface method on the previously selected object
        }
        Debug.Log("Cancel");
    }

    private void EndInteract()
    {
        isInteracting = false;
        readyToInteract = false; //Set it to false so that we are allowed to search for a new object to interact

        previousInteractSelection.OnEndInteract();

        if (interactionUI != null)
        {

            interactionUI.HideTextMessage(); //to hide the ui text element
            interactionUI.gameObject.SetActive(false);
        }
    }

    private bool CancelButtonPressed()
    {
        for (int i = 0; i < interactCancelKeys.Length; i++)
        {
            //if button was pressed down this frame
            if (Input.GetKeyDown(interactCancelKeys[i]))
            {
                return true;
            }
        }
        return false;
    }


    //we call OnInteract method on our interactables and we expect to receive something
    //this can be a string or the object itself
    //a string can be returned from the text of a sign for example
    //example of oveload methods
    public void ReceiveInteract(string message)
    {
        //Debug.Log(message);
        //call interactorUI (a separete script that handles UI elements) and show text on UI
        interactionUI.ShowTextMessage(message);
    }

    //overload for more abstract unimplemented received call
    public void ReceiveInteract(IInteractable interactable)
    {
        Debug.Log(interactable);
    }




}

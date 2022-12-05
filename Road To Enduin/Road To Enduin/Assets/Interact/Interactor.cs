using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour 
{
//------------------------------------------------------------------------- DECLARATIONS SECTION-----------------------------------------------------------

    //Setup Section----------------------------------------------------------------------------------------------------------------------------------------
    [Header("Setup")]

    [Tooltip("Tag used to identify interactable objects")]
    public string targetTag;

    [Tooltip("How far will the player look for interactable objects")]
    public float maxTargetRange;

    [Tooltip("All layers the player will search for interactable objects")]
    public LayerMask layerMask;

    [Tooltip("The UI that will be used by interactable items")]
    public GameObject InteractionUI;

    [Tooltip("The main camera object")]
    public GameObject mainCamera;

    [Header("Input")]
    [Tooltip("All the keys that will be able to cancel an interaction")]
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

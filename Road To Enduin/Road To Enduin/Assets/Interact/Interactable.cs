using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    //Abstract methods used by interactable objects
    void OnInteract();


    void OnReadyInteract();


    void OnStartInteract();

    void OnEndInteract();
    
}

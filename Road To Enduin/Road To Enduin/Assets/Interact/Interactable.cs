using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    //Abstract methods used by interactable objects
    void OnInteract(Interactor interactor);

    void OnReadyInteract();

    void OnCancelInteract();

    void OnEndInteract();
    
}

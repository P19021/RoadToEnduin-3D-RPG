using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText; //Dialogue with each NPC Type is stored in this variable

    [SerializeField] Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        HideTextMessage(); //Reset and hide

        //Console warning if dialogue is not set for current NPC
        if (messageText == null) Debug.LogWarning("InteractorUI: messageText was not set.");
    }

    // Update is called once per frame
    void Update()
    {
        if (messageText != null && messageText.IsActive())
        {
            messageText.transform.LookAt(mainCamera.transform);
        }
    }

    //Shows dialogue with the NPC
    public void ShowTextMessage(string message)
    {
        if (messageText == null)
        { //In case dialogue is not set for the current NPC
            Debug.LogWarning("ShowTextMessage (method): messageText was not found.");
            return;
        }
        messageText.text = message; //Set dialogue as text for the UI Element
        messageText.gameObject.SetActive(true); //Show dialogue in text form
    }

    //Hides dialogue with the NPC
    public void HideTextMessage()
    {
        if (messageText == null)
        { //In case dialogue is not set for the current NP
            Debug.LogWarning("HideTextMessage (method): messageText was not found.");
            return;
        }
        messageText.text = ""; //Empty text in the UI Element after finished
        messageText.gameObject.SetActive(false); //Hide dialogue
    }
}

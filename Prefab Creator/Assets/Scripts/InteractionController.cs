using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour {
    
    public GameObject m_InteractMenuUI;

    // Use this Instance to access public interactive functions
    public static InteractionController Instance;

    // This variable can be checked at anytime by the game to see whether we're in Interactive Mode
    public static bool PlayerInteracting = false;

    private GameObject InteractiveObject;
    private Vector3 m_SavedPosition;
    private Quaternion m_SavedRotation;
    private Vector3 m_DestinationPos;
    private Quaternion m_DestinationRot;
    
    void Awake () {
        // Singleton method. Who knows if this is a good idea? :D
        Instance = this;
    }

    // Brings an object to the player to interact with; returns false if we're already interacting with something else
    public void InteractWithObject(GameObject thing) {
        if (!PlayerInteracting)
        {
            // Indicate to the game we're interacting with something
            PlayerInteracting = true;
            InteractiveObject = thing;

            // Save object position so we can put it back later
            m_SavedPosition = thing.transform.position;
            m_SavedRotation = thing.transform.rotation;

            // Set object destination (right in front of the player)
            m_DestinationPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 3.0f * InteractiveObject.transform.localScale.z));
            m_DestinationRot = Quaternion.LookRotation(-Camera.main.transform.forward); // face the camera

            // Turn on the Interactive Menu UI
            m_InteractMenuUI.SetActive(true);

            // Move the object to the new position
            StartCoroutine("MoveObjectCoRoutine");
        }
        else
        {
            Debug.Log("Object interaction refused: Can only interact with one object at a time.");
        }
    }

    // This coroutine moves an object towards a destination every frame
    private IEnumerator MoveObjectCoRoutine() {
        bool temp = PlayerInteracting;

        // Start interaction BEFORE moving the object to the user
        if (temp)
        {
            InteractiveObject.SendMessage("SetInteractMode", true);
        }

        while (m_DestinationPos != InteractiveObject.transform.position || m_DestinationRot != InteractiveObject.transform.rotation)
        {
            if(PlayerInteracting != temp)
            {
                yield break;
            }
            
            InteractiveObject.transform.position = Vector3.Lerp(InteractiveObject.transform.position, m_DestinationPos, Time.deltaTime * 10.0f);
            InteractiveObject.transform.rotation = Quaternion.Slerp(InteractiveObject.transform.rotation, m_DestinationRot, Time.deltaTime * 10.0f);
            yield return null;
        }
        
        // Stop interaction AFTER moving the object back
        if(!temp)
        {
            InteractiveObject.SendMessage("SetInteractMode", false);
        }
    }

    // Put the object back where it was and continue the game
    public void ExitInteraction() {

        // Indicate to the game that we're not interacting with anything
        PlayerInteracting = false;

        // Set object destination to where we got it from
        m_DestinationPos = m_SavedPosition;
        m_DestinationRot = m_SavedRotation;

        // Turn off the Interactive Menu UI
        m_InteractMenuUI.SetActive(false);

        StartCoroutine("MoveObjectCoRoutine");
    }
}

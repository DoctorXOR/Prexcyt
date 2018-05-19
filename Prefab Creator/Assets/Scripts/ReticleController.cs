using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleController : MonoBehaviour {

    public Texture2D reticleTexture;
	
	// Update is called once per frame
	void OnGUI () {
        // If we're interacting with something, allow the cursor to move freely around the screen
		if (InteractionController.PlayerInteracting) {
            Vector2 mousePos = Event.current.mousePosition;
            GUI.DrawTexture(new Rect(mousePos.x - (reticleTexture.width / 8), mousePos.y - (reticleTexture.height / 8), reticleTexture.width / 4, reticleTexture.height / 4), reticleTexture);
        } else { // Otherwise keep the cursor locked in the center of the camera
            GUI.DrawTexture(new Rect(Screen.width / 2 - (reticleTexture.width / 8), Screen.height / 2 - 17 - (reticleTexture.height / 8), reticleTexture.width / 4, reticleTexture.height / 4), reticleTexture);
        }
	}
}

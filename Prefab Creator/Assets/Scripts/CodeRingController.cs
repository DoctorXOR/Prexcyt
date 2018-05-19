using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeRingController : MonoBehaviour {
    
    private bool m_SpinMode = false;
	
	void Update () {
		
	}

    private void OnMouseEnter()
    {
        // Change cursor to "grab"
    }

    private void OnMouseExit()
    {
        // Change cursor to "reticle"
    }

    private void OnMouseDown()
    {
        // Spin ring
        m_SpinMode = true;
    }

    private void OnMouseUp()
    {
        // Lock ring in place
        m_SpinMode = false;
    }
}

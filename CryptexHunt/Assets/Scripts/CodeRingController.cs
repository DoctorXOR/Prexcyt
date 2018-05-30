using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class CodeRingController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
    
    private bool m_IsRotating = false;

    private float m_rotation = 0f;
    private float m_sensitivity = 0.6f;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        // Spin ring
        m_IsRotating = true;
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        if (m_IsRotating)
        {
            m_rotation += pointerEventData.delta.y * m_sensitivity;

            // Keep the rotation within 0 and 360 degrees
            m_rotation = m_rotation % 360f;
            if (m_rotation < 0)
                m_rotation += 360f;

            // We want a negative mouse movement (down the screen) to give a positive rotation (letters move down)
            transform.localRotation = Quaternion.Euler(-m_rotation, 0f, 0f);
        }
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        // Lock ring in place
        m_IsRotating = false;
        LockToClosestLetter();
    }

    private void LockToClosestLetter()
    {
        // First, normalize rotation between 0 and 25
        // Then round it and multiply back to a rotation
        m_rotation = (Mathf.Round(m_rotation * 26 / 360) % 26) * 360 / 26;

        // Don't forget we use the negative rotation
        transform.localRotation = Quaternion.Euler(-m_rotation, 0f, 0f);
    }

    public string GetLetter()
    {
        // Normalize the rotation between 0 and 25
        int rotationInt = Mathf.RoundToInt(m_rotation * 26 / 360);
        
        // Convert the integer to a string
        char letter = (char) (rotationInt + 65);
        return letter.ToString();
    }
}

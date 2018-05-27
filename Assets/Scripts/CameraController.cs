using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CameraController : MonoBehaviour {

    public Transform m_character;

    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool smooth;
    public float smoothTime = 5f;

    private Transform m_camera;
    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;

    // Use this for initialization
    void Start () {
        m_camera = GetComponent<Camera>().transform;
        m_camera.position = m_character.position + new Vector3(0f, 0.8f, 0f);

        m_CharacterTargetRot = m_character.rotation;
        m_CameraTargetRot = m_camera.rotation;
    }
	
	// Update is called once per frame
	private void Update () {
        LookRotation();
	}

    private void LookRotation()
    {
        if (!InteractionController.PlayerInteracting) // Only move the camera if we're not focused on an object
        {
            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smooth)
            {
                m_character.rotation = Quaternion.Slerp(m_character.rotation, m_CharacterTargetRot, smoothTime * Time.deltaTime);
                m_camera.rotation = Quaternion.Slerp(m_camera.rotation, m_CameraTargetRot, smoothTime * Time.deltaTime);
            }
            else
            {
                m_character.rotation = m_CharacterTargetRot;
                m_camera.rotation = m_CameraTargetRot;
            }
        }
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class CursorController : MonoBehaviour
{
    [HideInInspector] public enum MouseMode { Locked, Mobile }
    [HideInInspector] public static MouseMode mouseMode;

    public GameObject m_PauseMenu;
    public Sprite m_ReticleSprite;
    public Sprite m_GrabSprite;
    public float XSensitivity = 8f;
    public float YSensitivity = 8f;

    private bool m_Paused = false;
    private Image m_Image;
    private RectTransform m_rectTransform;

    private void Start()
    {
        m_Image = GetComponent<Image>();
        m_rectTransform = GetComponent<RectTransform>();

        mouseMode = MouseMode.Locked;
        SetPauseCursor(false);
    }

    private void Update()
    {
        // Bug: The Escape key pressed DOWN is doing this stuff. Need to make sure the Input settings aren't duplicating this code (Escape)
        // Always be able to exit the game. Later this should be dependent on the PauseMenu (which will check for the Escape key)
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SetPauseCursor(true);
        }
        // Go back into the game. Should also be moved to PauseMenu eventually
        else if (Input.GetMouseButtonUp(0) && Cursor.visible)
        {
            SetPauseCursor(false);
        }
        
        if (mouseMode == MouseMode.Locked)
        {
            m_rectTransform.anchoredPosition = new Vector2(Screen.width / 2, Screen.height / 2);
        }
        else if (mouseMode == MouseMode.Mobile)
        {
            // move with the mouse delta
            float xDelta = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
            float yDelta = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;
            
            m_rectTransform.anchoredPosition += new Vector2(xDelta, yDelta);
            // Vector2 targetPos = m_rectTransform.anchoredPosition + new Vector2(xDelta, yDelta);
            // m_rectTransform.anchoredPosition = Vector2.Lerp(m_rectTransform.anchoredPosition, targetPos, 10f * Time.deltaTime);

            // Keep cursor in bounds
            if (m_rectTransform.anchoredPosition.x > Screen.width)
                m_rectTransform.anchoredPosition = new Vector2(Screen.width, m_rectTransform.anchoredPosition.y);
            else if (m_rectTransform.anchoredPosition.x < 0)
                m_rectTransform.anchoredPosition = new Vector2(0, m_rectTransform.anchoredPosition.y);

            if (m_rectTransform.anchoredPosition.y > Screen.height)
                m_rectTransform.anchoredPosition = new Vector2(m_rectTransform.anchoredPosition.x, Screen.height);
            else if (m_rectTransform.anchoredPosition.y < 0)
                m_rectTransform.anchoredPosition = new Vector2(m_rectTransform.anchoredPosition.x, 0);
        }
    }

    public void SetReticle()
    {
        // Put any customizations for the sprite here
        m_Image.sprite = m_ReticleSprite;
        m_rectTransform.sizeDelta = new Vector2(10f, 10f);
    }

    public void SetGrab()
    {
        // Put any customizations for the sprite here
        m_Image.sprite = m_GrabSprite;
        m_rectTransform.sizeDelta = new Vector2(20f, 20f);
    }

    public void SetPauseCursor(bool value)
    {
        m_PauseMenu.SetActive(value);
        Cursor.visible = value;
        m_Image.enabled = !value;

        m_rectTransform.anchoredPosition = new Vector2(Screen.width / 2, Screen.height / 2); // Put the cursor in the center

        if (value)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;

        m_Paused = value;
    }
}

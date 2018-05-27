using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CryptexController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {

    [SerializeField] private string password = "QUEST";
    [SerializeField] private GameObject m_CodeRingPrefab;
    [SerializeField] private GameObject m_SpacerRingPrefab;
    [SerializeField] private GameObject m_EndCapPrefab;
    [SerializeField] private GameObject m_CryptexOutlinePrefab;

    // Temporary indication that you have unlocked the cryptex
    [SerializeField] private GameObject m_winText;

    private int m_NumCodeRings;
    private int m_NumSpacerRings;
    private GameObject[] m_CodeRingArr;
    private GameObject[] m_SpacerRingArr;
    private GameObject[] m_EndCapArr;
    private GameObject m_OutlineObject;
    private int m_OutlineQueue;

    // Add this variable to all interactive objects
    private bool m_InteractMode = false;

	void Start()
    {
        // Standardize text
        password = password.ToUpper();

        CreateCryptex();
    }

    private void Update()
    {
        // Check the password in interactive mode
        if (m_InteractMode)
        {
            string cryptexInput = "";
            foreach (GameObject CodeRing in m_CodeRingArr)
            {
                cryptexInput += CodeRing.GetComponent<CodeRingController>().GetLetter();
            }

            m_winText.SetActive(cryptexInput.Equals(password));
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (!m_InteractMode)
        {
            // If we're not interacting, indicate this object can be interacted with
            m_OutlineObject.gameObject.SetActive(true);
            SetOutlineRenderQueue(gameObject);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (!m_InteractMode)
        {
            // If we turn away without interacting, remove the indicator
            m_OutlineObject.gameObject.SetActive(false);
            SetNormalRenderQueue(gameObject);
        }
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (!m_InteractMode)
        {
            // Use this line of code in any game object to interact with it
            InteractionController.Instance.InteractWithObject(gameObject);
        }
    }

    // Add this function to all interactive objects
    private void SetInteractMode(bool value)
    {
        // Let this object know we're interacting
        m_InteractMode = value;

        // Turn on/off the outline
        m_OutlineObject.gameObject.SetActive(false); // We don't want the outline on while interacting, nor enabling if we turn away upon exit
        SetNormalRenderQueue(gameObject);
        
        // Swap parent interaction for children interaction
        if (value)
            gameObject.layer = 0;
        else
            gameObject.layer = 8;

        foreach (GameObject CodeRing in m_CodeRingArr)
        {
            if (value)
                CodeRing.layer = 8;
            else
                CodeRing.layer = 0;
        }
    }

    // Bumps this object and its children up in the render queue to be displayed in front of an outline
    private void SetOutlineRenderQueue(GameObject thing)
    {
        // Check this object
        if (!thing.Equals(m_OutlineObject)) // Don't change the outline object
        {
            if (thing.GetComponent<MeshRenderer>() != null)
            {
                Material[] materialsToSet = thing.GetComponent<MeshRenderer>().materials;

                foreach (Material m in materialsToSet)
                {
                    m.renderQueue = m_OutlineQueue + 1;
                }
            }
        }

        // Check this object's children recursively
        foreach (Transform child in thing.transform)
        {
            SetOutlineRenderQueue(child.gameObject);
        }
    }

    // Resets this object and its children
    private void SetNormalRenderQueue(GameObject thing)
    {
        // Check this object
        if (!thing.Equals(m_OutlineObject)) // Don't change the outline object
        {
            if (thing.GetComponent<MeshRenderer>() != null)
            {
                Material[] materialsToSet = thing.GetComponent<MeshRenderer>().materials;

                foreach (Material m in materialsToSet)
                {
                    m.renderQueue = 2000; // Geometry value
                }
            }
        }

        // Check this object's children recursively
        foreach (Transform child in thing.transform)
        {
            SetNormalRenderQueue(child.gameObject);
        }
    }

    // Instantiates the number of parts needed and where to place them
    private void CreateCryptex()
    {
        // Delete the placeholder (and the old cryptex, if you're using this script in Edit mode)
        if (transform.childCount > 0)
        {
            GameObject[] allChildren = new GameObject[transform.childCount];
            int n = 0;

            // Put all parts of the cryptex in an array
            foreach (Transform child in transform)
            {
                allChildren[n] = child.gameObject;
                n++;
            }

            // Destroy the previous cryptex
            foreach (GameObject child in allChildren)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        // Every code ring should be between two spacer rings
        m_NumCodeRings = password.Length;
        m_NumSpacerRings = m_NumCodeRings + 1;
        m_CodeRingArr = new GameObject[m_NumCodeRings];
        m_SpacerRingArr = new GameObject[m_NumSpacerRings];
        m_EndCapArr = new GameObject[2];

        // Calculate the length of the Cryptex
        float CodeRingWidth = 2 * m_CodeRingPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x; // Why x2? Extents measures from the center out
        float SpacerRingWidth = 2 * m_SpacerRingPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x;
        float EndCapWidth = 2 * m_EndCapPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x * 40 / 42; // Custom ratio to subtract end-stone offset
        float CryptexLength = (m_NumCodeRings * CodeRingWidth) + (m_NumSpacerRings * SpacerRingWidth) + (2 * EndCapWidth);

        // Fit the collider to the length
        GetComponent<CapsuleCollider>().height = CryptexLength;

        // Instantiate the Cryptex outline
        m_OutlineObject = Instantiate(m_CryptexOutlinePrefab, transform.position, transform.rotation * Quaternion.Euler(0, 0, 90), transform);
        m_OutlineObject.transform.localScale = new Vector3(1, CryptexLength / 2, 1); // Using a Unity primitive cylinder, height needs to be halved

        // Get the outline's renderQueue (should be Transparent + 1 = 3001, but just in case...)
        m_OutlineQueue = m_OutlineObject.GetComponent<MeshRenderer>().material.renderQueue;

        // Disable the outline; it will enable on mouse hover
        m_OutlineObject.gameObject.SetActive(false);

        // Instantiate all code rings, and one spacer ring next to each
        for (int i = 0; i < m_NumSpacerRings; i++)
        {

            float NextSpacerPos = (CryptexLength / 2) - EndCapWidth - i * (SpacerRingWidth + CodeRingWidth) - (SpacerRingWidth / 2);
            m_SpacerRingArr[i] = Instantiate(m_SpacerRingPrefab, transform.position + new Vector3(NextSpacerPos * transform.localScale.x, 0, 0), transform.rotation, transform);

            // Don't make a code ring after the last spacer
            if (i != m_NumCodeRings)
            {
                float NextCodePos = NextSpacerPos - (SpacerRingWidth + CodeRingWidth) / 2;
                m_CodeRingArr[i] = Instantiate(m_CodeRingPrefab, transform.position + new Vector3(NextCodePos * transform.localScale.x, 0, 0), transform.rotation, transform);
            }

        }

        // Instantiate end caps
        m_EndCapArr[0] = Instantiate(m_EndCapPrefab, transform.position + new Vector3(((CryptexLength / 2) - (EndCapWidth / 2)) * transform.localScale.x, 0, 0), transform.rotation, transform);
        m_EndCapArr[1] = Instantiate(m_EndCapPrefab, transform.position + new Vector3(((EndCapWidth / 2) - (CryptexLength / 2)) * transform.localScale.x, 0, 0), transform.rotation, transform);
    }
}

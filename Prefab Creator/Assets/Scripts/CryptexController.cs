using UnityEngine;

public class CryptexController : MonoBehaviour {

    [SerializeField] private int m_NumCodeRings = 5;
    [SerializeField] private Transform m_CodeRing;
    [SerializeField] private Transform m_SpacerRing;
    [SerializeField] private Transform m_EndCap;
    [SerializeField] private Transform m_CryptexOutline;

    private int m_NumSpacerRings;
    private Transform[] m_CodeRingArr;
    private Transform[] m_SpacerRingArr;
    private Transform[] m_EndCapArr;
    private Transform m_OutlineObject;
    private int m_MinCodeRings = 3;
    private int m_MaxCodeRings = 20;

    // Add this variable to all interactive objects
    private bool m_InteractMode;

	void Start()
    {
        CreateCryptex();
    }

    private void Update()
    {
        // Wait for us to exit interactive mode, if applicable
        if (m_InteractMode)
        {
            m_InteractMode = InteractionController.PlayerInteracting;
        }
    }

    private void OnMouseEnter()
    {
        if (!m_InteractMode)
        {
            // If we're not interacting, indicate this object can be interacted with
            m_OutlineObject.gameObject.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        if (!m_InteractMode)
        {
            // If we turn away without interacting, remove the indicator
            m_OutlineObject.gameObject.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if(!m_InteractMode)
        {
            // Use this line of code in any game object to interact with it
            InteractionController.Instance.InteractWithObject(gameObject);
            
            // Turn off the outline
            m_OutlineObject.gameObject.SetActive(false);
        }
    }

    // Add this function to all interactive objects
    private void SetInteractMode(bool value)
    { 
        m_InteractMode = value;
    }

    // Instantiates the number of parts needed and where to place them
    private void CreateCryptex()
    {
        m_InteractMode = false;
        
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

        // Limit the number of code rings
        if (m_NumCodeRings < m_MinCodeRings)
        {
            m_NumCodeRings = m_MinCodeRings;
        }
        else if (m_NumCodeRings > m_MaxCodeRings)
        {
            m_NumCodeRings = m_MaxCodeRings;
        }

        // Every code ring should be between two spacer rings
        m_NumSpacerRings = m_NumCodeRings + 1;
        m_CodeRingArr = new Transform[m_NumCodeRings];
        m_SpacerRingArr = new Transform[m_NumSpacerRings];
        m_EndCapArr = new Transform[2];

        // Calculate the length of the Cryptex
        float CodeRingWidth = 2 * m_CodeRing.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x; // Why x2? Extents measures from the center out
        float SpacerRingWidth = 2 * m_SpacerRing.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x;
        float EndCapWidth = 2 * m_EndCap.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x * 40 / 42; // Custom ratio to subtract end-stone offset
        float CryptexLength = (m_NumCodeRings * CodeRingWidth) + (m_NumSpacerRings * SpacerRingWidth) + (2 * EndCapWidth);

        // Fit the collider to the length
        (GetComponent("CapsuleCollider") as CapsuleCollider).height = CryptexLength;

        // Instantiate the Cryptex outline
        m_OutlineObject = Instantiate(m_CryptexOutline, transform.position, transform.rotation * Quaternion.Euler(0, 0, 90), transform) as Transform;
        m_OutlineObject.localScale = new Vector3(1, CryptexLength / 2, 1); // Using a Unity primitive cylinder, height needs to be halved

        // Disable the outline; it will enable on mouse hover
        m_OutlineObject.gameObject.SetActive(false);

        // Instantiate all code rings, and one spacer ring next to each
        for (int i = 0; i < m_NumSpacerRings; i++)
        {

            float NextSpacerPos = (CryptexLength / 2) - EndCapWidth - i * (SpacerRingWidth + CodeRingWidth) - (SpacerRingWidth / 2);
            m_SpacerRingArr[i] = Instantiate(m_SpacerRing, transform.position + new Vector3(NextSpacerPos * transform.localScale.x, 0, 0), transform.rotation, transform) as Transform;

            // Don't make a code ring after the last spacer
            if (i != m_NumCodeRings)
            {
                float NextCodePos = NextSpacerPos - (SpacerRingWidth + CodeRingWidth) / 2;
                m_CodeRingArr[i] = Instantiate(m_CodeRing, transform.position + new Vector3(NextCodePos * transform.localScale.x, 0, 0), transform.rotation, transform) as Transform;
            }

        }

        // Instantiate end caps
        m_EndCapArr[0] = Instantiate(m_EndCap, transform.position + new Vector3(((CryptexLength / 2) - (EndCapWidth / 2)) * transform.localScale.x, 0, 0), transform.rotation, transform) as Transform;
        m_EndCapArr[1] = Instantiate(m_EndCap, transform.position + new Vector3(((EndCapWidth / 2) - (CryptexLength / 2)) * transform.localScale.x, 0, 0), transform.rotation, transform) as Transform;
    }
}

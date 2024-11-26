using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField]
    private string perceptionUIName;

    [SerializeField]
    private Vector3 perceptionPositionOffset;

    [SerializeField]
    private string nextSceneName;

    private Canvas perceptionCanvas;

    private void Awake()
    {
        perceptionCanvas = UIHelpers.CreateBillBoardCanvas(perceptionUIName, transform, Camera.main);
        perceptionCanvas.transform.localPosition += perceptionPositionOffset;
    }

    private void Update()
    {
        perceptionCanvas.transform.rotation = Camera.main.transform.rotation;

        if (DetectPlayer())
        {
            perceptionCanvas.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.T))
            {
                GameManager.Instance.StateManager.SaveState();
                LoadSceneManager.Instance.LoadScene(nextSceneName);
            }
        }
        else
        {
            perceptionCanvas.gameObject.SetActive(false);
        }            
    }

    private bool DetectPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2.0f, 1 << 6);
        foreach (Collider collider in colliders)
        {
            if (collider.transform.CompareTag("Player"))
                return true;
        }
        return false;
    }

}
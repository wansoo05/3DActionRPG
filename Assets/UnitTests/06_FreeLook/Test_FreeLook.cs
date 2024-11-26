using UnityEngine;

public class Test_FreeLook : MonoBehaviour
{
    //[SerializeField]
    //private bool bLeft;
    //[SerializeField]
    //private bool bRight;
    //[SerializeField]
    //private bool bForward;
    //[SerializeField]
    //private bool bBackward;
    [SerializeField]
    private float speed = 2.5f;

    private CharacterController controller;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        Vector3 direction = Vector3.zero;
        Vector3 targetDirection = Vector3.zero;
        if(Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += -Vector3.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += -Vector3.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }
        if (direction.magnitude > 0.0f)
        {
            float rotationValue = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;

            transform.rotation = Quaternion.Euler(0.0f, rotationValue, 0.0f);
            targetDirection = rotationValue * transform.forward;
        }
        controller.Move(targetDirection.normalized * speed * Time.deltaTime);
    }

}
using UnityEngine;

public class Test_SignedAngle : MonoBehaviour
{
    [SerializeField]
    private GameObject[] boxes;

    private float angle;
    private void Update()
    {
        if (boxes.Length != 2)
            return;

        Vector3 direction = boxes[0].transform.position - boxes[1].transform.position;
        Vector3 direction2 = direction;
        direction2.y = 0.0f;
        Debug.DrawRay(boxes[1].transform.position, direction2, Color.blue);
        //Debug.DrawRay(boxes[1].transform.position, direction, Color.blue);
        Debug.DrawRay(boxes[1].transform.position, boxes[1].transform.forward * 10.0f, Color.blue);
        //angle = Vector3.SignedAngle(direction, direction2, Vector3.Cross(direction, direction2));
        angle = Vector3.Angle(boxes[1].transform.forward, direction2);
    }

    private void OnGUI()
    {
        GUI.color = Color.red;
        GUILayout.Label($"{angle}");
    }
}
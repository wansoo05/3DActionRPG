using Cinemachine;
using UnityEngine;

public class Test_CameraRevise : MonoBehaviour
{
    private CinemachineBrain brain;
    private Transform rightShoulderTransform;

    public Shader transparentShader;
    public Shader opaqueShader;

    private Material material;

    private void Awake()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
    }
    private void Start()
    {
        rightShoulderTransform = transform.FindChildByName("mixamorig:RightShoulder");
    }

    private void Update()
    {
        Vector3 direction = rightShoulderTransform.position - brain.transform.position;
        Debug.DrawRay(brain.transform.position, direction, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(brain.transform.position, direction.normalized, out hit))
        {
            if (hit.transform.name == "Player")
            {
                if (material == null)
                    return;

                material.shader = opaqueShader;

                return;
            }

            Renderer mesh = hit.transform.GetComponent<MeshRenderer>();

            if (mesh == null)
            {
                Transform t = hit.transform.FindChildByName("Surface");
                mesh = t.GetComponent<SkinnedMeshRenderer>();
            }

            Debug.Assert(mesh != null);

            material = mesh.material;
            material.shader = transparentShader;
            Color color = material.color;
            color.a = 0.1f;
            material.color = color;
        }
    }
}
using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class PlayerCameraController : MonoBehaviour
{
    /// <summary>
    /// 플레이어에 붙은 카메라 종류
    /// </summary>
    public enum CameraType
    {
        Basic = 0, Front,
    }

    [SerializeField]
    private float shortFov = 40.0f;

    [SerializeField]
    private float longFov = 60.0f;

    /// <summary>
    /// 투명화를 위한 shader
    /// </summary>
    [SerializeField]
    private Shader transparentShader;

    /// <summary>
    /// 투명화를 진행시킨 객체를 돌려놓기 위한 shader
    /// </summary>
    [SerializeField]
    private Shader opaqueShader;

    /// <summary>
    /// 플레이어의 오른쪽 어깨
    /// </summary>
    [SerializeField]
    private Transform rightShoulderTransform;

    /// <summary>
    /// 플레이어의 붙은 카메라들
    /// </summary>
    [SerializeField]
    private CinemachineVirtualCameraBase[] cameras;

    private CinemachineFreeLook currentCamera;
    private CinemachineBrain brain;
    private Dictionary<CameraType, CinemachineVirtualCameraBase> cameraDic = new Dictionary<CameraType, CinemachineVirtualCameraBase>();
    private RaycastHit[] hits = new RaycastHit[10];
    private Stack<Material> stack = new Stack<Material>();

    private WeaponComponent weapon;

    private void Awake()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
        weapon = GetComponent<WeaponComponent>();
        weapon.OnWeaponTypeChanged += OnWeaponTypeChanged;

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority = 0;
            cameraDic.Add((CameraType)i, cameras[i]);
        }
    }

    private void OnWeaponTypeChanged(WeaponType type1, WeaponType type2)
    {
        if (type2 == WeaponType.Unarmed)
            SetCameraShort();
        else
            SetCameraLong();
    }

    private void OnEnable()
    {
        cameraDic[CameraType.Basic].Priority = 1;
    }

    private void Update()
    {
        while (stack.Count > 0)
        {
            Material mat = stack.Pop();
            mat.shader = opaqueShader;
        }

        Vector3 direction = rightShoulderTransform.position - brain.transform.position;
        Debug.DrawRay(brain.transform.position, direction, Color.red);

        int hitCount = Physics.RaycastNonAlloc(brain.transform.position, direction.normalized, hits, direction.magnitude);
        for (int i = 0; i < hitCount; i++)
        {
            if (hits[i].transform == null) continue;
            if (hits[i].transform.name == "Player")
            {
                continue;
            }

            Renderer[] meshes = hits[i].transform.GetComponentsInChildren<Renderer>();

            if (meshes.Length == 0)
                continue;

            foreach (Renderer mesh in meshes)
            {
                if (mesh as TrailRenderer || mesh as ParticleSystemRenderer)
                    continue;

                Material[] materials = mesh.materials;
                foreach (Material mat in materials)
                {
                    stack.Push(mat);
                    mat.shader = transparentShader;
                    Color color = mat.color;
                    color.a = 0.2f;
                    mat.color = color;
                }
            }
        }

    }

    private void LateUpdate()
    {
        currentCamera = brain.ActiveVirtualCamera as CinemachineFreeLook;
    }

    public void ChangeCamera(CameraType type)
    {
        var sortedCameras = from cam in cameras
                            orderby cam.Priority descending
                            select cam;

        sortedCameras.First().Priority = 0;
        cameraDic[type].Priority = 1;
    }

    Coroutine convertRoutine = null;
    public void SetCameraShort()
    {
        if (currentCamera == null)
            return;

        if (convertRoutine != null)
            StopCoroutine(convertRoutine);
        convertRoutine = StartCoroutine(ConvertCamera(shortFov));
    }

    public void SetCameraLong()
    {
        if (currentCamera == null)
            return;

        if (convertRoutine != null)
            StopCoroutine(convertRoutine);

        convertRoutine = StartCoroutine(ConvertCamera(longFov));
    }

    public void CameraShaking(CinemachineImpulseSource impulse, NoiseSettings settings, Vector3 direction)
    {
        if (currentCamera != null)
        {
            CinemachineImpulseListener listener = currentCamera.GetComponent<CinemachineImpulseListener>();
            if (listener != null)
                listener.m_ReactionSettings.m_SecondaryNoise = settings;
        }

        impulse.GenerateImpulse(direction);
    }

    private IEnumerator ConvertCamera(float value)
    {
        while(currentCamera.m_Lens.FieldOfView != value)
        {
            yield return new WaitForFixedUpdate();

            currentCamera.m_Lens.FieldOfView = Mathf.Lerp(currentCamera.m_Lens.FieldOfView, value, Time.fixedDeltaTime * 3.0f);

            if (Mathf.Abs(currentCamera.m_Lens.FieldOfView - value) <= 1.0f)
            {
                currentCamera.m_Lens.FieldOfView = value;
            }
        }
        convertRoutine = null;
    }

}
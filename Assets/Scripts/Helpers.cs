using UnityEngine;

public static class Extend_TransformHelpers
{
    public static Transform FindChildByName(this Transform transform, string name)
    {
        Transform[] transforms = transform.GetComponentsInChildren<Transform>();

        foreach(Transform t in transforms)
        {
            if (t.gameObject.name.Equals(name))
                return t;
        }

        return null;
    }
}

public static class UIHelpers
{
    public static Canvas CreateBillBoardCanvas(string resourceName, Transform transform, Camera camera)
    {
        GameObject prefab = Resources.Load<GameObject>(resourceName);
        GameObject obj = GameObject.Instantiate<GameObject>(prefab, transform);

        Canvas canvas = obj.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        return canvas;
    }
}

public static class CameraHelpers
{
    public static bool GetCursorLocation(float distance, LayerMask mask)
    {
        Vector3 position;
        Vector3 normal;

        return GetCursorLocation(out position, out normal, distance, mask);
    }

    public static bool GetCursorLocation(out Vector3 position, float distance, LayerMask mask)
    {
        Vector3 normal;

        return GetCursorLocation(out position, out normal, distance, mask);
    }

    public static bool GetCursorLocation(out Vector3 position, out Vector3 normal, float distance, LayerMask layerMask)
    {
        position = Vector3.zero;
        normal = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance, layerMask))
        {
            position = hit.point;
            normal = hit.normal;
            return true;
        }
        return false;
    }
}

public static class DamageHelpers
{
    public static bool[,] damageTable = new bool[6, 4]
    {
        {true, true, true, false },
        {false, true, true, false },
        {false, false, true, true },
        {false,  true, true, false },
        {false, false, false, false },
        {true, true, true, true},
    };
}

public enum GroundType
{
    Grass = 0,
    Rock,
    Max,
}
public static class GroundHelpers
{
    public static bool IsSlope(Vector3 origin, out RaycastHit hit)
    {
        Ray ray = new Ray(origin + Vector3.up * 0.5f, Vector3.down);

        if (Physics.Raycast(ray, out hit, 1.0f, 1 << 7))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle != 0.0f && angle <= 45.0f;
        }
        return false;
    }

    public static Vector3 AdjustDirectionToSlope(Vector3 direction, Vector3 normal)
    {
        return Vector3.ProjectOnPlane(direction, normal).normalized;
    }

    public static string GetGroundName(Transform transform)
    {
        Ray ray = new Ray(transform.position, -transform.up);

        RaycastHit hit;
        Physics.Raycast(ray, out hit, 0.5f, 1 << 7);

        for(int i = 0; i < (int)GroundType.Max; i++)
        {
            if (hit.transform != null && hit.transform.tag == ((GroundType)i).ToString())
            {
                return hit.transform.tag;
            }
        }
        
        
        return null;
    }
}

#if UNITY_EDITOR
public static class DirectoryHelpers
{
    public static void ToRelativePath(ref string absolutePath)
    {
        int start = absolutePath.IndexOf("/Assets/");

        Debug.Assert(start > 0, "올바른 에셋 경로가 아닙니다.");

        absolutePath = absolutePath.Substring(start + 1, absolutePath.Length - start - 1);
    }
}
#endif
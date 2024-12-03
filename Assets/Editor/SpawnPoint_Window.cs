using UnityEditor;
using UnityEngine;

public class SpawnData_Window : EditorWindow
{
    private static SpawnData_Window window;
    private static Vector2 windowSize = new Vector2(319, 200);

    [MenuItem("Window/SpawnData/Data %&F3", priority = 1)]
    private static void OpenWindow()
    {
        if (window == null)
            window = EditorWindow.CreateInstance<SpawnData_Window>();

        window.titleContent = new GUIContent("Create Spawn Data");
        window.minSize = window.maxSize = windowSize;

        window.ShowUtility();
    }

    private SpawnData spawnData;
    private SerializedObject serializedObject;
    private Vector2 scrollPosition;

    private void OnEnable()
    {
        spawnData = CreateInstance<SpawnData>();
        serializedObject = new SerializedObject(spawnData);

        Selection.activeObject = null;
    }

    private void OnGUI()
    {
        //SpawnPrefab
        {
            SerializedProperty property = serializedObject.FindProperty("SpawnPrefab");
            EditorGUILayout.PropertyField(property);
        }

        //SpawnCount
        {
            SerializedProperty property = serializedObject.FindProperty("SpawnCount");
            EditorGUILayout.PropertyField(property);
        }

        //Save Button
        if (GUILayout.Button("Save SO File"))
        {
            string path = $"{Application.dataPath}/SpawnDatas/Enemy/";
            path = EditorUtility.SaveFilePanel("Save SO File", path, "SpawnData", "asset");

            if (path.Length > 0)
            {
                DirectoryHelpers.ToRelativePath(ref path);

                serializedObject.ApplyModifiedProperties();

                SpawnData obj = serializedObject.targetObject as SpawnData;

                bool bCheck = true;
                bCheck &= (obj.SpawnPrefab != null);
                bCheck &= (obj.SpawnCount > 0);
                if (bCheck)
                {
                    bCheck &= (obj.SpawnPrefab.GetComponent<Character>() != null);

                    AssetDatabase.CreateAsset(obj, path);   //에셋생성
                    AssetDatabase.SaveAssets();             //에셋저장
                    AssetDatabase.Refresh();                //새로고침

                    EditorUtility.FocusProjectWindow();     //포커스 이동

                    Selection.activeObject = obj;          //생성한 오브젝트로 커서 이동

                    EditorUtility.DisplayDialog("Create SO File", "생성이 완료되었습니다.", "확인");
                }

            }
        }
    }
}
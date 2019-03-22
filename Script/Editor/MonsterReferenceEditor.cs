
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(MonsterReference))]
public class MonsterReferenceEditor : Editor
{
    [SerializeField] string DataPath;
    List<UpgradeData> m_List;
    private MonsterReference m_target;
    //bool listVisibility = true;
    string assetPath = "Assets/Project/Prefabs/Enemies";

    enum displayFieldType { DisplayAsAutomaticFields, DisplayAsCustomizableGUIFields }
    displayFieldType DisplayFieldType = displayFieldType.DisplayAsAutomaticFields;
    SerializedObject GetTarget;
    SerializedProperty ThisList;
    int ListSize;

    void OnEnable()
    {
        m_target = (MonsterReference)target;

        GetTarget = new SerializedObject(m_target);
        ThisList = GetTarget.FindProperty("m_Data"); // Find the List in our script and create a refrence of it
    }

    public override void OnInspectorGUI()
    {
        SerializedProperty prop = serializedObject.FindProperty("m_Script");
        EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
        assetPath = EditorGUILayout.TextField("Asset path", assetPath);
        if (GUILayout.Button("Get Data"))
        {
            LoadUpgrade();
            //EditorUtility.SetDirty(m_target.gameObject);
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();
        }
        GetTarget.Update();
        EditorGUILayout.Space();
        ListSize = ThisList.arraySize;
        ListSize = EditorGUILayout.IntField("List Size", ListSize);
        if (ListSize != ThisList.arraySize)
        {
            while (ListSize > ThisList.arraySize)
            {
                ThisList.InsertArrayElementAtIndex(ThisList.arraySize);
            }
            while (ListSize < ThisList.arraySize)
            {
                ThisList.DeleteArrayElementAtIndex(ThisList.arraySize - 1);
            }
        }
        EditorGUILayout.Space();
        for (int i = 0; i < ThisList.arraySize; i++)
        {
            SerializedProperty MyListRef = ThisList.GetArrayElementAtIndex(i);
            if (DisplayFieldType == 0)
            {
                EditorGUILayout.PropertyField(MyListRef);
            }
            GUILayout.BeginHorizontal();
            GUILayout.Space(200f);
            if (GUILayout.Button("Remove (" + i.ToString() + ")"))
            {
                ThisList.DeleteArrayElementAtIndex(i);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
        if (GUILayout.Button(new GUIContent("Export monster data", "Export monsters' health to debug log to paste into a sheet")))
        {
            LogMonsterData();
        }
        GetTarget.ApplyModifiedProperties();
    }

    public void LoadUpgrade()
    {
        string[] guids1 = AssetDatabase.FindAssets("t:GameObject", new[] { assetPath });
        //Debug.Log(Application.dataPath + assetPath);
        //string[] aMaterialFiles = Directory.GetFiles(Application.dataPath + assetPath, "*.prefab", SearchOption.TopDirectoryOnly);
        List<GameObject> t_list = new List<GameObject>();
        foreach (string guid1 in guids1)
        {
            //Debug.Log(AssetDatabase.GUIDToAssetPath(guid1));
            string assetPath = AssetDatabase.GUIDToAssetPath(guid1);
            t_list.Add((GameObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)));
        }
        m_target.m_Data = t_list;
    }

    public string ExportHealthToText()
    {
        string export = "Monsters' health:\n";
        foreach (var item in m_target.m_Data)
        {
            export += string.Format("{0}\n", item.GetComponent<EnemyScript>().m_Data.health);
        }
        return export;
    }

    public void LogMonsterData()
    {
        string export = "Monsters' name:\n";
        foreach (var item in m_target.m_Data)
        {
            export += string.Format("{0}\n", item.GetComponent<EnemyScript>().m_Data.name);
        }
        Debug.Log(export);

        export = "Monsters' health:\n";
        foreach (var item in m_target.m_Data)
        {
            export += string.Format("{0}\n", item.GetComponent<EnemyScript>().m_Data.health);
        }
        Debug.Log(export);

        export = "Monsters' spawn delay:\n";
        foreach (var item in m_target.m_Data)
        {
            export += string.Format("{0}\n", WaveSpawner.GetSpawnDelay(item.GetComponent<EnemyScript>()));
        }
        Debug.Log(export);
    }
}

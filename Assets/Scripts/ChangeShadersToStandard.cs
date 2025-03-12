using UnityEngine;
using UnityEditor;

public class ConvertToStandard : MonoBehaviour
{
    [MenuItem("Tools/Convert URP Materials to Standard")]
    static void ConvertMaterials()
    {
        string[] materialGUIDs = AssetDatabase.FindAssets("t:Material");

        foreach (string guid in materialGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat.shader.name.Contains("Universal Render Pipeline")) // Check if it's a URP shader
            {
                mat.shader = Shader.Find("Standard"); // Convert to Standard shader
                Debug.Log($"Converted: {mat.name} to Standard Shader");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("All URP materials converted to Standard Shader!");
    }
}

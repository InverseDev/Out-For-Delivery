using UnityEngine;
public class WaterSurface : MonoBehaviour 
{
    public float primaryWaveHeight = 0.5f;
    public float primaryWaveFrequency = 1f;
    public float primaryWaveSpeed = 1f;

    public float secondaryWaveHeight = 0.3f;
    public float secondaryWaveFrequency = 2f;
    public float secondaryWaveSpeed = 1.5f;

    private Vector3[] baseVertices;
    private Mesh waterMesh;

    void Start()
    {
        waterMesh = GetComponent<MeshFilter>().mesh;
        baseVertices = waterMesh.vertices.Clone() as Vector3[];
    }

    void FixedUpdate()
    {
        Vector3[] vertices = waterMesh.vertices;
        
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = baseVertices[i];
            
            // Primary wave (slower, larger movements)
            float primaryWave = Mathf.Sin(Time.time * primaryWaveSpeed + 
                                   vertex.x * primaryWaveFrequency) * primaryWaveHeight;
            
            // Secondary wave (faster, smaller ripples)
            float secondaryWave = Mathf.Sin(Time.time * secondaryWaveSpeed + 
                                    vertex.z * secondaryWaveFrequency * 1.3f + 1.5f) * secondaryWaveHeight;
            
            // Combine waves for more complex movement
            vertex.y += primaryWave + secondaryWave;
            
            vertices[i] = vertex;
        }

        waterMesh.vertices = vertices;
        waterMesh.RecalculateNormals();
    }
}
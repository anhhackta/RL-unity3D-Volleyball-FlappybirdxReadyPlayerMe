using UnityEngine;
using TMPro;

public class ScoreboardSetup : MonoBehaviour
{
    [Header("Scoreboard Creation")]
    public bool createOnStart = true;
    public Vector3 scoreboardPosition = new Vector3(0, 3, 5);
    public Vector3 scoreboardScale = new Vector3(4, 0.1f, 2);
    public Material scoreboardMaterial;
    
    [Header("Text Settings")]
    public Font textFont;
    public float textSize = 0.5f;
    public Color textColor = Color.white;
    
    void Start()
    {
        if (createOnStart)
        {
            CreateScoreboard();
        }
    }
    
    public GameObject CreateScoreboard()
    {
        // Tạo cube cho scoreboard
        GameObject scoreboard = GameObject.CreatePrimitive(PrimitiveType.Cube);
        scoreboard.name = "Scoreboard";
        scoreboard.transform.position = scoreboardPosition;
        scoreboard.transform.localScale = scoreboardScale;
        
        // Gán material nếu có
        if (scoreboardMaterial != null)
        {
            Renderer renderer = scoreboard.GetComponent<Renderer>();
            renderer.material = scoreboardMaterial;
        }
        
        // Tạo GameObject cho TextMeshPro
        GameObject textObject = new GameObject("ScoreText");
        textObject.transform.SetParent(scoreboard.transform);
        textObject.transform.localPosition = new Vector3(0, 0.51f, 0); // Đặt trên mặt cube
        textObject.transform.localRotation = Quaternion.Euler(90, 0, 0); // Xoay để nhìn từ trên xuống
        textObject.transform.localScale = Vector3.one;
        
        // Thêm TextMeshPro component
        TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();
        textMesh.text = "Air Hockey\\n0 : 0";
        textMesh.fontSize = textSize;
        textMesh.color = textColor;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.sortingOrder = 1;
        
        // Thêm ScoreboardController
        ScoreboardController controller = scoreboard.AddComponent<ScoreboardController>();
        controller.scoreText = textMesh;
        
        Debug.Log("Scoreboard created at position: " + scoreboardPosition);
        return scoreboard;
    }
    
    [ContextMenu("Create Scoreboard")]
    public void CreateScoreboardFromEditor()
    {
        CreateScoreboard();
    }
}

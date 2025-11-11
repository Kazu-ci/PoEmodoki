using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class StatusControllerWindow : EditorWindow
{
    [MenuItem("Tools/Status Controller")]

    public static void ControllerWindow()
    {
        GetWindow<StatusControllerWindow>("Controller Window");
    }

    public void CreateGUI()
    {
        rootVisualElement.Add(new Label("ÉvÉåÉCÉÑÅ["));
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

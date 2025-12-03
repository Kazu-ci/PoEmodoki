using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string sceneName;
    private void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.F12))
        {
            FadeManager.Instance.LoadScene(sceneName,1f);
        }
    }
}

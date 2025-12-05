using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoRequired : MonoBehaviour
{
    private void Awake()
    {
        if (Demo.Instance == null)
        {
            Demo.StartingScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("Demo");
        }
    }
}
    
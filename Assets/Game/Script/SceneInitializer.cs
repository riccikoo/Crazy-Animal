using UnityEngine;

/// <summary>
/// Script ini di-attach ke scene untuk disable/cleanup player lama
/// sebelum GameManager spawn yang baru
/// </summary>
public class SceneInitializer : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("[SceneInitializer] Scene loading, cleaning up old player instance...");
        
        // Cari GameObject bernama "Player" yang merupakan PrefabInstance
        GameObject oldPlayer = GameObject.Find("Player");
        if (oldPlayer != null)
        {
            Debug.Log("[SceneInitializer] Found old player, disabling it");
            oldPlayer.SetActive(false);
        }
        
        // Juga cari any animal prefab instance yang tidak memiliki PlayerController
        GameObject[] allGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject go in allGameObjects)
        {
            if (go == gameObject) continue;
            
            // Skip UI
            if (go.GetComponent<RectTransform>() != null) continue;
            
            // Cari character instance yang tidak memiliki controller
            if (go.GetComponent<PlayerController>() == null && 
                go.GetComponent<Animator>() != null &&
                go.GetComponent<CharacterController>() == null &&
                go.GetComponent<RectTransform>() == null)
            {
                // Ini mungkin character placeholder
                if (go.name.Contains("animal") || go.name.Contains("Animal"))
                {
                    Debug.Log($"[SceneInitializer] Disabling character placeholder: {go.name}");
                    go.SetActive(false);
                }
            }
        }
        
        Debug.Log("[SceneInitializer] Cleanup complete, ready for GameManager");
    }
}

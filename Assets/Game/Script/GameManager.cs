using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manager untuk menghandle game flow dan character loading
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("Character Setup")]
    public GameObject[] characterPrefabs;  // Assign yang sama dengan CharacterSelectManager
    public Transform playerSpawnPoint;     // Posisi spawn player di SampleScene
    
    private GameObject currentPlayer;
    private int selectedCharacterIndex = 0;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Register scene load event
            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("[GameManager] Initialized with DontDestroyOnLoad");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Spawn player hanya saat load SampleScene
        if (scene.name == "SampleScene")
        {
            Debug.Log("[GameManager] SampleScene loaded, spawning player");
            
            selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
            Debug.Log($"[GameManager] Loading character index: {selectedCharacterIndex}");
            
            // Cleanup old player prefab instances di scene
            CleanupOldPlayers();
            
            // Spawn player dengan character yang dipilih
            SpawnPlayer();
        }
    }

    void CleanupOldPlayers()
    {
        // Cari dan hapus semua PrefabInstance dari character yang mungkin sudah ada
        // Biasanya dari MainMenu scene atau test setup
        GameObject[] allGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject go in allGameObjects)
        {
            // Skip GameManager sendiri
            if (go == gameObject) continue;
            
            // Cari GameObject yang punya tag "Player" atau nama "Player"
            if (go.CompareTag("Player") || go.name == "Player")
            {
                Debug.Log($"[GameManager] Destroying old player: {go.name}");
                Destroy(go);
                continue;
            }
            
            // Cari character instance yang tidak memiliki PlayerController (placeholder)
            if (go.GetComponent<PlayerController>() == null && 
                go.GetComponent<Animator>() != null &&
                go.GetComponent<CharacterController>() == null)
            {
                // Mungkin ini character model saja tanpa controller
                // Check apakah di-parent oleh Canvas atau UI
                if (go.GetComponent<RectTransform>() == null)
                {
                    // Kemungkinan character idle placeholder dari menu, hapus
                    if (go.name.Contains("animal-") || go.name.StartsWith("animal"))
                    {
                        Debug.Log($"[GameManager] Destroying old character model: {go.name}");
                        Destroy(go);
                    }
                }
            }
        }
    }

    void SpawnPlayer()
    {
        if (characterPrefabs == null || characterPrefabs.Length == 0)
        {
            Debug.LogError("[GameManager] characterPrefabs tidak ada atau kosong!");
            return;
        }

        if (selectedCharacterIndex < 0 || selectedCharacterIndex >= characterPrefabs.Length)
        {
            Debug.LogError($"[GameManager] selectedCharacterIndex {selectedCharacterIndex} out of range!");
            selectedCharacterIndex = 0;
        }

        // Tentukan posisi spawn
        Vector3 spawnPos = playerSpawnPoint != null 
            ? playerSpawnPoint.position 
            : new Vector3(0, 2, 0);

        Debug.Log($"[GameManager] Spawning character prefab {selectedCharacterIndex} at {spawnPos}");

        // Instantiate character
        currentPlayer = Instantiate(
            characterPrefabs[selectedCharacterIndex],
            spawnPos,
            Quaternion.identity
        );

        if (currentPlayer == null)
        {
            Debug.LogError("[GameManager] Gagal instantiate character!");
            return;
        }

        // Rename untuk lebih mudah di-track
        currentPlayer.name = "Player";

        // Setup tag untuk EnemyAI bisa menemukan player
        currentPlayer.tag = "Player";

        Debug.Log($"[GameManager] Player spawned: {currentPlayer.name} at {currentPlayer.transform.position}");

        // Ensure player punya component yang dibutuhkan
        EnsurePlayerComponents();
        
        // Setup camera follow jika perlu
        SetupCamera();
    }

    void EnsurePlayerComponents()
    {
        if (currentPlayer == null) return;

        // Pastikan ada CharacterController
        CharacterController cc = currentPlayer.GetComponent<CharacterController>();
        if (cc == null)
        {
            Debug.Log("[GameManager] Menambahkan CharacterController...");
            cc = currentPlayer.AddComponent<CharacterController>();
            cc.height = 1.38f;
            cc.radius = 0.6f;
            cc.center = new Vector3(0, 0.7f, 0);
        }

        // Pastikan ada Animator
        Animator anim = currentPlayer.GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogWarning("[GameManager] Animator tidak ditemukan!");
        }
        else
        {
            Debug.Log("[GameManager] Animator ditemukan");
        }

        // Pastikan ada PlayerController
        PlayerController pc = currentPlayer.GetComponent<PlayerController>();
        if (pc == null)
        {
            Debug.Log("[GameManager] Menambahkan PlayerController...");
            currentPlayer.AddComponent<PlayerController>();
        }

        // Pastikan ada PlayerStats
        PlayerStats ps = currentPlayer.GetComponent<PlayerStats>();
        if (ps == null)
        {
            Debug.Log("[GameManager] Menambahkan PlayerStats...");
            currentPlayer.AddComponent<PlayerStats>();
        }

        Debug.Log("[GameManager] Semua required components setup complete");
    }

    void SetupCamera()
    {
        if (currentPlayer == null) return;

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("[GameManager] Main Camera tidak ditemukan!");
            return;
        }

        // Add CameraFollow script jika belum ada
        CameraFollow cf = mainCamera.GetComponent<CameraFollow>();
        if (cf == null)
        {
            Debug.Log("[GameManager] Menambahkan CameraFollow ke MainCamera");
            cf = mainCamera.gameObject.AddComponent<CameraFollow>();
        }

        // Set target ke player
        cf.target = currentPlayer.transform;
        Debug.Log("[GameManager] Camera follow setup complete");
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public int GetSelectedCharacterIndex()
    {
        return selectedCharacterIndex;
    }

    public GameObject GetCurrentPlayer()
    {
        return currentPlayer;
    }
}

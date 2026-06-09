using UnityEngine;

/// <summary>
/// Helper script untuk auto-sync character prefabs dari CharacterSelectManager ke MainMenuManager
/// </summary>
public class GameSetupHelper : MonoBehaviour
{
    void Start()
    {
        Debug.Log("[GameSetupHelper] Starting auto-setup");
        
        // Cari CharacterSelectManager
        CharacterSelectManager csm = FindFirstObjectByType<CharacterSelectManager>();
        if (csm == null)
        {
            Debug.LogWarning("[GameSetupHelper] CharacterSelectManager tidak ditemukan!");
            return;
        }

        // Get characterPrefabs dari CharacterSelectManager
        GameObject[] characterPrefabs = csm.GetCharacterPrefabs();
        if (characterPrefabs == null || characterPrefabs.Length == 0)
        {
            Debug.LogWarning("[GameSetupHelper] Character prefabs tidak ada!");
            return;
        }

        // Cari MainMenuManager
        MainMenuManager mmm = FindFirstObjectByType<MainMenuManager>();
        if (mmm == null)
        {
            Debug.LogWarning("[GameSetupHelper] MainMenuManager tidak ditemukan!");
            return;
        }

        // Assign character prefabs ke MainMenuManager
        mmm.SetCharacterPrefabs(characterPrefabs);
        Debug.Log($"[GameSetupHelper] Auto-setup complete: {characterPrefabs.Length} prefabs assigned");
    }
}

using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{
    public Transform modelPivot;
    public GameObject[] characterPrefabs;
    public CharacterButton[] buttons;
    
    private GameObject currentCharacter;
    private int selectedIndex = 0;

    void Start()
    {
        SelectCharacter(0);
    }

    public void SelectCharacter(int index)
    {
        if (index < 0 || index >= characterPrefabs.Length)
        {
            Debug.LogError($"[CharacterSelectManager] Invalid index: {index}");
            return;
        }

        selectedIndex = index;

        if (currentCharacter != null)
            Destroy(currentCharacter);

        currentCharacter = Instantiate(
            characterPrefabs[index],
            modelPivot.position,
            modelPivot.rotation,
            modelPivot
        );

        currentCharacter.transform.localPosition = Vector3.zero;
        currentCharacter.transform.localRotation = Quaternion.identity;
        currentCharacter.transform.localScale = Vector3.one * 3f;

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetSelected(i == index);
        }

        // Save selected character untuk di-load di game
        PlayerPrefs.SetInt("SelectedCharacter", selectedIndex);
        PlayerPrefs.Save(); // Pastikan di-save ke disk
        
        Debug.Log($"[CharacterSelectManager] Character {index} selected and saved");

        currentCharacter.AddComponent<CharacterIdle>();
    }

    public GameObject[] GetCharacterPrefabs()
    {
        return characterPrefabs;
    }
}
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

        PlayerPrefs.SetInt("SelectedCharacter", selectedIndex);

        currentCharacter.AddComponent<CharacterIdle>();

    }
}
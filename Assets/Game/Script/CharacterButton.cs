using UnityEngine;

public class CharacterButton : MonoBehaviour
{
    public GameObject selectedOverlay;

    public void SetSelected(bool selected)
    {
        selectedOverlay.SetActive(selected);
        transform.localScale = selected ? Vector3.one * 1.08f : Vector3.one;
    }
}
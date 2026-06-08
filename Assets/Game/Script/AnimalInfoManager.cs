using UnityEngine;

public class AnimalInfoManager : MonoBehaviour
{
    public GameObject[] infoPanels;

    public void ShowPanel(int index)
    {
        for (int i = 0; i < infoPanels.Length; i++)
        {
            infoPanels[i].SetActive(i == index);
        }
    }
}
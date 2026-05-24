using UnityEngine;

public class MenuCameraSlider : MonoBehaviour
{
    public Transform camPointMainMenu;
    public Transform camPointSelectCharacter;
    public float speed = 3f;

    private Transform targetPoint;

    void Start()
    {
        targetPoint = camPointMainMenu;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPoint.position, Time.deltaTime * speed);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetPoint.rotation, Time.deltaTime * speed);
    }

    public void GoToSelectCharacter()
    {
        targetPoint = camPointSelectCharacter;
    }

    public void GoToMainMenu()
    {
        targetPoint = camPointMainMenu;
    }
}
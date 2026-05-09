using UnityEngine;

public class PulseButton : MonoBehaviour
{
    Vector3 startScale;
    public float speed = 1.5f;
    public float amount = 0.08f;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        float pulse = 1f + Mathf.Sin(Time.time * speed * 2f) * amount;
        transform.localScale = startScale * pulse;
    }
}
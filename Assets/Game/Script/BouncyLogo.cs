using UnityEngine;

public class BouncyLogo : MonoBehaviour
{
    Vector3 startPos;
    Vector3 startScale;

    void Start()
    {
        startPos = transform.localPosition;
        startScale = transform.localScale;
    }

    void Update()
    {
        float t = Time.time;
        
        // bounce naik turun
        transform.localPosition = startPos + 
            Vector3.up * Mathf.Sin(t * 3f) * 15f;
        
        // squeeze effect
        float squeeze = Mathf.Sin(t * 3f) * 0.05f;
        transform.localScale = startScale + 
            new Vector3(squeeze, -squeeze, 0);
    }
}
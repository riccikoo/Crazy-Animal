using UnityEngine;

public class CharacterIdle : MonoBehaviour
{
    public float speed = 3f;
    public float swayAngle = 6f;
    public float bounceAmount = 0.025f;
    public float scaleAmount = 0.025f;

    private Vector3 startLocalPos;
    private Quaternion startLocalRot;
    private Vector3 startLocalScale;

    void Start()
    {
        startLocalPos = transform.localPosition;
        startLocalRot = transform.localRotation;
        startLocalScale = transform.localScale;
    }

    void Update()
    {
        float s = Mathf.Sin(Time.time * speed);

        // gerak kecil, bukan terbang
        transform.localPosition = startLocalPos + new Vector3(
            0,
            Mathf.Abs(s) * bounceAmount,
            0
        );

        // badan goyang kanan-kiri
        transform.localRotation = startLocalRot * Quaternion.Euler(
            0,
            s * swayAngle,
            s * 2f
        );

        // sedikit squash/stretch biar gemas
        transform.localScale = startLocalScale + new Vector3(
            -Mathf.Abs(s) * scaleAmount,
            Mathf.Abs(s) * scaleAmount,
            -Mathf.Abs(s) * scaleAmount
        );
    }
}
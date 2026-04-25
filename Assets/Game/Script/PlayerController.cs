using UnityEngine;

public class PlayerController: MonoBehaviour
{
    public float moveSpeed = 5f;      // Kecepatan jalan
    public float rotationSpeed = 100f; // Kecepatan putar badan
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Ambil Input Keyboard
        // Vertical: W/S atau Arrow Up/Down
        // Horizontal: A/D atau Arrow Left/Right
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        // 2. Logika Berpindah (Transform)
        // Gerak maju/mundur
        transform.Translate(Vector3.forward * moveInput * moveSpeed * Time.deltaTime);

        // Rotasi (putar kiri/kanan)
        transform.Rotate(Vector3.up * turnInput * rotationSpeed * Time.deltaTime);

        // 3. Update Animasi
        // Jika moveInput tidak nol, berarti lagi jalan
        float speedVisual = Mathf.Abs(moveInput) + Mathf.Abs(turnInput);
        anim.SetFloat("Speed", speedVisual);

        // 4. Coba Animasi Lain (Contoh: Tekan E untuk Makan)
        if (Input.GetKeyDown(KeyCode.E))
        {
            anim.Play("eat");
        }
    }
}
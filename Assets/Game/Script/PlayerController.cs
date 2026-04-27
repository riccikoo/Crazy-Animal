using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float rotationSpeed = 100f;
    public float sprintEnergyCost = 20f; // Energi habis per detik saat lari

    private float currentSpeed;
    private Animator anim;
    private PlayerStats stats;

    void Start()
    {
        anim = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        // 1. Input Gerakan
        float moveInput = Input.GetAxis("Vertical"); // W/S atau Arrow Up/Down
        float turnInput = Input.GetAxis("Horizontal"); // A/D atau Arrow Left/Right

        // 2. Logika Sprint (Hanya lari jika maju dan ada energi)
        bool isSprinting = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) 
                            && moveInput > 0.1f 
                            && stats.energy > 0 
                            && !stats.isExhausted;

        if (isSprinting)
        {
            currentSpeed = sprintSpeed;
            stats.UseEnergy(sprintEnergyCost); 
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        // 3. Eksekusi Gerakan & Rotasi
        transform.Translate(Vector3.forward * moveInput * currentSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * turnInput * rotationSpeed * Time.deltaTime);

        // 4. Animasi Movement
        float speedVisual = Mathf.Abs(moveInput);
        if (isSprinting) speedVisual *= 2.0f; 
        anim.SetFloat("Speed", speedVisual);

        // 5. Input Attack Manual (Tombol J)
        if (Input.GetKeyDown(KeyCode.J))
        {
            stats.ManualAttack();
        }
    }
}
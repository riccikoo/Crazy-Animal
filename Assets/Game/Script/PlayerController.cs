using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))] // Otomatis nambahin komponen ini kalau belum ada
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float rotationSpeed = 100f;
    public float sprintEnergyCost = 20f;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float gravity = -20f; // Saya buat lebih berat agar tidak melayang (floaty)
    public AudioClip attackSFX;
    public AudioClip jumpSFX;          // Slot file suara pas melompat (Space)
    public AudioClip footstepSFX;      // Slot file suara langkah kaki (Walk/Run)
    private AudioSource sfxSource;     // Tempat pinjem speaker SFX Manager

    private CharacterController controller;
    private Animator anim;
    private PlayerStats stats;

    private Vector3 playerVelocity;
    private bool isGrounded;
    private float stepTimer = 0f;
    private float stepInterval = 0.5f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
        GameObject managerObj = GameObject.Find("SFX Manager");
            if (managerObj != null)
            {
                sfxSource = managerObj.GetComponent<AudioSource>();
            }
    }

    void Update()
    {
        // 1. Cek Grounded (Menggunakan fitur bawaan CharacterController)
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // Pastikan tetap menempel tanah
        }

        // 2. Input Gerakan
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        // 3. Logika Sprint
        bool isSprinting = (Input.GetKey(KeyCode.LeftShift))
                            && moveInput > 0.1f
                            && stats.energy > 0
                            && !stats.isExhausted;

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        if (isSprinting) stats.UseEnergy(sprintEnergyCost);

        // 4. Proses Rotasi (Kanan-Kiri)
        transform.Rotate(Vector3.up * turnInput * rotationSpeed * Time.deltaTime);

        // 5. Proses Jalan (Maju-Mundur)
        Vector3 move = transform.forward * moveInput;
        controller.Move(move * Time.deltaTime * currentSpeed);

        // 6. Logika Animasi
        float speedVisual = Mathf.Abs(moveInput);
        if (isSprinting) speedVisual *= 2.0f;
        anim.SetFloat("Speed", speedVisual);

        // 7. Logika Jump (Hanya jika di tanah)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Formula: v = sqrt(h * -2 * g)
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            StartCoroutine(JumpJuice(0.8f, 1.2f, 0.1f));
            if (sfxSource != null && jumpSFX != null && !sfxSource.mute)
            {
                sfxSource.PlayOneShot(jumpSFX);
            }
        }

        // 8. Terapkan Gravitasi (Selalu jalan setiap frame)
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // 9. Input Attack
        if (Input.GetKeyDown(KeyCode.J))
        {
            stats.ManualAttack();

            // Sound Effect untuk serangan
            if (sfxSource != null && attackSFX != null && !sfxSource.mute)
            {
                sfxSource.PlayOneShot(attackSFX);
            }
        }

        // Karakter harus di tanah, bergerak maju/mundur, dan speaker ada
        if (isGrounded && Mathf.Abs(moveInput) > 0.1f && sfxSource != null && footstepSFX != null && !sfxSource.mute)
        {
            // Jalankan timer gess
            stepTimer += Time.deltaTime;

            // Kalau lagi Lari (Shift), jeda suara dipercepat biar kedengaran buru-buru (0.28 detik)
            // Kalau Jalan biasa, jedanya santai (0.45 detik)
            float currentInterval = isSprinting ? 0.28f : 0.45f;

            if (stepTimer >= currentInterval)
            {
                // Mainkan suara langkah! Volume lari (0.6f) dibuat agak lebih keras dari jalan (0.35f)
                float currentVolume = isSprinting ? 0.6f : 0.35f;
                sfxSource.PlayOneShot(footstepSFX, currentVolume);

                stepTimer = 0f; // Reset timer gess
            }
        }
        else
        {
            stepTimer = 0f; // Reset timer kalau player berhenti diam
        }

    }

    public void PlayFootstepSound()
    {
        // Pastikan player lagi di tanah, speaker ada, dan audio klipnya gak kosong
        if (isGrounded && sfxSource != null && footstepSFX != null && !sfxSource.mute)
        {
            // Kita set volumenya agak kecil (0.4f) biar suara langkahnya gak berisik banget gess
            sfxSource.PlayOneShot(footstepSFX, 0.4f);
        }
    }

    IEnumerator JumpJuice(float stretchX, float stretchY, float duration)
    {
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = new Vector3(stretchX, stretchY, stretchX);

        float elapsed = 0;
        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }
}
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float rotationSpeed = 100f;
    public float consumptionRate = 20f; // Berapa energi yang habis per detik saat lari

    private float currentSpeed;
    private Animator anim;
    private PlayerStats stats; // Referensi ke script stats

    void Start()
    {
        anim = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        bool isSprinting = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) 
                            && moveInput > 0.1f 
                            && stats.energy > 0;

        if (isSprinting)
        {
            currentSpeed = sprintSpeed;
            stats.UseEnergy(consumptionRate); 
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        transform.Translate(Vector3.forward * moveInput * currentSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * turnInput * rotationSpeed * Time.deltaTime);

        float speedVisual = Mathf.Abs(moveInput);
        
        if (isSprinting)
        {
            speedVisual *= 2.0f; 
        }

        anim.SetFloat("Speed", speedVisual);
    }
}
using UnityEngine;

/// <summary>
/// Script untuk membuat camera follow player character
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    
    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 5, -8);
    public float smoothSpeed = 5f;
    public float rotationSpeed = 2f;
    
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // Jika tidak ada target di-assign, cari player
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("[CameraFollow] Found player, attaching camera");
            }
            else
            {
                Debug.LogWarning("[CameraFollow] Player tidak ditemukan!");
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly move camera to desired position
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            1f / smoothSpeed
        );

        // Rotate camera to look at target
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        if (directionToTarget != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }
}

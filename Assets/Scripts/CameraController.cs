using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float smoothTime = 1;
    [SerializeField] private float maxSpeed = 10;

    private Vector3 currentVelocity = Vector3.zero;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        var move = new Vector3(horizontal, vertical);
        var current = transform.position;

        transform.position = Vector3.SmoothDamp(current,
            current + (move * speedMultiplier), ref currentVelocity, smoothTime,
            maxSpeed, Time.fixedDeltaTime);
    }
}
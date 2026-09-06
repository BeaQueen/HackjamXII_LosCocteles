using UnityEngine;

public class LaserRotateOscillator : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float angle = 35f;
    [SerializeField] private float speed = 1.5f;

    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        float rotation =
            Mathf.Sin(Time.time * speed) * angle;

        transform.localRotation =
            initialRotation *
            Quaternion.Euler(rotation, 0f, 0f);
    }
}
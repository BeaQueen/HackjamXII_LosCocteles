using UnityEngine;

public class LaserVerticalOscillator : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float distance = 2f;
    [SerializeField] private float speed = 1.5f;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        float offset =
            Mathf.Sin(Time.time * speed) * distance;

        transform.localPosition =
            initialPosition +
            Vector3.up * offset;
    }
}
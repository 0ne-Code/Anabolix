using UnityEngine;

public class FloatingCube : MonoBehaviour
{
    [Header("Floating Settings")]
    [SerializeField] private float floatHeight = 1.5f;
    [SerializeField] private float floatSpeed = 1f;

    [Header("Rotation Settings")]
    [SerializeField] private Transform cubeVisual;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 30f, 0f);

    private Vector3 startingPosition;

    private void Start()
    {
        startingPosition = transform.position;
    }

    private void Update()
    {
        // Move the parent up and down smoothly
        float verticalOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = startingPosition + Vector3.up * verticalOffset;

        // Continuously rotate the cube itself
        if (cubeVisual != null)
        {
            cubeVisual.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 7f; // Adjust this to set the movement speed
    public float rotationSpeed = 500f; // Adjust this to set the rotation speed

    void Update()
    {
        // Translation (Movement)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);

        // Rotation
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        Vector3 rotation = new Vector3(-mouseY, mouseX, 0f) * rotationSpeed * Time.deltaTime;
        transform.Rotate(rotation);
    }
}

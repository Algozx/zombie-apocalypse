using UnityEngine;

public class Movement : MonoBehaviour
{
    public bool isRotating;
    public bool isMoving;
    
    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Call this function to move the character.
    /// </summary>
    public void Move(Vector3 direction, float speed)
    {
        if (direction != Vector3.zero)
        {
            isMoving = true;
            _rb.MovePosition(_rb.position + direction.normalized * speed * Time.deltaTime);
        }
        else
        {
            isMoving = false;
        }
    }

    /// <summary>
    /// Call this function to rotate the character.
    /// </summary>
    public void Rotate(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            isRotating = true;
            Quaternion newRotation = Quaternion.LookRotation(direction);
            _rb.MoveRotation(newRotation);
        }
        else
        {
            isRotating = false;
        }
    }
    
    /// <summary>
    /// Call this function to turn the character towards a direction without triggering the shooting.
    /// </summary>
    public void Face(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            _rb.MoveRotation(Quaternion.LookRotation(direction));
        }
    }

    /// <summary>
    /// Call this function to stop character from moving after being dead.
    /// </summary>v
    public void Die()
    {
        _rb.constraints = RigidbodyConstraints.None;
        _rb.linearVelocity = Vector3.zero;
        _rb.isKinematic = false;
        GetComponent<Collider>().enabled = false;
    }
}

using UnityEngine;

public class Passage : MonoBehaviour
{
    public Transform connection;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Vector3 position = other.transform.position;
        position.x = this.connection.position.x; //the z axis in a 2D game only affects draw order, therefore setting z would potentially
        position.y = this.connection.position.y; //screw things up, in addition to being unnecessary itself
        other.transform.position = position;

    }
}

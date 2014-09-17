using UnityEngine;
using System.Collections;

public class PlayerTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Rabbit"))
        {
            Debug.Log("Collided with rabbit");

            other.GetComponent<RabbitController>().IsIdle = false;
        }

        else if (other.tag.Equals("Door"))
        {
            Debug.Log("Collided with Door");

            other.transform.parent.GetComponent<Door>().Open();
        }
    }
}

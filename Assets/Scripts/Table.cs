using UnityEngine;
using System.Collections;

public class Table : MonoBehaviour
{
	public void Start()
	{
	    
	}
	
	public void Update()
	{
	
	}

    public void OnTriggerEnter(Collider collider)
    {
        if (!collider.tag.Equals("Player"))
            return;

        Debug.Log("Player Entering RabbitHole!");

        var playerPosition = collider.transform.position;

        transform.position = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
        transform.rotation = collider.transform.GetChild(0).rotation;

        transform.Translate(new Vector3(0, 0, 5.0f), Space.Self);
        transform.Rotate(Vector3.up, 180.0f);

        //GetComponentInChildren<Instruction>().enabled = true;
    }
}

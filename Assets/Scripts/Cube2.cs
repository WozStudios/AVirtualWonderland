using UnityEngine;
using System.Collections;

public class Cube2 : MonoBehaviour
{
    public float Speed = 1.0f;
    private Vector3 _position;
	
	public void Update()
	{
	    _position = transform.localPosition;
	    _position.z += Speed * Time.deltaTime;

        transform.Translate(new Vector3(0f, 0f, Speed * Time.deltaTime), Space.Self);
	}
}

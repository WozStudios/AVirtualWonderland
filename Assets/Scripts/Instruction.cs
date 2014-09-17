using UnityEngine;
using System.Collections;

public class Instruction : MonoBehaviour
{
    private GameObject _player;
    private Vector3 _rotation;

	public void Start()
	{
	    _player = GameObject.FindGameObjectWithTag("Player");
	}
	
	public void Update()
	{
        transform.LookAt(_player.transform, Vector3.up);
        _rotation = transform.localEulerAngles;
	    _rotation.y += 90;
	    _rotation.x = 110;
	    transform.rotation = Quaternion.Euler(_rotation);
	}
}

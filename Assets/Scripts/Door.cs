using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public Player.PlayerState DoorSize;

    public float RotateSpeed;
    public float RotationAmount;

    private bool _isClosed;
    private Player _player;

	public void Start()
	{
	    _isClosed = true;

	    _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}
	
	public void Update()
	{
	    if (Input.GetKeyDown(KeyCode.Keypad0))
            Open();
	}

    public void Open()
    {
        if (!_isClosed || _player.CurrentState != DoorSize)
            return;

        _isClosed = false;

        StartCoroutine("OpenDoor");
    }

    private IEnumerator OpenDoor()
    {
        var rotation = transform.localEulerAngles;

        while (rotation.y < RotationAmount)
        {
            var speed = rotation.y < RotationAmount * 05f ? rotation.y : RotationAmount - rotation.y;
            rotation.y += (speed * RotateSpeed + RotateSpeed) * Time.deltaTime;
            transform.rotation = Quaternion.Euler(rotation);

            yield return null;
        }

        GetComponentInChildren<MeshRenderer>().enabled = false;
    }
}

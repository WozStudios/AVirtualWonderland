using UnityEngine;
using System.Collections;

public class Cube : MonoBehaviour
{
    public float Distance;
    public float LoopTime;

    private float _elapsedTime;
    private Vector3 _position;

    private float _originalZ;
    private int _loopsCount;

    public void Start()
	{
	    _position = transform.position;
	    _originalZ = _position.z;

        _loopsCount = 0;
	}
	
	public void Update()
	{
	    _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= LoopTime)
	    {
            _elapsedTime -= LoopTime;
	        _loopsCount++;
	    }

	    var z = Distance / (1 + Mathf.Exp(-_elapsedTime)) + Distance * _loopsCount;

        //if (_loopsCount == 0)
        //    Debug.Log("Z: " + z);

	    _position.z = z + _originalZ;
	    transform.position = _position;

	}
}

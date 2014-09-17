using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour
{
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public Axis SpinAxis;
    public float Speed;

    private Vector3 _spinVector;
    private float _currentAngle;

	public void Start()
	{
	    switch (SpinAxis)
        {
            case Axis.X:
                _spinVector = new Vector3(1f, 0f, 0f);
                break;

            case Axis.Y:
                _spinVector = new Vector3(0f, 1f, 0f);
                break;

            case Axis.Z:
                _spinVector = new Vector3(0f, 0f, 1f);
                break;

	    }
	}
	
	public void Update()
	{
        _currentAngle = Speed * Time.deltaTime;

        if (_currentAngle > 360.0f)
            _currentAngle -= 360.0f;

        transform.Rotate(_spinVector, _currentAngle);
	}
}

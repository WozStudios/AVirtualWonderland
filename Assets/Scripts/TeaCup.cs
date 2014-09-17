using UnityEngine;
using System.Collections;

public class TeaCup : MonoBehaviour
{
    public bool IsAnimating = true;
    public float FallSpeed = 0f;
    public float MaxRotationSpeed = 80f;

    private float _rotationSpeed;

    public float KillY;
    private Vector3 _position;

    public void Start()
    {
        _rotationSpeed = Random.Range(MaxRotationSpeed / 4f, MaxRotationSpeed);
        _rotationSpeed *= Random.Range(0, 1) == 0 ? -1 : 1;

        var rotX = Random.Range(0, 360.0f);
        var rotY = Random.Range(0, 360.0f);
        var rotZ = Random.Range(0, 360.0f);

        transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);

        _position = transform.position;
    }

    public void Update()
    {
        if (IsAnimating)
        {
            //transform.Translate(0, -FallSpeed * Time.deltaTime, 0);

            _position.y -= FallSpeed * (Time.deltaTime);
            transform.position = _position;

            transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);
        }

        if (transform.position.y < KillY)
        {
            Destroy(gameObject);
        }
    }
}

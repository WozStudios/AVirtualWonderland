using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Rabbit))]
public class RabbitController : MonoBehaviour
{
    public bool IsIdle;

    public GameObject[] Waypoints;

    public float MoveSpeed;
    public float RotateSpeed;
    public float DeadZone;

    public float PauseLength = 1.0f;

    private Rabbit _rabbitComponent;
    public  GameObject LookAt;

    private int _currentWaypoint;

    public void Start()
    {
        _rabbitComponent = GetComponent<Rabbit>();
        _rabbitComponent.SetIdle(IsIdle);
    }

    public void Update()
    {
        _rabbitComponent.SetIdle(IsIdle);

        if (_rabbitComponent.IsIdle)
            return;

        var normalizedTime = _rabbitComponent.ClipNormalizedTime;

        var speed = normalizedTime > 0.5f ? (1.0f - _rabbitComponent.ClipNormalizedTime) : normalizedTime;

        //Debug.Log("speed: " + speed);

        transform.Translate(new Vector3(0, 0, speed * speed * MoveSpeed * Time.deltaTime), Space.Self);
    }

    public void OnTriggerEnter(Collider other)
    {
        //if (!other.tag.Equals("Player"))
        //{
        //    IsIdle = true;
        //    _rabbitComponent.SetIdle(IsIdle);
        //    return;
        //}

        if (!other.tag.Equals("Waypoint"))
            return;

        _currentWaypoint++;

        if (_currentWaypoint == Waypoints.Length - 1)
            StartCoroutine("Pause");
        else if (_currentWaypoint < Waypoints.Length)
            StartCoroutine("RotateTowardsWaypoint");

        //transform.LookAt(Waypoints[_currentWaypoint].transform, Vector3.up);
    }

    private IEnumerator RotateTowardsWaypoint()
    {
        Debug.Log("RotateTowardsWaypoint");
        var ideal = (Waypoints[_currentWaypoint].transform.position - transform.position).normalized;
        var current = (LookAt.transform.position - transform.position).normalized;

        var cross = Vector3.Cross(ideal, current);
        Debug.Log("Cross: " + cross);

        var rotateDirection = (cross.y > 0) ? -1 : 1;

        var rotation = transform.localRotation.eulerAngles;

        while ((ideal - current).magnitude > DeadZone)
        {
            rotation.y += rotateDirection * RotateSpeed * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(rotation);

            current = (LookAt.transform.position - transform.position).normalized;

            yield return null;
        }
    }

    private IEnumerator Pause()
    {
        Debug.Log("Pausing");

        IsIdle = true;
        _rabbitComponent.SetIdle(IsIdle);
        
        yield return new WaitForSeconds(PauseLength);
        
        IsIdle = false;
        _rabbitComponent.SetIdle(IsIdle);

        StartCoroutine("RotateTowardsWaypoint");
    }
}

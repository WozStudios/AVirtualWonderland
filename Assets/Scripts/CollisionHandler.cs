using UnityEngine;
using System.Collections;

public class CollisionHandler : MonoBehaviour
{
    public float FallSpeed;
    public float FallDelay;
    public GameObject RabbitHoleFloor;

    private Rabbit _rabbitComponent;

	public void Start()
	{
	    _rabbitComponent = GetComponent<Rabbit>();
	}
	
    public void OnTriggerEnter(Collider colliderObject)
    {
        if (!colliderObject.name.Equals("Transporter"))
            return;

        Debug.Log("Collided");

        StartCoroutine("EnterHole");
    }

    private IEnumerator EnterHole()
    {
        _rabbitComponent.SetIdle(true);

        yield return new WaitForSeconds(FallDelay);

        var position = transform.position;

        while (position.y >= RabbitHoleFloor.transform.position.y)
        {
            position.y -= FallSpeed * Time.deltaTime;
            transform.position = position;

            yield return null;
        }


        position.y = RabbitHoleFloor.transform.position.y;
        transform.position = position;

        transform.Translate(new Vector3(0, 0, 10.0f), Space.Self);
        transform.Rotate(Vector3.up, 180.0f);
    }
}

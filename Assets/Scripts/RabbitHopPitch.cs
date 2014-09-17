using System.Security.Permissions;
using UnityEngine;
using System.Collections;

public class RabbitHopPitch : MonoBehaviour
{
    public float MaxRotate;
    public float xOffset;
    public float yOffset;

    private Rabbit _rabbitComponent;

	public void Start()
	{
	    _rabbitComponent = transform.parent.GetComponent<Rabbit>();
	}
	
	public void Update()
	{
        if (_rabbitComponent.IsIdle)
	        return;

	    var normalizedTime = _rabbitComponent.ClipNormalizedTime;

        var x = (1 - Mathf.Cos(normalizedTime * 2 * Mathf.PI + xOffset * Mathf.PI));
	    var rotateAmount = MaxRotate * (x * x * x / 9f) - yOffset;

        transform.localRotation = Quaternion.Euler(rotateAmount, 0f, 0f);
	}
}

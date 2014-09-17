using System.Collections;
using UnityEngine;

public class PlayingCard : MonoBehaviour
{
    public Texture2D[] CardTextures;
    public bool IsAnimating = true;
    public float FallSpeed = 0f;
    public float MaxRotationSpeed = 80f;

    private float _rotationSpeed;

    public float KillY;

	public void Start()
	{
	    if (CardTextures.Length > 0)
	    {
	        var textureIndex = Random.Range(0, CardTextures.Length - 1);
	        renderer.material.SetTexture("_MainTex", CardTextures[textureIndex]);
	    }

	    _rotationSpeed = Random.Range(MaxRotationSpeed / 4f, MaxRotationSpeed);
	    _rotationSpeed *= Random.Range(0, 1) == 0 ? -1 : 1;

	}
	
	public void Update()
	{
	    if (IsAnimating)
	    {
	        transform.Translate(0, -FallSpeed * Time.deltaTime, 0);
            transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);
	    }

	    if (transform.position.y < KillY)
	    {
	        Destroy(gameObject);
	    }
	}
}

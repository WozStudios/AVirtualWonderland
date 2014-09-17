using UnityEngine;
using System.Collections;

public class MushroomColorChooser : MonoBehaviour
{
	public void Start()
    {
        var color = new Color(Random.Range(0.2f, 1), Random.Range(0.2f, 1), Random.Range(0.2f, 1));
        transform.renderer.material.SetColor("_ColorTint", color);

        color = new Color(Random.Range(0.6f, 1), Random.Range(0.6f, 1), Random.Range(0.6f, 1));
        transform.renderer.material.SetColor("_RimColor", color);
	}
	
	public void Update()
	{
	
	}
}

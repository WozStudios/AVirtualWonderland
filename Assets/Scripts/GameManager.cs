using TreeEditor;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject Player;

    public GameObject Park;
    public GameObject MushroomForest;
    public GameObject RabbitHole;

	public void Start()
	{
	    MushroomForest.SetActive(false);
	}
	
	public void Update()
	{
	    if (Input.GetButtonDown("Exit"))
            Application.Quit();

        if (Input.GetButtonDown("Restart"))
            Application.LoadLevel("Park");

        if (Input.GetKeyDown(KeyCode.UpArrow))
            Application.LoadLevel("MushroomDemo");
	}

    public void DisablePark()
    {
        Park.SetActive(false);
    }

    public void EnablePark()
    {
        Park.SetActive(true);

        MushroomForest.SetActive(false);
    }

    public void EnableMushroomForest()
    {
        MushroomForest.SetActive(true);

        RabbitHole.SetActive(false);
    }

}

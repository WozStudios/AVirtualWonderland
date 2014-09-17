using UnityEngine;
using System.Collections;

public class PlayingCardGenerator : MonoBehaviour
{
    public GameObject PlayingCardPrototype;
    public int NumberOfCards;
    public float xzOffsetRange;
    public float yOffsetRange;

    public bool IsSpawning;
    public float TimeBetweenSpawns;



	public void Start()
	{
        StartCoroutine("CreateCards");
	}
	
	public void Update()
	{
	
	}


    public IEnumerator CreateCards()
    {
        while (IsSpawning)
        {
            for (var i = 0; i < NumberOfCards; i++)
                CreateCard();

            yield return new WaitForSeconds(TimeBetweenSpawns);
        }
    }

    public void CreateCard()
    {
        var position = transform.position + new Vector3(Random.Range(-xzOffsetRange, xzOffsetRange), Random.Range(-yOffsetRange, yOffsetRange), Random.Range(-xzOffsetRange, xzOffsetRange));
        var card = Instantiate(PlayingCardPrototype, position, new Quaternion()) as GameObject;
        card.transform.parent = transform;
    }
}

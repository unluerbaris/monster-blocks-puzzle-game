using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject[] animals = null;

    // Start is called before the first frame update
    void Start()
    {
        SpawnObject();
    }

    public void SpawnObject()
    {
        int randomIndex = Random.Range(0, animals.Length);
        Vector2 randomPosition = new Vector2(Random.Range(transform.position.x - 10, transform.position.x + 10), transform.position.y);
        Debug.Log(randomIndex);
        Debug.Log(randomPosition.x);
        GameObject animalInstance = Instantiate(animals[randomIndex], randomPosition, Quaternion.identity);
    }
}

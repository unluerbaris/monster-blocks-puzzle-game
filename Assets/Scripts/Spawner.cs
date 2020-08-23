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
        int randomXPos = Random.Range((int)transform.position.x - 3, (int)transform.position.x + 3);
        Vector2 randomPosition = new Vector2(randomXPos, transform.position.y);
        Debug.Log($"Random index number: {randomIndex}");
        Debug.Log($"Random start position: {randomPosition.x}");
        GameObject animalInstance = Instantiate(animals[randomIndex], randomPosition, Quaternion.identity);
    }
}

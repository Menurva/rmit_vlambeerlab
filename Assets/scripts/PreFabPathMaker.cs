using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreFabPathmaker : MonoBehaviour
{
    // STEP 2: Declare class member variables
    private int counter = 0; // Counter to track how manyS floor tiles have been instantiated
    public Transform floorPrefab; // Assign the floor tile prefab in the Inspector
    public Transform pathmakerSpherePrefab; // Assign the pathmaker sphere prefab in the Inspector

    // Static variable to keep track of the total number of tiles created
    public static int globalTileCount = 0;
    public static int maxTileCount = 500; // Maximum allowed number of tiles in the world

    void Update()
    {
        // If the counter is less than 50 and globalTileCount is below maxTileCount
        if (counter < 50 && globalTileCount < maxTileCount)
        {
            float randomNumber = Random.Range(0.0f, 1.0f); // Generate a random number between 0.0 and 1.0
            Debug.Log("Generated random number: " + randomNumber);

            // Rotate the object based on the generated random number
            if (randomNumber < 0.25f)
            {
                transform.Rotate(0, 90, 0); // Rotate 90 degrees
                Debug.Log("Rotated 90 degrees");
            }
            else if (randomNumber < 0.5f)
            {
                transform.Rotate(0, -90, 0); // Rotate -90 degrees
                Debug.Log("Rotated -90 degrees");
            }
            else if (randomNumber > 0.99f)
            {
                Instantiate(pathmakerSpherePrefab, transform.position, Quaternion.identity); // Instantiate a new pathmaker sphere
                Debug.Log("Instantiated a new pathmaker sphere");
            }

            // Instantiate a floor tile at the current position
            Instantiate(floorPrefab, transform.position, Quaternion.identity);
            Debug.Log("Instantiated a floor tile at position: " + transform.position);
            globalTileCount++; // Increment global tile count
            Debug.Log("Global tile count: " + globalTileCount);

            // Move forward by 5 units
            transform.Translate(0, 0, 5);
            Debug.Log("Moved forward by 5 units");

            // Increment the counter
            counter++;
            Debug.Log("Counter incremented to: " + counter);
        }
        else
        {
            // Destroy this game object if enough tiles have been made
            Debug.Log("Maximum tiles reached or counter limit reached. Destroying game object.");
            Destroy(gameObject);
        }
    }
}

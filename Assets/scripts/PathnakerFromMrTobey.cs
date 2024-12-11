using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathnakerFromMrTobey : MonoBehaviour
{
    private int counter = 0; // Tracks the number of tiles instantiated
    public Transform floorPrefab; // Assign this in the Inspector
    public Transform pathmakerSpherePrefab; // Assign this in the Inspector
    public Transform mainCamera;

    private Vector3 cameraVelocity = Vector3.zero;
    static List<Transform> allTiles = new List<Transform>(); // List to track all tiles

    // Static variable to track the total number of tiles in the world
    public static int globalTileCount = 0;
    public static int maxTiles = 500; // Maximum number of tiles allowed in the scene

    void Update()
    {
        // Check if the total tile count is within the limit
        if (globalTileCount >= maxTiles)
        {
            Destroy(gameObject);
            return;
        }

        // If counter is less than 50
        if (counter < 50)
        {
            float randomNumber = Random.Range(0f, 1f);

            // Rotate based on random number
            if (randomNumber < 0.25f)
            {
                transform.Rotate(0f, 90f, 0f);
            }
            else if (randomNumber < 0.5f)
            {
                transform.Rotate(0f, -90f, 0f);
            }
            else if (randomNumber < 0.99f)
            {
                Instantiate(pathmakerSpherePrefab, transform.position, Quaternion.identity);
            }

            // Instantiate a floor tile and move forward
            Transform newTile = Instantiate(floorPrefab, transform.position, Quaternion.Euler(90, 0, 0));
            allTiles.Add(newTile); // Add the new tile to the list

            transform.Translate(0f, 0f, 5f);
            counter++;
            globalTileCount++;
        }
        else
        {
            // Destroy the Pathmaker object if it has reached its tile limit
            Destroy(gameObject);
        }
    }
}
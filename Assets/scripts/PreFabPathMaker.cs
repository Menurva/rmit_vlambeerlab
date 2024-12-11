using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is version 2.0//
public class PreFabPathmaker : MonoBehaviour
{
    // STEP 2: Declare class member variables
    private int counter = 0; // Counter to track how many floor tiles have been instantiated
    public Transform floorPrefab; // Assign the floor tile prefab in the Inspector
    public Transform pathmakerSpherePrefab; // Assign the pathmaker sphere prefab in the Inspector
    public KeyCode startGenerationKey = KeyCode.Space; // Assign key to start generation in the Inspector
    public KeyCode stopAndResetKey = KeyCode.R; // Assign key to stop and reset generation in the Inspector
    public Camera mainCamera; // Assign the main camera in the Inspector

    // Static variable to keep track of the total number of tiles created
    public static int globalTileCount = 0;
    public static int maxTileCount = 550; // Maximum allowed number of tiles in the world

    private bool isGenerating = false; // Track whether generation is active
    private List<GameObject> generatedTiles = new List<GameObject>(); // Track all generated tiles

    void Update()
    {
        // Start generation if the assigned key is pressed
        if (Input.GetKeyDown(startGenerationKey))
        {
            isGenerating = !isGenerating;
            Debug.Log(isGenerating ? "Started generating." : "Paused generating.");
        }

        // Stop and reset if the assigned key is pressed
        if (Input.GetKeyDown(stopAndResetKey))
        {
            ResetGeneration();
        }

        // If generation is active, generate tiles
        if (isGenerating && counter < 50 && globalTileCount < maxTileCount)
        {
            GeneratePath();
        }
    }

    void GeneratePath()
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

        // Ensure the position stays within the camera's field of view (dead zone) and on the ZX plane
        Vector3 position = transform.position;
        position.y = 0; // Lock Y position to 0 to ensure the sphere stays on the ZX plane

        // Calculate the distance from the camera
        float distanceFromCamera = Vector3.Distance(new Vector3(position.x, 0, position.z), new Vector3(mainCamera.transform.position.x, 0, mainCamera.transform.position.z));

        // Ensure the position is within the limits (20m to 100m from the camera)
        if (distanceFromCamera > 100f)
        {
            Vector3 directionToCamera = (mainCamera.transform.position - position).normalized;
            position = mainCamera.transform.position + directionToCamera * 100f;
            position.y = 0; // Ensure Y stays locked to 0
            Debug.Log("Adjusted position to maintain maximum distance from camera (100m).");
        }
        else if (distanceFromCamera < 20f)
        {
            Vector3 directionAwayFromCamera = (position - mainCamera.transform.position).normalized;
            position = mainCamera.transform.position + directionAwayFromCamera * 20f;
            position.y = 0; // Ensure Y stays locked to 0
            Debug.Log("Adjusted position to maintain minimum distance from camera (20m).");
        }

        // Ensure the sphere stays in front of the camera
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 toSphere = position - mainCamera.transform.position;
        if (Vector3.Dot(cameraForward, toSphere) < 0)
        {
            position = mainCamera.transform.position + cameraForward * 20f;
            position.y = 0; // Ensure Y stays locked to 0
            Debug.Log("Adjusted position to stay within the camera's field of view.");
        }

        transform.position = position;

        // Instantiate a floor tile at the current position
        GameObject newTile = Instantiate(floorPrefab, transform.position, Quaternion.identity).gameObject;
        generatedTiles.Add(newTile);
        Debug.Log("Instantiated a floor tile at position: " + transform.position);
        globalTileCount++; // Increment global tile count
        Debug.Log("Global tile count: " + globalTileCount);

        // Move forward by 5 units on the ZX plane only
        transform.Translate(0, 0, 5);
        transform.position = new Vector3(transform.position.x, 0, transform.position.z); // Lock Y position
        Debug.Log("Moved forward by 5 units on the ZX plane");

        // Increment the counter
        counter++;
        Debug.Log("Counter incremented to: " + counter);
    }

    void ResetGeneration()
    {
        // Stop generating
        isGenerating = false;
        Debug.Log("Stopped generating and resetting tiles.");

        // Destroy all generated tiles
        foreach (GameObject tile in generatedTiles)
        {
            if (tile != null)
            {
                Destroy(tile);
            }
        }
        generatedTiles.Clear();
        globalTileCount = 0;
        counter = 0;
    }
}






/*//This is version 3.0//
public class PreFabPathmaker : MonoBehaviour
{
    // STEP 2: Declare class member variables
    private int counter = 0; // Counter to track how many floor tiles have been instantiated
    public Transform floorPrefab; // Assign the floor tile prefab in the Inspector
    public Transform pathmakerSpherePrefab; // Assign the pathmaker sphere prefab in the Inspector
    public KeyCode startGenerationKey = KeyCode.Space; // Assign key to start generation in the Inspector
    public KeyCode stopAndResetKey = KeyCode.R; // Assign key to stop and reset generation in the Inspector
    public Camera mainCamera; // Assign the main camera in the Inspector

    // Static variable to keep track of the total number of tiles created
    public static int globalTileCount = 0;
    public static int maxTileCount = 550; // Maximum allowed number of tiles in the world

    private bool isGenerating = false; // Track whether generation is active
    private List<GameObject> generatedTiles = new List<GameObject>(); // Track all generated tiles

    void Update()
    {
        // Start generation if the assigned key is pressed
        if (Input.GetKeyDown(startGenerationKey))
        {
            isGenerating = !isGenerating;
            Debug.Log(isGenerating ? "Started generating." : "Paused generating.");
        }

        // Stop and reset if the assigned key is pressed
        if (Input.GetKeyDown(stopAndResetKey))
        {
            ResetGeneration();
        }

        // If generation is active, generate tiles
        if (isGenerating && counter < 50 && globalTileCount < maxTileCount)
        {
            GeneratePath();
        }
    }

    void GeneratePath()
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

        // Ensure the position stays within the camera's field of view (dead zone) and on the ZX plane
        Vector3 position = transform.position;
        position.y = 0; // Lock Y position to 0 to ensure the sphere stays on the ZX plane

        // Calculate the distance from the camera
        float distanceFromCamera = Vector3.Distance(new Vector3(position.x, 0, position.z), new Vector3(mainCamera.transform.position.x, 0, mainCamera.transform.position.z));

        // Ensure the position is within the limits (20m to 100m from the camera)
        if (distanceFromCamera > 50f)
        {
            Vector3 directionToCamera = (mainCamera.transform.position - position).normalized;
            position = mainCamera.transform.position + directionToCamera * 50f;
            position.y = 0; // Ensure Y stays locked to 0
            Debug.Log("Adjusted position to maintain maximum distance from camera (100m).");
        }
        else if (distanceFromCamera < 20f)
        {
            Vector3 directionAwayFromCamera = (position - mainCamera.transform.position).normalized;
            position = mainCamera.transform.position + directionAwayFromCamera * 20f;
            position.y = 0; // Ensure Y stays locked to 0
            Debug.Log("Adjusted position to maintain minimum distance from camera (20m).");
        }

        transform.position = position;

        // Instantiate a floor tile at the current position
        GameObject newTile = Instantiate(floorPrefab, transform.position, Quaternion.identity).gameObject;
        generatedTiles.Add(newTile);
        Debug.Log("Instantiated a floor tile at position: " + transform.position);
        globalTileCount++; // Increment global tile count
        Debug.Log("Global tile count: " + globalTileCount);

        // Move forward by 5 units on the ZX plane only
        transform.Translate(0, 0, 5);
        transform.position = new Vector3(transform.position.x, 0, transform.position.z); // Lock Y position
        Debug.Log("Moved forward by 5 units on the ZX plane");

        // Increment the counter
        counter++;
        Debug.Log("Counter incremented to: " + counter);
    }

    void ResetGeneration()
    {
        // Stop generating
        isGenerating = false;
        Debug.Log("Stopped generating and resetting tiles.");

        // Destroy all generated tiles
        foreach (GameObject tile in generatedTiles)
        {
            if (tile != null)
            {
                Destroy(tile);
            }
        }
        generatedTiles.Clear();
        globalTileCount = 0;
        counter = 0;
    }
}*/


/*//This is version 2.0//
public class PreFabPathmaker : MonoBehaviour
{
    // STEP 2: Declare class member variables
    private int counter = 0; // Counter to track how many floor tiles have been instantiated
    public Transform floorPrefab; // Assign the floor tile prefab in the Inspector
    public Transform pathmakerSpherePrefab; // Assign the pathmaker sphere prefab in the Inspector
    public KeyCode startGenerationKey = KeyCode.Space; // Assign key to start generation in the Inspector
    public KeyCode stopAndResetKey = KeyCode.R; // Assign key to stop and reset generation in the Inspector
    public Camera mainCamera; // Assign the main camera in the Inspector

    // Static variable to keep track of the total number of tiles created
    public static int globalTileCount = 0;
    public static int maxTileCount = 550; // Maximum allowed number of tiles in the world

    private bool isGenerating = false; // Track whether generation is active
    private List<GameObject> generatedTiles = new List<GameObject>(); // Track all generated tiles

    void Update()
    {
        // Start generation if the assigned key is pressed
        if (Input.GetKeyDown(startGenerationKey))
        {
            isGenerating = !isGenerating;
            Debug.Log(isGenerating ? "Started generating." : "Paused generating.");
        }

        // Stop and reset if the assigned key is pressed
        if (Input.GetKeyDown(stopAndResetKey))
        {
            ResetGeneration();
        }

        // If generation is active, generate tiles
        if (isGenerating && counter < 50 && globalTileCount < maxTileCount)
        {
            GeneratePath();
        }
    }

    void GeneratePath()
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

        // Ensure the position stays within the camera's field of view (dead zone)
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
        viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.05f, 0.95f);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.05f, 0.95f);
        transform.position = mainCamera.ViewportToWorldPoint(viewportPosition);

        // Instantiate a floor tile at the current position
        GameObject newTile = Instantiate(floorPrefab, transform.position, Quaternion.identity).gameObject;
        generatedTiles.Add(newTile);
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

    void ResetGeneration()
    {
        // Stop generating
        isGenerating = false;
        Debug.Log("Stopped generating and resetting tiles.");

        // Destroy all generated tiles
        foreach (GameObject tile in generatedTiles)
        {
            if (tile != null)
            {
                Destroy(tile);
            }
        }
        generatedTiles.Clear();
        globalTileCount = 0;
        counter = 0;
    }
}*/

/* //this is version 1.0 - it worked just fine//
public class PreFabPathmaker : MonoBehaviour
{
    // STEP 2: Declare class member variables
    private int counter = 0; // Counter to track how manyS floor tiles have been instantiated
    public Transform floorPrefab; // Assign the floor tile prefab in the Inspector
    public Transform pathmakerSpherePrefab; // Assign the pathmaker sphere prefab in the Inspector

    // Static variable to keep track of the total number of tiles created
    public static int globalTileCount = 0;
    public static int maxTileCount = 550; // Maximum allowed number of tiles in the world

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
*/
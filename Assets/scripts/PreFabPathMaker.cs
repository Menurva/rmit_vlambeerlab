using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Version 4.3 - Fixed for land-boundary constraint and added clone count control

public class PreFabPathmaker : MonoBehaviour
{
    private int counter = 0; // Counter to track how many objects have been instantiated
    public List<Transform> prefabsToSpawn; // List of prefabs to spawn (e.g., trees, rocks)
    public Transform pathmakerSpherePrefab; // The prefab for the pathmaker sphere
    public KeyCode startGenerationKey = KeyCode.Space; // Key to start generation
    public KeyCode stopAndResetKey = KeyCode.R; // Key to stop and reset generation
    public GameObject landObject; // The object serving as the land boundary
    public LayerMask landLayer; // The layer assigned to the land object

    public static int globalTileCount = 0;
    public int maxClonesToGenerate = 100; // Maximum number of clones allowed (configurable in Inspector)
    public float minSpacing = 0.03f; // Minimum spacing between objects on the XZ plane

    private bool isGenerating = false; // Whether generation is active
    private List<GameObject> generatedObjects = new List<GameObject>(); // List of generated objects
    private List<Vector3> occupiedPositions = new List<Vector3>(); // List of positions already used
    private Bounds landBounds; // The bounds of the land's collider

    void Start()
    {
        // Get the bounds of the land's collider
        Collider landCollider = landObject.GetComponent<Collider>();
        if (landCollider != null)
        {
            landBounds = landCollider.bounds;
            Debug.Log($"Land bounds set to: {landBounds}");
        }
        else
        {
            Debug.LogError("Land object does not have a Collider. Cannot set bounds.");
        }
    }

    void Update()
    {
        // Start or stop generation
        if (Input.GetKeyDown(startGenerationKey))
        {
            isGenerating = !isGenerating;
            Debug.Log(isGenerating ? "Started generating." : "Paused generating.");
        }

        // Stop and reset generation
        if (Input.GetKeyDown(stopAndResetKey))
        {
            ResetGeneration();
        }

        // Generate if active
        if (isGenerating && counter < maxClonesToGenerate && globalTileCount < maxClonesToGenerate)
        {
            GeneratePath();
        }
    }

    void GeneratePath()
    {
        float randomNumber = Random.Range(0.0f, 1.0f); // Generate a random number for logic
        Debug.Log($"Random Number: {randomNumber}");

        // Random rotation logic
        if (randomNumber < 0.25f)
        {
            transform.Rotate(0, 90, 0); // Rotate 90 degrees
            Debug.Log("Rotated 90 degrees.");
        }
        else if (randomNumber < 0.5f)
        {
            transform.Rotate(0, -90, 0); // Rotate -90 degrees
            Debug.Log("Rotated -90 degrees.");
        }

        // Check and snap back if outside land bounds
        if (!landBounds.Contains(transform.position))
        {
            Debug.LogWarning("Pathmaker is outside land bounds. Snapping back...");
            SnapToLandSurface();
            return;
        }

        // Attempt to find a valid spawn position
        Vector3 spawnPosition = FindValidPositionWithinLand();

        if (spawnPosition == Vector3.zero)
        {
            Debug.LogWarning("Failed to find a valid spawn position. Skipping this generation step.");
            return;
        }

        // Pick a random prefab to spawn
        if (prefabsToSpawn.Count > 0)
        {
            Transform prefabToSpawn = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Count)];
            Debug.Log($"Spawning {prefabToSpawn.name} at {spawnPosition}");

            GameObject newObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity).gameObject;
            generatedObjects.Add(newObject);
            occupiedPositions.Add(new Vector3(spawnPosition.x, 0, spawnPosition.z)); // Track the position on the XZ plane

            globalTileCount++;
        }
        else
        {
            Debug.LogError("No prefabs assigned in the list!");
        }

        // Move forward by a random distance
        transform.Translate(0, 0, Random.Range(5.0f, 10.0f));

        // Lock Y position
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        // Increment the counter
        counter++;
    }

    /// <summary>
    /// Finds a valid spawn position within the land's bounds and at least `minSpacing` away from other objects.
    /// </summary>
    private Vector3 FindValidPositionWithinLand()
    {
        for (int i = 0; i < 10; i++) // Try up to 10 times to find a valid position
        {
            Vector3 proposedPosition = transform.position + new Vector3(
                Random.Range(-2.0f, 2.0f),
                0,
                Random.Range(-2.0f, 2.0f)
            );

            // Ensure the position is within the land's bounds
            if (!landBounds.Contains(proposedPosition))
            {
                continue;
            }

            // Check against existing positions
            bool isValid = true;
            foreach (Vector3 pos in occupiedPositions)
            {
                if (Vector3.Distance(new Vector3(proposedPosition.x, 0, proposedPosition.z), pos) < minSpacing)
                {
                    isValid = false;
                    break;
                }
            }

            if (isValid)
            {
                return proposedPosition;
            }
        }

        return Vector3.zero; // Return zero if no valid position is found
    }

    /// <summary>
    /// Snap the Pathmaker object back to a valid position on the land surface.
    /// </summary>
    private void SnapToLandSurface()
    {
        if (landBounds.Contains(transform.position))
        {
            return; // Already within bounds, no need to snap
        }

        transform.position = landBounds.ClosestPoint(transform.position);
        Debug.Log($"Snapped Pathmaker back to: {transform.position}");
    }

    void ResetGeneration()
    {
        // Stop generating
        isGenerating = false;

        // Destroy all generated objects
        foreach (GameObject obj in generatedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        generatedObjects.Clear();
        occupiedPositions.Clear();
        globalTileCount = 0;
        counter = 0;

        Debug.Log("Reset complete. All generated objects destroyed.");
    }
}


// Version 4.2 - Enhanced for spacing and collision prevention
/*
public class PreFabPathmaker : MonoBehaviour
{
    private int counter = 0; // Counter to track how many objects have been instantiated
    public List<Transform> prefabsToSpawn; // List of prefabs to spawn (e.g., trees, rocks)
    public Transform pathmakerSpherePrefab; // The prefab for the pathmaker sphere
    public KeyCode startGenerationKey = KeyCode.Space; // Key to start generation
    public KeyCode stopAndResetKey = KeyCode.R; // Key to stop and reset generation
    public GameObject landObject; // The object serving as the land boundary
    public LayerMask landLayer; // The layer assigned to the land object

    public static int globalTileCount = 0;
    public static int maxTileCount = 500; // Maximum allowed number of objects in the world
    public float minSpacing = 0.03f; // Minimum spacing between objects on the XZ plane

    private bool isGenerating = false; // Whether generation is active
    private List<GameObject> generatedObjects = new List<GameObject>(); // List of generated objects
    private List<Vector3> occupiedPositions = new List<Vector3>(); // List of positions already used

    void Update()
    {
        // Start or stop generation
        if (Input.GetKeyDown(startGenerationKey))
        {
            isGenerating = !isGenerating;
            Debug.Log(isGenerating ? "Started generating." : "Paused generating.");
        }

        // Stop and reset generation
        if (Input.GetKeyDown(stopAndResetKey))
        {
            ResetGeneration();
        }

        // Generate if active
        if (isGenerating && counter < 50 && globalTileCount < maxTileCount)
        {
            GeneratePath();
        }
    }

    void GeneratePath()
    {
        float randomNumber = Random.Range(0.0f, 1.0f); // Generate a random number for logic
        Debug.Log($"Random Number: {randomNumber}");

        // Random rotation logic
        if (randomNumber < 0.25f)
        {
            transform.Rotate(0, 90, 0); // Rotate 90 degrees
            Debug.Log("Rotated 90 degrees.");
        }
        else if (randomNumber < 0.5f)
        {
            transform.Rotate(0, -90, 0); // Rotate -90 degrees
            Debug.Log("Rotated -90 degrees.");
        }

        // Adjust position to stay on land
        if (!AdjustPivotToLand())
        {
            Debug.LogWarning("Pathmaker is off the land. Snapping back to land surface...");
            SnapToLandSurface();
            return; // Exit this generation step
        }

        // Attempt to find a valid spawn position
        Vector3 spawnPosition = FindValidPosition();

        if (spawnPosition == Vector3.zero)
        {
            Debug.LogWarning("Failed to find a valid spawn position. Skipping this generation step.");
            return;
        }

        // Pick a random prefab to spawn
        if (prefabsToSpawn.Count > 0)
        {
            Transform prefabToSpawn = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Count)];
            Debug.Log($"Spawning {prefabToSpawn.name} at {spawnPosition}");

            GameObject newObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity).gameObject;
            generatedObjects.Add(newObject);
            occupiedPositions.Add(new Vector3(spawnPosition.x, 0, spawnPosition.z)); // Track the position on the XZ plane

            globalTileCount++;
        }
        else
        {
            Debug.LogError("No prefabs assigned in the list!");
        }

        // Move forward by a random distance
        transform.Translate(0, 0, Random.Range(5.0f, 10.0f));

        // Lock Y position
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        // Increment the counter
        counter++;
    }

    /// <summary>
    /// Finds a valid spawn position that is at least `minSpacing` away from other objects on the XZ plane.
    /// </summary>
    private Vector3 FindValidPosition()
    {
        Vector3 proposedPosition = transform.position + new Vector3(
            Random.Range(-2.0f, 2.0f),
            0,
            Random.Range(-2.0f, 2.0f)
        );

        // Check against existing positions
        foreach (Vector3 pos in occupiedPositions)
        {
            if (Vector3.Distance(new Vector3(proposedPosition.x, 0, proposedPosition.z), pos) < minSpacing)
            {
                return Vector3.zero; // Invalid position
            }
        }

        return proposedPosition; // Valid position
    }

    /// <summary>
    /// Adjust the pivot point of the Pathmaker object to stay on the land surface.
    /// </summary>
    /// <returns>True if successfully adjusted to the land, false otherwise.</returns>
    private bool AdjustPivotToLand()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 10f; // Cast ray from above the object
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, landLayer))
        {
            // Check if the hit object is the landObject or belongs to its layer
            if (hit.collider.gameObject == landObject || hit.collider.gameObject.layer == landLayer.value)
            {
                transform.position = new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z); // Slight offset above surface
                return true;
            }
        }
        return false; // Return false if not on the land surface
    }

    /// <summary>
    /// Snap the Pathmaker object back to a valid position on the land surface.
    /// </summary>
    private void SnapToLandSurface()
    {
        Collider landCollider = landObject.GetComponent<Collider>();
        if (landCollider != null)
        {
            Vector3 closestPoint = landCollider.ClosestPoint(transform.position);
            transform.position = new Vector3(closestPoint.x, closestPoint.y + 0.1f, closestPoint.z);
            Debug.Log("Snapped back to land surface at: " + transform.position);
        }
        else
        {
            Debug.LogError("Land object does not have a Collider. Unable to snap back.");
        }
    }

    void ResetGeneration()
    {
        // Stop generating
        isGenerating = false;

        // Destroy all generated objects
        foreach (GameObject obj in generatedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        generatedObjects.Clear();
        occupiedPositions.Clear();
        globalTileCount = 0;
        counter = 0;

        Debug.Log("Reset complete. All generated objects destroyed.");
    }
}
*/

//this is version 4.0//
/*
public class PreFabPathmaker : MonoBehaviour
{
    private int counter = 0; // Counter to track how many floor tiles have been instantiated
    public Transform floorPrefab; // Assign the floor tile prefab in the Inspector
    public Transform pathmakerSpherePrefab; // Assign the pathmaker sphere prefab in the Inspector
    public KeyCode startGenerationKey = KeyCode.Space; // Assign key to start generation in the Inspector
    public KeyCode stopAndResetKey = KeyCode.R; // Assign key to stop and reset generation in the Inspector
    public Camera mainCamera; // Assign the main camera in the Inspector
    public GameObject landObject; // The object functioning as land for tile generation
    public LayerMask landLayer; // The layer assigned to the land object

    public static int globalTileCount = 0;
    public static int maxTileCount = 500; // Maximum allowed number of tiles in the world

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

        // Rotate the object based on the generated random number
        if (randomNumber < 0.25f)
        {
            transform.Rotate(0, 90, 0); // Rotate 90 degrees
        }
        else if (randomNumber < 0.5f)
        {
            transform.Rotate(0, -90, 0); // Rotate -90 degrees
        }
        //
        else if (randomNumber > 0.99f)
        {
            Instantiate(pathmakerSpherePrefab, transform.position, Quaternion.identity); // Instantiate a new pathmaker sphere
        }
        //
        // Check and adjust position to ensure it's on the land surface
        if (AdjustPivotToLand())
        {
            // Instantiate a floor tile at the current position
            GameObject newTile = Instantiate(floorPrefab, transform.position, Quaternion.identity).gameObject;
            generatedTiles.Add(newTile);

            globalTileCount++; // Increment global tile count

            // Move forward by 5 units
            transform.Translate(0, 0, 10); //Test//
            transform.position = new Vector3(transform.position.x, 0, transform.position.z); // Lock Y position

            // Increment the counter
            counter++;
        }
        else
        {
            // If not on land, snap back to the nearest valid position on the land
            SnapToLandSurface();
        }
    }

    /// <summary>
    /// Adjust the pivot point of the Pathmaker object to stay on the land surface.
    /// </summary>
    /// <returns>True if successfully adjusted to the land, false otherwise.</returns>
    private bool AdjustPivotToLand()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 10f; // Cast ray from above the object
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, landLayer))
        {
            // Check if the hit object is the landObject or belongs to its layer
            if (hit.collider.gameObject == landObject || hit.collider.gameObject.layer == landLayer.value)
            {
                // Snap the position to the hit point
                transform.position = new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z); // Slight offset above surface
                return true;
            }
        }

        return false; // Return false if not on the land surface
    }

    /// <summary>
    /// Snap the Pathmaker object back to a valid position on the land surface.
    /// </summary>
    private void SnapToLandSurface()
    {
        Collider landCollider = landObject.GetComponent<Collider>();
        if (landCollider != null)
        {
            // Find the closest point on the land object
            Vector3 closestPoint = landCollider.ClosestPoint(transform.position);

            // Snap the Pathmaker to this point, slightly above the surface
            transform.position = new Vector3(closestPoint.x, closestPoint.y + 0.1f, closestPoint.z);

            Debug.Log("Snapped back to the land surface at: " + transform.position);
        }
        else
        {
            Debug.LogError("Land object does not have a Collider. Unable to snap back to the land surface.");
        }
    }

    void ResetGeneration()
    {
        // Stop generating
        isGenerating = false;

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

        Debug.Log("Reset complete. All tiles removed.");
    }
}
*/

//This is version 2.0//
/*
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





/*
//This is version 3.0//
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
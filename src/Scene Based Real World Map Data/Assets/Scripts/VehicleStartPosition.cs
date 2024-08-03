using UnityEngine;
using System.Collections;

public class VehicleStartPosition : MonoBehaviour
{
    public GameObject vehicle; // Reference to the vehicle GameObject
    private MapReader mapReader;
    private Rigidbody vehicleRigidbody;
    public Vector3 mapOrigin; // Add a public field for the map origin
    public float scale = 0.0001f; // Scale factor to fit the converted coordinates within Unity's coordinate system

    void Start()
    {
        // Reference the MapReader component
        mapReader = FindObjectOfType<MapReader>();

        // Get the Rigidbody component from the vehicle
        vehicleRigidbody = vehicle.GetComponent<Rigidbody>();

        if (vehicleRigidbody == null)
        {
            Debug.LogError("Rigidbody component not found on the vehicle.");
            return;
        }

        // Start the coroutine to set the vehicle's start position
        StartCoroutine(SetVehicleStartPosition());
    }

    IEnumerator SetVehicleStartPosition()
    {
        Debug.Log("Starting SetVehicleStartPosition coroutine");

        // Wait until the MapReader is ready
        while (mapReader == null || !mapReader.IsReady || mapReader.ways.Count < 1)
        {
            Debug.Log("Waiting for MapReader to be ready...");
            yield return null;
        }

        Debug.Log("MapReader is ready");

        // Find the first road
        OsmWay road = mapReader.ways.Find(w => w.IsRoad);

        if (road != null)
        {
            Debug.Log($"Found road: {road.Name} with {road.NodeIDs.Count} nodes");

            // Place the vehicle at the midpoint of the first road segment
            Vector3 roadPosition = GetMidpointOfFirstSegment(road);

            // Log the calculated position
            Debug.Log($"Calculated Road Position: {roadPosition}");

            // Adjust the vehicle position to be on the road
            SetVehiclePosition(roadPosition);

            // Print the vehicle's new position
            Debug.Log($"Vehicle placed on road: {road.Name} at position {vehicle.transform.position}");
        }
        else
        {
            Debug.LogWarning("No roads found in MapReader.");
        }
    }

    Vector3 GetMidpointOfFirstSegment(OsmWay road)
    {
        for (int i = 1; i < road.NodeIDs.Count; i++)
        {
            if (mapReader.nodes.TryGetValue(road.NodeIDs[i - 1], out OsmNode node1) &&
                mapReader.nodes.TryGetValue(road.NodeIDs[i], out OsmNode node2))
            {
                Vector3 position1 = new Vector3(
                    (float)MercatorProjection.lonToX(node1.Longitude),
                    0,
                    (float)MercatorProjection.latToY(node1.Latitude)
                );

                Vector3 position2 = new Vector3(
                    (float)MercatorProjection.lonToX(node2.Longitude),
                    0,
                    (float)MercatorProjection.latToY(node2.Latitude)
                );

                // Log the positions
                Debug.Log($"Position 1: {position1}");
                Debug.Log($"Position 2: {position2}");

                // Calculate the midpoint
                Vector3 midpoint = (position1 + position2) / 2;

                // Log the midpoint
                Debug.Log($"Midpoint: {midpoint}");

                // Adjust by map origin and scale
                midpoint = (midpoint - mapOrigin) * scale;

                // Log the adjusted midpoint
                Debug.Log($"Adjusted Midpoint Position: {midpoint}");
                return midpoint;
            }
        }

        // Fallback to map origin if no valid nodes found
        return mapOrigin;
    }

    void SetVehiclePosition(Vector3 position)
    {
        // Disable the Rigidbody temporarily to manually set the position
        vehicleRigidbody.isKinematic = true;

        // Set the vehicle's position and rotation
        vehicle.transform.position = position;
        vehicle.transform.rotation = Quaternion.identity;

        // Re-enable the Rigidbody
        vehicleRigidbody.isKinematic = false;
        vehicleRigidbody.velocity = Vector3.zero;
        vehicleRigidbody.angularVelocity = Vector3.zero;

        // Ensure the vehicle is fully stopped
        vehicleRigidbody.Sleep();
    }
}
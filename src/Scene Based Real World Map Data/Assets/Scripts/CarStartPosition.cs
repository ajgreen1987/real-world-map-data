using UnityEngine;

public class CarStartPosition : MonoBehaviour
{
    public GameObject car; // Reference to the car object
    private MapReader mapReader;

    void Start()
    {
        // Reference the MapReader component
        mapReader = FindObjectOfType<MapReader>();

        if (mapReader != null && mapReader.IsReady && mapReader.ways.Count > 0)
        {
            // Choose a road to start on (e.g., the first road)
            OsmWay road = mapReader.ways.Find(w => !w.IsBoundary); // Assuming non-boundary ways are roads
            if (road != null)
            {
                Vector3 roadPosition = GetRoadPosition(road);

                // Adjust the car position to be on the road
                car.transform.position = new Vector3(roadPosition.x, roadPosition.y + 1, roadPosition.z); // Adjust y position to be above the road
                car.transform.rotation = Quaternion.identity; // Optional: Match the car's rotation to the road's rotation
            }
            else
            {
                Debug.LogWarning("No road data found in MapReader.");
            }
        }
        else
        {
            Debug.LogWarning("MapReader is not ready or contains no ways.");
        }
    }

    Vector3 GetRoadPosition(OsmWay road)
    {
        // Calculate the center of the road or use any point on the road
        Vector3 total = Vector3.zero;
        foreach (var id in road.NodeIDs)
        {
            total += mapReader.nodes[id];
        }
        return total / road.NodeIDs.Count;
    }
}

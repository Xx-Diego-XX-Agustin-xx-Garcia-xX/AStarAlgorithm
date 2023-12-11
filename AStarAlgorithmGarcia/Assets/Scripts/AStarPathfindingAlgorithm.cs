using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Timeline;

public class PathMarker
{
    public MapLocation location;
    public float A;
    public float B;
    public float C;
    public GameObject marker;
    public PathMarker markerParent;
    public PathMarker(MapLocation mapLocation, float a, float b, float c, GameObject gameObject, PathMarker pathMarker)
    {
        location = mapLocation;
        A = a;
        B = b;
        C = c;
        marker = gameObject;
        markerParent = pathMarker;
    }
    public override bool Equals(object a)
    {
        if((a == null) || !this.GetType().Equals(a.GetType()))
        {
            return false;
        }
        else
        {
            return location.Equals(((PathMarker)a).location);
        }
    }
    public override int GetHashCode()
    {
        return 0;
    }
}
public class AStarPathfindingAlgorithm : MonoBehaviour
{
    public Maze maze;
    public Material closedMaterial;
    public Material openMaterial;
    List<PathMarker> open = new List<PathMarker>();
    List<PathMarker> closed = new List<PathMarker>();
    public GameObject start;
    public GameObject end;
    public GameObject path;
    PathMarker startNode;
    PathMarker endNode;
    PathMarker lastPosition;
    bool done = false;
    void MarkerRemoval()
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");
        foreach(GameObject marker in markers)
        {
            Destroy(marker);
        }
    }
    void BeginSearch()
    {
        done = false;
        MarkerRemoval();
        List<MapLocation> locations = new List<MapLocation>();
        for(int z = 0; z < maze.depth - 1; z++)
        {
            for(int x = 0; x < maze.width - 1; x++)
            {
                if (maze.map[x,z] != 1)
                {
                    locations.Add(new MapLocation(x, z));
                }
            }
        }
        locations.Shuffle();
        Vector3 startLocation = new Vector3(locations[0].x * maze.scale, 0, locations[0].z * maze.scale);
        startNode = new PathMarker(new MapLocation(locations[0].x, locations[0].z), 0, 0, 0, Instantiate(start, startLocation, Quaternion.identity), null);
        Vector3 endLocation = new Vector3(locations[1].x * maze.scale, 0, locations[1].z * maze.scale);
        endNode = new PathMarker(new MapLocation(locations[1].x, locations[1].z), 0, 0, 0, Instantiate(end, endLocation, Quaternion.identity), null);
    }
    void Start()
    {
        
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            BeginSearch();
        }
    }
}

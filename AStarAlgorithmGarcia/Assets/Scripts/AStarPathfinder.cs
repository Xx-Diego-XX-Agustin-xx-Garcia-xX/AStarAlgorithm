using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class PathMarker
{
    public MapLocation location;
    public float A;
    public float B;
    public float C;
    public GameObject markerChild;
    public PathMarker markerParent;
    public PathMarker(MapLocation location, float a, float b, float c, GameObject markerChild, PathMarker markerParent)
    {
        this.location = location;
        A = a;
        B = b;
        C = c;
        this.markerChild = markerChild;
        this.markerParent = markerParent;
    }
    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        { 
            return false;
        }
        else
        {
            return location.Equals(((PathMarker)obj).location);
        }
    }
    public override int GetHashCode()
    {
        return 0;
    }
}
public class AStarPathfinder : MonoBehaviour
{
    public Maze maze;
    public Material closedMaterial;
    public Material openMaterial;
    List<PathMarker> open = new List<PathMarker>();
    List<PathMarker> closed = new List<PathMarker>();
    public GameObject start;
    public GameObject end;
    public GameObject path;
    PathMarker endNode;
    PathMarker startNode;
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
        for(int z = 1; z < maze.depth; z++)
        {
            for (int x = 1; x < maze.depth; x++)
            {
                if (maze.map[x,z] != 1)
                {
                    locations.Add(new MapLocation(x,z));
                }
            }
        }
        locations.Shuffle();
        Vector3 startLocation = new Vector3(locations[0].x * maze.scale,0, locations[0].z * maze.scale);
        startNode = new PathMarker(new MapLocation(locations[0].x, locations[0].z), 0, 0, 0, Instantiate(start, startLocation, Quaternion.identity), null);
        Vector3 endLocation = new Vector3(locations[1].x, 0, locations[1].z);
        endNode = new PathMarker(new MapLocation(locations[1].x, locations[1].z), 0, 0, 0, Instantiate(end, endLocation, Quaternion.identity), null);
        open.Clear();
        closed.Clear();
        open.Add(startNode);
        lastPosition = startNode;
    }
    void Search(PathMarker thisNode)
    {
        if (thisNode.Equals(endNode))
        {
            done = true;
            return;
        }
    }
    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BeginSearch();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Timeline;
using System.Dynamic;

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
        open.Clear();
        closed.Clear();
        open.Add(startNode);
        lastPosition = startNode;
    }
    void Search(PathMarker thisNode)
    {
        if(thisNode == null)
        {
            return;
        }
        if(thisNode.Equals(endNode))
        {
            done = true;
            return;
        }
        foreach (MapLocation direction in maze.directions)
        {
            MapLocation neighbor = direction + thisNode.location;
            if(maze.map[neighbor.x, neighbor.z] == 1)
            {
                continue;
            }
            if(neighbor.x < 1 || neighbor.x >= maze.width || neighbor.z < 1 || neighbor.z >= maze.width)
            {
                continue;
            }
            if(isClosed(neighbor))
            {
                continue;
            }
            float A = Vector2.Distance(thisNode.location.ToVector(), neighbor.ToVector()) + thisNode.A;
            float B = Vector2.Distance(neighbor.ToVector(), endNode.location.ToVector());
            float C = A + B;
            GameObject pathRod = Instantiate(path, new Vector3(neighbor.x * maze.scale, 0, neighbor.z * maze.scale), Quaternion.identity);
            TextMesh[] values = pathRod.GetComponentsInChildren<TextMesh>();
            values[0].text = "G: " + A.ToString("0.00");
            values[1].text = "H: " + B.ToString("0.00");
            values[2].text = "F: " + C.ToString("0.00");
            if(!MarkerUpdate(neighbor, A, B, C, thisNode))
            {
                open.Add(new PathMarker(neighbor, A, B, C, pathRod, thisNode));
            }
        }
        open = open.OrderBy(pathMarker => pathMarker.C).ThenBy(n => n.B).ToList<PathMarker>();
        PathMarker pM = (PathMarker) open.ElementAt(0);
        closed.Add(pM);
        open.RemoveAt(0);
        pM.marker.GetComponent<Renderer>().material = closedMaterial;
        lastPosition = pM;
    }
    bool MarkerUpdate(MapLocation position, float a, float b, float c, PathMarker path)
    {
        foreach(PathMarker pathMarker in open)
        {
            if(pathMarker.location.Equals(position))
            {
                return true;
            }
        }
        return false;
    }
    bool isClosed(MapLocation marker)
    {
        foreach(PathMarker pathMarker in closed)
        {
            if(pathMarker.location.Equals(marker))
            {
                return true;
            }
        }
        return false;
    }
    void Start()
    {
        
    }
    void GetPath()
    {
        MarkerRemoval();
        PathMarker begin = lastPosition;
        while(!startNode.Equals(begin) && begin != null)
        {
            Instantiate(path, new Vector3(begin.location.x * maze.scale, 0, begin.location.z * maze.scale), Quaternion.identity);
            begin = begin.markerParent;
        }
        Instantiate(path, new Vector3(startNode.location.x * maze.scale, 0, startNode.location.z * maze.scale), Quaternion.identity);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            BeginSearch();
        }
        if(Input.GetKeyDown(KeyCode.S) && !done)
        {
            Search(lastPosition);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GetPath();
        }
    }
}
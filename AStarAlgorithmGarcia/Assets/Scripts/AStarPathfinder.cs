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
        if((obj == null) || !this.GetType().Equals(obj.GetType()))
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
            for(int x = 1; x < maze.depth; x++)
            {
                if(maze.map[x,z] != 1)
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
        if(thisNode == null)
        {
            return;
        }
        if(thisNode.Equals(endNode))
        {
            done = true;
            return;
        }
        foreach(MapLocation dir in maze.directions)
        {
            MapLocation neighbor = dir + thisNode.location;
            if(maze.map[neighbor.x, neighbor.z] == 1)
            {
                continue;
            }
            if(neighbor.x < 1 || neighbor.x >= maze.width || neighbor.z < 1 || neighbor.z >= maze.depth)
            {
                continue;
            }
            if(IsClosed(neighbor))
            {
                continue;
            }
            float A = Vector2.Distance(thisNode.location.ToVector(), neighbor.ToVector()) + thisNode.A;
            float B = Vector2.Distance(neighbor.ToVector(), endNode.location.ToVector());
            float C = A + B;
            GameObject pathBlock = Instantiate(path, new Vector3(neighbor.x * maze.scale, 0, neighbor.z * maze.scale), Quaternion.identity);
            TextMesh[] values = pathBlock.GetComponentsInChildren<TextMesh>();
            values[0].text = "G: " + A.ToString("0.00");
            values[1].text = "H: " + B.ToString("0.00");
            values[2].text = "F: " + C.ToString("0.00");
            if(!UpdateMarker(neighbor, A, B, C, thisNode))
            {
                open.Add(new PathMarker(neighbor, A, B, C, pathBlock, thisNode));
            }
        }
        open = open.OrderBy(pathMarker => pathMarker.C).ThenBy(x => x.B).ToList<PathMarker>();
        PathMarker smallMarker = (PathMarker) open.ElementAt(0);
        closed.Add(smallMarker);
        open.RemoveAt(0);
        smallMarker.markerChild.GetComponent<Renderer>().material = closedMaterial;
        lastPosition = smallMarker;
    }
    bool UpdateMarker(MapLocation position, float a, float b, float c, PathMarker markpath)
    {
        foreach(PathMarker pathMarker in open)
        {
            pathMarker.A = a;
            pathMarker.B = b;
            pathMarker.C = c;
            pathMarker.markerParent = markpath;
            return true;
        }
        return false;
    }
    bool IsClosed(MapLocation marker)
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
        Debug.Log("Press W before you press S");
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

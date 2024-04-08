using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutingTable : MonoBehaviour
{
    private List<Route> routes;

    public RoutingTable()
    {
        routes = new List<Route>();
    }

    public void AddStaticRoute(Route route)
    {
        routes.Add(route);
    }

    public void RemoveRoute(Route route)
    {
        routes.Remove(route);
    }

    public Route[] GetAllRoutes()
    {
        return routes.ToArray();
    }
}

public class Route
{
    public string Destination { get; set; }
    public string SubnetMask { get; set; }
    public string NextHop { get; set; }
    public int Metric {  get; set; }

    public Route(string destination, string subnetMask, string nextHop, int metric)
    {
        Destination = destination;
        SubnetMask = subnetMask;
        NextHop = nextHop;
        Metric = metric;
    }
}



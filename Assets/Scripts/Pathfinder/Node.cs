using System;
using System.Collections.Generic;

public class Node<Coordinate> : INode<Coordinate>, INode where Coordinate : IEquatable<Coordinate>
{
    private Coordinate coordinate;

    private bool isBloqued;

    private ICollection<INode<Coordinate>> neighbors = new List<INode<Coordinate>>();

    private Dictionary<INode, int> neighborCost = new Dictionary<INode, int>();

    private int cost;

    public void SetCoordinate(Coordinate coordinate)
    {
        this.coordinate = coordinate;
    }

    public Coordinate GetCoordinate()
    {
        return coordinate;
    }

    public bool EqualsTo(INode newNode) 
    {
        return coordinate.Equals((newNode as Node<Coordinate>).coordinate);
    }

    public bool IsBloqued()
    {
        return isBloqued;
    }

    public ICollection<INode<Coordinate>> GetNeighbords()
    {
        return neighbors;
    }

    public int GetCost()
    {
        return cost;
    }

    public void SetCost(int newCost) 
    {
        cost = newCost;
    }

    public void AddNeighbor(INode<Coordinate> neighbor, int cost)
    {
        neighborCost[neighbor] = cost;
        neighbors.Add(neighbor);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INode
{
    enum NodesType
    {
        Start,
        Walkable,
        Bloqued,
        Mine,
        End
    }

    public bool IsBloqued();

    public bool IsAMine();
}

public interface INode<Coorninate> : INode where Coorninate : IEquatable<Coorninate>
{
    public void SetCoordinate(Coorninate coordinateType);
    public Coorninate GetCoordinate();
    public ICollection<INode<Coorninate>> GetNeighbords();
    public int GetCost();

    public void AddNeighbor(INode<Coorninate> neighbor, int cost);
}
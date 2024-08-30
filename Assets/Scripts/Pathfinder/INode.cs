using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INode
{
    public bool IsBloqued();
}

public interface INode<Coorninate> : INode where Coorninate : IEquatable<Coorninate>
{
    public void SetCoordinate(Coorninate coordinateType);
    public Coorninate GetCoordinate();
    public ICollection<INode<Coorninate>> GetNeighbords();
    public int GetCost();

    public void AddNeighbor(INode<Coorninate> neighbor, int cost);
}
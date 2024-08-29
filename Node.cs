using System.Collections.Generic;
using static TreeEditor.TreeEditorHelper;

public class Node<Coordinate> : INode, INode<Coordinate>
{
    private Coordinate coordinate;

    private bool isBloqued;

    private ICollection<NodeType> neighbors = new List<NodeType>();

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

    public ICollection<NodeType> GetNeightbors()
    {
        return neighbors;
    }

    public ICollection<INode<Coordinate>> GetNeighbords()
    {
        throw new System.NotImplementedException();
    }
}
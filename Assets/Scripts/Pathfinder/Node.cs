public class Node<Coordinate> : INode, INode<Coordinate>
{
    private Coordinate coordinate;

    private bool isBloqued;

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
}
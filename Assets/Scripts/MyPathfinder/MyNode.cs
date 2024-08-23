using UnityEngine;

public class MyNode
{
    public Vector2Int coordinate;

    public bool isBloqued;

    public void SetCoordinate(Vector2Int newCoordinate)
    {
        this.coordinate = newCoordinate;
    }

    public Vector2Int GetCoordinate()
    {
        return coordinate;
    }

    public bool IsBloqued()
    {
        return isBloqued;
    }

    public bool EqualsTo(MyNode newNode)
    {
        return coordinate.Equals(newNode.coordinate);
    }
}

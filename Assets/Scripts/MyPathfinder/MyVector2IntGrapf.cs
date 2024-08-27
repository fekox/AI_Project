using System.Collections.Generic;

public class MyVector2IntGrapf
{
    public List<MyNode> nodes = new List<MyNode>();

    public MyVector2IntGrapf(int x, int y)
    {
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                MyNode node = new MyNode();
                node.SetCoordinate(new UnityEngine.Vector2Int(i, j));
                nodes.Add(node);
            }
        }
    }
}

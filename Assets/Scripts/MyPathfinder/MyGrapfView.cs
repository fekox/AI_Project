using UnityEngine;

public class MyGrapfView : MonoBehaviour
{
    public MyVector2IntGrapf grapf = new MyVector2IntGrapf(10, 10);

    void Start()
    {
        grapf = new MyVector2IntGrapf(10, 10);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        foreach (MyNode node in grapf.nodes)
        {
            if (node.IsBloqued())
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(new Vector3(node.GetCoordinate().x, node.GetCoordinate().y), 0.1f);
        }
    }
}

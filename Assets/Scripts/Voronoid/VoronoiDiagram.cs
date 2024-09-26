using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteAlways]
public class VoronoiDiagram : MonoBehaviour
{
    public bool drawPolis;

    [SerializeField] private List<Vector2> intersections = new List<Vector2>();
    [Space(15), SerializeField]
    private List<ThiessenPolygon2D<SegmentVec2, Vector2>> polis =
        new List<ThiessenPolygon2D<SegmentVec2, Vector2>>();
    [SerializeField] private List<SegmentLimit> segmentLimit = new List<SegmentLimit>();
    [SerializeField]
    private Dictionary<ThiessenPolygon2D<SegmentVec2, Vector2>, Color> polyColors =
        new Dictionary<ThiessenPolygon2D<SegmentVec2, Vector2>, Color>();


    [SerializeField] private List<Vector2> pointsToCheck = new List<Vector2>();
    private Dictionary<(Vector2, Vector2), float> weight = new();

    public List<ThiessenPolygon2D<SegmentVec2, Vector2>> GetPoly => polis;
    public GrapfView graph;
    public GameObject test;


    public void AddNewItem(Transform item)
    {
        // transformPoints.Add(item);
        CreateSegments();
    }

    public void RemoveItem(Transform item)
    {
        // transformPoints.Remove(item);
        CreateSegments();
    }

    private IEnumerator Start()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        foreach (var Node in graph.GetMines())
        {
            pointsToCheck.Add(Node.GetCoordinate());
        }

        weight = new Dictionary<(Vector2, Vector2), float>();
        weight.Clear();
        foreach (Vector2 point in pointsToCheck)
        {
            foreach (Vector2 otherPoint in pointsToCheck)
            {
                weight.Add((point, otherPoint), 0.5f);
            }
        }

        CreateSegments();
    }

    private void Update()
    {
        if (test != null)
        {
            Vector2 testPos = new Vector2(test.transform.position.x, test.transform.position.y);
            foreach (ThiessenPolygon2D<SegmentVec2, Vector2> VARIABLE in polis)
            {
                if (VARIABLE.IsInside(testPos))
                {
                    Debug.Log($"The Object is inside the poly: {VARIABLE.itemSector}");
                }

                VARIABLE.IsInside(testPos);
            }
        }
    }

    [ContextMenu("CreateSegment")]
    private void CreateSegments()
    {
        pointsToCheck.Clear();
        foreach (var Node in graph.GetMines())
        {
            pointsToCheck.Add(Node.GetCoordinate());
        }

        if (pointsToCheck == null)
            return;
        if (pointsToCheck.Count < 1)
            return;

        SegmentVec2.amountSegments = 0;
        polis.Clear();
        intersections.Clear();
        polyColors.Clear();

        for (int i = 0; i < pointsToCheck.Count; i++)
        {
            ThiessenPolygon2D<SegmentVec2, Vector2> poli =
                new ThiessenPolygon2D<SegmentVec2, Vector2>(pointsToCheck[i], intersections, 0.5f);
            polis.Add(poli);
            poli.colorGizmos.r = Random.Range(0, 1.0f);
            poli.colorGizmos.g = Random.Range(0, 1.0f);
            poli.colorGizmos.b = Random.Range(0, 1.0f);
            poli.colorGizmos.a = 0.3f;
        }

        for (int i = 0; i < polis.Count; i++)
        {
            polis[i].AddSegmentsWithLimits(segmentLimit);
        }

        for (int i = 0; i < pointsToCheck.Count; i++)
        {
            for (int j = 0; j < pointsToCheck.Count; j++)
            {
                if (i == j)
                    continue;
                SegmentVec2 segment =
                    new SegmentVec2(pointsToCheck[i], pointsToCheck[j], 0.5f);
                polis[i].AddSegment(segment);
            }
        }

        for (int i = 0; i < polis.Count; i++)
        {
            polis[i].SetIntersections();
        }

        //SetWeightPoligons();
    }

    //private void SetWeightPoligons()
    //{
    //    float allWeight = 0;
    //    for (int i = 0; i < graph.GetMines().Count; i++)
    //    {
    //        allWeight += graph.graph.nodes[i].GetWeight();

    //        for (int j = 0; j < polis.Count; j++)
    //        {
    //            if (polis[j].IsInside(graph.graph.nodes[i].GetCoordinate()))
    //            {
    //                polis[j].weight += graph.graph.nodes[i].GetWeight();
    //                break;
    //            }
    //        }
    //    }

    //    CreateWeightedSegments();
    //}

    private void CreateWeightedSegments()
    {
        weight.Clear();
        // for (int index = 0; index < pointsToCheck.Count; index++)
        // {
        //     Vector2 point = pointsToCheck[index];
        //     for (int j = 0; j< pointsToCheck.Count; j++)
        //     {
        //         if (index == j)
        //         {
        //             continue;
        //         }
        //         Vector2 otherPoint = pointsToCheck[j];
        //         weight.TryAdd((point, otherPoint), 0.5f);
        //     }
        // }
        for (int i = 0; i < polis.Count; i++)
        {
            float totalNeighborWeight = 0f;

            // First pass: Calculate total weight of neighbors for polygon 'i'
            for (int j = 0; j < polis.Count; j++)
            {
                if (i == j || !polis[i].hasSameSegment(polis[j]))
                {
                    continue;
                }

                // Add the weight of the neighboring polygon 'j' to the total
                totalNeighborWeight += polis[j].weight;
            }

            for (int j = 0; j < polis.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                if (!polis[i].hasSameSegment(polis[j]))
                {
                    weight.TryAdd((pointsToCheck[i], pointsToCheck[j]), 0.5f);
                    weight.TryAdd((pointsToCheck[j], pointsToCheck[i]), 0.5f);
                    continue;
                }

                float weightA = polis[i].weight;
                float weightB = polis[j].weight;

                // Calculate percentage of influence relative to total neighbor weight
                float percentajePolyA = weightA / (weightA + totalNeighborWeight);
                float percentajePolyB = weightB / (weightB + totalNeighborWeight);

                // Ensure the percentages sum up to 1
                float totalPercentage = percentajePolyA + percentajePolyB;
                percentajePolyA /= totalPercentage;
                percentajePolyB /= totalPercentage;

                // Assign the calculated percentages to the weight dictionary
                weight.TryAdd((pointsToCheck[i], pointsToCheck[j]), percentajePolyA);
                weight.TryAdd((pointsToCheck[j], pointsToCheck[i]), percentajePolyB);
            }
        }
    }

    [ContextMenu("Create WeightedVornoid")]
    private void CreateWeightedVoronoid()
    {
        if (polis == null || polis.Count < 1)
            //       CreateSegments();

            if (polis == null || polis.Count < 1)
                return;
        CreateWeightedSegments();

        SegmentVec2.amountSegments = 0;
        polis.Clear();
        intersections.Clear();
        polyColors.Clear();

        for (int i = 0; i < pointsToCheck.Count; i++)
        {
            ThiessenPolygon2D<SegmentVec2, Vector2> poli =
                new ThiessenPolygon2D<SegmentVec2, Vector2>(pointsToCheck[i], intersections, 0.5f);
            polis.Add(poli);
            poli.colorGizmos.r = Random.Range(0, 1.0f);
            poli.colorGizmos.g = Random.Range(0, 1.0f);
            poli.colorGizmos.b = Random.Range(0, 1.0f);
            poli.colorGizmos.a = 0.3f;
        }

        for (int i = 0; i < polis.Count; i++)
        {
            polis[i].AddSegmentsWithLimits(segmentLimit);
        }

        for (int i = 0; i < pointsToCheck.Count; i++)
        {
            for (int j = 0; j < pointsToCheck.Count; j++)
            {
                if (i == j)
                    continue;

                float percentage = weight[(pointsToCheck[i], pointsToCheck[j])];
                SegmentVec2 segment =
                    new SegmentVec2(pointsToCheck[i], pointsToCheck[j], percentage);
                polis[i].AddSegment(segment);
            }
        }

        for (int i = 0; i < polis.Count; i++)
        {
            polis[i].SetIntersections();
        }

        //SetWeightPoligons();
    }

    bool IsNodeOutsideLimits(Node<Vector2> node)
    {
        Vector2 origin = segmentLimit[0].Origin;
        Vector2 final = segmentLimit[2].Origin;
        Vector2 point = node.GetCoordinate();

        return !(point.x > origin.x &&
                 point.y > origin.y &&
                 point.x < final.x &&
                 point.y < final.y);
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        DrawPolis(drawPolis);
    }

    private void DrawPolis(bool drawPolis)
    {
        if (polis != null)
        {
            foreach (ThiessenPolygon2D<SegmentVec2, Vector2> poli in polis)
            {
                if (poli.drawPoli || drawPolis)
                {
                    poli.DrawPoly();
                }
            }
        }
    }
#endif
}
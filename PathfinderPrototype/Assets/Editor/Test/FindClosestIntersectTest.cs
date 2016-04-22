using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

[TestFixture]
[Category("Helper Class Tests")]
public class FindClosestIntersectTest : MonoBehaviour
{

    private List<Vector3> createCurve()
    {
        List<Vector3> points = new List<Vector3>();
        points.Add(new Vector3(1, 1, 0));
        points.Add(new Vector3(2, 2, 0));
        points.Add(new Vector3(3, 1, 0));
        points.Add(new Vector3(5, 3, 0));
        points.Add(new Vector3(6, 2, 0));

        return points;
    }

    [Test]
    public void testIntersectPoint()
    {
        Vector3 testPoint = new Vector3(4, 2, 0);

        Vector3 closestPoint = FindClosestIntersect.search(createCurve(), testPoint);

        Assert.That(closestPoint, Is.Not.Null);
        Assert.That(closestPoint, Is.EqualTo(testPoint));
    }

    [Test]
    public void testClosePoint()
    {
        Vector3 testPoint = new Vector3(4, 2.1f, 0);

        Vector3 closestPoint = FindClosestIntersect.search(createCurve(), testPoint);

        Assert.That(closestPoint, Is.Not.Null);
        Assert.That(Vector3.Distance(closestPoint, testPoint), Is.LessThanOrEqualTo(0.1f));
    }

    [Test]
    public void testFarPoint()
    {
        Vector3 testPoint = new Vector3(5, 13, 0);

        Vector3 closestPoint = FindClosestIntersect.search(createCurve(), testPoint);

        Assert.That(closestPoint, Is.Not.Null);
        Assert.That(Vector3.Distance(closestPoint, testPoint), Is.GreaterThanOrEqualTo(10));
    }

}

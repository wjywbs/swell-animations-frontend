using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

[TestFixture]
[Category("Helper Class Tests")]
public class FindClosestIntersectTest : MonoBehaviour {


    [Test]
    public void findClosestPointTest()
    {
        float tolerance = 1.0f;
        FindClosestIntersect intersectFinder = new FindClosestIntersect();
        intersectFinder.searchTolerance = tolerance;

        IList<Vector3> points = new List<Vector3>();
        points.Add(new Vector3(1, 1, 0));
        points.Add(new Vector3(2, 2, 0));
        points.Add(new Vector3(3, 1, 0));
        points.Add(new Vector3(5, 3, 0));
        points.Add(new Vector3(6, 2, 0));

        Vector3 testPoint = new Vector3(4, 2, 0);

        Vector3? closestPoint = intersectFinder.search(points, testPoint);

        Assert.That(closestPoint, Is.Not.Null);
        Assert.That(Vector3.Distance(testPoint, closestPoint.Value), Is.LessThanOrEqualTo(tolerance));

    }

}

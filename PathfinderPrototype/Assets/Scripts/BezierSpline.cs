using System;
using System.Collections.Generic;
using UnityEngine;

public class BezierSpline
{

    public Vector3[] controlPoints;
    public Vector3[] points;
    public Transform transform;
    public bool controlPointsComputed = false;

    public int CurveCount
    {
        get
        {
            return points.Length - 1;
        }
    }

    public int ControlPointNum
    {
        get
        {
            return (CurveCount * 3) + 1;
        }
    }

    public Vector3 GetPoint(float t)
    {
        ComputeControlPoints();
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = controlPoints.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        //Debug.Log("GetPoint():: Control points count: " + controlPoints.Length + " Curve count: " + CurveCount + " first index: " + i);
        return Bezier.GetPoint(
            controlPoints[i], controlPoints[i + 1], controlPoints[i + 2], controlPoints[i + 3], t);
    }

    public Vector3 GetVelocity(float t)
    {
        ComputeControlPoints();
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = controlPoints.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetFirstDerivative(
            controlPoints[i], controlPoints[i + 1], controlPoints[i + 2], controlPoints[i + 3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }

    public void AddCurve()
    {
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 3);
        point.x += 1f;
        points[points.Length - 3] = point;
        point.x += 1f;
        points[points.Length - 2] = point;
        point.x += 1f;
        points[points.Length - 1] = point;
    }


    public void ComputeControlPoints()
    {
        //There are 10 points in each curve
        controlPoints = new Vector3[ControlPointNum];
        controlPoints[0] = points[0];
        for (int i = 0; i < CurveCount / 3; i ++)
        {
            float[] xPoints = new float[4];
            float[] yPoints = new float[4];
            float[] zPoints = new float[4];
            int nextIndex = i * 3;
            for (int n = 0; n < 4; n++)
            {
                xPoints[n] = points[nextIndex + n].x;
                yPoints[n] = points[nextIndex + n].y;
                zPoints[n] = points[nextIndex + n].z;
            }
            /*computes control points p1 and p2 for x, y and z directions*/
            ComputedAxis xAxis = ComputeControlPoints(xPoints);
            ComputedAxis yAxis = ComputeControlPoints(yPoints);
            ComputedAxis zAxis = ComputeControlPoints(zPoints);
            int pointIndex = nextIndex + 1;
            int offset = (nextIndex * 3) + 1;
            for (int n = 0; n < 3; n++)
            {
                int controlPointIndex = offset + (n * 3);
                controlPoints[controlPointIndex] = new Vector3(
                    xAxis.point1[n],
                    yAxis.point1[n],
                    zAxis.point1[n]);
                controlPoints[controlPointIndex + 1] = new Vector3(
                    xAxis.point2[n],
                    yAxis.point2[n],
                    zAxis.point2[n]);
                controlPoints[controlPointIndex + 2] = points[pointIndex++];
            }
        }
        //Debug.Log("ComputeControlPoints():: Control points count: " + controlPoints.Length + " Curve count: " + CurveCount);
            controlPointsComputed = true;
    }

    private struct ComputedAxis{
        public float[] point1;
        public float[] point2;
    };

    private ComputedAxis ComputeControlPoints(float[] knots)
    {
        //Algorithm Copied from https://www.particleincell.com/wp-content/uploads/2012/06/bezier-spline.js
        int n = knots.Length - 1;
        float[] p1 = new float[n];
        float[] p2 = new float[n];

        /*rhs vector*/
        float[] a = new float[n];
        float[] b = new float[n];
        float[] c = new float[n];
        float[] r = new float[n];

        /*left most segment*/
        a[0] = 0;
        b[0] = 2;
        c[0] = 1;
        r[0] = knots[0] + 2 * knots[1];

        /*internal segments*/
        for (int i = 1; i < n - 1; i++)
        {
            a[i] = 1;
            b[i] = 4;
            c[i] = 1;
            r[i] = 4 * knots[i] + 2 * knots[i + 1];
        }

        /*right segment*/
        a[n - 1] = 2;
        b[n - 1] = 7;
        c[n - 1] = 0;
        r[n - 1] = 8 * knots[n - 1] + knots[n];

        /*solves Ax=b with the Thomas algorithm (from Wikipedia)*/
        for (int i = 1; i < n; i++)
        {
            float m = a[i] / b[i - 1];
            b[i] = b[i] - m * c[i - 1];
            r[i] = r[i] - m * r[i - 1];
        }

        p1[n - 1] = r[n - 1] / b[n - 1];
        for (int i = n - 2; i >= 0; --i)
            p1[i] = (r[i] - c[i] * p1[i + 1]) / b[i];

        /*we have p1, now compute p2*/
        for (int i = 0; i < n - 1; i++)
            p2[i] = 2 * knots[i + 1] - p1[i + 1];

        p2[n - 1] = 0.5f * (knots[n] + p1[n - 1]);

        return new ComputedAxis()
        {
            point1 = p1,
            point2 = p2
        };
    }

    public void Reset()
    {
        points = new Vector3[] {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };
        controlPointsComputed = false;
    }
}
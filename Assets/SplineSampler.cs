using UnityEngine;

public interface ISpline
{
    Vector3 GetPoint(float t);
    Vector3 GetTangent(float t);
}

public abstract class SplineSampler
{
    public struct Point
    {
        public float t;
        public float distance;
        public Vector3 worldPosition;
        public Vector3 direction;

        public Point(float t, float distance, Vector3 worldPosition, Vector3 direction)
        {
            this.t = t;
            this.distance = distance;
            this.worldPosition = worldPosition;
            this.direction = direction;
        }
    }

    public struct Sample
    {
        public Point a;
        public Point b;
        public float t;
        private float delta;

        public Sample(Point a, Point b, float t , float delta)
        {
            this.a = a;
            this.b = b;
            this.t = t;
            this.delta = delta;
        }

        public float GetNormlizeT()
        {
            return a.t + t * delta;
        }

        public Vector3 GetPosition()
        {
            return Vector3.Lerp(a.worldPosition, b.worldPosition, t);
        }

        public Vector3 GetDirection()
        {
            return Vector3.Lerp(a.direction, b.direction, t).normalized;
        }

        public float GetDistance()
        {
            return Mathf.Lerp(a.distance, b.distance, t);
        }

        public bool isValid()
        {
            return ValidNormlize(t);
        }

        public static bool ValidNormlize(float t)
        {
            return t >= 0 && t <= 1f;
        }

    }

    protected ISpline spline;
    protected Point[] points;
    protected float delta;

    protected SplineSampler(ISpline spline, int sampleNum = 100)
    {
        this.spline = spline;
        GenratesPoints(sampleNum);
    }

    protected abstract void GenratesPoints(int number);

    protected void GenratesPoints(int number, float delta)
    {
        points = new Point[number];
        float distance = 0;
        this.delta = delta;

        points[0] = new Point(0, 0, spline.GetPoint(0), spline.GetTangent(0));
        for (int i = 1; i < number; i++)
        {
            float t = delta * i;
            var worldPosition = spline.GetPoint(t);
            distance += Vector3.Distance(points[i - 1].worldPosition, worldPosition);
            points[i] = new Point(t, distance, worldPosition, spline.GetTangent(t));
        }
    }

    public abstract Sample GetNextPosition(float t, float deltaDistance);

    public abstract Sample GetPreviousPosition(float t, float deltaDistance);

    public abstract Sample GetClosestSample(Vector3 position, int itterations = 10);

    public Vector3 GetPosition(float t)
    {
        var delta = GetClosestSample(t);
        return delta.GetPosition();
    }

    public Vector3 GetDirection(float t)
    {
        var delta = GetClosestSample(t);
        return delta.GetDirection();
    }

    public Sample GetClosestSample(float t)
    {
        var indies = GetClosestIndies(t);
        return IndiesToSample(indies, t);
    }

    protected Sample IndiesToSample(Vector2Int indies, float t)
    {
        return new Sample(points[indies.x], points[indies.y], GetNormlizeBetweenPoints(indies.x, indies.y, t), delta);
    }

    protected Sample GetClosestSample(float t, int min, int max)
    {
        if (max - min <= 1) return new Sample(points[min], points[max], GetNormlizeBetweenPoints(min, max, t), delta);

        int mid = min + (max - min) / 2;

        if (points[mid].t >= t)
            return GetClosestSample(t, min, mid);
        else
            return GetClosestSample(t, mid, max);
    }

    protected Vector2Int GetClosestIndies(float t)
    {
        return GetClosestIndies(t, 0, points.Length - 1);
    }

    protected Vector2Int GetClosestIndies (float t , int min , int max)
    {
        if (max - min <= 1) return new Vector2Int(min,max);

        int mid = min + (max - min) / 2;

        if (points[mid].t >= t)
            return GetClosestIndies(t, min, mid);
        else
            return GetClosestIndies(t, mid, max);
    }

    protected float GetNormlizedBetweenIndex(Vector3 position, int a, int b)
    {
        return GetNormlizePosition(points[a].worldPosition, points[b].worldPosition, position);
    }

    protected float GetNormlizePosition(Vector3 a, Vector3 b, Vector3 point)
    {
        var direction = (b - a).normalized;
        var lineToPoint = point - a;
        var distance = Vector3.Distance(b, a);

        if (distance != 0)
            return Vector3.Dot(lineToPoint, direction) / distance;
        else
            return float.PositiveInfinity;
    }

    protected float GetNormlizeBetweenPoints(int a, int b, float t)
    {
        return Mathf.InverseLerp(points[a].t, points[b].t, t);
    }

    protected float GetDistance(Point a, Point b)
    {
        return Vector3.Distance(a.worldPosition, b.worldPosition);
    }

    protected float GetDistance(int a , int b)
    {
        return Vector3.Distance(points[a].worldPosition, points[b].worldPosition);
    }

    public virtual void DrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 1; i < points.Length; i++)
        {
            Gizmos.DrawIcon(points[i - 1].worldPosition, (i - 1).ToString());
            Gizmos.DrawLine(points[i - 1].worldPosition, points[i].worldPosition);
        }
    }
}
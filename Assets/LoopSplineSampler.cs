using UnityEngine;
using System.Collections;

public class LoopSplineSampler : SplineSampler
{
    public LoopSplineSampler(ISpline spline, int sampleNum = 100) : base(spline, sampleNum)
    {
    }

    protected override void GenratesPoints(int number)
    {
        float delta = 1f / (number - 1);
        GenratesPoints(number - 1, delta);
    }

    public override Sample GetNextPosition(float t, float distance)
    {
        var indies = GetClosestIndies(t);
        Vector3 position = IndiesToSample(indies, t).GetPosition();
        float movedDistance = 0;
        LoopInt index = new LoopInt(indies.x,0, points.Length);

        while (movedDistance < distance)
        {
            index++;

            movedDistance += Vector3.Distance(position, points[index].worldPosition);
            position = points[index].worldPosition;
        }

        Sample result = default;

        float min = movedDistance - Vector3.Distance(points[index - 1].worldPosition, points[index].worldPosition);
        result = new Sample(points[index - 1],points[index], Mathf.InverseLerp(min, movedDistance, distance), delta);

        return result;
    }

    public override Sample GetPreviousPosition(float t, float distance)
    {
        var indies = GetClosestIndies(t);
        Vector3 position = IndiesToSample(indies, t).GetPosition();
        float movedDistance = 0;
        LoopInt index = new LoopInt(indies.y, 0, points.Length);

        while (movedDistance < distance)
        {
            index--;

            movedDistance += Vector3.Distance(position, points[index].worldPosition);
            position = points[index].worldPosition;
        }

        Sample result = default;

        float min = movedDistance - Vector3.Distance(points[index + 1].worldPosition, points[index].worldPosition);
        result = new Sample(points[index], points[index + 1], Mathf.InverseLerp(movedDistance,min, distance), delta);

        return result;
    }

    public override Sample GetClosestSample(Vector3 position , int itterations = 10)
    {
        Sample[] results = new Sample[itterations];
        int step = points.Length / itterations;

        for (int i = 0; i < itterations; i++)
        {
            var startIndex = new LoopInt(step * i, 0, points.Length - 1);
            results[i] = GetClosestLocalSample(position, step , startIndex, startIndex - 2 );
        }

        Sample closest = default;
        float clostestDistance = float.MaxValue;

        for (int i = 0; i < itterations; i++)
        {
            float dis = Vector3.Distance(position, results[i].GetPosition());
            if(dis < clostestDistance)
            {
                closest = results[i];
                clostestDistance = dis;
            }
        }

        return closest;
    }

    private Sample GetClosestLocalSample(Vector3 position, int step, LoopInt index, LoopInt redirectIndex)
    {

        LoopInt nextIndex = index + step;

        if (Vector3.Distance(position, points[index].worldPosition) <
            Vector3.Distance(position, points[nextIndex].worldPosition))
        {
            int preStep = step;
            step = Mathf.Abs(step) == 1 ? 1 : -step / 2;

            if (Mathf.Abs(index.Delta(redirectIndex)) <= 1 && Mathf.Abs(preStep) == 1)
            {
                LoopInt mid = index;
                LoopInt before = mid - 1;
                LoopInt after = mid + 1;

                float beforeNormlize = GetNormlizedBetweenIndex(position, before, mid);
                float afterNormlize = GetNormlizedBetweenIndex(position, mid, after);

                if (Sample.ValidNormlize(afterNormlize))
                {
                    return new Sample(points[mid],points[after], afterNormlize, delta);
                }
                else if (Sample.ValidNormlize(beforeNormlize))
                {
                    return new Sample(points[before], points[mid], beforeNormlize, delta);
                }else
                {
                    return new Sample(points[mid], points[after], Mathf.Clamp01(afterNormlize), delta);
                }

            }

            return GetClosestLocalSample(position, step, nextIndex, index);
        }
        else
        {
            return GetClosestLocalSample(position, step, nextIndex, redirectIndex);
        }
    }

    public override void DrawGizmos()
    {
        base.DrawGizmos();
        Gizmos.DrawIcon(points[points.Length-1].worldPosition, (points.Length - 1).ToString());
        Gizmos.DrawLine(points[0].worldPosition, points[points.Length - 1].worldPosition);
    }
}
using UnityEngine;
using System.Collections;

public class NormalSplineSampler : SplineSampler
{
    public NormalSplineSampler(ISpline spline, int sampleNum) : base (spline,sampleNum)
    {

    }

    protected override void GenratesPoints(int number)
    {
        float delta = 1f / (number - 1);
        GenratesPoints(number, delta);
    }

    public override Sample GetNextPosition(float t, float distance)
    {
        var indies = GetClosestIndies(t);
        Vector3 position = IndiesToSample(indies,t).GetPosition();
        float movedDistance = 0;
        int index = indies.x;

        while (movedDistance < distance && index < points.Length-1)
        {
            index++;

            movedDistance += Vector3.Distance(position, points[index].worldPosition);
            position = points[index].worldPosition;
        }

        Sample result = default;

        if(index == 0)
        {
            result = new Sample(points[0],points[1], Mathf.Clamp01(movedDistance/GetDistance(0,1)), delta);
        }else
        {
            float min = movedDistance - Vector3.Distance(points[index - 1].worldPosition, points[index].worldPosition);
            result = new Sample(points[index - 1],points[index], Mathf.Clamp01(Mathf.InverseLerp(min, movedDistance, distance)), delta);
        }

        return result;
    }

    public override Sample GetPreviousPosition(float t, float distance)
    {
        var indies = GetClosestIndies(t);
        Vector3 position = IndiesToSample(indies, t).GetPosition();
        float movedDistance = 0;
        LoopInt index = new LoopInt(indies.y, 0, points.Length);

        while (movedDistance < distance && index > 0)
        {
            index--;

            movedDistance += Vector3.Distance(position, points[index].worldPosition);
            position = points[index].worldPosition;
        }

        Sample result = default;

        if(index == points.Length-1)
        {
            result = new Sample(points[points.Length - 2],points[index], Mathf.Clamp01(1f - movedDistance/GetDistance(points.Length - 2,index)), delta);
        }
        else
        {
            float min = movedDistance - Vector3.Distance(points[index + 1].worldPosition, points[index].worldPosition);
            result = new Sample(points[index], points[index + 1], Mathf.InverseLerp(movedDistance, min, distance), delta);
        }

        return result;
    }

    public override Sample GetClosestSample(Vector3 position, int itterations = 10)
    {
        Sample[] results = new Sample[itterations];
        int step = points.Length / itterations;

        for (int i = 0; i < itterations; i++)
        {
            results[i] = GetClosestLocalSample(position, step, step * i, step * i - 2);
        }

        Sample closest = default;
        float clostestDistance = float.MaxValue;

        for (int i = 0; i < itterations; i++)
        {
            float dis = Vector3.Distance(position, results[i].GetPosition());
            if (dis < clostestDistance)
            {
                closest = results[i];
                clostestDistance = dis;
            }
        }

        return closest;
    }

    private Sample GetClosestLocalSample(Vector3 position, int step, int index, int redirectIndex)
    {

        int nextIndex = index + step;

        nextIndex = Mathf.Clamp(nextIndex, 0, points.Length - 1);

        if (Vector3.Distance(position, points[index].worldPosition) <=
            Vector3.Distance(position, points[nextIndex].worldPosition))
        {
            int preStep = step;
            step = Mathf.Abs(step) == 1 ? 1 : -step / 2;

            if (Mathf.Abs(index - redirectIndex) <= 1 && Mathf.Abs(preStep) == 1)
            {
                int mid = index;
                int before = Mathf.Clamp(mid - 1, 0, points.Length - 1);
                int after = Mathf.Clamp(mid + 1, 0, points.Length - 1);

                float beforeNormlize = GetNormlizedBetweenIndex(position, before, mid);
                float afterNormlize = GetNormlizedBetweenIndex(position, mid, after);

                if (Sample.ValidNormlize(afterNormlize))
                {
                    return new Sample( points[mid],points[after], afterNormlize, delta);
                }
                else if (Sample.ValidNormlize(beforeNormlize))
                {
                    return new Sample(points[before], points[mid], beforeNormlize, delta);
                }
                else
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
}

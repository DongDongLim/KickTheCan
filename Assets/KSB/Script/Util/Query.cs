using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Query
{
    // 가장 가까운 거리의 오브젝트
    public GameObject NearestPoint(List<GameObject> list, GameObject target, int index = 0, bool isInverse = false)
    {
        //https://hijuworld.tistory.com/56
        var vertexQuery = isInverse ?
            from vertex in list
            where vertex != target
            orderby Vector3.SqrMagnitude(
                target.transform.position
                - vertex.transform.position) descending
            select vertex
                          :
                          from vertex in list
                          where vertex != target
                          orderby Vector3.SqrMagnitude(
                              target.transform.position
                              - vertex.transform.position)
                          select vertex;

        int count = 0;
        foreach (var vertex in vertexQuery)
        {
            if (count == index)
                return vertex;
            ++count;
        }
        return null;
    }
    public GameObject NearestPoint(List<GameObject> list, Vector3 target, int index = 0, bool isInverse = false)
    {
        //https://hijuworld.tistory.com/56
        var vertexQuery = isInverse ?
            from vertex in list
            orderby Vector3.SqrMagnitude(
                target
                - vertex.transform.position) descending
            select vertex
                          :
                          from vertex in list
                          orderby Vector3.SqrMagnitude(
                              target
                              - vertex.transform.position)
                          select vertex;

        int count = 0;
        foreach (var vertex in vertexQuery)
        {
            if (count == index)
                return vertex;
            ++count;
        }
        return null;
    }


    // 가장 작은 각도의 오브젝트
    public GameObject SmallestAngle(List<GameObject> list, GameObject target, GameObject comparison, int index = 0)
    {
        var vertexQuery = from vertex in list
                          where vertex != target
                          orderby Mathf.Abs((Mathf.Atan2((target.transform.position - comparison.transform.position).x
                          , (target.transform.position - comparison.transform.position).z) * Mathf.Rad2Deg)
             - (Mathf.Atan2((vertex.transform.position - comparison.transform.position).x
             , (vertex.transform.position - comparison.transform.position).z) * Mathf.Rad2Deg))
                          select vertex;
        int count = 0;
        foreach (var vertex in vertexQuery)
        {
            if (count == index)
                return vertex;
            ++count;
        }
        return null;
    }

    // 두 리스트가 모두 포함하는 오브젝트
    public GameObject IntersectObj(List<GameObject> list, List<GameObject> targetList)
    {
        var vertexQuery = from vertex in list.Intersect(targetList)
                          select vertex;
        foreach (var vertex in vertexQuery)
        {
            return vertex;
        }
        return null;
    }

    // 두 리스트가 모두 포함하는 오브젝트들
    public List<GameObject> IntersectObj(List<GameObject> list, List<GameObject> targetList, out List<GameObject> allList)
    {
        allList = new List<GameObject>();
        var vertexQuery = from vertex in list.Intersect(targetList)
                          select vertex;
        foreach (var vertex in vertexQuery)
        {
            allList.Add(vertex);
        }
        return allList;
    }

}

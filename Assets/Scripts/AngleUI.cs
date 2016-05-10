using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AngleUI
{
    List<GameObject> points;
    Vector2 position;
    int numberOfPoints;

    public AngleUI(Vector2 position, int numberOfPoints, float radius, Color color)
    {
        this.position = position;
        this.numberOfPoints = numberOfPoints;
        points = new List<GameObject>();
        for (var k = 0; k < numberOfPoints; k++)
        {
            var angle = Mathf.PI * 2 * k / numberOfPoints;
            var p = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius) + position;
            var go = Object.Instantiate(Resources.Load("LittleCircle"), p, Quaternion.identity) as GameObject;
            go.GetComponent<SpriteRenderer>().color = color;
            points.Add(go);
        }
        points[0].GetComponent<SpriteRenderer>().color = Color.black;
        points[numberOfPoints / 4].GetComponent<SpriteRenderer>().color = Color.black;
        points[numberOfPoints / 4 * 2].GetComponent<SpriteRenderer>().color = Color.black;
        points[numberOfPoints / 4 * 3].GetComponent<SpriteRenderer>().color = Color.black;
    }

    public void SetPosition(Vector2 position)
    {
        for (var k = 0; k < numberOfPoints; k++)
        {
            points[k].transform.position = points[k].transform.position + (Vector3)position - (Vector3)this.position;
        }
        this.position = position;
    }

    public void SetAngle(float angle)
    {
        foreach (var p in points)
        {
            p.SetActive(false);
        }
        for (var k = 0; k < numberOfPoints; k++)
        {
            var a = Mathf.PI * 2 * k / numberOfPoints;
            if (a < angle)
            {
                points[k].SetActive(true);
            }
        }
    }

    public void SetActive(bool value)
    {
        foreach (var p in points)
        {
            p.SetActive(value);
        }
    }

    public void Destroy()
    {
        foreach (var p in points)
        {
            Object.Destroy(p);
        }
    }

}

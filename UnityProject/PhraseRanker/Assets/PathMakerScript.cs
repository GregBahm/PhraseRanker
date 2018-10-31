using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMakerScript : MonoBehaviour
{
    public GameObject QuaterPathPrefab;
    public GameObject QuadPrefab;
    public bool Regenerate = true;
    public Transform TargetPoint;

    private List<GameObject> _components;

	void Start ()
    {
        _components = new List<GameObject>();
	}
	
	void Update ()
    {
		if(Regenerate)
        {
            Regenerate = false;
            Generate();
        }
	}

    private void Generate()
    {
        foreach (GameObject component in _components)
        {
            Destroy(component);
        }
        _components.Clear();

        GameObject quaterPath = Instantiate(QuaterPathPrefab);
        GameObject startQuad = Instantiate(QuadPrefab);
        GameObject endQuad = Instantiate(QuadPrefab);

        _components.Add(quaterPath);
        _components.Add(startQuad);
        _components.Add(endQuad);

        PlaceComponents(quaterPath.transform, startQuad.transform, endQuad.transform);
    }

    private void PlaceComponents(Transform quaterPath, Transform startQuad, Transform endQuad)
    {
        Vector3 pathStart = new Vector3(0, transform.position.y, 0);
        Vector3 quaterPathStart = new Vector3(0, 1, 0);
        Vector3 quaterPathEnd = new Vector3(0, 0, 1);
        Vector3 pathEnd = new Vector3(0, 0, TargetPoint.position.z);

        PlaceStartQuad(startQuad, pathStart, quaterPathStart);
        PlaceEndQuad(endQuad, quaterPathEnd, pathEnd);
    }

    private void PlaceEndQuad(Transform endQuad, Vector3 quaterPathEnd, Vector3 pathEnd)
    {
        endQuad.rotation = Quaternion.Euler(90, 0, 0);
        float length = (quaterPathEnd - pathEnd).magnitude;
        endQuad.localScale = new Vector3(1, length, 1);
        endQuad.position = (quaterPathEnd + pathEnd) / 2;
    }

    private void PlaceStartQuad(Transform startQuad, Vector3 pathStart, Vector3 quaterPathStart)
    {
        float length = (quaterPathStart - pathStart).magnitude;
        startQuad.localScale = new Vector3(1, length, 1);
        startQuad.position = (quaterPathStart + pathStart) / 2;
    }
}

using UnityEngine;

public class NPBehaveExampleMouseController : MonoBehaviour
{
    private Camera theCamera;

    public bool topDown = false;

    void Start()
    {
        theCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void Update()
    {
        Ray ray = theCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 pos = topDown ? RayCastXZ(ray) : RayCastXY(ray);
        this.transform.position = pos;
    }

    private Vector3 RayCastXY(Ray r)
    {
        float z = 0f;
        float delta = (z - r.origin.z) / r.direction.z;
        if (delta > 0.1f && delta < 10000f)
        {
            return r.origin + r.direction * delta;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private Vector3 RayCastXZ(Ray r)
    {
        float y = 0f;
        float delta = (y - r.origin.y) / r.direction.y;
        if (delta > 0.1f && delta < 10000f)
        {
            return r.origin + r.direction * delta;
        }
        else
        {
            return Vector3.zero;
        }
    }

}
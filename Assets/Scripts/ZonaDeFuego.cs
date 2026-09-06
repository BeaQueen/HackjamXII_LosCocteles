using UnityEngine;

public class ZonaDeFuego : MonoBehaviour
{
    [SerializeField] Transform xminTransform, xmaxTransform, yminTransform, ymaxTransform;
    public float xmin, xmax, ymin, ymax;
    // Start is called before the first frame update
    void Start()
    {
        xmin = xminTransform.position.x;
        xmax = xmaxTransform.position.x;
        ymin = yminTransform.position.z;
        ymax = ymaxTransform.position.z;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public float XMin()
    {
        return xmin;
    }
    public float XMax()
    {
        return xmax;
    }
    public float YMin()
    {
        return ymin;
    }
    public float YMax()
    {
        return ymax;
    }
   
}

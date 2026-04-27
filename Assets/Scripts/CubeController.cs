using UnityEngine;

public class CubeController : MonoBehaviour
{

    private bool isLit = false;

    private MeshRenderer r;

    public Material unlitTexture;
    public Material litTexture;
    public Material greenTexture;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        r = GetComponent<MeshRenderer>();

        updateMaterial();
    }

    public void updateMaterial()
    {
        if (isLit)
        {
            r.material = litTexture;
        } else
        {
            r.material = unlitTexture;
        }
    }

    public void turnGreen()
    {
        r.material = greenTexture;
    }

    public void turnOn()
    {
        isLit = true;
        updateMaterial();
    }

    public void turnOff()
    {
        isLit = false;
        updateMaterial();
    }

    public void toggle()
    {
        isLit = !isLit;
        updateMaterial();
    }

    public bool getIsLit()
    {
        return isLit;
    }

    public void setIsLit(bool b)
    {
        isLit = b;
        updateMaterial();
    }
}

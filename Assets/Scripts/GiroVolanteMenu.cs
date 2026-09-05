using UnityEngine;
using UnityEngine.InputSystem;

public class GiroVolanteMenu : MonoBehaviour
{
    Vector2 giro;
    [SerializeField] Transform volante;
    [SerializeField] float sensibilidad;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Volante(InputAction.CallbackContext context)
    {
        giro = context.ReadValue<Vector2>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        volante.eulerAngles = new Vector3(0, 0,- giro.x*sensibilidad);
    }
}

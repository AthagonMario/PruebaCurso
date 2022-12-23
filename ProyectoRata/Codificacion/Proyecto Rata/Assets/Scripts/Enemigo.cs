using UnityEngine;

public class Enemigo : MonoBehaviour
{
    [SerializeField] GameObject punto_a;
    [SerializeField] GameObject punto_b;

    Vector3? destino = null;

    [SerializeField]
    [Tooltip("True si debe cambiar de sentido, cuando retrocede")] bool GiroCambioSentido = true;

    [SerializeField]
    [Range(1, 8)]
    [Tooltip("Velocidad del enemigo")] int velocidad = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (GiroCambioSentido)
        {
            transform.rotation = destino == punto_a.transform.position
                ? Quaternion.Euler(0f, 180f, 0f)
                : Quaternion.Euler(0f, 0f, 0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Mueve un enemigo entre dos puntos, fotograma a fotograma
        transform.position = Vector3.MoveTowards(transform.position,
            destino ?? punto_b.transform.position,
            velocidad * Time.deltaTime);

        //Si el enemigo está muy próximo al punto de llegada, cambia de sentido y avanza al siguiente punto. El siguiente punto será el punto contrario
        if (Vector3.Distance(transform.position, destino ?? punto_b.transform.position) < 0.001f)
        {
            destino = destino == punto_a.transform.position
                ? punto_b.transform.position
                : punto_a.transform.position;

            //Giro del enemigo, al cambio de sentido
            if (GiroCambioSentido)
            {
                transform.rotation = destino == punto_a.transform.position
                    ? Quaternion.Euler(0f, 180f, 0f)
                    : Quaternion.Euler(0f, 0f, 0f);
            }
        }
    }

    //Muestra los gizmos, para saber dónde están los puntos entre los que se va mover el enemigo
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(punto_a.transform.position, .2f);
        Gizmos.DrawSphere(punto_b.transform.position, .2f);
    }
}

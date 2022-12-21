using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField][Tooltip("Nombre de la escena a la que se quiere cambiar")] string escena;

    //Texto de los marcadores
    [SerializeField] Text TextoMarcador;
    [SerializeField] Text TextoMarcadorMaximo;
    [SerializeField] Text TextoVidas;

    //Las variables deben ser declaradas est�ticas, para que se conserven durante la partida
    static int marcador = 0;
    static int marcador_maximo = 0;

    [SerializeField]
    [Tooltip("Vidas del personaje")]
    [Range(1f, 5)] static int vidasPersonaje = 3;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        //Necesario para conservar los valores de las escenas a lo largo del desarrollo del juego
        DontDestroyOnLoad(transform.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //M�todo an�nimo para actualizar el marcador, cuando ocurre un evento en el script de rata
        Rata.OnMarcador = (puntuacion) =>
        {
            marcador += puntuacion;
            TextoMarcador.text = $"Puntuaci�n: {marcador}";

            //Guarda la puntuaci�n m�xima
            if (marcador >= marcador_maximo)
            {
                marcador_maximo = marcador;
            }
        };

        //Reinicio del marcador
        Rata.OnResetMarcador = (puntuacion) =>
        {
            marcador = puntuacion;

            if (TextoMarcador != null)
            {
                TextoMarcador.text = $"Puntuaci�n: {marcador}";
            }
        };

        Rata.OnMuertePersonaje = () =>
        {
            vidasPersonaje--;

            if (vidasPersonaje == 0)
            {
                SceneManager.LoadScene("Inicio");

                escena = string.Empty;
            }

            TextoVidas.text = $"Vidas: {vidasPersonaje}";
        };

        if (TextoVidas != null)
        {
            TextoVidas.text = $"Vidas: {vidasPersonaje}";
        }

        if (TextoMarcador != null)
        {
            TextoMarcador.text = $"Puntuaci�n: {marcador}";
        }

        if (TextoMarcadorMaximo != null)
        {
            TextoMarcadorMaximo.text = $"Puntuaci�n m�xima: {marcador_maximo}";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Cambio:" + escena);
        CambioDeEscenario(escena);
    }

    /// <summary>
    /// Cambio de escenario
    /// </summary>
    /// <param name="escena_destino">Nombre del escenario</param>
    public void CambioDeEscenario(string escena_destino)
    {
        if (!string.IsNullOrEmpty(escena_destino))
        {
            SceneManager.LoadScene(escena_destino);

            escena = string.Empty;
        }
        else
        {
            Debug.LogWarning("No se ha especificado ninguna escena a la que navegar");
        }
    }

    /// <summary>
    /// Gesti�n de la muerte del personaje y reaparecer del mismo
    /// </summary>
    /// <param name="personaje">Posici�n del personaje</param>
    /// <param name="origen">Origen en el que va a reaparecer el personaje</param>
    public void Reaparecer(Transform personaje, Vector3 origen)
    {
        //Desaparece, asigna el origen y vuelve a aparecer
        personaje.gameObject.GetComponent<CharacterController>().enabled = false;
        personaje.position = origen;
        personaje.gameObject.GetComponent<CharacterController>().enabled = true;
    }
}

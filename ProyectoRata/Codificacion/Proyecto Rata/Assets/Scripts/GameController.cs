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

    //Las variables deben ser declaradas estáticas, para que se conserven durante la partida
    static int marcador = 0;
    static int marcador_maximo = 0;

    [SerializeField]
    [Tooltip("Vidas del personaje")]
    [Range(1f, 10)] static int vidasPersonaje = 3;

    AudioSource musica_juego;

    [SerializeField]
    [Tooltip("Sonido a lo largo del juego")] bool SonidoJuego = true;

    private void Awake()
    {
        //Máximo número de frames en el juego
        Application.targetFrameRate = 60;

        //Necesario para conservar los valores de las escenas a lo largo del desarrollo del juego
        DontDestroyOnLoad(transform.gameObject);

        musica_juego = GetComponent<AudioSource>();

        MusicaDelJuego();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Método anónimo para actualizar el marcador, cuando ocurre un evento en el script de rata
        Rata.OnMarcador = (puntuacion) =>
        {
            marcador += puntuacion;
            TextoMarcador.text = $"Puntuación: {marcador}";

            //Guarda la puntuación máxima
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
                TextoMarcador.text = $"Puntuación: {marcador}";
            }
        };

        //Gestión de vidas del personaje
        Rata.OnMuertePersonaje = () =>
        {
            vidasPersonaje--;

            //Si llega a cero vidas, vuelve a la pantalla de inicio y se reinician las vidas
            if (vidasPersonaje == 0)
            {
                vidasPersonaje = 3;

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
            TextoMarcador.text = $"Puntuación: {marcador}";
        }

        if (TextoMarcadorMaximo != null)
        {
            TextoMarcadorMaximo.text = $"Puntuación máxima: {marcador_maximo}";
        }
    }

    /// <summary>
    /// Establece la música a lo largo del juego
    /// </summary>
    public void MusicaDelJuego()
    {
        if (musica_juego != null)
        {
            SonidoJuego = !SonidoJuego;

            if (SonidoJuego)
            {
                musica_juego.Play();
            }
            else
            {
                musica_juego.Stop();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
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

            if (musica_juego != null)
            {
                if (escena == "Inicio")
                {
                    musica_juego.Play();
                }
                else
                {
                    musica_juego.Stop();
                }
            }

            escena = string.Empty;
        }
        else
        {
            Debug.LogWarning("No se ha especificado ninguna escena a la que navegar");
        }
    }

    /// <summary>
    /// Gestión de la muerte del personaje y reaparecer del mismo
    /// </summary>
    /// <param name="personaje">Posición del personaje</param>
    /// <param name="origen">Origen en el que va a reaparecer el personaje</param>
    public void Reaparecer(Transform personaje, Vector3 origen)
    {
        //Desaparece, asigna el origen y vuelve a aparecer
        personaje.gameObject.GetComponent<CharacterController>().enabled = false;
        personaje.position = origen;
        personaje.gameObject.GetComponent<CharacterController>().enabled = true;
    }

    /// <summary>
    /// Salida del juego
    /// </summary>
    public static void Salir()
    {
        Application.Quit();
    }
}
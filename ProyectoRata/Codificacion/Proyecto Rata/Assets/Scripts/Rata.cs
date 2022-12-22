using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Rata. Personaje, protagonista del juego
/// </summary>
public class Rata : MonoBehaviour
{
    //Delegado para acceder a eventos
    public delegate void Marcador(int puntuacion);
    public static Marcador OnMarcador;
    public static Marcador OnResetMarcador;

    //Delegado para gestionar el marcador de vidas
    public delegate void MuertePersonaje();
    public static MuertePersonaje OnMuertePersonaje;

    //Vector para crear el movimiento del personaje
    Vector3 movimiento = Vector3.zero;

    //Origen de coordenadas de inicio del personaje
    Vector3 origen;

    [SerializeField][Tooltip("Velocidad del personaje")] float velocidad = 2;

    //Aceleración del salto
    float aceleracionSalto = 0.0f;

    //Gravedad
    float aceleracionGravedad = -10.0f;

    [SerializeField][Tooltip("Altura máxima del salto")] float alturaMaxima = 2f;

    [SerializeField][Tooltip("Tiempo máximo de vuelo en el salto")] float tiempoMaximoVuelo = .5f;

    //Se sustituye el Collider original de la cápsula, por Charcter Controller 
    CharacterController characterController;

    //GameController del juego
    GameController gameController;

    //Controla el instante del salto en el trigger de muerte del enemigo
    bool instante_salto = false;

    //Audios que interactuan con el personaje
    /*
     * 0 - Salto
     * 1 - Muerte
     * 2 - MuerteEnemigo
     */
    AudioSource[] audioSource;

    [SerializeField] Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //Búsqueda del GameController por el tag
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();

        //Objetos de audio
        audioSource = gameObject.GetComponents<AudioSource>();

        //Obtener el CharacterControler del personaje
        characterController = GetComponent<CharacterController>();

        //Origen del personaje al comienzo del script
        origen = characterController.transform.position;

        aceleracionGravedad = InicializacionGravedad();
        aceleracionSalto = InicializacionSalto();

        MuerteEnemigo.OnMuerteDelEnemigo = (game) =>
        {
            audioSource[2].Play();

            //Destruye el gameObject de un enemigo
            Destroy(game.transform.parent.transform.parent.gameObject);

            //Hace que en el update salte el personaje, cuando mata un enemigo
            instante_salto = true;

            //Asigno 100 puntos al marcador
            OnMarcador.Invoke(100);
        };

        //Pone a cero el marcado al inicio de una nueva partida
        if (SceneManager.GetActiveScene().name == "Escena_1")
        {
            OnResetMarcador.Invoke(0);
        }
    }

    /// <summary>
    /// Inicialización de la gravedad
    /// </summary>
    /// <returns>Valor de la gravedad</returns>
    private float InicializacionGravedad()
    {
        return -8 * alturaMaxima / Mathf.Sqrt(tiempoMaximoVuelo);
    }

    /// <summary>
    /// Inicialización del valor del saltp
    /// </summary>
    /// <returns>Valor del salto</returns>
    private float InicializacionSalto()
    {
        return Mathf.Sqrt(-2f * alturaMaxima * aceleracionGravedad);
    }

    // Update is called once per frame
    void Update()
    {
        //La aceleración del salto será 0 al estar en el suelo
        if (characterController.isGrounded)
        {
            aceleracionSalto = 0;

            animator.SetTrigger("cayendo");
        }

        //Salto del persnaje a los eventos de los controles de juego
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetButtonDown("Fire1"))
            && characterController.isGrounded)
        {
            Salto();
        }

        //Activa el salto, a la orden del trigger de muerte del enemigo
        if (instante_salto)
        {
            Salto();

            instante_salto = false;
        }

        //Consecución del salto
        aceleracionSalto += aceleracionGravedad * Time.deltaTime;
        movimiento.y = aceleracionSalto * Time.deltaTime;

        //Orientación del personaje, con respecto del movimiento
        if ((Input.GetAxis("Horizontal") > 0) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if ((Input.GetAxis("Horizontal") < 0) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0f, -180f, 0f);
        }

        //Asignación del valor x al movimiento, en función de los cursores o del gamepad
        movimiento.x = (Input.GetKey(KeyCode.LeftArrow) ? 1 : Input.GetKey(KeyCode.RightArrow) ? -1 : 0) * velocidad;
        movimiento.x = Input.GetAxis("Horizontal") * velocidad * Time.deltaTime;
        animator.SetBool("andando", movimiento.x != 0 ? true : false);

        //Movimiento del personaje
        characterController.Move(movimiento);
    }

    /// <summary>
    /// Salto del personaje
    /// </summary>
    private void Salto()
    {
        //Activa el audio a cada salto
        audioSource[0].Play();

        //aceleracionGravedad = InicializacionGravedad();
        aceleracionSalto = InicializacionSalto();

        animator.SetTrigger("saltando");
    }

    /// <summary>
    /// Muerte del personaje
    /// </summary>
    public void Muerte()
    {
        audioSource[1].Play();

        OnMuertePersonaje.Invoke();

        //Vuelve al inicio
        gameObject.transform.position = origen;
        gameController.Reaparecer(gameObject.transform, origen);
    }
}

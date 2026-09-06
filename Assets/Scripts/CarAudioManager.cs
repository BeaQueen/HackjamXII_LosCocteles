using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarAudioManager : MonoBehaviour
{
    [System.Serializable]
    public class CarAudioProfile
    {
        public string carName;

        [Header("Car")]
        public GameObject carRoot;

        [Header("Unique Sounds")]
        public AudioClip startupClip;
        public AudioClip engineLoopClip;
        public AudioClip accelerationClip;
    }


    // =========================================================
    // CARS
    // =========================================================

    [Header("Cars")]
    [SerializeField] private CarAudioProfile[] cars;


    // =========================================================
    // GENERIC SOUNDS
    // =========================================================

    [Header("Generic Sounds")]
    [SerializeField] private AudioClip brakeClip;
    [SerializeField] private AudioClip skidClip;
    [SerializeField] private AudioClip crashClip;


    // =========================================================
    // INPUT
    // =========================================================

    [Header("Wheel Input")]
    [SerializeField] private InputActionReference brakeAction;
    [SerializeField] private InputActionReference steeringAction;


    // =========================================================
    // STARTUP
    // =========================================================

    [Header("Startup")]

    [Tooltip("Tiempo desde que empieza el arranque hasta que entra el motor.")]
    [SerializeField] private float engineStartDelay = 0.15f;


    // =========================================================
    // ENGINE
    // =========================================================

    [Header("Engine RPM")]

    [SerializeField] private float maxRevolutions = 10f;

    [SerializeField] private float idlePitch = 0.75f;
    [SerializeField] private float maxEnginePitch = 1.6f;

    [SerializeField] private float idleVolume = 0.35f;
    [SerializeField] private float maxEngineVolume = 0.85f;

    [SerializeField] private float engineSmoothness = 5f;


    // =========================================================
    // ACCELERATION
    // =========================================================

    [Header("Acceleration")]
    [SerializeField] private float accelerationVolume = 0.7f;


    // =========================================================
    // BRAKE
    // =========================================================

    [Header("Brake")]
    [SerializeField] private float brakeThreshold = 0.2f;
    [SerializeField] private float brakeVolume = 0.8f;


    // =========================================================
    // SKID
    // =========================================================

    [Header("Skid")]
    [SerializeField] private float skidSteeringThreshold = 0.3f;
    [SerializeField] private float minimumSkidRevolutions = 1f;
    [SerializeField] private float skidVolume = 0.8f;


    // =========================================================
    // CRASH
    // =========================================================

    [Header("Crash")]
    [SerializeField] private float collisionResetDelay = 0.15f;
    [SerializeField] private float minimumCrashInterval = 0.1f;


    // =========================================================
    // AUDIO SOURCES
    // =========================================================

    private AudioSource startupSource;
    private AudioSource engineSource;
    private AudioSource accelerationSource;
    private AudioSource brakeSource;
    private AudioSource skidSource;
    private AudioSource crashSource;


    // =========================================================
    // ACTIVE CAR
    // =========================================================

    private CarAudioProfile activeProfile;
    private Transform activeCar;
    private LogicaConductor conductor;


    // =========================================================
    // INTERNAL
    // =========================================================

    private bool brakeReady = false;
    private bool wasBraking = false;

    private GameObject lastCollisionObject;

    private float lastCollisionContactTime = -100f;
    private float lastCrashSoundTime = -100f;

    private Coroutine startupCoroutine;


    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        CreateAudioSources();
    }


    private void OnEnable()
    {
        if (brakeAction != null)
            brakeAction.action.Enable();

        if (steeringAction != null)
            steeringAction.action.Enable();
    }


    private void OnDisable()
    {
        if (brakeAction != null)
            brakeAction.action.Disable();

        if (steeringAction != null)
            steeringAction.action.Disable();
    }


    private void Start()
    {
        CheckForCarChange();
    }


    private void Update()
    {
        // IMPORTANTE:
        // comprobamos continuamente si el modelo activo ha cambiado.
        CheckForCarChange();

        if (activeProfile == null || conductor == null)
            return;

        UpdateEngine();
        UpdateAcceleration();
        UpdateBrake();
        UpdateSkid();
    }


    // =========================================================
    // CAR CHANGE
    // =========================================================

    private void CheckForCarChange()
    {
        CarAudioProfile detectedProfile = null;

        foreach (CarAudioProfile profile in cars)
        {
            if (
                profile.carRoot != null &&
                profile.carRoot.activeInHierarchy
            )
            {
                detectedProfile = profile;
                break;
            }
        }

        // No hay ningún coche activo
        if (detectedProfile == null)
            return;


        // Es el mismo que ya tenemos
        if (detectedProfile == activeProfile)
            return;


        // HA CAMBIADO EL COCHE
        ChangeCarAudio(detectedProfile);
    }


    private void ChangeCarAudio(CarAudioProfile newProfile)
    {
        Debug.Log(
            "CarAudioManager: CAMBIO DE COCHE -> "
            + newProfile.carName
        );


        // Detenemos cualquier coroutine de arranque anterior

        if (startupCoroutine != null)
        {
            StopCoroutine(startupCoroutine);
            startupCoroutine = null;
        }


        // Apagamos todos los sonidos del coche anterior

        StopAllCarSounds();


        // Guardamos el nuevo coche

        activeProfile = newProfile;
        activeCar = newProfile.carRoot.transform;


        // Buscamos nuevamente LogicaConductor

        FindConductor();


        // Reseteamos estados de audio

        brakeReady = false;
        wasBraking = false;

        lastCollisionObject = null;


        // Arrancamos los sonidos del nuevo coche

        startupCoroutine =
            StartCoroutine(StartCarAudio());
    }


    private void StopAllCarSounds()
    {
        startupSource.Stop();
        engineSource.Stop();
        accelerationSource.Stop();
        brakeSource.Stop();
        skidSource.Stop();

        // No es imprescindible parar el crash,
        // pero evitamos que un golpe antiguo siga sonando.
        crashSource.Stop();
    }


    // =========================================================
    // FIND CONDUCTOR
    // =========================================================

    private void FindConductor()
    {
        conductor = null;

        if (activeCar == null)
            return;


        conductor =
            activeCar.GetComponent<LogicaConductor>();


        if (conductor == null)
        {
            conductor =
                activeCar.GetComponentInChildren<LogicaConductor>();
        }


        if (conductor == null)
        {
            conductor =
                activeCar.GetComponentInParent<LogicaConductor>();
        }


        if (conductor == null)
        {
            Debug.LogError(
                "CarAudioManager: no se encuentra LogicaConductor para "
                + activeProfile.carName
            );
        }
    }


    // =========================================================
    // STARTUP
    // =========================================================

    private IEnumerator StartCarAudio()
    {
        if (
            activeProfile != null &&
            activeProfile.startupClip != null
        )
        {
            startupSource.clip =
                activeProfile.startupClip;

            startupSource.loop = false;

            startupSource.Play();


            float delay =
                Mathf.Min(
                    engineStartDelay,
                    activeProfile.startupClip.length
                );


            yield return new WaitForSeconds(delay);
        }


        StartEngineLoop();

        startupCoroutine = null;
    }


    // =========================================================
    // ENGINE
    // =========================================================

    private void StartEngineLoop()
    {
        if (
            activeProfile == null ||
            activeProfile.engineLoopClip == null
        )
            return;


        engineSource.Stop();

        engineSource.clip =
            activeProfile.engineLoopClip;

        engineSource.loop = true;

        engineSource.pitch =
            idlePitch;

        engineSource.volume =
            idleVolume;

        engineSource.Play();


        Debug.Log(
            "Motor activo: "
            + activeProfile.engineLoopClip.name
        );
    }


    private void UpdateEngine()
    {
        if (!engineSource.isPlaying)
            return;


        float normalizedRPM =
            Mathf.Clamp01(
                conductor.revoluciones /
                maxRevolutions
            );


        float targetPitch =
            Mathf.Lerp(
                idlePitch,
                maxEnginePitch,
                normalizedRPM
            );


        float targetVolume =
            Mathf.Lerp(
                idleVolume,
                maxEngineVolume,
                normalizedRPM
            );


        engineSource.pitch =
            Mathf.Lerp(
                engineSource.pitch,
                targetPitch,
                Time.deltaTime * engineSmoothness
            );


        engineSource.volume =
            Mathf.Lerp(
                engineSource.volume,
                targetVolume,
                Time.deltaTime * engineSmoothness
            );
    }


    // =========================================================
    // ACCELERATION
    // =========================================================

    private void UpdateAcceleration()
    {
        bool accelerating =
            conductor.acelerando;


        if (accelerating)
        {
            StartAcceleration();
        }
        else
        {
            StopAcceleration();
        }
    }


    private void StartAcceleration()
    {
        if (
            activeProfile == null ||
            activeProfile.accelerationClip == null
        )
            return;


        if (accelerationSource.isPlaying)
            return;


        accelerationSource.clip =
            activeProfile.accelerationClip;

        accelerationSource.loop = true;

        accelerationSource.volume =
            accelerationVolume;

        accelerationSource.Play();
    }


    private void StopAcceleration()
    {
        if (accelerationSource.isPlaying)
        {
            accelerationSource.Stop();
        }
    }


    // =========================================================
    // BRAKE
    // =========================================================

    private void UpdateBrake()
    {
        if (brakeAction == null)
            return;


        float rawBrake =
            brakeAction.action.ReadValue<float>();


        // Primero esperamos a detectar
        // el pedal completamente suelto.

        if (!brakeReady)
        {
            if (rawBrake >= 0.95f)
            {
                brakeReady = true;
            }

            return;
        }


        float brakeAmount =
            Mathf.Clamp01(
                1f - rawBrake
            );


        bool braking =
            brakeAmount >
            brakeThreshold;


        // El freno solo suena AL PULSAR,
        // no mientras se mantiene.

        if (
            braking &&
            !wasBraking
        )
        {
            PlayBrake();
        }


        wasBraking =
            braking;
    }


    private void PlayBrake()
    {
        if (brakeClip == null)
            return;


        brakeSource.loop = false;

        brakeSource.PlayOneShot(
            brakeClip,
            brakeVolume
        );
    }


    // =========================================================
    // SKID
    // =========================================================

    private void UpdateSkid()
    {
        if (steeringAction == null)
            return;


        Vector2 steering =
            steeringAction.action.ReadValue<Vector2>();


        float horizontalSteering =
            Mathf.Abs(
                steering.x
            );


        bool enoughSteering =
            horizontalSteering >
            skidSteeringThreshold;


        bool enoughSpeed =
            conductor.revoluciones >
            minimumSkidRevolutions;


        bool shouldSkid =
            enoughSteering &&
            enoughSpeed;


        if (shouldSkid)
        {
            StartSkid();
        }
        else
        {
            StopSkid();
        }
    }


    private void StartSkid()
    {
        if (skidClip == null)
            return;


        if (skidSource.isPlaying)
            return;


        skidSource.clip =
            skidClip;

        skidSource.loop = true;

        skidSource.volume =
            skidVolume;

        skidSource.Play();
    }


    private void StopSkid()
    {
        if (skidSource.isPlaying)
        {
            skidSource.Stop();
        }
    }


    // =========================================================
    // COLLISION
    // =========================================================

    public void NotifyCollision(GameObject hitObject)
    {
        if (hitObject == null)
            return;


        bool differentObject =
            hitObject != lastCollisionObject;


        bool contactWasLost =
            Time.time -
            lastCollisionContactTime >
            collisionResetDelay;


        lastCollisionContactTime =
            Time.time;


        if (
            !differentObject &&
            !contactWasLost
        )
        {
            return;
        }


        lastCollisionObject =
            hitObject;


        if (
            Time.time -
            lastCrashSoundTime <
            minimumCrashInterval
        )
        {
            return;
        }


        PlayCrash();

        lastCrashSoundTime =
            Time.time;
    }


    private void PlayCrash()
    {
        if (crashClip == null)
            return;


        crashSource.loop = false;

        crashSource.PlayOneShot(
            crashClip
        );
    }


    // =========================================================
    // AUDIO SOURCES
    // =========================================================

    private void CreateAudioSources()
    {
        startupSource =
            CreateAudioSource();

        engineSource =
            CreateAudioSource();

        accelerationSource =
            CreateAudioSource();

        brakeSource =
            CreateAudioSource();

        skidSource =
            CreateAudioSource();

        crashSource =
            CreateAudioSource();
    }


    private AudioSource CreateAudioSource()
    {
        AudioSource source =
            gameObject.AddComponent<AudioSource>();

        source.playOnAwake = false;

        source.spatialBlend = 0f;

        return source;
    }
}
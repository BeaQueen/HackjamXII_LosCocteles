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

    [Header("Cars")]
    [SerializeField] private CarAudioProfile[] cars;

    [Header("Generic Sounds")]
    [SerializeField] private AudioClip brakeClip;
    [SerializeField] private AudioClip skidClip;
    [SerializeField] private AudioClip crashClip;

    [Header("Wheel Input")]
    [SerializeField] private InputActionReference brakeAction;
    [SerializeField] private InputActionReference steeringAction;

    [Header("Engine RPM")]
    [SerializeField] private float maxRevolutions = 10f;

    [Tooltip("Pitch del motor cuando está al ralentí")]
    [SerializeField] private float idlePitch = 0.75f;

    [Tooltip("Pitch del motor a máximas revoluciones")]
    [SerializeField] private float maxEnginePitch = 1.6f;

    [Tooltip("Volumen del motor al ralentí")]
    [SerializeField] private float idleVolume = 0.35f;

    [Tooltip("Volumen del motor a máximas revoluciones")]
    [SerializeField] private float maxEngineVolume = 0.85f;

    [Tooltip("Suavidad con la que cambia el sonido del motor")]
    [SerializeField] private float engineSmoothness = 5f;

    [Header("Brake")]
    [SerializeField] private float brakeThreshold = 0.2f;

    [Header("Skid")]
    [Tooltip("Giro mínimo del volante para considerar derrape")]
    [SerializeField] private float skidSteeringThreshold = 0.3f;

    [Tooltip("Revoluciones mínimas para que pueda sonar el derrape")]
    [SerializeField] private float minimumSkidRevolutions = 1f;

    [SerializeField] private float skidVolume = 0.8f;

    [Header("Crash")]
    [Tooltip("Caída brusca de velocidad necesaria para detectar un choque")]
    [SerializeField] private float crashSpeedDropThreshold = 7f;

    [Tooltip("Tiempo mínimo entre sonidos de choque")]
    [SerializeField] private float crashCooldown = 0.5f;

    private AudioSource startupSource;
    private AudioSource engineSource;
    private AudioSource accelerationSource;
    private AudioSource brakeSource;
    private AudioSource skidSource;
    private AudioSource crashSource;

    private CarAudioProfile activeProfile;
    private Transform activeCar;
    private LogicaConductor conductor;

    private Vector3 previousPosition;
    private float previousSpeed;
    private float lastCrashTime = -100f;

    private bool wasAccelerating = false;
    private bool wasBraking = false;

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
        FindActiveCar();

        if (activeCar == null)
        {
            Debug.LogError(
                "CarAudioManager: No se ha encontrado ningún coche activo."
            );

            return;
        }

        FindConductor();

        if (conductor == null)
            return;

        previousPosition = activeCar.position;
        previousSpeed = 0f;

        StartCoroutine(StartCarAudio());
    }

    private void Update()
    {
        if (activeCar == null || conductor == null)
            return;

        UpdateEngine();
        UpdateAcceleration();
        UpdateBrake();
        UpdateSkid();
        UpdateCrashDetection();

        previousPosition = activeCar.position;
    }

    private void FindActiveCar()
    {
        foreach (CarAudioProfile profile in cars)
        {
            if (
                profile.carRoot != null &&
                profile.carRoot.activeInHierarchy
            )
            {
                activeProfile = profile;
                activeCar = profile.carRoot.transform;

                Debug.Log(
                    "CarAudioManager: coche activo = "
                    + profile.carName
                );

                return;
            }
        }
    }

    private void FindConductor()
    {
        conductor = activeCar.GetComponent<LogicaConductor>();

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
                "CarAudioManager: No se ha encontrado LogicaConductor en el coche activo."
            );
        }
    }

    private IEnumerator StartCarAudio()
    {
        if (
            activeProfile != null &&
            activeProfile.startupClip != null
        )
        {
            startupSource.clip =
                activeProfile.startupClip;

            startupSource.Play();

            yield return new WaitForSeconds(
                activeProfile.startupClip.length
            );
        }

        StartEngineLoop();
    }

    private void StartEngineLoop()
    {
        if (
            activeProfile == null ||
            activeProfile.engineLoopClip == null
        )
            return;

        engineSource.clip =
            activeProfile.engineLoopClip;

        engineSource.loop = true;
        engineSource.pitch = idlePitch;
        engineSource.volume = idleVolume;

        engineSource.Play();
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

    private void UpdateAcceleration()
    {
        bool accelerating = conductor.acelerando;

        if (
            accelerating &&
            !wasAccelerating
        )
        {
            if (
                activeProfile != null &&
                activeProfile.accelerationClip != null
            )
            {
                accelerationSource.PlayOneShot(
                    activeProfile.accelerationClip
                );
            }
        }

        wasAccelerating = accelerating;
    }

    private void UpdateBrake()
    {
        if (brakeAction == null)
            return;

        float rawBrake =
            brakeAction.action.ReadValue<float>();

        // En vuestro código:
        // freno == 1 -> no está pulsado
        // freno != 1 -> está frenando
        float brakeAmount =
            Mathf.Clamp01(1f - rawBrake);

        bool braking =
            brakeAmount > brakeThreshold;

        if (
            braking &&
            !wasBraking
        )
        {
            if (brakeClip != null)
            {
                brakeSource.PlayOneShot(brakeClip);
            }
        }

        wasBraking = braking;
    }

    private void UpdateSkid()
    {
        if (steeringAction == null)
            return;

        Vector2 steering =
            steeringAction.action.ReadValue<Vector2>();

        float horizontalSteering =
            Mathf.Abs(steering.x);

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

        skidSource.clip = skidClip;
        skidSource.loop = true;
        skidSource.volume = skidVolume;

        skidSource.Play();
    }

    private void StopSkid()
    {
        if (skidSource.isPlaying)
        {
            skidSource.Stop();
        }
    }

    private void UpdateCrashDetection()
    {
        if (Time.deltaTime <= 0f)
            return;

        Vector3 movement =
            activeCar.position -
            previousPosition;

        float currentSpeed =
            movement.magnitude /
            Time.deltaTime;

        float speedDrop =
            previousSpeed -
            currentSpeed;

        bool bigSpeedDrop =
            speedDrop >
            crashSpeedDropThreshold;

        bool cooldownFinished =
            Time.time - lastCrashTime >
            crashCooldown;

        if (
            bigSpeedDrop &&
            cooldownFinished
        )
        {
            PlayCrash();

            lastCrashTime = Time.time;
        }

        previousSpeed = currentSpeed;
    }

    private void PlayCrash()
    {
        if (crashClip == null)
            return;

        crashSource.PlayOneShot(crashClip);
    }

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
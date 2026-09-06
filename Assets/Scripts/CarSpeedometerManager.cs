using UnityEngine;
using UnityEngine.UI;

public class CarSpeedometerManager : MonoBehaviour
{
    public enum SpeedometerType
    {
        FilledImage,
        Needle
    }

    [System.Serializable]
    public class CarSpeedometerProfile
    {
        [Header("Car")]
        public string carName;
        public GameObject carRoot;

        [Header("Speedometer")]
        public GameObject speedometerRoot;
        public SpeedometerType speedometerType;

        [Header("Filled Image")]
        public Image fillImage;

        [Header("Needle")]
        public RectTransform needle;
    }


    // =========================================================
    // CARS
    // =========================================================

    [Header("Cars")]
    [SerializeField] private CarSpeedometerProfile[] cars;


    // =========================================================
    // RPM
    // =========================================================

    [Header("RPM")]
    [SerializeField] private float maxRevolutions = 10f;

    [Tooltip("Suavidad del movimiento del velocímetro")]
    [SerializeField] private float smoothness = 8f;


    // =========================================================
    // NEEDLE
    // =========================================================

    [Header("Needle Rotation")]
    [SerializeField] private float minimumNeedleAngle = 217.9f;
    [SerializeField] private float maximumNeedleAngle = 315.6f;


    // =========================================================
    // INTERNAL
    // =========================================================

    private CarSpeedometerProfile activeProfile;
    private LogicaConductor conductor;

    private float displayedRPM = 0f;


    // =========================================================
    // UNITY
    // =========================================================

    private void Start()
    {
        FindActiveCar();
    }


    private void Update()
    {
        // Por si cambia el coche activo,
        // comprobamos si sigue siendo el mismo.
        CheckActiveCar();

        if (activeProfile == null || conductor == null)
            return;

        UpdateSpeedometer();
    }


    // =========================================================
    // FIND ACTIVE CAR
    // =========================================================

    private void FindActiveCar()
    {
        // Primero apagamos todos los velocímetros
        foreach (CarSpeedometerProfile profile in cars)
        {
            if (profile.speedometerRoot != null)
            {
                profile.speedometerRoot.SetActive(false);
            }
        }

        // Buscamos el coche que esté activo
        foreach (CarSpeedometerProfile profile in cars)
        {
            if (
                profile.carRoot != null &&
                profile.carRoot.activeInHierarchy
            )
            {
                SetActiveProfile(profile);
                return;
            }
        }

        Debug.LogWarning(
            "CarSpeedometerManager: No se ha encontrado ningún coche activo."
        );
    }


    private void CheckActiveCar()
    {
        // Si el coche actual sigue activo, no hacemos nada
        if (
            activeProfile != null &&
            activeProfile.carRoot != null &&
            activeProfile.carRoot.activeInHierarchy
        )
        {
            return;
        }

        // Si ha cambiado, buscamos de nuevo
        FindActiveCar();
    }


    private void SetActiveProfile(
        CarSpeedometerProfile newProfile)
    {
        // Apagamos todos
        foreach (CarSpeedometerProfile profile in cars)
        {
            if (profile.speedometerRoot != null)
            {
                profile.speedometerRoot.SetActive(false);
            }
        }

        activeProfile = newProfile;

        // Activamos solo el correspondiente al coche
        if (activeProfile.speedometerRoot != null)
        {
            activeProfile.speedometerRoot.SetActive(true);
        }

        FindConductor();

        displayedRPM = 0f;

        Debug.Log(
            "CarSpeedometerManager: velocímetro activo = "
            + activeProfile.carName
        );
    }


    // =========================================================
    // FIND CONDUCTOR
    // =========================================================

    private void FindConductor()
    {
        conductor = null;

        if (activeProfile.carRoot == null)
            return;

        conductor =
            activeProfile.carRoot.GetComponent<LogicaConductor>();

        if (conductor == null)
        {
            conductor =
                activeProfile.carRoot
                    .GetComponentInChildren<LogicaConductor>();
        }

        if (conductor == null)
        {
            conductor =
                activeProfile.carRoot
                    .GetComponentInParent<LogicaConductor>();
        }

        if (conductor == null)
        {
            Debug.LogError(
                "CarSpeedometerManager: No se ha encontrado LogicaConductor para "
                + activeProfile.carName
            );
        }
    }


    // =========================================================
    // SPEEDOMETER
    // =========================================================

    private void UpdateSpeedometer()
    {
        float targetRPM =
            Mathf.Clamp01(
                conductor.revoluciones /
                maxRevolutions
            );

        // Suavizamos para que no tiemble
        displayedRPM =
            Mathf.Lerp(
                displayedRPM,
                targetRPM,
                Time.deltaTime * smoothness
            );

        switch (activeProfile.speedometerType)
        {
            case SpeedometerType.FilledImage:
                UpdateFilledImage();
                break;

            case SpeedometerType.Needle:
                UpdateNeedle();
                break;
        }
    }


    // =========================================================
    // FILLED IMAGE
    // =========================================================

    private void UpdateFilledImage()
    {
        if (activeProfile.fillImage == null)
            return;

        activeProfile.fillImage.fillAmount =
            displayedRPM;
    }


    // =========================================================
    // NEEDLE
    // =========================================================

    private void UpdateNeedle()
    {
        if (activeProfile.needle == null)
            return;

        float angle =
            Mathf.Lerp(
                minimumNeedleAngle,
                maximumNeedleAngle,
                displayedRPM
            );

        Vector3 rotation =
            activeProfile.needle.localEulerAngles;

        rotation.z = angle;

        activeProfile.needle.localEulerAngles =
            rotation;
    }
}
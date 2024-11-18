using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CicloDiaNoche : MonoBehaviour
{
    [Header("Configuraciones del Ciclo")]
    [Tooltip("Duración de un día completo en segundos")]
    public float dayDuration = 10f; // Duración del día en segundos
    public Transform sunTransform;  // El objeto que representa al Sol
    public Light sunLight;          // La luz del Sol

    [Header("Configuraciones de Luz")]
    [Tooltip("Intensidad de la luz al mediodía")]
    public float maxSunIntensity = 1.2f;
    [Tooltip("Intensidad de la luz al anochecer")]
    public float minSunIntensity = 0.2f;

    [Header("Colores de cielo")]
    public Color dayColor = Color.cyan; // Color del cielo durante el día
    public Color nightColor = Color.black; // Color del cielo durante la noche

    private float time; // Tiempo interno para el ciclo

    void Update()
    {
        // Avanzar el tiempo
        time += Time.deltaTime / dayDuration;

        // Mantener el tiempo en un rango de 0 a 1
        time %= 1f;

        // Rotar el Sol según el tiempo
        float sunRotation = time * 360f; // Convertir tiempo a grados (0-360)
        sunTransform.rotation = Quaternion.Euler(sunRotation - 90f, 170f, 0f);

        // Ajustar la intensidad de la luz según la posición del Sol
        sunLight.intensity = Mathf.Lerp(minSunIntensity, maxSunIntensity, Mathf.Clamp01(Mathf.Cos(time * Mathf.PI * 2)));

        // Cambiar el color del cielo
        RenderSettings.ambientLight = Color.Lerp(nightColor, dayColor, Mathf.Clamp01(Mathf.Cos(time * Mathf.PI * 2)));
    }
}

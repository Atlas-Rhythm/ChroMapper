using UnityEngine;

public class RotationCompassController : MonoBehaviour
{
    [SerializeField] private RotationCallbackController rotationCallback;
    [SerializeField] private Transform arrowTransform;

    private int currentRotation = 0;

    void Start()
    {
        gameObject.SetActive(rotationCallback.IsActive);
        rotationCallback.RotationChangedEvent += RotationChanged;
    }

    void FixedUpdate()
    {
        arrowTransform.localRotation = Quaternion.Lerp(arrowTransform.localRotation, Quaternion.Euler(0, 0, -currentRotation), 0.1f);
    }

    private void RotationChanged(bool natural, int rotation)
    {
        currentRotation = rotation;
    }
    
    private void OnDestroy()
    {
        rotationCallback.RotationChangedEvent -= RotationChanged;
    }
}

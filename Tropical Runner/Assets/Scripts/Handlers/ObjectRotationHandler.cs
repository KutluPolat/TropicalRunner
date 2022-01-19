using UnityEngine;
using Kupinteractive.TropicalRunner.Enums;

public class ObjectRotationHandler : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private float _rotationSpeed = 10f;

    [SerializeField]
    private RotationDirection _rotationDirection;

    #endregion // Variables

    #region FixedUpdate

    private void FixedUpdate()
    {
        RotateAroundDirection(_rotationDirection);
    }

    #endregion // FixedUpdate

    #region Methods

    private void RotateAroundDirection(RotationDirection rotationDirection)
    {
        if (IsRotationDirectionEqualsTo(RotationDirection.Forward))
        {
            transform.Rotate(transform.forward, _rotationSpeed);
        }

        else if (IsRotationDirectionEqualsTo(RotationDirection.Upward))
        {
            transform.Rotate(transform.up, _rotationSpeed);
        }

        else if (IsRotationDirectionEqualsTo(RotationDirection.Right))
        {
            transform.Rotate(transform.right, _rotationSpeed);
        }
    }

    private bool IsRotationDirectionEqualsTo(RotationDirection rotationDirection)
    {
        return _rotationDirection == rotationDirection;
    }

    #endregion // Methods
}

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Control : MonoBehaviour
{
    private Rigidbody _rb;
    private Collider _col;
    private RaycastHit hit;
    private bool isGrounded = false;
    private float adjustment = 0f;

    public float raycastDistance = 1.0f;
    public float rotationSpeed = 5.0f;

    protected Vector3 _gravity;
    protected Vector3 _gravity_normal;
    protected float _gravity_magnitude;
    public Vector3 Gravity
    {
        get { return _gravity; }
        set
        {
            _gravity = value;
            _gravity_normal = value.normalized;
            _gravity_magnitude = value.magnitude;
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _col = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        _rb.velocity += _gravity * Time.fixedDeltaTime;

        isGrounded = false;
        adjustment = 0f;

        if (_col is BoxCollider) {
            BoxCollider boxCol = _col as BoxCollider;
            Vector3 halfExtents = Vector3.Scale(boxCol.size, transform.lossyScale) * 0.5f;
            Vector3 origin = _rb.position + _rb.rotation * boxCol.center;

            if (Physics.BoxCast(origin, halfExtents, _gravity_normal, out hit, _rb.rotation, raycastDistance))
            {
                isGrounded = true;
                adjustment = raycastDistance - hit.distance;
            }
        }
        else if (_col is SphereCollider) {
            SphereCollider sphereCol = _col as SphereCollider;
            float radius = sphereCol.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
            Vector3 origin = _rb.position + _rb.rotation * sphereCol.center;

            if (Physics.SphereCast(origin, radius, _gravity_normal, out hit, raycastDistance))
            {
                isGrounded = true;
                adjustment = raycastDistance - hit.distance;
            }
        }
        else if (_col is CapsuleCollider) {
            CapsuleCollider capsuleCol = _col as CapsuleCollider;
            float radius = capsuleCol.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);
            float height = (capsuleCol.height * transform.lossyScale.y) / 2 - radius;
            Vector3 center = _rb.position + _rb.rotation * capsuleCol.center;
            Vector3 point1 = center + transform.up * height;
            Vector3 point2 = center - transform.up * height;

            if (Physics.CapsuleCast(point1, point2, radius, _gravity_normal, out hit, raycastDistance))
            {
                isGrounded = true;
                adjustment = raycastDistance - hit.distance;
            }
        }
        else {
            Vector3 origin = _col.bounds.center;

            if (Physics.Raycast(origin, _gravity_normal, out hit, raycastDistance))
            {
                isGrounded = true;
                adjustment = raycastDistance - hit.distance;
            }
        }

        if (isGrounded)
        {
            _rb.freezeRotation = true;

            // ������ ������������ ������� � ������� ��������
            Vector3 desiredVelocity = -_gravity_normal * (adjustment-0.1f);
            _rb.velocity = desiredVelocity;

            // ����������� ������ � ������������ � �������� �����������
            /*Quaternion targetRotation = Quaternion.FromToRotation(_rb.rotation * Vector3.up, hit.normal) * _rb.rotation;
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
        else
        {
            _rb.freezeRotation = false;
        }
    }

}*/





using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Control : MonoBehaviour
{
    private Rigidbody _rb;
    private Collider _col;
    private bool isGrounded = false;
    private float adjustment = 0f;

    public float raycastDistance = 1.0f;
    public float rotationSpeed = 5.0f;
    public float surfaceCurve = 0.75f;

    protected Vector3 NormalSurface;
    protected Quaternion targetRotationSurface;

    protected Vector3 _gravity;
    protected Vector3 _gravity_normal;
    protected float _gravity_magnitude;
    public Vector3 Gravity
    {
        get { return _gravity; }
        set
        {
            _gravity = value;
            _gravity_normal = value.normalized;
            _gravity_magnitude = value.magnitude;
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _col = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        _rb.linearVelocity += _gravity * Time.fixedDeltaTime;

        isGrounded = false;
        adjustment = 0f;

        List<Vector3> hitNormals = new List<Vector3>();

        if (_col is BoxCollider)
        {
            BoxCollider boxCol = _col as BoxCollider;
            Vector3 halfExtents = Vector3.Scale(boxCol.size, transform.lossyScale) * 0.5f;
            Vector3 origin = _rb.position + _rb.rotation * boxCol.center + _gravity_normal * 0.1f;

            RaycastHit[] hits = Physics.BoxCastAll(origin, halfExtents, _gravity_normal, _rb.rotation, raycastDistance);
            foreach (RaycastHit hit in hits)
            {
                // ���������� ����������� ���������
                if (hit.collider != _col)
                {
                    hitNormals.Add(hit.normal);
                    adjustment += hit.distance;
                }
            }
            /*BoxCollider boxCol = _col as BoxCollider;
            Vector3 halfExtents = Vector3.Scale(boxCol.size, transform.lossyScale) * 0.5f;
            Vector3 center = _rb.position + _rb.rotation * boxCol.center;

            Collider[] overlappingColliders = Physics.OverlapBox(center, halfExtents, _rb.rotation);
            foreach (Collider col in overlappingColliders)
            {
                if (col != _col)
                {
                    // The cube is intersecting with another collider
                    isGrounded = true;

                    // Calculate normal if necessary
                    // For example, you might cast a ray to get the surface normal
                }
            }*/
        }
        else if (_col is SphereCollider)
        {
            SphereCollider sphereCol = _col as SphereCollider;
            float radius = sphereCol.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
            Vector3 origin = _rb.position + _rb.rotation * sphereCol.center + _gravity_normal * 0.1f;

            RaycastHit[] hits = Physics.SphereCastAll(origin, radius, _gravity_normal, raycastDistance);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != _col)
                {
                    hitNormals.Add(hit.normal);
                    adjustment += hit.distance;
                }
            }
        }
        else if (_col is CapsuleCollider)
        {
            CapsuleCollider capsuleCol = _col as CapsuleCollider;
            float radius = capsuleCol.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);
            float height = (capsuleCol.height * transform.lossyScale.y) / 2 - radius;
            Vector3 center = _rb.position + _rb.rotation * capsuleCol.center + _gravity_normal * 0.1f;
            Vector3 point1 = center + transform.up * height;
            Vector3 point2 = center - transform.up * height;

            RaycastHit[] hits = Physics.CapsuleCastAll(point1, point2, radius, _gravity_normal, raycastDistance);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != _col)
                {
                    hitNormals.Add(hit.normal);
                    adjustment += hit.distance;
                }
            }
        }
        else
        {
            Vector3 origin = _col.bounds.center;

            RaycastHit[] hits = Physics.RaycastAll(origin, _gravity_normal, raycastDistance);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != _col)
                {
                    hitNormals.Add(hit.normal);
                    adjustment += hit.distance;
                }
            }
        }

        // ��������� ������� �������� ��������
        NormalSurface = Vector3.zero;
        foreach (Vector3 normal in hitNormals)
        {
            if (Vector3.Dot(NormalSurface, -_gravity_normal) > surfaceCurve) NormalSurface += normal;
        }
        NormalSurface.Normalize();

        if (Vector3.Dot(NormalSurface, -_gravity_normal) < surfaceCurve) NormalSurface = Vector3.zero;


        if (NormalSurface != Vector3.zero)
        {
            isGrounded = true;


            // ������������ ������� � ���������� �� ������ ������� �������
            adjustment = (raycastDistance * 0.5f) - (adjustment / hitNormals.Count);
            _rb.freezeRotation = true;

            Vector3 desiredVelocity = -_gravity_normal * adjustment;
            _rb.linearVelocity = desiredVelocity;

            //targetRotationSurface = Quaternion.FromToRotation(_rb.rotation * Vector3.up, NormalSurface) * _rb.rotation;
            //_rb.rotation = Quaternion.Slerp(_rb.rotation, targetRotationSurface, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            isGrounded = false;
            _rb.freezeRotation = false;
        }
        OnFixedUpdate();
    }
    public virtual void OnFixedUpdate()
    {
        // ��� ��������� ���
    }
}

using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Transform))]
public class Sword : MonoBehaviour
{
    public GameObject phasedModel;
    [FormerlySerializedAs("OGModel")] public GameObject originalModel;
    private Collider col;
    public bool isPhased;
    public bool inContact;
    public float phaseTime;

    [FormerlySerializedAs("lightVelocityTrigger")]
    public float lightCollisionTrigger;

    [FormerlySerializedAs("mediumVelocityTrigger")]
    public float mediumCollisionTrigger;

    [FormerlySerializedAs("highVelocityTrigger")]
    public float fastCollisionTrigger;

    [FormerlySerializedAs("lowAngularVelocityTrigger")]
    public float lowVelocityTrigger;

    [FormerlySerializedAs("mediumAngularVelocityTrigger")]
    public float mediumVelocityTrigger;

    [FormerlySerializedAs("highAngularVelocityTrigger")]
    public float highVelocityTrigger;

    public AudioSource hitSFXAudioSource;
    public AudioSource swingSFXAudioSource;
    private Rigidbody rb;
    private Vector3 oldPosition;
    private Vector3 newPosition;
    private Vector3 currentVelocity;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        oldPosition = transform.position;
    }

    private void Update()
    {
        if (phaseTime > 0)
        {
            phaseTime -= Time.deltaTime;
        }

        VelocityActions();
    }

    private void OnCollisionEnter(Collision collision)
    {
        CollisionSpeedActions(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (col.gameObject.CompareTag("Sword"))
        {
            inContact = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (col.gameObject.CompareTag("Sword"))
        {
            inContact = false;
        }
    }

    private void VelocityActions()
    {
        newPosition = transform.position;
        var difference = newPosition - oldPosition;
        currentVelocity = difference / Time.deltaTime;
        oldPosition = newPosition;

        if (ReachedVelocity(highVelocityTrigger)) // fast swipe
        {
            if (!swingSFXAudioSource.isPlaying)
            {
                swingSFXAudioSource.Play();
                Debug.Log("fast swipe!");
            }
        }
        else if (ReachedVelocity(mediumVelocityTrigger)) // medium swipe
        {
            if (!swingSFXAudioSource.isPlaying)
            {
                swingSFXAudioSource.Play();
                Debug.Log("med swipe!");
            }
        }
        else if (ReachedVelocity(lowVelocityTrigger)) // small swipe
        {
            if (!swingSFXAudioSource.isPlaying)
            {
                swingSFXAudioSource.Play();
                Debug.Log("small swipe!");
            }
        }
        
        
    }

    void CollisionSpeedActions(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sword"))
        {
            PhaseSword();
            Invoke(nameof(OriginalSword), 1f);
        }

        if (ReachedTriggerVelocity(lightCollisionTrigger, collision))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                // Play stab sound light
            }
            else if (collision.gameObject.CompareTag("Sword"))
            {
                hitSFXAudioSource.Play(); // TODO: turn this into PlayOneShotClip light sfx
            }
            else
            {
                hitSFXAudioSource.Play(); // TODO: turn this into PlayOneShotClip light sfx
            }
        }

        if (ReachedTriggerVelocity(mediumCollisionTrigger, collision))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                // Play stab sound medium
            }
            else if (collision.gameObject.CompareTag("Sword"))
            {
                hitSFXAudioSource.Play(); // TODO: turn this into PlayOneShotClip medium sfx
            }
            else
            {
                hitSFXAudioSource.Play(); // TODO: turn this into PlayOneShotClip medium sfx
            }
        }

        if (ReachedTriggerVelocity(fastCollisionTrigger, collision))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                // Play stab sound fast
            }
            else if (collision.gameObject.CompareTag("Sword"))
            {
                hitSFXAudioSource.Play(); // TODO: turn this into PlayOneShotClip fast sfx
            }
            else
            {
                hitSFXAudioSource.Play(); // TODO: turn this into PlayOneShotClip fast sfx
            }
        }
    }

    public bool ReachedTriggerVelocity(float requiredVelocity, Collision collision)
    {
        return collision.relativeVelocity.x > requiredVelocity ||
               collision.relativeVelocity.x < -requiredVelocity ||
               collision.relativeVelocity.y > requiredVelocity ||
               collision.relativeVelocity.y < -requiredVelocity ||
               collision.relativeVelocity.z > requiredVelocity ||
               collision.relativeVelocity.z < -requiredVelocity;
    }

    public bool ReachedVelocity(float requiredVelocity)
    {
        return currentVelocity.x > requiredVelocity ||
               currentVelocity.x < -requiredVelocity ||
               currentVelocity.y > requiredVelocity ||
               currentVelocity.y < -requiredVelocity ||
               currentVelocity.z > requiredVelocity ||
               currentVelocity.z < -requiredVelocity;
    }

    private void PhaseSword()
    {
        phasedModel.SetActive(true);
        originalModel.SetActive(false);
        Invoke(nameof(ColTriggerSwitch), 0.5f);
    }

    private void OriginalSword()
    {
        if (inContact)
        {
            Invoke(nameof(OriginalSword), 1f);
            return;
        }

        phasedModel.SetActive(false);
        originalModel.SetActive(true);
        Invoke(nameof(ColTriggerSwitch), 0.5f);
    }

    public void ColTriggerSwitch()
    {
        col.isTrigger = !col.isTrigger;
    }
}
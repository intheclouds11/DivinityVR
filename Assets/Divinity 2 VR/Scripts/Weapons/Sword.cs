using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Transform))]
public class Sword : MonoBehaviour
{
    public GameObject phasedModel;
    public GameObject originalModel;
    public bool isPhased;
    public bool inContact;
    public float phaseTime;

    public float lightHitTriggerSpeed;
    public float mediumHitTriggerSpeed;
    public float fastHitTriggerSpeed;
    public float lowVelSwingTriggerSpeed;
    public float mediumVelSwingTriggerSpeed;
    public float highVelSwingTriggerSpeed;

    public AudioSource hitSFXAudioSource;
    public AudioSource swingSFXAudioSource;
    private Vector3 oldPosition;
    private Vector3 newPosition;
    private Vector3 currentVelocity;

    // Start is called before the first frame update
    void Start()
    {
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
        if (other.gameObject.CompareTag("Sword"))
        {
            inContact = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Sword"))
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

        if (ReachedVelocity(highVelSwingTriggerSpeed)) // fast swipe
        {
            if (!swingSFXAudioSource.isPlaying)
            {
                swingSFXAudioSource.Play();
                Debug.Log("fast swipe!");
            }
        }
        else if (ReachedVelocity(mediumVelSwingTriggerSpeed)) // medium swipe
        {
            if (!swingSFXAudioSource.isPlaying)
            {
                swingSFXAudioSource.Play();
                Debug.Log("med swipe!");
            }
        }
        else if (ReachedVelocity(lowVelSwingTriggerSpeed)) // small swipe
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

        if (ReachedTriggerVelocity(lightHitTriggerSpeed, collision))
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
                // hitSFXAudioSource.Play(); // TODO: turn this into PlayOneShotClip light sfx
            }
        }

        if (ReachedTriggerVelocity(mediumHitTriggerSpeed, collision))
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
                // hitSFXAudioSource.Play(); // TODO: turn this into PlayOneShotClip medium sfx
            }
        }

        if (ReachedTriggerVelocity(fastHitTriggerSpeed, collision))
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
                // hitSFXAudioSource.Play(); // TODO: turn this into PlayOneShotClip fast sfx
            }
        }
    }

    public bool ReachedTriggerVelocity(float requiredVelocity, Collision collision)
    {
        return collision.relativeVelocity.magnitude > requiredVelocity;
    }

    public bool ReachedVelocity(float requiredVelocity)
    {
        return currentVelocity.magnitude > requiredVelocity;
    }

    private void PhaseSword()
    {
        phasedModel.SetActive(true);
        originalModel.SetActive(false);
        // Invoke(nameof(ColTriggerSwitch), 0.5f);
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
        // Invoke(nameof(ColTriggerSwitch), 0.5f);
    }

    public void ColliderToggle()
    {
        // col.isTrigger = !col.isTrigger;
    }
}
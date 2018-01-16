using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {
    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending };
    State state;

    [SerializeField] float rcsRotation = 180f;
    [SerializeField] float rcsThrust = 20f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip crashSound;
    [SerializeField] AudioClip finishSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem crashParticles;
    [SerializeField] ParticleSystem finishParticles;



    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        state = State.Alive;
    }


    void Update() {

        if (state == State.Alive) {
            RespndToThrustInput();
            RespondToRotateInput();
        }

    }

    void LoadNextLevel() {

       finishParticles.Stop();
        SceneManager.LoadScene(1);
        state = State.Alive;
    }

    void RespawnWhenDead() {

       crashParticles.Stop();
        SceneManager.LoadScene(0);
        state = State.Alive;
    }


    private void OnCollisionEnter(Collision collision) {
        if (state != State.Alive) { return; }

        switch (collision.gameObject.tag) {
            case "Friendly":
                //Do nothing
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }
    private void StartSuccessSequence() {
        state = State.Transcending;
        audioSource.Stop();
        mainEngineParticles.Stop();
        audioSource.PlayOneShot(finishSound);
        finishParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void StartDeathSequence() {
        state = State.Dying;
        audioSource.Stop();
        mainEngineParticles.Stop();
        audioSource.PlayOneShot(crashSound);
        crashParticles.Play();
        Invoke("RespawnWhenDead", levelLoadDelay);
    }



    void RespndToThrustInput() {

        if (Input.GetKey(KeyCode.Space)) {
            ApplyThrust();
        } else {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust() {
        rigidBody.AddRelativeForce(Vector3.up * rcsThrust * Time.deltaTime);
        if (!audioSource.isPlaying) {
            audioSource.PlayOneShot(mainEngine);
            mainEngineParticles.Play();
        }
    }

    private void RespondToRotateInput() {
        rigidBody.freezeRotation = true;
        float rotationThisFrame = rcsRotation * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) {

            transform.Rotate(Vector3.forward * rotationThisFrame);
        } else if (Input.GetKey(KeyCode.D)) {

            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;
    }
}

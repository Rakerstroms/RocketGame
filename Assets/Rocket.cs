using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {
    Rigidbody rigidBody;
    AudioSource audioSource;
    

    enum State { Alive, Dying, Transcending };
    State state;
    bool collisionsEnabled = true;
    int currentSceneIndex;
    bool isWinner = false;
  

    [SerializeField] float rcsRotation = 180f;
    [SerializeField] float rcsThrust = 20f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip crashSound;
    [SerializeField] AudioClip finishSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem crashParticles;
    [SerializeField] ParticleSystem finishParticles;

    [SerializeField] ParticleSystem winnerParticle;




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
        RespondToDebugKeys();

    }

    private void RespondToDebugKeys() {
        if (Input.GetKeyDown(KeyCode.L)) {
            LoadNextLevel();
        }else if (Input.GetKeyDown(KeyCode.C)){
            collisionsEnabled = !collisionsEnabled;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.Return) && isWinner) {
            isWinner = false;
            Oscillator.isMoving = true;
            SceneManager.LoadScene(0);
        }

    }

    void LoadNextLevel() {

        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentSceneIndex +1;
        if(nextIndex == SceneManager.sceneCountInBuildSettings) {
            GameCompleted();
        }
        finishParticles.Stop();
        SceneManager.LoadScene(nextIndex);
        state = State.Alive;
    }

    private void GameCompleted() {
        isWinner = true;
        winnerParticle.Play();
        Oscillator.isMoving = false;

    }

void RespawnWhenDead() {

        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        crashParticles.Stop();
        SceneManager.LoadScene(currentSceneIndex);
        state = State.Alive;
    }


    private void OnCollisionEnter(Collision collision) {
        if (state != State.Alive  ) { return; }

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
        if (collisionsEnabled) {
            state = State.Dying;
            audioSource.Stop();
            mainEngineParticles.Stop();
            audioSource.PlayOneShot(crashSound);
            crashParticles.Play();
            Invoke("RespawnWhenDead", levelLoadDelay);
        }
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

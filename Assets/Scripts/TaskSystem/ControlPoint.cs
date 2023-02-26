using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : Task
{
    public enum Phase
    {
        NonCapture,
        Capturing,
        Captured,
    }
    public Phase phase;
    [SerializeField] private float captureSpeed = 1f;
    [SerializeField] private float decaySpeed = 2f;
    public float duration = 60f;
    public float defenseDuration = 30f;
    private float timer;
    private bool isStarted = false;
    private bool enterDefense = false;
    private float progress = 0f;
    [SerializeField] private string targetTag = "Untagged";
    [SerializeField] private Spawner spawnerCP;

    [SerializeField] private SinglePattern[] sp;
    public event System.Action OnDefense;
    public event System.Action<float> OnProgress;
    [SerializeField] private Spawner.WaveData wavePattern_Capture;
    [SerializeField] private Spawner.WaveData wavePattern_Defense;
    [SerializeField] private GameObject escapeZone;

    [Header("Debug Mode")]
    [SerializeField] private KeyCode debugKey = KeyCode.F12;
    [SerializeField] private KeyCode combineKey = KeyCode.LeftControl;
    private bool pressAnyKey = false;
    private bool condition1, condition2;
    private bool debugMode = false;
    [SerializeField] private float detectDebugDuration = 0.15f;
    private float detectTimer = 0f;
    [System.Serializable]
    public struct SinglePattern
    {
        public int[] pattern;
    }

    private void Start()
    {
        GameManager.Instance.OnPlayerDied += StopThing;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnPlayerDied -= StopThing;
    }

    private void StopThing()
    {
        StopAllCoroutines();
        this.enabled = false;
    }

    private void LateUpdate()
    {
        if(Input.GetKeyDown(debugKey))
        {
            condition1 = true;
            if(pressAnyKey == false)
            {
                pressAnyKey = true;
            }
        }
        if (Input.GetKeyDown(combineKey))
        {
            condition2 = true;
            if (pressAnyKey == false)
            {
                pressAnyKey = true;
            }
        }
        if(pressAnyKey)
        {
            detectTimer += Time.deltaTime;
            if(detectTimer > detectDebugDuration)
            {
                detectTimer = 0f;
                pressAnyKey = false;
                condition1 = false;
                condition1 = false;
            }
        }
        if(condition1 && condition2)
        {
            detectTimer = 0f;
            condition1 = false;
            condition2 = false;
            pressAnyKey = false;
            debugMode = !debugMode;
            Debug.LogWarning($"DebugMode Status [{debugMode}]");
        }
        if(debugMode)
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                duration = 10f;
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                defenseDuration = 10f;
            }
        }
        if (isStarted == false) return;
        ProgressCP();
        PhaseControl();
    }

    private void PhaseControl()
    {
        if (enterDefense) return;
        switch (phase)
        {
            case Phase.NonCapture:
                break;
            case Phase.Capturing:
                if(progress >= 1f)
                {
                    phase = Phase.Captured;
                    print("You've control The ControlPoint!");
                    return;
                }
                break;
            case Phase.Captured:
                spawnerCP.StopWave();
                if(enterDefense == false)
                {
                    enterDefense = true;
                    OnDefense?.Invoke();
                    DefensePhase();
                    /*
                    spawnerCP.DirectWave(wavePattern_Defense);
                    print("Entering Defensive Mode!");
                    */
                }
                break;
            default:
                break;
        }
    }

    private void ProgressCP()
    {
        if (phase == Phase.Captured) return;
        switch (phase)
        {
            case Phase.NonCapture:
                if(timer <= 0f)
                {
                    timer = 0f;
                }
                timer -= decaySpeed * Time.deltaTime;
                break;
            case Phase.Capturing:
                timer += captureSpeed * Time.deltaTime;
                break;
        }
        progress = timer / duration;
        OnProgress?.Invoke(progress);
    }

    private void SpawnPhase()
    {
        print("Wave has begin!");
        spawnerCP.DirectWave(wavePattern_Capture);
        //spawnerCP.WaveSpawn(0);
    } 

    private void DefensePhase()
    {
        print("Wave(Defensive) has begin!");
        spawnerCP.DirectWave(wavePattern_Defense);
        StartCoroutine(DefensiveThread());
    }

    private IEnumerator DefensiveThread()
    {
        float timer = 0f;
        float percentage;
        while (true)
        {
            timer += Time.deltaTime;
            percentage = timer / defenseDuration;
            OnProgress?.Invoke(defenseDuration - timer);
            //OnProgress?.Invoke(percentage);
            if (timer >= defenseDuration)
            {
                percentage = 1f;
                break;
            }
            yield return null;
        }
        //OnProgress?.Invoke(1f);
        //yield return new WaitForSeconds(defenseDuration);
        spawnerCP.StopWave();
        escapeZone.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (phase == Phase.Captured) return;
        if (other.CompareTag(targetTag) == false) return;
        if(phase == Phase.NonCapture)
        {
            if(isStarted == false)
            {
                isStarted = true;
                SpawnPhase();
            }
            phase = Phase.Capturing;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (phase == Phase.Captured) return;
        if (other.CompareTag(targetTag) == false) return;
        if (phase == Phase.Capturing)
        {
            phase = Phase.NonCapture;
        }
    }
}

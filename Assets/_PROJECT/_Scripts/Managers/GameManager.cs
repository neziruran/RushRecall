using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public DollyPlayer DollyPlayer => dollyPlayer;
    public string Username { get; private set; }
    public int SimulationSpeed { get; private set; }
    
    [Header("Components")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject placementManager;
    [SerializeField] private Camera placementCamera;
    [SerializeField] private DollyPlayer dollyPlayer;
    [SerializeField] private CountdownTimer countdownTimer;
    [SerializeField] private DataManager dataManager;
    
    [Header("Level settings")]
    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private GameObject[] buildingPrefabs;
    [SerializeField] private List<Landmark> landmarks;
    [SerializeField] private int loadingTime;

    
    
    private int _placedLandmarks;
    private GameObject _currentLevelInstance;

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        EventManager.OnLandmarkPlaced += OnLandmarkPlaced;
        EventManager.OnLevelStart += LoadLevel;
    }

    private void OnDisable()
    {
        EventManager.OnLandmarkPlaced -= OnLandmarkPlaced;
        EventManager.OnLevelStart -= LoadLevel;
    }

    private void OnDestroy()
    {
        EventManager.ClearAllSubscriptions();
    }

    
    private void OnLandmarkPlaced(Landmark obj)
    {
        _placedLandmarks++;
        if (_placedLandmarks == landmarks.Count)
        {
            
            dataManager.SaveData();
            RestartProject();
        }
    }

    
    private void SpawnRandomBuildings()
    {
        int requiredCount = landmarks.Count;

        var uniquePrefabs = buildingPrefabs.Distinct().ToArray();

        if (uniquePrefabs.Length < requiredCount)
        {
            Debug.LogError("Not enough unique building prefabs for all landmarks!");
            return;
        }

        GameObject[] shuffledPrefabs;
        do
        {
            shuffledPrefabs = ShuffleArray(uniquePrefabs).Take(requiredCount).ToArray();
        } while (HasDuplicates(shuffledPrefabs)); 

        for (int i = 0; i < landmarks.Count; i++)
        {
            Landmark landmark = landmarks[i];
            GameObject spawnedPrefab = Instantiate(shuffledPrefabs[i], landmark.transform);
            spawnedPrefab.transform.localPosition = Vector3.zero;
            landmark.Initialize();
            landmark.Prefab = spawnedPrefab;
        }
    }

    private bool HasDuplicates(GameObject[] elements)
    {
        var seen = new HashSet<GameObject>();
        return elements.Any(element => !seen.Add(element));
    }

    private T[] ShuffleArray<T>(T[] array)
    {
        System.Random random = new System.Random();
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);
            (array[i], array[randomIndex]) = (array[randomIndex], array[i]);
        }
        return array;
    }


    public void TriggerStartGame()
    {
        if (!IsValidGameStart())
            return;

        EventManager.TriggerLevelStart();
        _placedLandmarks = 0;
    }

    private bool IsValidGameStart()
    {
        var userName = uiManager.ReadName();
        var difficulty = uiManager.ReadDifficulty();
        
        if (string.IsNullOrEmpty(uiManager.ReadName()))
            return false;

        Username = userName;
        SimulationSpeed = difficulty;
        var difficultyInt = difficulty.ToString();
        return !string.IsNullOrEmpty(difficultyInt) && int.TryParse(difficultyInt, out _);
    }


    private async void RestartProject()
    {
        await Task.Delay(1000);
        SceneManager.LoadScene(0);
    }
    
    private void LoadLevel()
    {
        uiManager.TriggerLoadingBar(loadingTime, (isComplete) => 
        {
            if (isComplete)
            {
                if (_currentLevelInstance != null)
                {
                    Destroy(_currentLevelInstance);
                }
                
                placementManager.gameObject.SetActive(false);
                placementCamera.gameObject.SetActive(false);

                if (levelPrefab != null)
                {
                    _currentLevelInstance = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
                    landmarks = _currentLevelInstance
                        .GetComponentsInChildren<Landmark>()
                        .OrderBy(landmark => landmark.order)
                        .ToList();
                    SpawnRandomBuildings();
                }
                else
                {
                    Debug.LogError("Level prefab is missing!");
                    return;
                }

                if (dollyPlayer != null)
                {
                    StartCoroutine(AsyncStartLevel());
                }
                else
                {
                    Debug.LogError("DollyPlayer reference is missing!");
                }
            }
        });
    }

    private IEnumerator AsyncStartLevel()
    {
        var timer = Instantiate(countdownTimer, Vector3.zero, Quaternion.identity);
        yield return new WaitUntil(() => timer == null);
        dollyPlayer.StartCart(SimulationSpeed);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void TriggerSecondPhase()
    {
        EventManager.TriggerSecondPhaseStart();
        dollyPlayer.gameObject.SetActive(false);
        placementManager.gameObject.SetActive(true);
        placementCamera.gameObject.SetActive(true);
        _currentLevelInstance.SetActive(false);
    }
}

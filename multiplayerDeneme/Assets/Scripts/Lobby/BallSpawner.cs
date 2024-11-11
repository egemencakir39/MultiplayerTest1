using Mirror.FizzySteam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class BallSpawner : NetworkBehaviour
{
    public GameObject ballPrefab;
    public string gameSceneName = "GameScene"; // Oyun sahnesinin ad�n� burada belirt

    public override void OnStartServer()
    {
        // Sunucu sahne y�klendi�inde topu spawn eder
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // E�er oyun sahnesindeysek topu spawn et
        if (scene.name == gameSceneName)
        {
            SpawnBall();
        }
    }

    private void SpawnBall()
    {
        // Topu sadece sunucu taraf�ndan spawn et ve a� �zerinden payla�
        GameObject ball = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(ball);
    }

    private void OnDestroy()
    {
        // Olay dinleyicisini temizle
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
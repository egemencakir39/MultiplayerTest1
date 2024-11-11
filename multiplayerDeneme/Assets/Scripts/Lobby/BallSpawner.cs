using Mirror.FizzySteam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class BallSpawner : NetworkBehaviour
{
    public GameObject ballPrefab;
    public string gameSceneName = "GameScene"; // Oyun sahnesinin adýný burada belirt

    public override void OnStartServer()
    {
        // Sunucu sahne yüklendiðinde topu spawn eder
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Eðer oyun sahnesindeysek topu spawn et
        if (scene.name == gameSceneName)
        {
            SpawnBall();
        }
    }

    private void SpawnBall()
    {
        // Topu sadece sunucu tarafýndan spawn et ve að üzerinden paylaþ
        GameObject ball = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(ball);
    }

    private void OnDestroy()
    {
        // Olay dinleyicisini temizle
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
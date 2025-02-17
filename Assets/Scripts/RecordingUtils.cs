using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal.Commands;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RecordingUtils : MonoBehaviour
{
    [SerializeField] List<GameObject> UIItems = new List<GameObject>();
    [SerializeField] AudioSource music;
    [SerializeField] Light2D bigLight;
    [SerializeField] Camera mainCam;
    [SerializeField] GameObject enemyB;
    [SerializeField] GameObject enemyR;
    [SerializeField] GameObject enemyM;
    [SerializeField] GameObject player;
    [SerializeField] GameObject torch;
    [SerializeField] ScreenShake cameraScript;
    bool UIActive = false;
    bool musicActive = false;
    bool enemiesActive = true;

    enum States
    {
        None,
        Enemies,
        Torches
    }
    States state = States.None;
    void Start()
    {
        mainCam = Camera.main;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if(state == States.Enemies)
            {
                state = States.None;
            }
            else
            {
                state = States.Enemies;
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (state == States.Torches)
            {
                state = States.None;
            }
            else
            {
                state = States.Torches;
            }
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            UIActive = !UIActive;
            ToggleUI(UIActive);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            enemiesActive = !enemiesActive;
            ToggleEnemyAI(enemiesActive);
        }
        if (Input.GetKeyDown(KeyCode.M)) 
        {
            musicActive = !musicActive;
            ToggleMusic(musicActive);
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            bigLight.enabled = true;
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            bigLight.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            mainCam.orthographicSize = 40;
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            mainCam.orthographicSize = 5;
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Main");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameObject clickedPlayer = GetClosestObjectToCursor<Player>();
            if(!cameraScript.trackedTransform.gameObject.TryGetComponent<Player>(out _))
            {
                Destroy(cameraScript.trackedTransform.gameObject);
            }
            if(clickedPlayer != null)
            {
                cameraScript.trackedTransform = clickedPlayer.transform;
            }
            else
            {
                GameObject tempTransform = new GameObject();
                cameraScript.trackedTransform = tempTransform.transform;
                tempTransform.transform.position = mainCam.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if(state == States.Enemies)
            {
                GameObject clickedEnemy = GetClosestObjectToCursor<IEnemy>();
                if(clickedEnemy == null)
                {
                    GameObject newEnemy = Instantiate(enemyB, (Vector2)mainCam.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
                    newEnemy.GetComponent<ProjectileEnemy>().SetTarget(player);
                    if (!enemiesActive)
                    {
                        newEnemy.GetComponent<ProjectileEnemy>().enabled = false;
                    }
                }
                else
                {
                    if(clickedEnemy.TryGetComponent<ProjectileEnemy>(out _))
                    {
                        GameObject newEnemy = Instantiate(enemyR, clickedEnemy.transform.position, Quaternion.identity);
                        newEnemy.GetComponent<DashEnemy>().SetTarget(player);
                        if(!enemiesActive)
                        { 
                            newEnemy.GetComponent<DashEnemy>().enabled = false;
                        }
                        Destroy(clickedEnemy);
                    }
                    else if (clickedEnemy.TryGetComponent<DashEnemy>(out _))
                    {
                        GameObject newEnemy = Instantiate(enemyM, clickedEnemy.transform.position, Quaternion.identity);
                        newEnemy.GetComponent<StalagmiteEnemy>().SetTarget(player);
                        if (!enemiesActive)
                        {
                            newEnemy.GetComponent<StalagmiteEnemy>().enabled = false;
                        }
                        Destroy(clickedEnemy);
                    }
                    else
                    {
                        GameObject newEnemy = Instantiate(enemyB, clickedEnemy.transform.position, Quaternion.identity);
                        newEnemy.GetComponent<ProjectileEnemy>().SetTarget(player);
                        if (!enemiesActive)
                        {
                            newEnemy.GetComponent<ProjectileEnemy>().enabled = false;
                        }
                        Destroy(clickedEnemy);
                    }
                }
            }
            if (state == States.Torches)
            {
                GameObject clickedTorch = GetClosestObjectToCursor<Torch>();
                if (clickedTorch == null)
                {
                    Instantiate(torch, (Vector2)mainCam.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
                }
                else
                {
                    Torch torchScript = clickedTorch.GetComponent<Torch>();
                    torchScript.lightOn = !torchScript.lightOn;
                    torchScript.Update();
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (state == States.Enemies)
            {
                GameObject clickedEnemy = GetClosestObjectToCursor<IEnemy>();
                if (clickedEnemy != null)
                {
                    Destroy(clickedEnemy);
                }
            }
            if (state == States.Torches)
            {
                GameObject clickedTorch = GetClosestObjectToCursor<Torch>();
                if (clickedTorch != null)
                {
                    Destroy(clickedTorch);
                }
            }
        }
    }

    void ToggleUI(bool toggle)
    {
        foreach (GameObject item in UIItems)
        {
            if (item.TryGetComponent(out Image image))
            {
                image.enabled = toggle;
            }
            if (item.TryGetComponent(out TextMeshProUGUI tmp))
            {
                tmp.enabled = toggle;
            }
        }
    }

    void ToggleMusic(bool toggle)
    {
        if(toggle)
        {
            music.volume = 0.44f;
        }
        else
        {
            music.volume = 0;
        }   
    }

    void ToggleEnemyAI(bool toggle)
    {
        foreach (ProjectileEnemy enemy in FindObjectsByType(typeof(ProjectileEnemy), FindObjectsSortMode.None))
        {
            enemy.enabled = toggle;
        }
        foreach (DashEnemy enemy in FindObjectsByType(typeof(DashEnemy), FindObjectsSortMode.None))
        {
            enemy.enabled = toggle;
        }
        foreach (StalagmiteEnemy enemy in FindObjectsByType(typeof(StalagmiteEnemy), FindObjectsSortMode.None))
        {
            enemy.enabled = toggle;
        }
    }

    GameObject GetClosestObjectToCursor<targetComponent>()
    {
        Vector2 cursorPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        List<RaycastHit2D> hits = (Physics2D.CircleCastAll(cursorPos, 1, Vector2.zero, 0)).ToList();
        GameObject closest = null;
        float closestDistance = 999;
        foreach (RaycastHit2D hit in hits)
        {
            if(hit.collider == null) continue;
            if(hit.collider.TryGetComponent<targetComponent>(out _))
            {
                if((cursorPos - (Vector2) hit.collider.transform.position).magnitude < closestDistance)
                {
                    closest = hit.collider.gameObject;
                }
            }
        }
        return closest;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFirst : MonoBehaviour
{
    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != Constants.SCENE_NAME_MAIN_GAME)
        {
            SceneManager.LoadScene(Constants.SCENE_NAME_MAIN_GAME, LoadSceneMode.Additive);
        }
    }
}

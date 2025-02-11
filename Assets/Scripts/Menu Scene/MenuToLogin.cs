using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuToLogin : MonoBehaviour
{
    public void GoToLoginScene()
    {
        SceneManager.LoadScene("Login");
    }
}

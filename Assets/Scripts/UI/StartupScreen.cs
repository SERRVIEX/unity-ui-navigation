using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartupScreen : ScreenController
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3);

        SceneManager.LoadScene("Menu");
    }
}

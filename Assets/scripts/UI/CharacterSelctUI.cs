using UnityEngine;
using UnityEngine.SceneManagement;


public class CharacterSelctUI : MonoBehaviour
{
    public GameObject[] characters;
    public int selectedCharcter = 0;

    public void NextCharacter()
    {
        characters[selectedCharcter].SetActive(false);
        selectedCharcter = (selectedCharcter + 1) % characters.Length;
        characters[selectedCharcter].SetActive(true);
    }

    public void PreviousCharacter()
    {
        characters[selectedCharcter].SetActive(false);
        selectedCharcter--;
        if (selectedCharcter < 0)
        {
            selectedCharcter += characters.Length;
        }
        characters[selectedCharcter].SetActive(true);
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("selectedCharcter", selectedCharcter);
        SceneManager.LoadScene(1);
    }

}



//https://www.youtube.com/watch?v=7glCsF9fv3s&list=PLzDRvYVwl53sSmEcIgZyDzrc0Smpq_9fN
//https://www.youtube.com/watch?v=3qlRgICRoeA
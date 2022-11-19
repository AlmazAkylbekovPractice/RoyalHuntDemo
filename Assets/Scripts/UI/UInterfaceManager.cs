using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UInterfaceManager : MonoBehaviour
{
    public static UInterfaceManager instance;

    [SerializeField] private Text score_text;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void UpdateScore(int inputScore)
    {
        score_text.text = inputScore.ToString();
    }
}

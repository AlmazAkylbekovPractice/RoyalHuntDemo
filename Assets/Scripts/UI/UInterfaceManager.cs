using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UInterfaceManager : MonoBehaviour
{
    public static UInterfaceManager instance;

    [SerializeField] private Text hitNum_text;
    [SerializeField] private Text injuredNum_text;

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

    public void UpdateScore(int scoreIn, int damageIn)
    {
        hitNum_text.text = scoreIn.ToString();
        injuredNum_text.text = damageIn.ToString();
    }
}

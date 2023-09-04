using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static bool Playing = false;

    private static Game _instance;
    [SerializeField] UIController uIController;
    [SerializeField] Animator titleScreenAnimator;

    public bool UIVisible
    {
        set
        {
            uIController.UIVisible = value;
        }
        get
        {
            return uIController.UIVisible;
        }
    }

    public void StartGame()
    {
        titleScreenAnimator.SetBool("GameStart", true);
        Playing = true;
    }

    private void Reset()
    {
        if(uIController == null)
        {
            uIController = GetComponent<UIController>();
        }
    }

    // Static property to access the Singleton instance
    public static Game Instance
    {
        get
        {
            if (_instance == null)
            {
                // This should not happen, but just in case, create a new instance
                _instance = new GameObject("Game").AddComponent<Game>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // Check if an instance already exists
        if (_instance == null)
        {
            // If not, set this instance as the Singleton
            _instance = this;
        }
        else
        {
            // If an instance already exists, destroy this one
            Destroy(gameObject);
        }
    }
}

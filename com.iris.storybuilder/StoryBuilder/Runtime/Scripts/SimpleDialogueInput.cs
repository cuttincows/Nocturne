using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Dialogue))]
public class SimpleDialogueInput : MonoBehaviour
{
    private Dialogue _dialogue;
    private int _currentOption = -1;

    public bool circular;
    
    private void Awake()
    {
        _dialogue = GetComponent<Dialogue>();
        _dialogue.OnSayOrChoice += OnSayOrChoice;
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)

        // if (Input.GetButtonDown("Submit")/* Input.GetKeyDown(KeyCode.Return)*/)
        {
            if (_dialogue.optionCount > 0 && _currentOption >= 0)
            {
                _dialogue.HandleInput(_dialogue.GetOption(_currentOption));
            }
            else
            {
                _dialogue.HandleInput();
            } 
        }
        else if (_dialogue.optionCount > 0)
        {
            float vertical = Input.GetButtonDown("Vertical")?Input.GetAxis("Vertical"):0;
            if (vertical > 0)
            {
                _currentOption = circular
                    ? (int)Mathf.Repeat(_currentOption - 1,_dialogue.optionCount)
                    : Mathf.Clamp(_currentOption - 1, 0, _dialogue.optionCount - 1);
                _dialogue.HighlightChoice(_currentOption);
            }
            else if (vertical < 0)
            {
                _currentOption = circular
                    ? (int)Mathf.Repeat(_currentOption + 1,_dialogue.optionCount)
                    : Mathf.Clamp(_currentOption + 1, 0, _dialogue.optionCount - 1);
                _dialogue.HighlightChoice(_currentOption);
            }
        }
    }

    private void OnSayOrChoice()
    {
        _currentOption = -1;
    }

}

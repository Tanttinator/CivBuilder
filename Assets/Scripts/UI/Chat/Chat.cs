using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Chat : MonoBehaviour {

    static Chat instance;

    public Text[] messageTexts;
    public InputField input;
    public Button send;
    public Scrollbar scroll;
    static int Step
    {
        get
        {
            return (int)(instance.scroll.numberOfSteps * instance.scroll.value);
        }
        set
        {
            instance.scroll.value = value / (instance.scroll.numberOfSteps * 1f);
        }
    }

    public static List<string> messages = new List<string>();

    public static void SendInput()
    {
        Message(instance.input.text);
        instance.input.text = "";
        instance.input.DeactivateInputField();
    }

    public static bool IsFocused()
    {
        return instance.input.isFocused;
    }

    public static void Open()
    {
        instance.input.Select();
        instance.input.ActivateInputField();
    }

    public static void Message(string message)
    {
        if (message.Length == 0)
            return;
        messages.Add("> " + message);
        instance.scroll.numberOfSteps = Mathf.Max(1, messages.Count - 5);
        if (messages.Count <= 5)
            instance.scroll.size = 1;
        else
            instance.scroll.size = 5f / messages.Count;
        if (Step != 0 && instance.scroll.numberOfSteps > 1)
            Step++;
        DisplayTexts(Step);
        if (message.StartsWith("/"))
        {
            string[] args = message.Substring(1).ToLower().Split(' ');
            if (args.Length == 0)
                return;
            else
                ExecuteCommand(args[0], args.Skip(1).ToArray());
        }
    }

    static void DisplayTexts(int startID)
    {
        for(int i = 0; i < instance.messageTexts.Length; i++)
        {
            int message = i + startID;
            if(i < messages.Count)
            {
                instance.messageTexts[i].text  = messages[messages.Count - message - 1];
            }
            else
            {
                instance.messageTexts[i].text = "";
            }
        }
    }

    static void ExecuteCommand(string command, string[] args)
    {
        switch (command)
        {
            case "help":
                Help();
                break;
            case "seed":
                Seed();
                break;
            default:
                Message("Command not recognized (type /help for a list of commands)");
                break;
        }
    }

    static void Seed()
    {
        Message("Seed: " + World.seed);
    }

    static void Help()
    {
        Message("Commands: \nHelp: Get a list of all commands. \nSeed: Get the seed of the current world");
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            send.onClick.AddListener(delegate { Chat.SendInput(); });
            Step = 0;
            DisplayTexts(0);
            scroll.onValueChanged.AddListener(delegate { DisplayTexts(Step); });
        }
    }
}

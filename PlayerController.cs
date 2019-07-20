using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerController : PlayerActionSet {
    
    public PlayerAction Left;
    public PlayerAction Right;
    public PlayerAction Up;
    public PlayerAction Down;
    public PlayerTwoAxisAction Move;
    public PlayerAction Jump;
    public PlayerAction Melee;
    public PlayerAction Dash;
    public PlayerAction ClearMind;
    public PlayerAction Start;
    public PlayerAction Select;
    public PlayerAction Confirm;
    public PlayerAction Return;
    public PlayerAction Delete;

    public string InputLastUsed;



    public PlayerController()
    {
        Left = CreatePlayerAction("Move Left");
        Right = CreatePlayerAction("Move Right");
        Up = CreatePlayerAction("Move Up");
        Down = CreatePlayerAction("Move Down");
        Melee = CreatePlayerAction("Melee");
        Jump = CreatePlayerAction("Jump");
        Dash = CreatePlayerAction("Dash");
        ClearMind = CreatePlayerAction("ClearMind");
        Select = CreatePlayerAction("Select");
        Start = CreatePlayerAction("Start");
        Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
        Confirm = CreatePlayerAction("Confirm");
        Return = CreatePlayerAction("Return");
        Delete = CreatePlayerAction("Delete");

    }
    public static PlayerController CreateWithDefaultBindings()
    {
        PlayerController playerActions = new PlayerController();
        //Jump
        playerActions.Jump.AddDefaultBinding(Key.Space);
        playerActions.Jump.AddDefaultBinding(Key.UpArrow);
        playerActions.Jump.AddDefaultBinding(Key.Z);
        playerActions.Jump.AddDefaultBinding(InputControlType.Action1);
        //Arrow Keys
        playerActions.Up.AddDefaultBinding(Key.UpArrow);
        playerActions.Down.AddDefaultBinding(Key.DownArrow);
        playerActions.Left.AddDefaultBinding(Key.LeftArrow);
        playerActions.Right.AddDefaultBinding(Key.RightArrow);
        //Arrow Keys
        playerActions.Up.AddDefaultBinding(Key.W);
        playerActions.Down.AddDefaultBinding(Key.S);
        playerActions.Left.AddDefaultBinding(Key.A);
        playerActions.Right.AddDefaultBinding(Key.D);
        //Analog Stick
        playerActions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
        playerActions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
        playerActions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
        playerActions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
        //D-Pad
        playerActions.Left.AddDefaultBinding(InputControlType.DPadLeft);
        playerActions.Right.AddDefaultBinding(InputControlType.DPadRight);
        playerActions.Up.AddDefaultBinding(InputControlType.DPadUp);
        playerActions.Down.AddDefaultBinding(InputControlType.DPadDown);
        //Start
        playerActions.Start.AddDefaultBinding(InputControlType.Start);
        playerActions.Start.AddDefaultBinding(InputControlType.Options);
        playerActions.Start.AddDefaultBinding(Key.Escape);
        //Dash
        playerActions.Dash.AddDefaultBinding(Key.V);
        playerActions.Dash.AddDefaultBinding(Key.Pad6);
        playerActions.Dash.AddDefaultBinding(InputControlType.Action2);
        //Melee
        playerActions.Melee.AddDefaultBinding(Key.E);
        playerActions.Melee.AddDefaultBinding(Key.Pad4);
        playerActions.Melee.AddDefaultBinding(InputControlType.Action3);
        //ClearMind
        playerActions.ClearMind.AddDefaultBinding(Key.Q);
        playerActions.ClearMind.AddDefaultBinding(Key.Pad8);
        playerActions.ClearMind.AddDefaultBinding(InputControlType.Action4);
        playerActions.ClearMind.AddDefaultBinding(InputControlType.LeftTrigger);
        playerActions.ClearMind.AddDefaultBinding(InputControlType.LeftStickButton);
        //Select
        playerActions.Select.AddDefaultBinding(Key.Alt);
        playerActions.Select.AddDefaultBinding(Key.Return);
        playerActions.Select.AddDefaultBinding(InputControlType.Select);
        playerActions.Select.AddDefaultBinding(InputControlType.TouchPadButton);
        playerActions.Select.AddDefaultBinding(InputControlType.Back);
        //Confirm
        playerActions.Confirm.AddDefaultBinding(Key.Return);
        playerActions.Confirm.AddDefaultBinding(Key.PadEnter);
        playerActions.Confirm.AddDefaultBinding(Key.Space);
        playerActions.Confirm.AddDefaultBinding(Key.Pad2);
        playerActions.Confirm.AddDefaultBinding(InputControlType.Action1);
        //Return
        playerActions.Return.AddDefaultBinding(Key.Escape);
        playerActions.Return.AddDefaultBinding(Key.Backspace);
        playerActions.Return.AddDefaultBinding(Key.Pad6);
        playerActions.Return.AddDefaultBinding(InputControlType.Action2);
        //Delete
        playerActions.Delete.AddDefaultBinding(Key.Delete);
        playerActions.Delete.AddDefaultBinding(InputControlType.Action3);

        playerActions.ListenOptions.IncludeUnknownControllers = true;
        playerActions.ListenOptions.MaxAllowedBindings = 4;
        //playerActions.ListenOptions.MaxAllowedBindingsPerType = 1;
        //playerActions.ListenOptions.AllowDuplicateBindingsPerSet = true;
        playerActions.ListenOptions.UnsetDuplicateBindingsOnSet = true;
        //playerActions.ListenOptions.IncludeMouseButtons = true;
        //playerActions.ListenOptions.IncludeModifiersAsFirstClassKeys = true;
        //playerActions.ListenOptions.IncludeMouseButtons = true;
        //playerActions.ListenOptions.IncludeMouseScrollWheel = true;

        playerActions.ListenOptions.OnBindingFound = (action, binding) => {
            if (binding == new KeyBindingSource(Key.Escape))
            {
                action.StopListeningForBinding();
                return false;
            }
            return true;
        };

        playerActions.ListenOptions.OnBindingAdded += (action, binding) => {
            Debug.Log("Binding added... " + binding.DeviceName + ": " + binding.Name);
        };

        playerActions.ListenOptions.OnBindingRejected += (action, binding, reason) => {
            Debug.Log("Binding rejected... " + reason);
        };

        return playerActions;
    }
}

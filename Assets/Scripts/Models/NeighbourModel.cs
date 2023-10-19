
using System;
using UnityEngine; 

[System.Serializable]
public class Neighbour
{
    public ClimbPoint point;
    public Vector2 direction;
    public ConnectionType connectionType;
    public bool isTwoWay = true;
}

[Flags]
public enum ConnectionType
{
    None = 0,         
    Jump = 1 << 0,    // 0001
    Move = 1 << 1,    // 0010
    GetDown = 1 << 2, // 0100
    GetUp = 1 << 3,   // 1000
}

[Flags]
public enum DirectionType
{
    Up = 0,
    Down = 1 << 0,
    Left = 1 << 1,
    Right = 1 << 2,
}

public static class ConnectionManage
{
    public static bool CanMoveAndJump(ConnectionType connectionType) =>
        (connectionType & (ConnectionType.Move | ConnectionType.Jump)) != 0;
    public static bool CanJump(ConnectionType connectionType) =>
        (connectionType & ConnectionType.Jump) != 0;

    public static bool CanMove(ConnectionType connectionType) =>
        (connectionType & ConnectionType.Move) != 0;

    public static bool CanGetUp(ConnectionType connectionType) =>
        (connectionType & ConnectionType.GetUp) != 0;

    public static bool CanGetDown(ConnectionType connectionType) =>
        (connectionType & ConnectionType.GetDown) != 0;
}

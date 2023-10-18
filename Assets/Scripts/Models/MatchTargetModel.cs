using UnityEngine;
public class ActionParameters
{
    public bool Rotate { get; set; } = false;
    public float PostDelay { get; set; } = 0.0f;
    public bool Mirror { get; set; } = false;
}

public class MatchTargetModel
{
    public Vector3 Position { get; set; }
    public AvatarTarget BodyPart { get; set; }
    public Vector3 PositionWeight { get; set; }
    public float StartTime { get; set; }
    public float TargetTime { get; set; }

}

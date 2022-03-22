using UnityEngine;

public abstract class Message
{
    public MessageType type { get; }
    public int droneId { get; }

    public Message(MessageType type, int droneId)
    {
        this.type = type;
        this.droneId = droneId;
    }

}

public class PositionMessage : Message
{
    public Vector3 position { get; }

    public PositionMessage(int droneId, Vector3 position) : base(MessageType.Position, droneId)
    {
        this.position = position;
    }
}

public class AntiCollisionMessage : Message
{
    public Vector3 position { get; }
    public bool stopInstruction { get; }

    public AntiCollisionMessage(int droneId, Vector3 position, bool stopInstruction) :  base(MessageType.AntiCollision, droneId)
    {
        this.position = position;
        this.stopInstruction = stopInstruction;
    }
}

public enum MessageType
{
    Position,
    AntiCollision
}
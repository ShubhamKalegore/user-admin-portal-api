namespace CleanArchDemo.Domain.Entities;

public class SupportMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SenderUserId { get; set; }

    public Guid? ReceiverUserId { get; set; }

    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReadAt { get; set; }

    public User SenderUser { get; set; } = null!;

    public User? ReceiverUser { get; set; }
}

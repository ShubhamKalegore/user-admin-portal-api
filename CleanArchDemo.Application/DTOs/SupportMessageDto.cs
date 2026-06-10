namespace CleanArchDemo.Application.DTOs;

public class SupportMessageDto
{
    public Guid Id { get; set; }

    public Guid SenderUserId { get; set; }

    public Guid? ReceiverUserId { get; set; }

    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public DateTime SentAt { get; set; }

    public DateTime? ReadAt { get; set; }
}

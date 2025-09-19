public class DeleteSkuResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public long DeletedId { get; set; }
}
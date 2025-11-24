namespace ZhmApi.Dtos
{
  public class HealthResponse
  {
    public string Api { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
  }
}
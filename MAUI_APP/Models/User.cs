using System.Text.Json.Serialization;

namespace MAUI_APP.Models;

public class User
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

public class LoginRequest
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    [JsonPropertyName("Token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("Oid")]
    public string Oid { get; set; } = string.Empty;

    [JsonPropertyName("UserName")]
    public string UserName { get; set; } = string.Empty;
}

public class Customer
{
    [JsonPropertyName("Oid")]
    public string Oid { get; set; } = string.Empty;

    [JsonPropertyName("Birthday")]
    public string Birthday { get; set; } = string.Empty;

    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("LastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("Active")]
    public bool Active { get; set; }

    // Additional computed properties for display
    public string FullName => $"{Name} {LastName}".Trim();
    public int Id => string.IsNullOrEmpty(Oid) ? 0 : Oid.GetHashCode();
    public string Email => string.Empty; // Can be updated when API provides this
    public string Phone => string.Empty; // Can be updated when API provides this  
    public string Address => string.Empty; // Can be updated when API provides this
}

// Modelo para manejar respuestas OData
public class ODataResponse<T>
{
    [JsonPropertyName("@odata.context")]
    public string? Context { get; set; }

    [JsonPropertyName("value")]
    public List<T> Value { get; set; } = [];
}
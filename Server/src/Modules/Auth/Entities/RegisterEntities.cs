namespace AuthModule.Entities;


public class RegisterEntities
{
    public int AccountId { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }   
    public bool IsActive { get; set; }
    public int TotalCompany { get; set; }    
    public DateTime CreatedAt { get; set; }
}

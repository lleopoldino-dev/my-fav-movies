﻿namespace Business.Models;

public class User: BaseEntity
{
    public string Name { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public DateTime CreatedDate { get; set; }
}

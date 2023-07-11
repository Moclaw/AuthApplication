﻿namespace AuthApplication.Models.Entity
{
    public class User : BaseEntity
    {
		public string? Address { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
    }
}

﻿namespace backendPointguessr.Classes
{
    /// <summary>
    /// Třída pro záznamy uživatelů
    /// </summary>
    public class Users
    {
        public int ID { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Username { get; set; }
    }
}

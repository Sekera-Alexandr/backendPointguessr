using System.Data;
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace backendPointguessr.Classes
{
    public class Database
    {
        private readonly IConfiguration _configuration;
        private SqlConnection _connection = null!;
        private readonly string connectionString = "Data Source=LAPTOP-T5LQ4LOJ\\SQLEXPRESS;initial catalog=userDatabase;Trusted_Connection=True;TrustServerCertificate=True";

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="configuration">Konfigurace připojení</param>
        public Database(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Připojení k databázi
        /// </summary>
        /// <returns></returns>
        private bool Connect()
        {
            try
            {
                _connection = new SqlConnection(connectionString);
                _connection.Open();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _connection.State == ConnectionState.Open;
        }

        /// <summary>
        /// Uzavření připojení k databázi
        /// </summary>
        private void Close()
        {
            _connection.Close();
        }
        
        /// <summary>
        /// Zjistí, jestli uživatel v databázi již existuje
        /// </summary>
        /// <param name="user">Uživatel</param>
        /// <returns>true=existuje</returns>
        private bool UserExists(Users user) 
        {
            try
            {
                if (Connect())
                {
                    SqlCommand cmd = _connection.CreateCommand();
                    cmd.CommandText = "select * from Users where username = @username";
                    cmd.Parameters.AddWithValue("username", user.Username);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if(reader.HasRows) return true;

                    cmd.CommandText = "select * from Users where email = @email";
                    cmd.Parameters.AddWithValue("email", user.Email);
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows) return true;

                    reader.Close();
                    Close();
                    return false;
                }
                return true;
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Zjistí, jestli jsou hodnoty hesla a uživatelského jména prázdné
        /// </summary>
        /// <param name="user">Uživatel</param>
        /// <returns></returns>
        private bool IsUserValid(Users user) 
        {
            bool isPasswordNull = user.Password.IsNullOrEmpty();
            bool isUsernameNull = user.Username.IsNullOrEmpty();

            if (isPasswordNull || isUsernameNull) return false;

            return true;
        }

        /// <summary>
        /// Zaregistruje uživatele do databáze
        /// </summary>
        /// <param name="user">Uživatel</param>
        /// <returns>Úspěšně vytvořené uživatele, nebo null</returns>
        public Users? SignUp(Users user)
        {
            if (UserExists(user)) return null;
            if (!IsUserValid(user)) return null;
            try
            {
                if (Connect()) 
                {
                    SqlCommand cmd = _connection.CreateCommand();
                    cmd.CommandText = "insert into Users (username, heslo, email) values (@username, @heslo, @email)";
                    cmd.Parameters.AddWithValue("username", user.Username);
                    cmd.Parameters.AddWithValue("heslo", user.Password);
                    cmd.Parameters.AddWithValue("email", user.Email);
                    SqlDataReader reader = cmd.ExecuteReader();
                    int rowsAffected = reader.RecordsAffected;
                    reader.Close();
                    Close();

                    return rowsAffected > 0 ? user : null;

                }
                return null;
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Přihlásí uživatele do aplikace
        /// </summary>
        /// <param name="user">Uživatel</param>
        /// <returns>Existujícího uživatele, nebo null</returns>
        public Users? Login(Users user)
        {
            if (!IsUserValid(user)) return null;

            Users uzivatel = null;
            try
            {
                if (Connect())
                {
                    SqlCommand cmd = _connection.CreateCommand();
                    cmd.CommandText = "select * from Users where username = @username and heslo = @heslo";
                    cmd.Parameters.AddWithValue("username", user.Username);
                    cmd.Parameters.AddWithValue("heslo", user.Password);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        user.Email = reader["email"].ToString();
                        user.ID = (int)reader["id"];
                        uzivatel = user;
                    }
                    reader.Close();
                    Close();

                    return uzivatel;
                }
                return uzivatel;
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Uloží záznam o mapě do databáze
        /// </summary>
        /// <param name="mapa">Záznam o mapě</param>
        /// <returns>true=úspěšně zaznamenáno</returns>
        public bool SaveImage(Maps mapa) 
        {
            try
            {
                if (Connect())
                {
                    SqlCommand cmd = _connection.CreateCommand();
                    cmd.CommandText = @"insert into Places (Jmeno, ImageMapa, ImageMisto, PoziceX, PoziceY, UserID) values
                                                           (@Jmeno, @ImageMapa, @ImageMisto, @PoziceX, @PoziceY, @UserID)";
                    cmd.Parameters.AddWithValue("Jmeno", mapa.Jmeno);
                    cmd.Parameters.AddWithValue("ImageMapa", mapa.ImageMapa);
                    cmd.Parameters.AddWithValue("ImageMisto", mapa.ImageMisto);
                    cmd.Parameters.AddWithValue("PoziceX", mapa.PoziceX);
                    cmd.Parameters.AddWithValue("PoziceY", mapa.PoziceY);
                    cmd.Parameters.AddWithValue("UserID", mapa.UserID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    int rowsAffected = reader.RecordsAffected;
                    reader.Close();
                    Close();

                    return rowsAffected > 0;
                }
                return false;
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        /// <summary>
        /// Získá náhodný záznam mapy z databáze
        /// </summary>
        /// <returns>Záznam typu mapa</returns>
        public Maps? GetMap()
        {
            Maps Mapa = new();
            List<int> listID = new();
            Random nc = new();
            int counter = 0;
            try
            {
                if (Connect()) 
                {
                    SqlCommand cmd = _connection.CreateCommand();
                    cmd.CommandText = "select PlaceID from Places";
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.HasRows) return null;
                    while (reader.Read()) 
                    {
                        listID.Add(reader.GetInt32(0));
                        counter++;
                    }
                    reader.Close();
                    int nahodnyZaznam = listID[nc.Next(counter)];

                    cmd.CommandText = "select * from Places where PlaceID = @PlaceID";
                    cmd.Parameters.AddWithValue("PlaceID", nahodnyZaznam);
                    reader = cmd.ExecuteReader();

                    while (reader.Read()) 
                    {
                        Mapa.PlaceID = reader.GetInt32(reader.GetOrdinal("PlaceID"));
                        Mapa.Jmeno = reader.GetString(reader.GetOrdinal("Jmeno"));
                        Mapa.ImageMapa = reader.GetString(reader.GetOrdinal("ImageMapa"));
                        Mapa.ImageMisto = reader.GetString(reader.GetOrdinal("ImageMisto"));
                        Mapa.PoziceX = reader.GetInt32(reader.GetOrdinal("PoziceX"));
                        Mapa.PoziceY = reader.GetInt32(reader.GetOrdinal("PoziceY"));
                        Mapa.UserID = reader.GetInt32(reader.GetOrdinal("UserID"));
                    }
                    reader.Close();
                    Close();
                    return Mapa;
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
            return null;
        }
    }
}
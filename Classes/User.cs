﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace regin.Classes
{
    internal class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public byte[] Image = new byte[0];
        public DateTime DateUpdate { get; set; }
        public DateTime DateCreate { get; set; }
        public string PinCode { get; set; }
        public CorrectLogin HandlerCorrectLogin;
        public InCorrectLogin HandlerInCorrectLogin;
        public delegate void CorrectLogin();
        public delegate void InCorrectLogin();

        public void GetUserLogin(string Login)
        {
            this.Id = -1;
            this.Login = String.Empty;
            this.Password = String.Empty;
            this.Name = String.Empty;
            this.Image = new byte[0];
            this.PinCode = String.Empty;

            MySqlConnection connection = WorkingDB.OpenConnection();
            if (WorkingDB.OpenConnection(connection))
            {
                MySqlDataReader userQuery = WorkingDB.Query($"SELECT * FROM `users` WHERE `Login` = '{Login}'", connection);
                if (userQuery.HasRows)
                {
                    userQuery.Read();
                    this.Id = userQuery.GetInt32(0);
                    this.Login = userQuery.GetString(1);
                    this.Password = userQuery.GetString(2);
                    this.Name = userQuery.GetString(3);
                    if (!userQuery.IsDBNull(4))
                    {
                        this.Image = new byte[64 * 1024];
                        userQuery.GetBytes(4, 0, Image, 0, Image.Length);
                    }
                    this.DateUpdate = userQuery.GetDateTime(5);
                    this.DateCreate = userQuery.GetDateTime(6);
                    this.PinCode = userQuery.GetString(7);
                    HandlerCorrectLogin.Invoke();
                }
                else HandlerInCorrectLogin.Invoke();
            }
            else HandlerInCorrectLogin.Invoke();
            WorkingDB.CloseConnection(connection);
        }

        public void SetUser()
        {
            MySqlConnection connection = WorkingDB.OpenConnection();
            if (WorkingDB.OpenConnection(connection))
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO `users` (`Login`, `Password`, `Name`, `Image`, `DateUpdate`, `DateCreate`, `PinCode`) VALUES (@Login, @Password, @Name, @Image, @DateUpdate, @DateCreate, @PinCode)", connection);
                command.Parameters.AddWithValue("@Login", this.Login);
                command.Parameters.AddWithValue("@Password", this.Password);
                command.Parameters.AddWithValue("@Name", this.Name);
                command.Parameters.AddWithValue("@Image", this.Image);
                command.Parameters.AddWithValue("@DateUpdate", this.DateUpdate);
                command.Parameters.AddWithValue("@DateCreate", this.DateCreate);
                command.Parameters.AddWithValue("@PinCode", String.Empty);
                command.ExecuteNonQuery();
            }
            WorkingDB.CloseConnection(connection);
        }

        public void CreateNewPassword()
        {
            if (Login != String.Empty)
            {
                Password = GeneratePassword();
                MySqlConnection connection = WorkingDB.OpenConnection();
                if (WorkingDB.OpenConnection(connection)) WorkingDB.Query($"UPDATE `users` SET `Password` = '{this.Password}' WHERE `Login` = '{this.Login}'", connection);
                WorkingDB.CloseConnection(connection);
                SendMail.SendMessage($"Your account password has been changed.\nNew password: {this.Password}", this.Login);
            }
        }

        public void AddPinCode(string PinCode)
        {
            if (Login != String.Empty)
            {
                MySqlConnection connection = WorkingDB.OpenConnection();
                if (WorkingDB.OpenConnection(connection)) WorkingDB.Query($"UPDATE `users` SET `PinCode` = '{PinCode.ToString()}' WHERE `Login` = '{this.Login}'", connection);
                WorkingDB.CloseConnection(connection);
            }
        }

        public string GeneratePassword()
        {
            List<char> newPassword = new List<char>();
            Random random = new Random();
            char[] ArrNumbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] ArrSymbol = { '|', '-', '_', '!', '@', '#', '$', '%', '&', '*', '=', '+' };
            char[] ArrUpperCase = { 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's'
                    , 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm' };
            for (int i = 0; i < 1; i++)
                newPassword.Add(ArrNumbers[random.Next(ArrNumbers.Length)]);
            for (int i = 0; i < 2; i++)
                newPassword.Add(ArrSymbol[random.Next(ArrSymbol.Length)]);
            for (int i = 0; i < 2; i++)
                newPassword.Add(char.ToUpper(ArrUpperCase[random.Next(ArrUpperCase.Length)]));
            for (int i = 0; i < 6; i++)
                newPassword.Add(ArrUpperCase[random.Next(ArrUpperCase.Length)]);
            for (int i = 0; i < newPassword.Count; i++)
            {
                int RandomSymbol = random.Next(0, newPassword.Count);
                char Symbol = newPassword[RandomSymbol];
                newPassword[RandomSymbol] = newPassword[i];
                newPassword[i] = Symbol;
            }
            string password = "";
            for (int i = 0; i < newPassword.Count; i++)
                password += newPassword[i];
            return password;
        }
    }
}

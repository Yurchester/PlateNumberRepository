using MySql.Data.MySqlClient;
using PlateNumberRecognition.DAL.Models;
using PlateNumberRecognition.OCR.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Text;

namespace PlateNumberRecognition.OCR.BLL
{
    public class Queries : IQueries
    {
        private MySqlConnection _conn;
        private MySqlCommand _cmd;
        private List<Digits> _listOfDigitsVectors = new List<Digits>();
        private List<Letters> _listOfLettersVectors = new List<Letters>();
        public Queries()
        {
            this._conn = null;
            this._conn = OCR.Connection;
        }
        public void InsertDigits(Digits model)
        {
            try
            {
                //_conn.Open();
                string query =
                    $"INSERT INTO `digits` (`InputVector`,`OutputVector`) " +
                    $"VALUES ('{model.InputVector}', '{model.OutputVector}');";
                _cmd = new MySqlCommand() { Connection = _conn, CommandText = query };
                _cmd.ExecuteNonQuery();
                //_conn.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        public void InsertLetters(Letters model)
        {
            try
            {
                string query =
                    $"INSERT INTO `Letters` (`InputVector`,`OutputVector`) " +
                    $"VALUES ('{model.InputVector}', '{model.OutputVector}');";
                _cmd = new MySqlCommand() { Connection = _conn, CommandText = query };
                _cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        public List<Digits> GetDigitsFromDB()
        {
            try
            {
                string query =
                   $"SELECT `InputVector`, `OutputVector` FROM `Digits`";
                _cmd = new MySqlCommand() { Connection = _conn, CommandText = query };

                using (DbDataReader reader = _cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            _listOfDigitsVectors.Add(new Digits()
                            {
                                InputVector = reader.GetString(reader.GetOrdinal("InputVector")),
                                OutputVector = reader.GetString(reader.GetOrdinal("OutputVector"))
                            });
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return _listOfDigitsVectors;
        }

        public List<Letters> GetLettersFromDB()
        {
            try
            {
                string query =
                   $"SELECT `InputVector`, `OutputVector` FROM `Letters`";
                _cmd = new MySqlCommand() { Connection = _conn, CommandText = query };

                using (DbDataReader reader = _cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            _listOfLettersVectors.Add(new Letters()
                            {
                                InputVector = reader.GetString(reader.GetOrdinal("InputVector")),
                                OutputVector = reader.GetString(reader.GetOrdinal("OutputVector"))
                            });
                        }
                    }
                };

                //   _conn.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return _listOfLettersVectors;
        }
    }
}
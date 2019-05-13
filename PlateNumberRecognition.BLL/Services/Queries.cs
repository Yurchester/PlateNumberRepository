using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using PlateNumberRecognition.DAL.Models;
using PlateNumberRecognition.Vision.Logic.Converters;

namespace PlateNumberRecognition.BLL.Services
{
    public class Queries : IQueries
    {
        private MySqlConnection _conn;
        private MySqlCommand _cmd;
        private List<Numbers> _listOfPhoto = new List<Numbers>();
        public Queries(MySqlConnection _conn)
        {
            this._conn = null;
            this._conn = _conn;
        }

        public List<Numbers> GetNumbersFromDB()
        {
            try
            {
                string query =
                   $"SELECT `id`, `image` FROM `numbers` WHERE recognize = 0";
                _cmd = new MySqlCommand() { Connection = _conn, CommandText = query };

                using (DbDataReader reader = _cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var image = Converter.Base64StringToBitmap(reader.GetString(reader.GetOrdinal("Image")));
                            _listOfPhoto.Add(new Numbers()
                            {
                                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                Image = image
                            });
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return _listOfPhoto;
        }

        public void UpdateResultToDB(int id, string result)
        {
            try
            {
                string query =
                    $"UPDATE `numbers` SET recognize = 1, price = {result} WHERE id = {id};";
                _cmd = new MySqlCommand() { Connection = _conn, CommandText = query };
                _cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}

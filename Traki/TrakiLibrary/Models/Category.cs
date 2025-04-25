﻿using SQLite;

namespace TrakiLibrary.Models
{
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Type { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{    
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string EmpName { get; set; }
        public decimal Salary { get; set; }
        [StringLength(2083)]
        [DisplayName("Full-size Image")]
        public string ImageURL { get; set; }
        [StringLength(2083)]
        [DisplayName("Thumbnail")]
        public string ThumbnailURL { get; set; }
    }
    public class MyDemoContext : DbContext
    {
        public MyDemoContext() : base("name=MyDemoContext") { }
        public DbSet<Employee> Employees { get; set; }
    }
    public class BlobInformation
    {
        public Uri BlobUri { get; set; }
        public string BlobName
        {
            get
            {
                return BlobUri.Segments[BlobUri.Segments.Length - 1];
            }
        }
        public string BlobNameWithoutExtension
        {
            get
            {
                return Path.GetFileNameWithoutExtension(BlobName);
            }
        }
        public int EmpId { get; set; }
    }
}

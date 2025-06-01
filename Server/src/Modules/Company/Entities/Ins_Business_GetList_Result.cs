using System;
using System.Collections.Generic;

namespace SqlMapper.Models
{
    public class Ins_Business_GetList_Result
    {
        public int Id { get; set; }
        public string Business { get; set; }
        public string BusinessType { get; set; }
        public int? IndexNum { get; set; }
        public string Alias { get; set; }
    }
}

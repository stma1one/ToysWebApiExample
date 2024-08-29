using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToysWebApiExample.Models;

/// <summary>
/// מחלקה המייצגת צעצוע
/// </summary>
public class Toy
    {
        public int Id { get; set; }//קוד זיהוי
        public string? Name { get; set; }    //שם צעצוע
        public double Price {  get; set; }  //מחיר
        public string? Image { get; set; }//קישור לתמונה
        public ToyTypes? Type { get; set; }//סוג צעצוע
        public bool IsSecondHand { get; set; }//האם צעצוע יד שניה


}


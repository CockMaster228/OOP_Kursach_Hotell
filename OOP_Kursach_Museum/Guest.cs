using System;

namespace OOP_Kursach_Guest
{
    public class Guest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public long PhoneNumber { get; set; }
        public int Room { get; set; }
        public bool OnLiving { get; set; }
        public Guest(string name, string surname, long phonenumber, int room, bool onliving)
        {
            Name = name;
            Surname = surname;
            PhoneNumber = phonenumber;
            Room = room;
            OnLiving = onliving;
        }
    }
}

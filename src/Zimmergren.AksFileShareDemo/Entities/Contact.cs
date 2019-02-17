namespace Zimmergren.AksFileShareDemo.Entities
{
    public class Contact
    {
        public Contact(string name, string email)
        {
            Name = name;
            Email = email;
        }
        public string Name { get; set; }
        public string Email { get; set; }
    }

}

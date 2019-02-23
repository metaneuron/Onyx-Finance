/*
The user entity class represents the data stored in the database for users.
It's used by the Entity Framework Core to map relational data from the DB
into .NET objects for data management and CRUD operations.
*/

namespace WebApi.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        
        // Banking
        public string AccountBalance { get; set; }
    }
}
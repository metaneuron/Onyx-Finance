/*
User DTO is a data transfer object used to send selected user data to and from
the user's API endpoints. This DTO doesn't contain the PasswordHash
and PasswordSalt fields of the user entity class, so not included in responses from 
the web api when the controller maps data from user entities to user dtos.
 
Password property in the DTO is only used for model binding data coming into the
controller from HTTP requests (e.g. authenticate, register, etc.) passwords are 
never included in responses from the web api. 
*/

namespace WebApi.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AccountBalance { get; set; }
    }
}
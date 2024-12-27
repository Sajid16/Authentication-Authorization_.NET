Authentication & Authorization using .NET with custom attribute & custom filter
=============

### Features
- JWT Bearer token generation
- validation of bearer token
- custom validation filter
- custom validation attribute
- secure endpoint based on roles
- despite validation make some endpoints anonymous

#### nuget packages for JWT
- Microsoft.AspNetCore.Authentication.JwtBearer
#### nuget packages for password encryption
- BCrypt.Net-Next

#### Highlighted discussions
- Authentication using custom authentication & authorization filter
>   prepared a custom authentication filter and register it into program.cs file for using it and resolved it's dependency through DI. Mentioned it at the controller level and kept those methods anonymous those didn't need any validation or checkpoints and used by all the users. Custom filters can be made parameterized if it has some fixed paramters to check and can be registered those params into DI.
- Authentication using custom authentication & authorization attribute
>   prepared a custom authorization attribute & mentioned it at the action methods level to keep those methods validated before it comes to main methods. Here we can check the validation of JWT token as well as more validations if needed. Additionally we can pass parameters through attribute. For example: here we have secured the endpoints based on user role and those kinda checks has been done inside the custom attribute to make it available to certailn levels.

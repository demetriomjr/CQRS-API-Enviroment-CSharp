global using FrontDoor.Models;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.IdentityModel.Tokens;
global using Models.Users;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Text;

public record UserCredentials(string username, string password);

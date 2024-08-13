using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PostGreDbContext.Enums;
using PostGreDbContext.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using UserService.Interfaces;
using UserService.Utils;

namespace UserService.Models
{
    public partial class UserRepository : IUserRepository
    {
        private readonly string _objectName = nameof(UserRepository);

        private readonly ILogger _logger;
        private readonly GradationTaskContext _context;

        public UserRepository(GradationTaskContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger(_objectName);
        }

        public async Task<IActionResult> AddAdministratorAsync(UserDto user)
        {
            _logger.LogInformation($"Try to add first administrator");

            try
            {
                // проверка есть ли в базе пользователи

                var users = await _context.UserDbs.Select(x => x.UserRoleId == 0).ToListAsync().ConfigureAwait(false);

                if (users.Any())
                {
                    return new BadRequestObjectResult("В базе данных уже есть созданные пользователи");
                }

                if (user.UserRoleId != (int)UserRoleTypeEnum.Administrator)
                    return new BadRequestObjectResult($"Не верный тип пользователя");

                var result = await AddUserToDbAsync(user).ConfigureAwait(false);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to add first administrator");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }

        }

        public async Task<IActionResult> AddUserAsync(UserDto user)
        {
            _logger.LogInformation($"Try to add first user");

            try
            {
                if (user.UserRoleId != (int)UserRoleTypeEnum.User)
                    return new BadRequestObjectResult($"Не верный тип пользователя");

                var result = await AddUserToDbAsync(user).ConfigureAwait(false);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to add user");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> GetMyId()
        {
            _logger.LogInformation($"Try to get user guid");

            try
            {
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to get user guid");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> GetUserListAsync()
        {
            _logger.LogInformation($"Try to add get user list");

            try
            {
                var users = await _context.UserDbs
                    .Select(x => new UserResponse
                    {
                        UserEmail = x.UserEmail,
                        UserRoleId = x.UserRoleId,
                    })
                    .ToListAsync()
                    .ConfigureAwait(false);

                return new OkObjectResult(users);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to get user list");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> RemoveUserAsync(Guid id)
        {
            _logger.LogInformation($"Try to remove user");

            try
            {
                var user = await _context.UserDbs
                    .Where(x => x.UserRoleId == (int)UserRoleTypeEnum.User && x.Id == id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                if (user == null)
                    return new BadRequestObjectResult($"User with id: {id} not found in db");

                _context.UserDbs.Remove(user);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to remove user");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        private static string GetPasswordHash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
            Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashed;
        }

        private async Task<AuthorizationResponse> AddUserToDbAsync(UserDto user)
        {
            var user_db = new UserDb
            {
                UserEmail = user.Email,
                UserPasswordHash = GetPasswordHash(user.Password),
                UserRoleId = user.UserRoleId,
            };

            await _context.UserDbs.AddAsync(user_db).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            user.Guid = user_db.Id;

            var privateKey = RSA.Create();
            privateKey.ImportFromPem(File.ReadAllText("./Keys/key"));

            var claims = new[]
            {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.UserRole),
                    new Claim(ClaimTypes.NameIdentifier, user.Guid.ToString()),
                };

            var token = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: new SigningCredentials(new RsaSecurityKey(privateKey), SecurityAlgorithms.RsaSha256)
                );

            return new AuthorizationResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ValidTo = token.ValidTo,
            };
        }

    }
}

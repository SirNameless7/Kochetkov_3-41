using Microsoft.EntityFrameworkCore;
using KPO_Cursovoy.Models;

namespace KPO_Cursovoy.Services
{
    public class AuthenticationService
    {
        private readonly AppDbContext _context;
        public static User? CurrentUser { get; set; }

        public AuthenticationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> LoginAsync(string loginInput, string password)
        {
            try
            {
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.Login == loginInput && a.PasswordHash == password);

                if (account != null)
                {
                    CurrentUser = await _context.Users.FindAsync(account.UserId);
                    App.CurrentUser = CurrentUser;
                    return true;
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Phone == loginInput);

                if (user != null)
                {
                    account = await _context.Accounts
                        .FirstOrDefaultAsync(a => a.UserId == user.UserId && a.PasswordHash == password);

                    if (account != null)
                    {
                        CurrentUser = user;
                        App.CurrentUser = user;
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RegisterAsync(string phone, string password, string fullName)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Phone == phone))
                    return false;

                var user = new User
                {
                    UserId = 0,
                    FullName = fullName,
                    Phone = phone,
                    LoyaltyStatus = "обычный"
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var account = new Account
                {
                    AccountId = 0,
                    UserId = user.UserId,
                    Login = phone,
                    PasswordHash = password,
                    Role = "client"
                };

                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                CurrentUser = user;
                App.CurrentUser = user;
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Register error: {ex.Message}");
                return false;
            }
        }

        public void Logout()
        {
            CurrentUser = null;
            App.CurrentUser = null;
        }

        public async Task<User?> GetUserByPhoneAsync(string phone)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Phone == phone);
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using KPO_Cursovoy.Models;
using Microsoft.EntityFrameworkCore;

namespace KPO_Cursovoy.Services;

public class AuthenticationService
{
    private readonly AppDbContext _context;

    public static User? CurrentUser { get; set; }

    public AuthenticationService(AppDbContext context)
    {
        _context = context;
    }

    private static string DigitsOnly(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return new string(input.Where(char.IsDigit).ToArray());
    }
    private static string ToCanonicalPhone(string input)
    {
        var digits = DigitsOnly(input);

        if (digits.Length == 11 && digits.StartsWith("8"))
            digits = "7" + digits.Substring(1);

        if (digits.Length == 11 && digits.StartsWith("7"))
            return "+" + digits;

        return input.Trim();
    }

    public async Task<bool> LoginAsync(string loginInput, string password)
    {
        try
        {
            var input = (loginInput ?? string.Empty).Trim();

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Login == input && a.PasswordHash == password);

            if (account != null)
            {
                CurrentUser = await _context.Users.FindAsync(account.UserId);
                App.CurrentUser = CurrentUser;
                return CurrentUser != null;
            }

            var canonicalPhone = ToCanonicalPhone(input);

            var altPhone = DigitsOnly(input);
            if (altPhone.Length == 11 && altPhone.StartsWith("8"))
                altPhone = "7" + altPhone.Substring(1);

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Phone == canonicalPhone || u.Phone == altPhone);

            if (user == null)
                return false;

            account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == user.UserId && a.PasswordHash == password);

            if (account == null)
                return false;

            CurrentUser = user;
            App.CurrentUser = user;
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RegisterAsync(string login, string phone, string password)
    {
        try
        {
            var loginTrimmed = (login ?? string.Empty).Trim();
            var canonicalPhone = ToCanonicalPhone(phone);

            if (string.IsNullOrWhiteSpace(loginTrimmed) ||
                string.IsNullOrWhiteSpace(canonicalPhone) ||
                string.IsNullOrWhiteSpace(password))
                return false;

            if (await _context.Accounts.AnyAsync(a => a.Login == loginTrimmed))
                return false;

            var altPhone = DigitsOnly(phone);
            if (altPhone.Length == 11 && altPhone.StartsWith("8"))
                altPhone = "7" + altPhone.Substring(1);

            if (await _context.Users.AnyAsync(u => u.Phone == canonicalPhone || u.Phone == altPhone))
                return false;

            var user = new User
            {
                UserId = 0,
                FullName = "",
                Phone = canonicalPhone,
                LoyaltyStatus = "обычный"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var account = new Account
            {
                AccountId = 0,
                UserId = user.UserId,
                Login = loginTrimmed,
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
        var canonicalPhone = ToCanonicalPhone(phone);

        var altPhone = DigitsOnly(phone);
        if (altPhone.Length == 11 && altPhone.StartsWith("8"))
            altPhone = "7" + altPhone.Substring(1);

        return await _context.Users.FirstOrDefaultAsync(u =>
            u.Phone == canonicalPhone || u.Phone == altPhone);
    }
}
